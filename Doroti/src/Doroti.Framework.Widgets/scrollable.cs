// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scrollable.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget ViewportBuilder(BuildContext context, ViewportOffset position);

public delegate Widget TwoDimensionalViewportBuilder(BuildContext context, ViewportOffset verticalPosition, ViewportOffset horizontalPosition);

internal delegate void _EnsureVisibleResults__scrollable();

public class Scrollable : StatefulWidget
{
    public virtual AxisDirection axisDirection { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual Func<BuildContext, ViewportOffset, Widget> viewportBuilder { get; private set; } = default!;
    public virtual Func<ScrollIncrementDetails, double>? incrementCalculator { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual long? semanticChildCount { get; private set; }
    public virtual DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Scrollable(Key? key = null, AxisDirection axisDirection = AxisDirection.down, ScrollController? controller = null, ScrollPhysics? physics = null, Func<BuildContext, ViewportOffset, Widget> viewportBuilder = default!, Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, long? semanticChildCount = null, DragStartBehavior dragStartBehavior = DragStartBehavior.start, string? restorationId = null, ScrollBehavior? scrollBehavior = null, Clip clipBehavior = Clip.hardEdge, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque) : base(key: key)
    {
        this.axisDirection = axisDirection;
        this.controller = controller;
        this.physics = physics;
        this.viewportBuilder = viewportBuilder;
        this.incrementCalculator = incrementCalculator;
        this.excludeFromSemantics = excludeFromSemantics;
        this.semanticChildCount = semanticChildCount;
        this.dragStartBehavior = dragStartBehavior;
        this.restorationId = restorationId;
        this.scrollBehavior = scrollBehavior;
        this.clipBehavior = clipBehavior;
        this.hitTestBehavior = hitTestBehavior;
        System.Diagnostics.Debug.Assert((semanticChildCount is null) || (semanticChildCount >= 0L));
    }

    public virtual Axis axis => Basic_typesLibrary.axisDirectionToAxis(axisDirection);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new ScrollableState());
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<AxisDirection>("axisDirection", axisDirection));
        properties.add(new DiagnosticsProperty<ScrollPhysics>("physics", physics));
        properties.add(new StringProperty("restorationId", restorationId));
    }

    public static ScrollableState? maybeOf(BuildContext context, Axis? axis = null)
    {
        var originalContext = context;
        InheritedElement? element = context.getElementForInheritedWidgetOfExactType<_ScrollableScope__scrollable>();
        while (element is not null)
        {
            ScrollableState scrollableLocal = ((_ScrollableScope__scrollable?)element.widget)!.scrollable;
            if ((axis is null) || Equals(Basic_typesLibrary.axisDirectionToAxis(scrollableLocal.axisDirection), DartRuntimePrimitives.RequireValue(axis)))
            {
                originalContext.dependOnInheritedElement(element);
                return scrollableLocal;
            }
            context = scrollableLocal.context;
            element = context.getElementForInheritedWidgetOfExactType<_ScrollableScope__scrollable>();
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScrollableState of(BuildContext context, Axis? axis = null)
    {
        ScrollableState? scrollableState = maybeOf(context, axis: axis);
        DartRuntimePrimitives.Assert(() =>
            {
                if (scrollableState is null)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Scrollable.of() was called with a context that does not contain a " + "Scrollable widget."), new ErrorDescription("No Scrollable widget ancestor could be found " + $"{((axis is null) ? "" : $"for the provided Axis: {DartRuntimePrimitives.RequireValue(axis)} ")}" + "starting from the context that was passed to Scrollable.of(). This " + "can happen because you are using a widget that looks for a Scrollable " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return scrollableState!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool recommendDeferredLoadingForContext(BuildContext context, Axis? axis = null)
    {
        _ScrollableScope__scrollable? widget = context.getInheritedWidgetOfExactType<_ScrollableScope__scrollable>();
        while (widget is not null)
        {
            if ((axis is null) || Equals(Basic_typesLibrary.axisDirectionToAxis(widget.scrollable.axisDirection), DartRuntimePrimitives.RequireValue(axis)))
            {
                return widget.position.recommendDeferredLoading(context);
            }
            context = widget.scrollable.context;
            widget = context.getInheritedWidgetOfExactType<_ScrollableScope__scrollable>();
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future ensureVisible(BuildContext context, double alignment = 0.0, Duration duration = default, Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit)
    {
        var futures = new List<Future>();
        RenderObject? targetRenderObjectLocal = default!;
        ScrollableState? scrollable = maybeOf(context);
        while (scrollable is not null)
        {
            List<Future> newFutures = default!;
            DartRuntimePrimitives.Ignore((newFutures, scrollable) = scrollable._performEnsureVisible(context.findRenderObject()!, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy, targetRenderObject: targetRenderObjectLocal));
            futures.AddRange(newFutures.Cast<Future>());
            targetRenderObjectLocal ??= context.findRenderObject();
            context = scrollable.context;
            scrollable = maybeOf(context);
        }
        if (!Enumerable.Any(futures) || Equals(duration, Duration.zero))
        {
            return Future.value();
        }
        if (checked(futures.Count) == 1L)
        {
            return futures.Single();
        }
        return DartAsyncRuntime.wait<object?>(futures).then((_) => { });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ScrollableScope__scrollable : InheritedWidget
{
    public virtual ScrollableState scrollable { get; private set; } = default!;
    public virtual ScrollPosition position { get; private set; } = default!;

    internal _ScrollableScope__scrollable(ScrollableState scrollable, ScrollPosition position, Widget child) : base(child: child)
    {
        this.scrollable = scrollable;
        this.position = position;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget)
    {
        var __old = (_ScrollableScope__scrollable)oldWidget;
        return !Equals(position, __old.position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ScrollableState : State<Scrollable>, TickerProviderStateMixin<Scrollable>, RestorationMixin<Scrollable>, ScrollContext
{
    internal virtual ScrollPosition? _position { get; set; } = default;
    internal virtual ScrollPhysics? _physics { get; set; } = default;
    internal virtual double _devicePixelRatio { get; set; } = default!;
    internal virtual _RestorableScrollOffset__scrollable _persistedScrollOffset { get; private set; } = new _RestorableScrollOffset__scrollable();
    internal virtual ScrollBehavior _configuration { get; set; } = default!;
    internal virtual ScrollController? _fallbackScrollController { get; set; } = default;
    internal virtual DeviceGestureSettings? _mediaQueryGestureSettings { get; set; } = default;
    internal virtual GlobalKey<IState> _scrollSemanticsKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual GlobalKey<RawGestureDetectorState> _gestureDetectorKey { get; private set; } = GlobalKey<RawGestureDetectorState>.Create();
    internal virtual GlobalKey<IState> _ignorePointerKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual DartMap<Type, dynamic> _gestureRecognizers { get; set; } = new DartMap<Type, dynamic>();
    internal virtual bool _shouldIgnorePointer { get; set; } = false;
    internal virtual bool? _lastCanDrag { get; set; } = default;
    internal virtual Axis? _lastAxisDirection { get; set; } = default;
    internal virtual Drag? _drag { get; set; } = default;
    internal virtual ScrollHoldController? _hold { get; set; } = default;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<IRestorableProperty, Action> _properties { get; set; } = new DartMap<IRestorableProperty, Action>();
    public virtual List<IRestorableProperty>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual RestorationBucket? _currentParent { get; set; } = default;

    public virtual ScrollPosition position => DartRuntimePrimitives.ConvertValue<ScrollPosition>(_position!);
    public virtual ScrollPhysics? resolvedPhysics => _physics;
    public virtual Offset deltaToScrollOrigin => DartRuntimePrimitives.ConvertValue<Offset>(axisDirection switch { AxisDirection.up => new Offset(0, -position.pixels), AxisDirection.down => new Offset(0, position.pixels), AxisDirection.left => new Offset(-position.pixels, 0), AxisDirection.right => new Offset(position.pixels, 0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual ScrollController _effectiveScrollController => DartRuntimePrimitives.ConvertValue<ScrollController>(widget.controller ?? _fallbackScrollController!);
    public virtual AxisDirection axisDirection => widget.axisDirection;
    public virtual Scheduler.TickerProvider vsync => DartRuntimePrimitives.ConvertValue<Scheduler.TickerProvider>(this);
    public virtual double devicePixelRatio => _devicePixelRatio;
    public virtual BuildContext? notificationContext => _gestureDetectorKey.currentContext;
    public virtual BuildContext storageContext => context;
    public virtual string? restorationId => widget.restorationId;
    internal virtual void _updatePosition()
    {
        _configuration = widget.scrollBehavior ?? ScrollConfiguration.of(context);
        ScrollPhysics? physicsFromWidget = widget.physics ?? (widget.scrollBehavior?.getScrollPhysics(context));
        _physics = _configuration.getScrollPhysics(context);
        _physics = physicsFromWidget?.applyTo(_physics) ?? _physics;
        ScrollPosition? oldPosition = _position;
        if (oldPosition is not null)
        {
            _effectiveScrollController.detach(oldPosition);
            DartAsyncRuntime.scheduleMicrotask(oldPosition.dispose);
        }
        _position = _effectiveScrollController.createScrollPosition(_physics!, this, oldPosition);
        DartRuntimePrimitives.Assert(() => _position is not null);
        _effectiveScrollController.attach(position);
    }

    public virtual void restoreState(RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(_persistedScrollOffset, "offset");
        DartRuntimePrimitives.Assert(() => _position is not null);
        if (_persistedScrollOffset.value is not null)
        {
            position.restoreOffset(DartRuntimePrimitives.RequireValue(_persistedScrollOffset.value), initialRestore: initialRestore);
        }
    }

    public virtual void saveOffset(double offset)
    {
        DartRuntimePrimitives.Assert(() => RestorationLibrary.debugIsSerializableForRestoration(offset));
        _persistedScrollOffset.value = offset;
        ServicesBinding.instance.restorationManager.flushData();
    }

    public override void initState()
    {
        if (widget.controller is null)
        {
            _fallbackScrollController = new ScrollController();
        }
        base.initState();
    }

    public override void didChangeDependencies()
    {
        _mediaQueryGestureSettings = MediaQuery.maybeGestureSettingsOf(context);
        _devicePixelRatio = MediaQuery.maybeDevicePixelRatioOf(context) ?? View.of(context).devicePixelRatio;
        _updatePosition();
        base.didChangeDependencies();
        RestorationBucket? oldBucket = _bucket;
        bool needsRestore = restorePending;
        _currentParent = RestorationScope.maybeOf(context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            oldBucket?.dispose();
        }
    }

    internal virtual bool _shouldUpdatePosition(Scrollable oldWidget)
    {
        if (widget.scrollBehavior is null != oldWidget.scrollBehavior is null)
        {
            return true;
        }
        if ((widget.scrollBehavior is not null) && (oldWidget.scrollBehavior is not null) && widget.scrollBehavior!.shouldNotify(oldWidget.scrollBehavior!))
        {
            return true;
        }
        ScrollPhysics? newPhysics = widget.physics ?? (widget.scrollBehavior?.getScrollPhysics(context));
        ScrollPhysics? oldPhysics = oldWidget.physics ?? (oldWidget.scrollBehavior?.getScrollPhysics(context));
        do
        {
            if (!Equals(DartRuntimePrimitives.RuntimeType(newPhysics), DartRuntimePrimitives.RuntimeType(oldPhysics)))
            {
                return true;
            }
            newPhysics = newPhysics?.parent;
            oldPhysics = oldPhysics?.parent;
        }
        while ((newPhysics is not null) || (oldPhysics is not null));
        return !Equals(DartRuntimePrimitives.RuntimeType(widget.controller), DartRuntimePrimitives.RuntimeType(oldWidget.controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(Scrollable oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if (!Equals(widget.controller, oldWidget.controller))
        {
            if (oldWidget.controller is null)
            {
                DartRuntimePrimitives.Assert(() => _fallbackScrollController is not null);
                DartRuntimePrimitives.Assert(() => widget.controller is not null);
                _fallbackScrollController!.detach(position);
                _fallbackScrollController!.dispose();
                _fallbackScrollController = null;
            }
            else
            {
                oldWidget.controller?.detach(position);
                if (widget.controller is null)
                {
                    _fallbackScrollController = new ScrollController();
                }
            }
            _effectiveScrollController.attach(position);
        }
        if (_shouldUpdatePosition(oldWidget))
        {
            _updatePosition();
        }
    }

    public override void dispose()
    {
        if (widget.controller is not null)
        {
            widget.controller!.detach(position);
        }
        else
        {
            _fallbackScrollController?.detach(position);
            _fallbackScrollController?.dispose();
        }
        position.dispose();
        _persistedScrollOffset.dispose();
        _properties.forEach((property, listener) =>
        {
            if (!property._disposed)
            {
                property.removeListener(listener);
            }
        });
        _bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual void setSemanticsActions(HashSet<SemanticsAction> actions)
    {
        if (_gestureDetectorKey.currentState is not null)
        {
            _gestureDetectorKey.currentState!.replaceSemanticsActions(actions);
        }
    }

    public virtual void setCanDrag(bool value)
    {
        if ((value == _lastCanDrag) && (!value || Equals(widget.axis, _lastAxisDirection)))
        {
            return;
        }
        if (!value)
        {
            _gestureRecognizers = new DartMap<Type, dynamic>();
            _handleDragCancel();
        }
        else
        {
            switch (widget.axis)
            {
                case Axis.vertical:
                    {
                        _gestureRecognizers = new DartMap<Type, dynamic>
                        {
                            [typeof(VerticalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<VerticalDragGestureRecognizer>(() => new VerticalDragGestureRecognizer(supportedDevices: _configuration.dragDevices), (instance) =>
                            {
                                DartRuntimePrimitives.Ignore(((Func<VerticalDragGestureRecognizer>)(() =>
                                {
                                    var __cascade = instance;
                                    __cascade.onDown = _handleDragDown;
                                    __cascade.onStart = _handleDragStart;
                                    __cascade.onUpdate = _handleDragUpdate;
                                    __cascade.onEnd = _handleDragEnd;
                                    __cascade.onCancel = _handleDragCancel;
                                    __cascade.minFlingDistance = _physics?.minFlingDistance;
                                    __cascade.minFlingVelocity = _physics?.minFlingVelocity;
                                    __cascade.maxFlingVelocity = _physics?.maxFlingVelocity;
                                    __cascade.velocityTrackerBuilder = _configuration.velocityTrackerBuilder(context);
                                    __cascade.dragStartBehavior = widget.dragStartBehavior;
                                    __cascade.multitouchDragStrategy = _configuration.getMultitouchDragStrategy(context);
                                    __cascade.gestureSettings = _mediaQueryGestureSettings;
                                    __cascade.supportedDevices = _configuration.dragDevices;
                                    return __cascade;
                                }))());
                            })
                        };
                        break;
                    }
                case Axis.horizontal:
                    {
                        _gestureRecognizers = new DartMap<Type, dynamic>
                        {
                            [typeof(HorizontalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<HorizontalDragGestureRecognizer>(() => new HorizontalDragGestureRecognizer(supportedDevices: _configuration.dragDevices), (instance) =>
                            {
                                DartRuntimePrimitives.Ignore(((Func<HorizontalDragGestureRecognizer>)(() =>
                                {
                                    var __cascade = instance;
                                    __cascade.onDown = _handleDragDown;
                                    __cascade.onStart = _handleDragStart;
                                    __cascade.onUpdate = _handleDragUpdate;
                                    __cascade.onEnd = _handleDragEnd;
                                    __cascade.onCancel = _handleDragCancel;
                                    __cascade.minFlingDistance = _physics?.minFlingDistance;
                                    __cascade.minFlingVelocity = _physics?.minFlingVelocity;
                                    __cascade.maxFlingVelocity = _physics?.maxFlingVelocity;
                                    __cascade.velocityTrackerBuilder = _configuration.velocityTrackerBuilder(context);
                                    __cascade.dragStartBehavior = widget.dragStartBehavior;
                                    __cascade.multitouchDragStrategy = _configuration.getMultitouchDragStrategy(context);
                                    __cascade.gestureSettings = _mediaQueryGestureSettings;
                                    __cascade.supportedDevices = _configuration.dragDevices;
                                    return __cascade;
                                }))());
                            })
                        };
                        break;
                    }
            }
        }
        _lastCanDrag = value;
        _lastAxisDirection = widget.axis;
        if (_gestureDetectorKey.currentState is not null)
        {
            _gestureDetectorKey.currentState!.replaceGestureRecognizers(_gestureRecognizers);
        }
    }

    public virtual void setIgnorePointer(bool value)
    {
        if (_shouldIgnorePointer == value)
        {
            return;
        }
        _shouldIgnorePointer = value;
        if (_ignorePointerKey.currentContext is not null)
        {
            var renderBox = ((RenderIgnorePointer?)_ignorePointerKey.currentContext!.findRenderObject()!)!;
            renderBox.ignoring = _shouldIgnorePointer;
        }
    }

    internal virtual void _handleDragDown(DragDownDetails details)
    {
        DartRuntimePrimitives.Assert(() => _drag is null);
        DartRuntimePrimitives.Assert(() => _hold is null);
        _hold = position.hold(() => _disposeHold());
    }

    internal virtual void _handleDragStart(DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => _drag is null);
        _drag = position.drag(details, () => _disposeDrag());
        DartRuntimePrimitives.Assert(() => _drag is not null);
        if (_hold is not null)
        {
            _disposeHold();
        }
    }

    internal virtual void _handleDragUpdate(DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => (_hold is null) || (_drag is null));
        _drag?.update(details);
    }

    internal virtual void _handleDragEnd(DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => (_hold is null) || (_drag is null));
        _drag?.end(details);
        DartRuntimePrimitives.Assert(() => _drag is null);
    }

    internal virtual void _handleDragCancel()
    {
        if (_gestureDetectorKey.currentContext is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (_hold is null) || (_drag is null));
        _hold?.cancel();
        _drag?.cancel();
        DartRuntimePrimitives.Assert(() => _hold is null);
        DartRuntimePrimitives.Assert(() => _drag is null);
    }

    internal virtual void _disposeHold()
    {
        _hold = null;
    }

    internal virtual void _disposeDrag()
    {
        _drag = null;
    }

    internal virtual double _targetScrollOffsetForPointerScroll(double delta)
    {
        return Math.Min(Math.Max(position.pixels + delta, position.minScrollExtent), position.maxScrollExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _pointerSignalEventDelta(PointerScrollEvent @event)
    {
        HashSet<LogicalKeyboardKey> pressed = HardwareKeyboard.instance.logicalKeysPressed;
        bool flipAxes = pressed.any(__item => _configuration.pointerAxisModifiers.Contains(__item)) && Equals(@event.kind, PointerDeviceKind.mouse);
        Axis axisLocal = flipAxes ? Basic_typesLibrary.flipAxis(widget.axis) : widget.axis;
        double delta = axisLocal switch { Axis.horizontal => @event.scrollDelta.dx, Axis.vertical => @event.scrollDelta.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return Basic_typesLibrary.axisDirectionIsReversed(widget.axisDirection) ? -delta : delta;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _receivedPointerSignal(PointerSignalEvent @event)
    {
        if ((@event is PointerScrollEvent) && (_position is not null))
        {
            PointerScrollEvent @event__as37801 = (PointerScrollEvent)@event;
            if ((_physics is not null) && !_physics!.shouldAcceptUserOffset(position))
            {
                return;
            }
            double delta = _pointerSignalEventDelta(@event__as37801);
            double targetScrollOffset = _targetScrollOffsetForPointerScroll(delta);
            if ((delta != 0.0) && (targetScrollOffset != position.pixels))
            {
                GestureBinding.instance.pointerSignalResolver.register(@event__as37801, (__arg0) => ((System.Action<PointerEvent>)_handlePointerScroll)(DartRuntimePrimitives.ConvertValue<PointerEvent>(__arg0)));
                return;
            }
        }
        else
        {
            if (@event is PointerScrollInertiaCancelEvent)
            {
                PointerScrollInertiaCancelEvent @event__as38382 = (PointerScrollInertiaCancelEvent)@event;
                position.pointerScroll(0);
            }
        }
    }

    internal virtual void _handlePointerScroll(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => @event is PointerScrollEvent);
        var scrollEvent = ((PointerScrollEvent?)@event)!;
        double delta = _pointerSignalEventDelta(scrollEvent);
        double targetScrollOffset = _targetScrollOffsetForPointerScroll(delta);
        if ((delta != 0.0) && (targetScrollOffset != position.pixels))
        {
            position.pointerScroll(delta);
            scrollEvent.respond(allowPlatformDefault: false);
        }
    }

    internal virtual bool _handleScrollMetricsNotification(ScrollMetricsNotification notification)
    {
        if (notification.depth == 0L)
        {
            RenderObject? scrollSemanticsRenderObject = _scrollSemanticsKey.currentContext?.findRenderObject();
            if (scrollSemanticsRenderObject is not null)
            {
                scrollSemanticsRenderObject.markNeedsSemanticsUpdate();
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildChrome(BuildContext context, Widget child)
    {
        var details = new ScrollableDetails(direction: widget.axisDirection, controller: _effectiveScrollController, decorationClipBehavior: widget.clipBehavior);
        return _configuration.buildScrollbar(context, _configuration.buildOverscrollIndicator(context, child, details), details);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => _position is not null);
        Widget result = new _ScrollableScope__scrollable(scrollable: this, position: position, child: new Listener(onPointerSignal: _receivedPointerSignal, child: new RawGestureDetector(key: _gestureDetectorKey, gestures: _gestureRecognizers, behavior: widget.hitTestBehavior, excludeFromSemantics: widget.excludeFromSemantics, child: new Semantics(explicitChildNodes: !widget.excludeFromSemantics, child: new IgnorePointer(key: _ignorePointerKey, ignoring: _shouldIgnorePointer, child: widget.viewportBuilder(context, position))))));
        if (!widget.excludeFromSemantics)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new NotificationListener<ScrollMetricsNotification>(onNotification: _handleScrollMetricsNotification, child: new _ScrollSemantics__scrollable(key: _scrollSemanticsKey, position: position, allowImplicitScrolling: _physics!.allowImplicitScrolling, axis: widget.axis, semanticChildCount: widget.semanticChildCount, child: result)));
        }
        result = _buildChrome(context, result);
        SelectionRegistrar? registrarLocal = SelectionContainer.maybeOf(context);
        if (registrarLocal is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new _ScrollableSelectionHandler__scrollable(state: this, position: position, registrar: registrarLocal, child: result));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (List<Future>, ScrollableState) _performEnsureVisible(RenderObject @object, double alignment = 0.0, Duration duration = default, Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, RenderObject? targetRenderObject = null)
    {
        Future ensureVisibleFuture = position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy, targetRenderObject: targetRenderObject);
        return (new List<Future> { ensureVisibleFuture }, this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
        properties.add(new DiagnosticsProperty<ScrollPosition>("position", _position));
        properties.add(new DiagnosticsProperty<ScrollPhysics>("effective physics", _physics));
    }

    public virtual Scheduler.Ticker createTicker(System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public virtual RestorationBucket? bucket => _bucket;
    public virtual void didToggleBucket(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => _bucket?.isReplacing != true);
    }

    public virtual void registerForRestoration(IRestorableProperty property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => (property._restorationId is null) || _debugDoingRestore && (property._restorationId == restorationId), () => (object?)$"Property is already registered under {property._restorationId}.");
        DartRuntimePrimitives.Assert(() => _debugDoingRestore || !_properties.Keys.map((r) => r._restorationId).contains(restorationId), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = bucket?.contains(restorationId) ?? false;
        object? initialValue = hasSerializedValue ? property.fromPrimitivesObject(bucket!.read<object>(restorationId)) : property.createDefaultValueObject();
        if (!property.isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if (bucket is null)
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener(listener);
            _properties[property] = listener;
        }
        DartRuntimePrimitives.Assert(() => (property._restorationId == restorationId) && Equals(property._owner, this) && _properties.ContainsKey(property));
        property.initWithValueObject(initialValue);
        if (!hasSerializedValue && property.enabled && (bucket is not null))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void unregisterFromRestoration(IRestorableProperty property)
    {
        DartRuntimePrimitives.Assert(() => Equals(property._owner, this));
        _bucket?.remove<object?>(property._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((_currentParent is null) || (_bucket?.restorationId == restorationId) || restorePending)
        {
            return;
        }
        RestorationBucket? oldBucket = _bucket;
        DartRuntimePrimitives.Assert(() => !restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: _currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => !Equals(oldBucket, _bucket));
            DartRuntimePrimitives.Assert(() => (_bucket is null) || (oldBucket is null));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (_firstRestorePending)
            {
                return true;
            }
            if (restorationId is null)
            {
                return false;
            }
            RestorationBucket? potentialNewParent = RestorationScope.maybeOf(context);
            return (!Equals(potentialNewParent, _currentParent)) && (potentialNewParent?.isReplacing ?? false);
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>(_debugPropertiesWaitingForReregistration is not null);
    public virtual void _doRestore(RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration = _properties.Keys.ToList();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        restoreState(oldBucket, _firstRestorePending);
        _firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (Enumerable.Any(_debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\"."), new ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:") }));
                }
                _debugPropertiesWaitingForReregistration = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual bool _updateBucketIfNecessary(RestorationBucket? parent, bool restorePending)
    {
        if ((restorationId is null) || (parent is null))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: null, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => _bucket is null);
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => restorationId is not null);
        if (restorePending || (_bucket is null))
        {
            RestorationBucket newBucketLocal = parent.claimChild(restorationId!, debugOwner: this);
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => Equals(_bucket, newBucketLocal));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => _bucket is not null);
        DartRuntimePrimitives.Assert(() => !restorePending);
        _bucket!.rename(restorationId!);
        parent.adoptChild(_bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(RestorationBucket? newBucket, bool restorePending)
    {
        if (Equals(newBucket, _bucket))
        {
            return false;
        }
        RestorationBucket? oldBucket = _bucket;
        _bucket = newBucket;
        if (!restorePending)
        {
            if (_bucket is not null)
            {
                _properties.Keys.forEach((__arg0) => ((System.Action<IRestorableProperty>)_updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(IRestorableProperty property)
    {
        if (property.enabled)
        {
            _bucket?.write(property._restorationId!, property.toPrimitives());
        }
        else
        {
            _bucket?.remove<object>(property._restorationId!);
        }
    }

    public virtual void _unregister(IRestorableProperty property)
    {
        Action listener = _properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        property.removeListener(listener);
        property._unregister();
    }

}

public class _ScrollableSelectionHandler__scrollable : StatefulWidget
{
    public virtual ScrollableState state { get; private set; } = default!;
    public virtual ScrollPosition position { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual SelectionRegistrar registrar { get; private set; } = default!;

    internal _ScrollableSelectionHandler__scrollable(ScrollableState state, ScrollPosition position, SelectionRegistrar registrar, Widget child)
    {
        this.state = state;
        this.position = position;
        this.registrar = registrar;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ScrollableSelectionHandlerState__scrollable());
}

public class _ScrollableSelectionHandlerState__scrollable : State<_ScrollableSelectionHandler__scrollable>
{
    internal virtual _ScrollableSelectionContainerDelegate__scrollable _selectionDelegate { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _selectionDelegate = new _ScrollableSelectionContainerDelegate__scrollable(state: widget.state, position: widget.position);
    }

    public override void didUpdateWidget(_ScrollableSelectionHandler__scrollable oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.position, widget.position))
        {
            _selectionDelegate.position = widget.position;
        }
    }

    public override void dispose()
    {
        _selectionDelegate.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new SelectionContainer(registrar: widget.registrar, @delegate: _selectionDelegate, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ScrollableSelectionContainerDelegate__scrollable : MultiSelectableSelectionContainerDelegate
{
    internal const double _kDefaultDragTargetSize = 0;
    internal const double _kDefaultSelectToScrollVelocityScalar = 30;
    public virtual ScrollableState state { get; private set; } = default!;
    internal virtual EdgeDraggingAutoScroller _autoScroller { get; private set; } = default!;
    internal virtual bool _scheduledLayoutChange { get; set; } = false;
    internal virtual Offset? _currentDragStartRelatedToOrigin { get; set; } = default;
    internal virtual Offset? _currentDragEndRelatedToOrigin { get; set; } = default;
    internal virtual bool _selectionStartsInScrollable { get; set; } = false;
    internal virtual ScrollPosition _position { get; set; } = default!;
    internal virtual DartMap<Selectable, double> _selectableStartEdgeUpdateRecords { get; private set; } = new DartMap<Selectable, double>();
    internal virtual DartMap<Selectable, double> _selectableEndEdgeUpdateRecords { get; private set; } = new DartMap<Selectable, double>();

    internal _ScrollableSelectionContainerDelegate__scrollable(ScrollableState state, ScrollPosition position)
    {
        this.state = state;
        _position = position;
        _autoScroller = new EdgeDraggingAutoScroller(state, velocityScalar: _kDefaultSelectToScrollVelocityScalar);
        _position.addListener(_scheduleLayoutChange);
    }

    public virtual ScrollPosition position
    {
        get => _position;
        set
        {
            var other = value;
            if (Equals(other, _position))
            {
                return;
            }
            _position.removeListener(_scheduleLayoutChange);
            _position = other;
            _position.addListener(_scheduleLayoutChange);
        }
    }
    internal virtual void _scheduleLayoutChange()
    {
        if (_scheduledLayoutChange)
        {
            return;
        }
        _scheduledLayoutChange = true;
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((timeStamp) =>
        {
            if (!_scheduledLayoutChange)
            {
                return;
            }
            _scheduledLayoutChange = false;
            layoutDidChange();
        }, debugLabel: "ScrollableSelectionContainer.layoutDidChange");
    }

    public override void didChangeSelectables()
    {
        HashSet<Selectable> selectableSet = selectables.toSet();
        _selectableStartEdgeUpdateRecords.removeWhere((key, value) => !selectableSet.Contains(key));
        _selectableEndEdgeUpdateRecords.removeWhere((key, value) => !selectableSet.Contains(key));
        base.didChangeSelectables();
    }

    public override SelectionResult handleClearSelection(ClearSelectionEvent @event)
    {
        _selectableStartEdgeUpdateRecords.Clear();
        _selectableEndEdgeUpdateRecords.Clear();
        _currentDragStartRelatedToOrigin = null;
        _currentDragEndRelatedToOrigin = null;
        _selectionStartsInScrollable = false;
        return base.handleClearSelection(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult handleSelectionEdgeUpdate(SelectionEdgeUpdateEvent @event)
    {
        if ((_currentDragEndRelatedToOrigin is null) && (_currentDragStartRelatedToOrigin is null))
        {
            DartRuntimePrimitives.Assert(() => !_selectionStartsInScrollable);
            _selectionStartsInScrollable = _globalPositionInScrollable(@event.globalPosition);
        }
        Offset deltaToOrigin = ScrollableLibrary._getDeltaToScrollOrigin(state);
        if (Equals(@event.type, SelectionEventType.endEdgeUpdate))
        {
            _currentDragEndRelatedToOrigin = _inferPositionRelatedToOrigin(@event.globalPosition);
            Offset endOffset = DartRuntimePrimitives.RequireValue(_currentDragEndRelatedToOrigin).translate(-deltaToOrigin.dx, -deltaToOrigin.dy);
            @event = SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: endOffset, granularity: @event.granularity);
        }
        else
        {
            _currentDragStartRelatedToOrigin = _inferPositionRelatedToOrigin(@event.globalPosition);
            Offset startOffset = DartRuntimePrimitives.RequireValue(_currentDragStartRelatedToOrigin).translate(-deltaToOrigin.dx, -deltaToOrigin.dy);
            @event = new SelectionEdgeUpdateEvent(globalPosition: startOffset, granularity: @event.granularity);
        }
        SelectionResult result = base.handleSelectionEdgeUpdate(@event);
        if (Equals(result, SelectionResult.pending))
        {
            _autoScroller.stopAutoScroll();
            return result;
        }
        if (_selectionStartsInScrollable)
        {
            _autoScroller.startAutoScrollIfNecessary(_dragTargetFromEvent(@event));
            if (_autoScroller.scrolling)
            {
                return SelectionResult.pending;
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Offset _inferPositionRelatedToOrigin(Offset globalPosition)
    {
        var box = ((RenderBox?)state.context.findRenderObject()!)!;
        Offset localPosition = box.globalToLocal(globalPosition);
        if (!_selectionStartsInScrollable)
        {
            if ((localPosition.dy < 0L) || (localPosition.dx < 0L))
            {
                return box.localToGlobal(Offset.zero);
            }
            if ((localPosition.dy > box.size.height) || (localPosition.dx > box.size.width))
            {
                return Offset.infinite;
            }
        }
        Offset deltaToOrigin = ScrollableLibrary._getDeltaToScrollOrigin(state);
        return box.localToGlobal(localPosition.translate(deltaToOrigin.dx, deltaToOrigin.dy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateDragLocationsFromGeometries(bool forceUpdateStart = true, bool forceUpdateEnd = true)
    {
        Offset deltaToOrigin = ScrollableLibrary._getDeltaToScrollOrigin(state);
        var box = ((RenderBox?)state.context.findRenderObject()!)!;
        Matrix4 transform = box.getTransformTo(null);
        if ((currentSelectionStartIndex != -1L) && ((_currentDragStartRelatedToOrigin is null) || forceUpdateStart))
        {
            SelectionGeometry geometry = selectables[(int)currentSelectionStartIndex].value;
            DartRuntimePrimitives.Assert(() => geometry.hasSelection);
            SelectionPoint start = geometry.startSelectionPoint!;
            Matrix4 childTransform = selectables[(int)currentSelectionStartIndex].getTransformTo(box);
            Offset localDragStart = MatrixUtils.transformPoint(childTransform, start.localPosition + new Offset(0, -start.lineHeight / 2L));
            _currentDragStartRelatedToOrigin = MatrixUtils.transformPoint(transform, localDragStart + deltaToOrigin);
        }
        if ((currentSelectionEndIndex != -1L) && ((_currentDragEndRelatedToOrigin is null) || forceUpdateEnd))
        {
            SelectionGeometry geometryLocal = selectables[(int)currentSelectionEndIndex].value;
            DartRuntimePrimitives.Assert(() => geometryLocal.hasSelection);
            SelectionPoint end = geometryLocal.endSelectionPoint!;
            Matrix4 childTransformLocal = selectables[(int)currentSelectionEndIndex].getTransformTo(box);
            Offset localDragEnd = MatrixUtils.transformPoint(childTransformLocal, end.localPosition + new Offset(0, -end.lineHeight / 2L));
            _currentDragEndRelatedToOrigin = MatrixUtils.transformPoint(transform, localDragEnd + deltaToOrigin);
        }
    }

    public override SelectionResult handleSelectAll(SelectAllSelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !_selectionStartsInScrollable);
        SelectionResult result = base.handleSelectAll(@event);
        DartRuntimePrimitives.Assert(() => currentSelectionStartIndex == -1L == (currentSelectionEndIndex == -1L));
        if (currentSelectionStartIndex != -1L)
        {
            _updateDragLocationsFromGeometries();
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult handleSelectWord(SelectWordSelectionEvent @event)
    {
        _selectionStartsInScrollable = _globalPositionInScrollable(@event.globalPosition);
        SelectionResult result = base.handleSelectWord(@event);
        _updateDragLocationsFromGeometries();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult handleGranularlyExtendSelection(GranularlyExtendSelectionEvent @event)
    {
        SelectionResult result = base.handleGranularlyExtendSelection(@event);
        _updateDragLocationsFromGeometries(forceUpdateStart: !@event.isEnd, forceUpdateEnd: @event.isEnd);
        if (_selectionStartsInScrollable)
        {
            _jumpToEdge(@event.isEnd);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult handleDirectionallyExtendSelection(DirectionallyExtendSelectionEvent @event)
    {
        SelectionResult result = base.handleDirectionallyExtendSelection(@event);
        _updateDragLocationsFromGeometries(forceUpdateStart: !@event.isEnd, forceUpdateEnd: @event.isEnd);
        if (_selectionStartsInScrollable)
        {
            _jumpToEdge(@event.isEnd);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _jumpToEdge(bool isExtent)
    {
        Selectable selectable = default!;
        double? lineHeightLocal = default!;
        SelectionPoint? edge = default!;
        if (isExtent)
        {
            selectable = selectables[(int)currentSelectionEndIndex];
            edge = selectable.value.endSelectionPoint;
            lineHeightLocal = selectable.value.endSelectionPoint!.lineHeight;
        }
        else
        {
            selectable = selectables[(int)currentSelectionStartIndex];
            edge = selectable.value.startSelectionPoint;
            lineHeightLocal = selectable.value.startSelectionPoint?.lineHeight;
        }
        if ((lineHeightLocal is null) || (edge is null))
        {
            return;
        }
        var scrollableBox = ((RenderBox?)state.context.findRenderObject()!)!;
        Matrix4 transform = selectable.getTransformTo(scrollableBox);
        Offset edgeOffsetInScrollableCoordinates = MatrixUtils.transformPoint(transform, edge.localPosition);
        var scrollableRect = Rect.fromLTRB(0, 0, scrollableBox.size.width, scrollableBox.size.height);
        switch (state.axisDirection)
        {
            case AxisDirection.up:
                {
                    double edgeBottom = edgeOffsetInScrollableCoordinates.dy;
                    double edgeTop = edgeOffsetInScrollableCoordinates.dy - DartRuntimePrimitives.RequireValue(lineHeightLocal);
                    if ((edgeBottom >= scrollableRect.bottom) && (edgeTop <= scrollableRect.top))
                    {
                        return;
                    }
                    if (edgeBottom > scrollableRect.bottom)
                    {
                        position.jumpTo(position.pixels + scrollableRect.bottom - edgeBottom);
                        return;
                    }
                    if (edgeTop < scrollableRect.top)
                    {
                        position.jumpTo(position.pixels + scrollableRect.top - edgeTop);
                    }
                    return;
                }
            case AxisDirection.right:
                {
                    double edgeLocal = edgeOffsetInScrollableCoordinates.dx;
                    if ((edgeLocal >= scrollableRect.right) && (edgeLocal <= scrollableRect.left))
                    {
                        return;
                    }
                    if (edgeLocal > scrollableRect.right)
                    {
                        position.jumpTo(position.pixels + edgeLocal - scrollableRect.right);
                        return;
                    }
                    if (edgeLocal < scrollableRect.left)
                    {
                        position.jumpTo(position.pixels + edgeLocal - scrollableRect.left);
                    }
                    return;
                }
            case AxisDirection.down:
                {
                    double edgeBottomLocal = edgeOffsetInScrollableCoordinates.dy;
                    double edgeTopLocal = edgeOffsetInScrollableCoordinates.dy - DartRuntimePrimitives.RequireValue(lineHeightLocal);
                    if ((edgeBottomLocal >= scrollableRect.bottom) && (edgeTopLocal <= scrollableRect.top))
                    {
                        return;
                    }
                    if (edgeBottomLocal > scrollableRect.bottom)
                    {
                        position.jumpTo(position.pixels + edgeBottomLocal - scrollableRect.bottom);
                        return;
                    }
                    if (edgeTopLocal < scrollableRect.top)
                    {
                        position.jumpTo(position.pixels + edgeTopLocal - scrollableRect.top);
                    }
                    return;
                }
            case AxisDirection.left:
                {
                    double edgeAlternate = edgeOffsetInScrollableCoordinates.dx;
                    if ((edgeAlternate >= scrollableRect.right) && (edgeAlternate <= scrollableRect.left))
                    {
                        return;
                    }
                    if (edgeAlternate > scrollableRect.right)
                    {
                        position.jumpTo(position.pixels + scrollableRect.right - edgeAlternate);
                        return;
                    }
                    if (edgeAlternate < scrollableRect.left)
                    {
                        position.jumpTo(position.pixels + scrollableRect.left - edgeAlternate);
                    }
                    return;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    internal virtual bool _globalPositionInScrollable(Offset globalPosition)
    {
        var box = ((RenderBox?)state.context.findRenderObject()!)!;
        Offset localPosition = box.globalToLocal(globalPosition);
        var rect = Rect.fromLTWH(0, 0, box.size.width, box.size.height);
        return rect.contains(localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Rect _dragTargetFromEvent(SelectionEdgeUpdateEvent @event)
    {
        return Rect.fromCenter(center: @event.globalPosition, width: _kDefaultDragTargetSize, height: _kDefaultDragTargetSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override SelectionResult dispatchSelectionEventToChild(Selectable selectable, SelectionEvent @event)
    {
        switch (@event.type)
        {
            case SelectionEventType.startEdgeUpdate:
                {
                    _selectableStartEdgeUpdateRecords[selectable] = state.position.pixels;
                    ensureChildUpdated(selectable);
                    break;
                }
            case SelectionEventType.endEdgeUpdate:
                {
                    _selectableEndEdgeUpdateRecords[selectable] = state.position.pixels;
                    ensureChildUpdated(selectable);
                    break;
                }
            case SelectionEventType.granularlyExtendSelection:
            case SelectionEventType.directionallyExtendSelection:
                {
                    ensureChildUpdated(selectable);
                    _selectableStartEdgeUpdateRecords[selectable] = state.position.pixels;
                    _selectableEndEdgeUpdateRecords[selectable] = state.position.pixels;
                    break;
                }
            case SelectionEventType.clear:
                {
                    _selectableEndEdgeUpdateRecords.remove(selectable);
                    _selectableStartEdgeUpdateRecords.remove(selectable);
                    break;
                }
            case SelectionEventType.selectAll:
            case SelectionEventType.selectWord:
            case SelectionEventType.selectParagraph:
                {
                    _selectableEndEdgeUpdateRecords[selectable] = state.position.pixels;
                    _selectableStartEdgeUpdateRecords[selectable] = state.position.pixels;
                    break;
                }
        }
        return base.dispatchSelectionEventToChild(selectable, @event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void ensureChildUpdated(Selectable selectable)
    {
        double newRecord = state.position.pixels;
        double? previousStartRecord = DartCollectionRuntime.NullableMapValue<double>(_selectableStartEdgeUpdateRecords, selectable);
        if ((_currentDragStartRelatedToOrigin is not null) && ((previousStartRecord is null) || ((newRecord - DartRuntimePrimitives.RequireValue(previousStartRecord)).abs() > Foundation.ConstantsLibrary.precisionErrorTolerance)))
        {
            Offset deltaToOrigin = ScrollableLibrary._getDeltaToScrollOrigin(state);
            Offset startOffset = DartRuntimePrimitives.RequireValue(_currentDragStartRelatedToOrigin).translate(-deltaToOrigin.dx, -deltaToOrigin.dy);
            selectable.dispatchSelectionEvent(new SelectionEdgeUpdateEvent(globalPosition: startOffset));
            _selectableStartEdgeUpdateRecords[selectable] = state.position.pixels;
        }
        double? previousEndRecord = DartCollectionRuntime.NullableMapValue<double>(_selectableEndEdgeUpdateRecords, selectable);
        if ((_currentDragEndRelatedToOrigin is not null) && ((previousEndRecord is null) || ((newRecord - DartRuntimePrimitives.RequireValue(previousEndRecord)).abs() > Foundation.ConstantsLibrary.precisionErrorTolerance)))
        {
            Offset deltaToOriginLocal = ScrollableLibrary._getDeltaToScrollOrigin(state);
            Offset endOffset = DartRuntimePrimitives.RequireValue(_currentDragEndRelatedToOrigin).translate(-deltaToOriginLocal.dx, -deltaToOriginLocal.dy);
            selectable.dispatchSelectionEvent(SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: endOffset));
            _selectableEndEdgeUpdateRecords[selectable] = state.position.pixels;
        }
    }

    public override void dispose()
    {
        _selectableStartEdgeUpdateRecords.Clear();
        _selectableEndEdgeUpdateRecords.Clear();
        _scheduledLayoutChange = false;
        _autoScroller.stopAutoScroll();
        base.dispose();
    }

}

public static partial class ScrollableLibrary
{
    internal static Offset _getDeltaToScrollOrigin(ScrollableState scrollableState)
    {
        return scrollableState.axisDirection switch { AxisDirection.up => new Offset(0, -scrollableState.position.pixels), AxisDirection.down => new Offset(0, scrollableState.position.pixels), AxisDirection.left => new Offset(-scrollableState.position.pixels, 0), AxisDirection.right => new Offset(scrollableState.position.pixels, 0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ScrollSemantics__scrollable : SingleChildRenderObjectWidget
{
    public virtual ScrollPosition position { get; private set; } = default!;
    public virtual bool allowImplicitScrolling { get; private set; } = default!;
    public virtual long? semanticChildCount { get; private set; }
    public virtual Axis axis { get; private set; } = default!;

    internal _ScrollSemantics__scrollable(Key? key = null, ScrollPosition position = default!, bool allowImplicitScrolling = default!, Axis axis = default!, long? semanticChildCount = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.position = position;
        this.allowImplicitScrolling = allowImplicitScrolling;
        this.axis = axis;
        this.semanticChildCount = semanticChildCount;
        System.Diagnostics.Debug.Assert((semanticChildCount is null) || (semanticChildCount >= 0L));
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderScrollSemantics__scrollable(position: position, allowImplicitScrolling: allowImplicitScrolling, semanticChildCount: semanticChildCount, axis: axis);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderScrollSemantics__scrollable)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderScrollSemantics__scrollable>)(() =>
{
    var __cascade = __renderObject;
    __cascade.allowImplicitScrolling = allowImplicitScrolling;
    __cascade.axis = axis;
    __cascade.position = position;
    __cascade.semanticChildCount = semanticChildCount;
    return __cascade;
}))());
    }

}

public class _RenderScrollSemantics__scrollable : RenderProxyBox
{
    internal virtual ScrollPosition _position { get; set; } = default!;
    internal virtual bool _allowImplicitScrolling { get; set; } = default!;
    public virtual Axis axis { get; set; } = default!;
    internal virtual long? _semanticChildCount { get; set; } = default;
    internal virtual SemanticsNode? _innerNode { get; set; } = default;

    internal _RenderScrollSemantics__scrollable(ScrollPosition position, bool allowImplicitScrolling, Axis axis, long? semanticChildCount, RenderBox? child = null) : base(child)
    {
        this.axis = axis;
        _position = position;
        _allowImplicitScrolling = allowImplicitScrolling;
        _semanticChildCount = semanticChildCount;
        _position.addListener(markNeedsSemanticsUpdate);
    }

    public virtual ScrollPosition position
    {
        get => _position;
        set
        {
            var __value = value;
            if (Equals(__value, _position))
            {
                return;
            }
            _position.removeListener(markNeedsSemanticsUpdate);
            _position = __value;
            _position.addListener(markNeedsSemanticsUpdate);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool allowImplicitScrolling
    {
        get => _allowImplicitScrolling;
        set
        {
            var __value = value;
            if (DartRuntimePrimitives.RequireValue(__value) == _allowImplicitScrolling)
            {
                return;
            }
            _allowImplicitScrolling = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual long? semanticChildCount
    {
        get => _semanticChildCount;
        set
        {
            var __value = value;
            if (__value == semanticChildCount)
            {
                return;
            }
            _semanticChildCount = __value;
            markNeedsSemanticsUpdate();
        }
    }
    internal virtual void _onScrollToOffset(Offset targetOffset)
    {
        double offset = axis switch { Axis.horizontal => targetOffset.dx, Axis.vertical => targetOffset.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        _position.jumpTo(offset);
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        DartRuntimePrimitives.Ignore(((Func<SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.isSemanticBoundary = true;
    __cascade.hasImplicitScrolling = allowImplicitScrolling;
    return __cascade;
}))());
        if (position.haveDimensions)
        {
            DartRuntimePrimitives.Ignore(((Func<SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.scrollPosition = _position.pixels;
    __cascade.scrollExtentMax = _position.maxScrollExtent;
    __cascade.scrollExtentMin = _position.minScrollExtent;
    __cascade.scrollChildCount = semanticChildCount;
    return __cascade;
}))());
            if ((position.maxScrollExtent > position.minScrollExtent) && allowImplicitScrolling)
            {
                config.onScrollToOffset = _onScrollToOffset;
            }
        }
    }

    public override void assembleSemanticsNode(SemanticsNode node, SemanticsConfiguration config, IEnumerable<SemanticsNode> children)
    {
        if (!Enumerable.Any(children) || !children.First().isTagged(RenderViewport.useTwoPaneSemantics))
        {
            _innerNode = null;
            base.assembleSemanticsNode(node, config, children.Cast<SemanticsNode>());
            return;
        }
        (_innerNode ??= new SemanticsNode(showOnScreen: () => showOnScreen())).rect = node.rect;
        long? firstVisibleIndex = default!;
        var excluded = new List<SemanticsNode> { _innerNode! };
        var included = new List<SemanticsNode>();
        foreach (var child in children)
        {
            DartRuntimePrimitives.Assert(() => child.isTagged(RenderViewport.useTwoPaneSemantics));
            if (child.isTagged(RenderViewport.excludeFromScrolling))
            {
                excluded.Add(child);
            }
            else
            {
                if (!child.flagsCollection.isHidden)
                {
                    firstVisibleIndex ??= child.indexInParent;
                }
                included.Add(child);
            }
        }
        config.scrollIndex = firstVisibleIndex;
        node.updateWith(config: null, childrenInInversePaintOrder: excluded);
        _innerNode!.updateWith(config: config, childrenInInversePaintOrder: included);
    }

    public override void clearSemantics()
    {
        base.clearSemantics();
        _innerNode = null;
    }

}

internal class _RestorableScrollOffset__scrollable : RestorableValue<double?>
{
    public override double? createDefaultValue() => null;
    public override void didUpdateValue(double? oldValue)
    {
        notifyListeners();
    }

    public override double? fromPrimitives(object? data)
    {
        return (double)data!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives()
    {
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool enabled => DartRuntimePrimitives.ConvertValue<bool>(value is not null);
}

public enum DiagonalDragBehavior
{
    none,
    weightedEvent,
    weightedContinuous,
    free
}

public class TwoDimensionalScrollable : StatefulWidget
{
    public virtual DiagonalDragBehavior diagonalDragBehavior { get; private set; } = default!;
    public virtual ScrollableDetails horizontalDetails { get; private set; } = default!;
    public virtual ScrollableDetails verticalDetails { get; private set; } = default!;
    public virtual Func<BuildContext, ViewportOffset, ViewportOffset, Widget> viewportBuilder { get; private set; } = default!;
    public virtual Func<ScrollIncrementDetails, double>? incrementCalculator { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual DragStartBehavior dragStartBehavior { get; private set; } = default!;

    public TwoDimensionalScrollable(Key? key = null, ScrollableDetails horizontalDetails = default!, ScrollableDetails verticalDetails = default!, Func<BuildContext, ViewportOffset, ViewportOffset, Widget> viewportBuilder = default!, Func<ScrollIncrementDetails, double>? incrementCalculator = null, string? restorationId = null, bool excludeFromSemantics = false, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none, DragStartBehavior dragStartBehavior = DragStartBehavior.start, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque) : base(key: key)
    {
        this.horizontalDetails = horizontalDetails;
        this.verticalDetails = verticalDetails;
        this.viewportBuilder = viewportBuilder;
        this.incrementCalculator = incrementCalculator;
        this.restorationId = restorationId;
        this.excludeFromSemantics = excludeFromSemantics;
        this.diagonalDragBehavior = diagonalDragBehavior;
        this.dragStartBehavior = dragStartBehavior;
        this.hitTestBehavior = hitTestBehavior;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new TwoDimensionalScrollableState());
    public static TwoDimensionalScrollableState? maybeOf(BuildContext context)
    {
        _TwoDimensionalScrollableScope__scrollable? widget = context.dependOnInheritedWidgetOfExactType<_TwoDimensionalScrollableScope__scrollable>();
        return widget?.twoDimensionalScrollable;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TwoDimensionalScrollableState of(BuildContext context)
    {
        TwoDimensionalScrollableState? scrollableState = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
            {
                if (scrollableState is null)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("TwoDimensionalScrollable.of() was called with a context that does " + "not contain a TwoDimensionalScrollable widget.\n"), new ErrorDescription("No TwoDimensionalScrollable widget ancestor could be found starting " + "from the context that was passed to TwoDimensionalScrollable.of(). " + "This can happen because you are using a widget that looks for a " + "TwoDimensionalScrollable ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return scrollableState!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class TwoDimensionalScrollableState : State<TwoDimensionalScrollable>
{
    internal virtual ScrollController? _verticalFallbackController { get; set; } = default;
    internal virtual ScrollController? _horizontalFallbackController { get; set; } = default;
    internal virtual GlobalKey<ScrollableState> _verticalOuterScrollableKey { get; private set; } = GlobalKey<ScrollableState>.Create();
    internal virtual GlobalKey<ScrollableState> _horizontalInnerScrollableKey { get; private set; } = GlobalKey<ScrollableState>.Create();

    public virtual ScrollableState verticalScrollable
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _verticalOuterScrollableKey.currentState is not null);
            return _verticalOuterScrollableKey.currentState!;
        }
    }
    public virtual ScrollableState horizontalScrollable
    {
        get
        {
            DartRuntimePrimitives.Assert(() => _horizontalInnerScrollableKey.currentState is not null);
            return _horizontalInnerScrollableKey.currentState!;
        }
    }
    public override void initState()
    {
        if (widget.verticalDetails.controller is null)
        {
            _verticalFallbackController = new ScrollController();
        }
        if (widget.horizontalDetails.controller is null)
        {
            _horizontalFallbackController = new ScrollController();
        }
        base.initState();
    }

    public override void didUpdateWidget(TwoDimensionalScrollable oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.verticalDetails.controller, widget.verticalDetails.controller))
        {
            if (oldWidget.verticalDetails.controller is null)
            {
                DartRuntimePrimitives.Assert(() => _verticalFallbackController is not null);
                DartRuntimePrimitives.Assert(() => widget.verticalDetails.controller is not null);
                _verticalFallbackController!.dispose();
                _verticalFallbackController = null;
            }
            else
            {
                if (widget.verticalDetails.controller is null)
                {
                    DartRuntimePrimitives.Assert(() => _verticalFallbackController is null);
                    _verticalFallbackController = new ScrollController();
                }
            }
        }
        if (!Equals(oldWidget.horizontalDetails.controller, widget.horizontalDetails.controller))
        {
            if (oldWidget.horizontalDetails.controller is null)
            {
                DartRuntimePrimitives.Assert(() => _horizontalFallbackController is not null);
                DartRuntimePrimitives.Assert(() => widget.horizontalDetails.controller is not null);
                _horizontalFallbackController!.dispose();
                _horizontalFallbackController = null;
            }
            else
            {
                if (widget.horizontalDetails.controller is null)
                {
                    DartRuntimePrimitives.Assert(() => _horizontalFallbackController is null);
                    _horizontalFallbackController = new ScrollController();
                }
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => Equals(Basic_typesLibrary.axisDirectionToAxis(widget.verticalDetails.direction), Axis.vertical), () => (object?)"TwoDimensionalScrollable.verticalDetails are not Axis.vertical.");
        DartRuntimePrimitives.Assert(() => Equals(Basic_typesLibrary.axisDirectionToAxis(widget.horizontalDetails.direction), Axis.horizontal), () => (object?)"TwoDimensionalScrollable.horizontalDetails are not Axis.horizontal.");
        Widget result = new RestorationScope(restorationId: widget.restorationId, child: new _VerticalOuterDimension__scrollable(key: _verticalOuterScrollableKey, horizontalKey: _horizontalInnerScrollableKey, axisDirection: widget.verticalDetails.direction, controller: widget.verticalDetails.controller ?? _verticalFallbackController!, physics: widget.verticalDetails.physics, clipBehavior: (widget.verticalDetails.clipBehavior ?? widget.verticalDetails.decorationClipBehavior) ?? Clip.hardEdge, incrementCalculator: widget.incrementCalculator, excludeFromSemantics: widget.excludeFromSemantics, restorationId: "OuterVerticalTwoDimensionalScrollable", dragStartBehavior: widget.dragStartBehavior, diagonalDragBehavior: widget.diagonalDragBehavior, hitTestBehavior: widget.hitTestBehavior, viewportBuilder: (context, verticalOffset) =>
        {
            return new _HorizontalInnerDimension__scrollable(key: _horizontalInnerScrollableKey, verticalOuterKey: _verticalOuterScrollableKey, axisDirection: widget.horizontalDetails.direction, controller: widget.horizontalDetails.controller ?? _horizontalFallbackController!, physics: widget.horizontalDetails.physics, clipBehavior: (widget.horizontalDetails.clipBehavior ?? widget.horizontalDetails.decorationClipBehavior) ?? Clip.hardEdge, incrementCalculator: widget.incrementCalculator, excludeFromSemantics: widget.excludeFromSemantics, restorationId: "InnerHorizontalTwoDimensionalScrollable", dragStartBehavior: widget.dragStartBehavior, diagonalDragBehavior: widget.diagonalDragBehavior, hitTestBehavior: widget.hitTestBehavior, viewportBuilder: (context, horizontalOffset) =>
            {
                return widget.viewportBuilder(context, verticalOffset, horizontalOffset);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return new _TwoDimensionalScrollableScope__scrollable(twoDimensionalScrollable: this, child: result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _verticalFallbackController?.dispose();
        _horizontalFallbackController?.dispose();
        base.dispose();
    }

}

internal class _TwoDimensionalScrollableScope__scrollable : InheritedWidget
{
    public virtual TwoDimensionalScrollableState twoDimensionalScrollable { get; private set; } = default!;

    internal _TwoDimensionalScrollableScope__scrollable(TwoDimensionalScrollableState twoDimensionalScrollable, Widget child) : base(child: child)
    {
        this.twoDimensionalScrollable = twoDimensionalScrollable;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => false;
}

internal class _VerticalOuterDimension__scrollable : Scrollable
{
    public virtual DiagonalDragBehavior diagonalDragBehavior { get; private set; } = default!;
    public virtual GlobalKey<ScrollableState> horizontalKey { get; private set; } = default!;

    internal _VerticalOuterDimension__scrollable(Key? key = null, GlobalKey<ScrollableState> horizontalKey = default!, Func<BuildContext, ViewportOffset, Widget> viewportBuilder = default!, AxisDirection axisDirection = default!, ScrollController? controller = null, ScrollPhysics? physics = null, Clip clipBehavior = Clip.hardEdge, Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, DragStartBehavior dragStartBehavior = DragStartBehavior.start, string? restorationId = null, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none) : base(key: key, viewportBuilder: viewportBuilder, axisDirection: axisDirection, controller: controller, physics: physics, clipBehavior: clipBehavior, incrementCalculator: incrementCalculator, excludeFromSemantics: excludeFromSemantics, dragStartBehavior: dragStartBehavior, restorationId: restorationId, hitTestBehavior: hitTestBehavior)
    {
        this.horizontalKey = horizontalKey;
        this.diagonalDragBehavior = diagonalDragBehavior;
        System.Diagnostics.Debug.Assert(Equals(axisDirection, AxisDirection.up) || Equals(axisDirection, AxisDirection.down));
    }

    public override _VerticalOuterDimensionState__scrollable createState() => new _VerticalOuterDimensionState__scrollable();
}

internal class _VerticalOuterDimensionState__scrollable : ScrollableState
{
    public virtual Axis? lockedAxis { get; set; } = default;
    public virtual Offset? lastDragOffset { get; set; } = default;

    public virtual DiagonalDragBehavior diagonalDragBehavior => ((_VerticalOuterDimension__scrollable?)widget)!.diagonalDragBehavior;
    public virtual ScrollableState horizontalScrollable => DartRuntimePrimitives.ConvertValue<ScrollableState>(((_VerticalOuterDimension__scrollable?)widget)!.horizontalKey.currentState!);
    internal override (List<Future>, ScrollableState) _performEnsureVisible(RenderObject @object, double alignment = 0.0, Duration duration = default, Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, RenderObject? targetRenderObject = null)
    {
        DartRuntimePrimitives.Assert(() => false, () => (object?)"The _performEnsureVisible method was called for the vertical scrollable " + "of a TwoDimensionalScrollable. This should not happen as the horizontal " + "scrollable handles both axes.");
        return (new List<Future>(), this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _evaluateLockedAxis(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => lastDragOffset is not null);
        Offset offsetDelta = DartRuntimePrimitives.RequireValue(lastDragOffset) - offset;
        double axisDifferential = offsetDelta.dx.abs() - offsetDelta.dy.abs();
        if (axisDifferential.abs() >= Gestures.ConstantsLibrary.kTouchSlop)
        {
            lockedAxis = (axisDifferential > 0.0) ? Axis.horizontal : Axis.vertical;
        }
        else
        {
            lockedAxis = null;
        }
    }

    internal override void _handleDragDown(DragDownDetails details)
    {
        switch (diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
            case DiagonalDragBehavior.free:
                {
                    horizontalScrollable._handleDragDown(details);
                    break;
                }
        }
        base._handleDragDown(details);
    }

    internal override void _handleDragStart(DragStartDetails details)
    {
        lastDragOffset = details.globalPosition;
        switch (diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    break;
                }
            case DiagonalDragBehavior.free:
                {
                    horizontalScrollable._handleDragStart(details);
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
                {
                    _evaluateLockedAxis(details.globalPosition);
                    switch (lockedAxis)
                    {
                        case null:
                            {
                                horizontalScrollable._handleDragStart(details);
                                break;
                            }
                        case Axis.horizontal:
                            {
                                horizontalScrollable._handleDragStart(details);
                                return;
                            }
                        case Axis.vertical:
                            break;
                    }
                    break;
                }
        }
        base._handleDragStart(details);
    }

    internal override void _handleDragUpdate(DragUpdateDetails details)
    {
        var verticalDragDetails = new DragUpdateDetails(sourceTimeStamp: details.sourceTimeStamp, delta: new Offset(0.0, details.delta.dy), primaryDelta: details.delta.dy, globalPosition: details.globalPosition, localPosition: details.localPosition);
        var horizontalDragDetails = new DragUpdateDetails(sourceTimeStamp: details.sourceTimeStamp, delta: new Offset(details.delta.dx, 0.0), primaryDelta: details.delta.dx, globalPosition: details.globalPosition, localPosition: details.localPosition);
        switch (diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    base._handleDragUpdate(verticalDragDetails);
                    return;
                }
            case DiagonalDragBehavior.free:
                {
                    horizontalScrollable._handleDragUpdate(horizontalDragDetails);
                    base._handleDragUpdate(verticalDragDetails);
                    return;
                }
            case DiagonalDragBehavior.weightedContinuous:
                {
                    _evaluateLockedAxis(details.globalPosition);
                    lastDragOffset = details.globalPosition;
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
                {
                    if ((lockedAxis is null) && (lastDragOffset is not null))
                    {
                        _evaluateLockedAxis(details.globalPosition);
                    }
                    break;
                }
        }
        switch (lockedAxis)
        {
            case null:
                {
                    horizontalScrollable._handleDragUpdate(horizontalDragDetails);
                    break;
                }
            case Axis.horizontal:
                {
                    horizontalScrollable._handleDragUpdate(horizontalDragDetails);
                    return;
                }
            case Axis.vertical:
                break;
        }
        base._handleDragUpdate(verticalDragDetails);
    }

    internal override void _handleDragEnd(DragEndDetails details)
    {
        lastDragOffset = null;
        lockedAxis = null;
        double dxLocal = details.velocity.pixelsPerSecond.dx;
        double dyLocal = details.velocity.pixelsPerSecond.dy;
        var verticalDragDetails = new DragEndDetails(velocity: new Velocity(pixelsPerSecond: new Offset(0.0, dyLocal)), primaryVelocity: dyLocal);
        var horizontalDragDetails = new DragEndDetails(velocity: new Velocity(pixelsPerSecond: new Offset(dxLocal, 0.0)), primaryVelocity: dxLocal);
        switch (diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
            case DiagonalDragBehavior.free:
                {
                    horizontalScrollable._handleDragEnd(horizontalDragDetails);
                    break;
                }
        }
        base._handleDragEnd(verticalDragDetails);
    }

    internal override void _handleDragCancel()
    {
        lastDragOffset = null;
        lockedAxis = null;
        switch (diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
            case DiagonalDragBehavior.free:
                {
                    horizontalScrollable._handleDragCancel();
                    break;
                }
        }
        base._handleDragCancel();
    }

    public override void setCanDrag(bool value)
    {
        switch (diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    base.setCanDrag(value);
                    return;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
            case DiagonalDragBehavior.free:
                {
                    if (value)
                    {
                        _gestureRecognizers = new DartMap<Type, dynamic>
                        {
                            [typeof(PanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<PanGestureRecognizer>(() => new PanGestureRecognizer(supportedDevices: _configuration.dragDevices), (instance) =>
                            {
                                DartRuntimePrimitives.Ignore(((Func<PanGestureRecognizer>)(() =>
                                {
                                    var __cascade = instance;
                                    __cascade.onDown = _handleDragDown;
                                    __cascade.onStart = _handleDragStart;
                                    __cascade.onUpdate = _handleDragUpdate;
                                    __cascade.onEnd = _handleDragEnd;
                                    __cascade.onCancel = _handleDragCancel;
                                    __cascade.minFlingDistance = _physics?.minFlingDistance;
                                    __cascade.minFlingVelocity = _physics?.minFlingVelocity;
                                    __cascade.maxFlingVelocity = _physics?.maxFlingVelocity;
                                    __cascade.velocityTrackerBuilder = _configuration.velocityTrackerBuilder(context);
                                    __cascade.dragStartBehavior = widget.dragStartBehavior;
                                    __cascade.gestureSettings = _mediaQueryGestureSettings;
                                    return __cascade;
                                }))());
                            })
                        };
                        _handleDragCancel();
                        _lastCanDrag = value;
                        _lastAxisDirection = widget.axis;
                        if (_gestureDetectorKey.currentState is not null)
                        {
                            _gestureDetectorKey.currentState!.replaceGestureRecognizers(_gestureRecognizers);
                        }
                    }
                    return;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    internal override Widget _buildChrome(BuildContext context, Widget child)
    {
        var details = new ScrollableDetails(direction: widget.axisDirection, controller: _effectiveScrollController, clipBehavior: widget.clipBehavior);
        return _configuration.buildOverscrollIndicator(context, child, details);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HorizontalInnerDimension__scrollable : Scrollable
{
    public virtual GlobalKey<ScrollableState> verticalOuterKey { get; private set; } = default!;
    public virtual DiagonalDragBehavior diagonalDragBehavior { get; private set; } = default!;

    internal _HorizontalInnerDimension__scrollable(Key? key = null, GlobalKey<ScrollableState> verticalOuterKey = default!, Func<BuildContext, ViewportOffset, Widget> viewportBuilder = default!, AxisDirection axisDirection = default!, ScrollController? controller = null, ScrollPhysics? physics = null, Clip clipBehavior = Clip.hardEdge, Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, DragStartBehavior dragStartBehavior = DragStartBehavior.start, string? restorationId = null, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none) : base(key: key, viewportBuilder: viewportBuilder, axisDirection: axisDirection, controller: controller, physics: physics, clipBehavior: clipBehavior, incrementCalculator: incrementCalculator, excludeFromSemantics: excludeFromSemantics, dragStartBehavior: dragStartBehavior, restorationId: restorationId, hitTestBehavior: hitTestBehavior)
    {
        this.verticalOuterKey = verticalOuterKey;
        this.diagonalDragBehavior = diagonalDragBehavior;
        System.Diagnostics.Debug.Assert(Equals(axisDirection, AxisDirection.left) || Equals(axisDirection, AxisDirection.right));
    }

    public override _HorizontalInnerDimensionState__scrollable createState() => new _HorizontalInnerDimensionState__scrollable();
}

internal class _HorizontalInnerDimensionState__scrollable : ScrollableState
{
    public virtual ScrollableState verticalScrollable { get; set; } = default!;

    public virtual GlobalKey<ScrollableState> verticalOuterKey => ((_HorizontalInnerDimension__scrollable?)widget)!.verticalOuterKey;
    public virtual DiagonalDragBehavior diagonalDragBehavior => ((_HorizontalInnerDimension__scrollable?)widget)!.diagonalDragBehavior;
    public override void didChangeDependencies()
    {
        verticalScrollable = Scrollable.of(context);
        DartRuntimePrimitives.Assert(() => Equals(Basic_typesLibrary.axisDirectionToAxis(verticalScrollable.axisDirection), Axis.vertical));
        base.didChangeDependencies();
    }

    internal override (List<Future>, ScrollableState) _performEnsureVisible(RenderObject @object, double alignment = 0.0, Duration duration = default, Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, RenderObject? targetRenderObject = null)
    {
        var newFutures = new List<Future> { position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy), verticalScrollable.position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy) };
        return (newFutures, verticalScrollable);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setCanDrag(bool value)
    {
        switch (diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    base.setCanDrag(value);
                    return;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
            case DiagonalDragBehavior.free:
                {
                    if (value)
                    {
                        _gestureRecognizers = new DartMap<Type, dynamic>();
                        verticalOuterKey.currentState!.setCanDrag(value);
                        _handleDragCancel();
                        _lastCanDrag = value;
                        _lastAxisDirection = widget.axis;
                        if (_gestureDetectorKey.currentState is not null)
                        {
                            _gestureDetectorKey.currentState!.replaceGestureRecognizers(_gestureRecognizers);
                        }
                    }
                    return;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    internal override Widget _buildChrome(BuildContext context, Widget child)
    {
        var details = new ScrollableDetails(direction: widget.axisDirection, controller: _effectiveScrollController, clipBehavior: widget.clipBehavior);
        return _configuration.buildOverscrollIndicator(context, child, details);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
