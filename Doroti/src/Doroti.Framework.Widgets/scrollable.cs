// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scrollable.dart
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

public delegate Widget ViewportBuilder(BuildContext context, global::Doroti.Framework.Rendering.ViewportOffset position);

public delegate Widget TwoDimensionalViewportBuilder(BuildContext context, global::Doroti.Framework.Rendering.ViewportOffset verticalPosition, global::Doroti.Framework.Rendering.ViewportOffset horizontalPosition);

internal delegate void _EnsureVisibleResults__scrollable();

public class Scrollable : StatefulWidget
{
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget> viewportBuilder { get; private set; } = default!;
    public virtual global::System.Func<ScrollIncrementDetails, double>? incrementCalculator { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual long? semanticChildCount { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Scrollable(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = global::Doroti.Framework.Painting.AxisDirection.down, ScrollController? controller = null, ScrollPhysics? physics = null, global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::System.Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, string? restorationId = null, ScrollBehavior? scrollBehavior = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque) : base(key: key)
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
        System.Diagnostics.Debug.Assert(((semanticChildCount is null) || (semanticChildCount >= 0L)));
    }

    public virtual global::Doroti.Framework.Painting.Axis axis => global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this.axisDirection);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new ScrollableState());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("physics", this.physics));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("restorationId", this.restorationId));
    }

    public static ScrollableState? maybeOf(BuildContext context, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        var originalContext = context;
        InheritedElement? element = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<_ScrollableScope__scrollable>());
        while ((element is not null))
        {
            ScrollableState scrollableLocal = (((_ScrollableScope__scrollable?)(object?)element.widget)!).scrollable;
            if (((axis is null) || (object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableState)scrollableLocal).axisDirection), DartRuntimePrimitives.RequireValue(axis)))))
            {
                originalContext.dependOnInheritedElement(element);
                return scrollableLocal;
            }
            context = scrollableLocal.context;
            element = context.getElementForInheritedWidgetOfExactType<_ScrollableScope__scrollable>();
        }
        return ((ScrollableState)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScrollableState of(BuildContext context, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        ScrollableState? scrollableState = ((ScrollableState?)(object?)Scrollable.maybeOf(context, axis: axis));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((scrollableState is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Scrollable.of() was called with a context that does not contain a " + "Scrollable widget."), new global::Doroti.Framework.Foundation.ErrorDescription("No Scrollable widget ancestor could be found " + $"{((axis is null) ? "" : $"for the provided Axis: {DartRuntimePrimitives.RequireValue(axis)} ")}" + "starting from the context that was passed to Scrollable.of(). This " + "can happen because you are using a widget that looks for a Scrollable " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return scrollableState!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool recommendDeferredLoadingForContext(BuildContext context, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        _ScrollableScope__scrollable? widget = ((_ScrollableScope__scrollable?)(object?)context.getInheritedWidgetOfExactType<_ScrollableScope__scrollable>());
        while ((widget is not null))
        {
            if (((axis is null) || (object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((_ScrollableScope__scrollable)widget).scrollable.axisDirection), DartRuntimePrimitives.RequireValue(axis)))))
            {
                return ((_ScrollableScope__scrollable)widget).position.recommendDeferredLoading(context);
            }
            context = ((_ScrollableScope__scrollable)widget).scrollable.context;
            widget = context.getInheritedWidgetOfExactType<_ScrollableScope__scrollable>();
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future ensureVisible(BuildContext context, double alignment = 0.0, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit)
    {
        var futures = new List<Future>();
        global::Doroti.Framework.Rendering.RenderObject? targetRenderObjectLocal = default!;
        ScrollableState? scrollable = ((ScrollableState?)(object?)Scrollable.maybeOf(context));
        while ((scrollable is not null))
        {
            List<Future> newFutures = default!;
            DartRuntimePrimitives.Ignore((newFutures, scrollable) = scrollable._performEnsureVisible(context.findRenderObject()!, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy, targetRenderObject: targetRenderObjectLocal));
            futures.AddRange(newFutures.Cast<Future>());
            targetRenderObjectLocal ??= context.findRenderObject();
            context = scrollable.context;
            scrollable = Scrollable.maybeOf(context);
        }
        if ((!System.Linq.Enumerable.Any(futures) || (object.Equals(duration, Duration.zero))))
        {
            return Future.value();
        }
        if ((checked((long)(futures.Count)) == 1L))
        {
            return futures.Single();
        }
        return global::Doroti.Runtime.DartAsyncRuntime.wait<object?>(futures).then((global::System.Action<List<object?>>)((_) => { }));
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
        var __old = (_ScrollableScope__scrollable)(object)oldWidget;
        return (!object.Equals(this.position, ((_ScrollableScope__scrollable)__old).position));
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
    internal virtual global::Doroti.Framework.Gestures.DeviceGestureSettings? _mediaQueryGestureSettings { get; set; } = default;
    internal virtual GlobalKey<IState> _scrollSemanticsKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual GlobalKey<RawGestureDetectorState> _gestureDetectorKey { get; private set; } = GlobalKey<RawGestureDetectorState>.Create();
    internal virtual GlobalKey<IState> _ignorePointerKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual DartMap<Type, dynamic> _gestureRecognizers { get; set; } = new DartMap<Type, dynamic>();
    internal virtual bool _shouldIgnorePointer { get; set; } = false;
    internal virtual bool? _lastCanDrag { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.Axis? _lastAxisDirection { get; set; } = default;
    internal virtual global::Doroti.Framework.Gestures.Drag? _drag { get; set; } = default;
    internal virtual ScrollHoldController? _hold { get; set; } = default;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual ScrollPosition position => DartRuntimePrimitives.ConvertValue<ScrollPosition>(this._position!);
    public virtual ScrollPhysics? resolvedPhysics => this._physics;
    public virtual global::Doroti.Ui.Offset deltaToScrollOrigin => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>((this.axisDirection switch { global::Doroti.Framework.Painting.AxisDirection.up => new global::Doroti.Ui.Offset(0, -((ScrollPosition)this.position).pixels), global::Doroti.Framework.Painting.AxisDirection.down => new global::Doroti.Ui.Offset(0, ((ScrollPosition)this.position).pixels), global::Doroti.Framework.Painting.AxisDirection.left => new global::Doroti.Ui.Offset(-((ScrollPosition)this.position).pixels, 0), global::Doroti.Framework.Painting.AxisDirection.right => new global::Doroti.Ui.Offset(((ScrollPosition)this.position).pixels, 0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    internal virtual ScrollController _effectiveScrollController => DartRuntimePrimitives.ConvertValue<ScrollController>((((Scrollable)this.widget).controller ?? this._fallbackScrollController!));
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection => ((Scrollable)this.widget).axisDirection;
    public virtual global::Doroti.Framework.Scheduler.TickerProvider vsync => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Scheduler.TickerProvider>(this);
    public virtual double devicePixelRatio => this._devicePixelRatio;
    public virtual BuildContext? notificationContext => ((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentContext;
    public virtual BuildContext storageContext => this.context;
    public virtual string? restorationId => ((Scrollable)this.widget).restorationId;
    internal virtual void _updatePosition()
    {
        _configuration = ((((Scrollable)this.widget).scrollBehavior ?? (ScrollBehavior)ScrollConfiguration.of(this.context)));
        ScrollPhysics? physicsFromWidget = ((((Scrollable)this.widget).physics ?? (ScrollPhysics)((Scrollable)this.widget).scrollBehavior?.getScrollPhysics(this.context)));
        _physics = this._configuration.getScrollPhysics(this.context);
        _physics = (physicsFromWidget?.applyTo(this._physics) ?? this._physics);
        ScrollPosition? oldPosition = this._position;
        if ((oldPosition is not null))
        {
            this._effectiveScrollController.detach(oldPosition);
            DartAsyncRuntime.scheduleMicrotask(((ScrollPosition)oldPosition).dispose);
        }
        _position = this._effectiveScrollController.createScrollPosition(this._physics!, this, oldPosition);
        DartRuntimePrimitives.Assert(() => (this._position is not null));
        this._effectiveScrollController.attach(this.position);
    }

    public virtual void restoreState(global::Doroti.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
    {
        registerForRestoration(this._persistedScrollOffset, "offset");
        DartRuntimePrimitives.Assert(() => (this._position is not null));
        if ((this._persistedScrollOffset.value is not null))
        {
            this.position.restoreOffset(DartRuntimePrimitives.RequireValue(this._persistedScrollOffset.value), initialRestore: initialRestore);
        }
    }

    public virtual void saveOffset(double offset)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(offset));
        this._persistedScrollOffset.value = offset;
        global::Doroti.Framework.Services.ServicesBinding.instance.restorationManager.flushData();
    }

    public override void initState()
    {
        if ((((Scrollable)this.widget).controller is null))
        {
            _fallbackScrollController = new ScrollController();
        }
        base.initState();
    }

    public override void didChangeDependencies()
    {
        _mediaQueryGestureSettings = MediaQuery.maybeGestureSettingsOf(this.context);
        _devicePixelRatio = (MediaQuery.maybeDevicePixelRatioOf(this.context) ?? View.of(this.context).devicePixelRatio);
        _updatePosition();
        base.didChangeDependencies();
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        bool needsRestore = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore);
        if (needsRestore)
        {
            _doRestore(oldBucket);
        }
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            oldBucket?.dispose();
        }
    }

    internal virtual bool _shouldUpdatePosition(Scrollable oldWidget)
    {
        if ((((((Scrollable)this.widget).scrollBehavior is null)) != ((((Scrollable)oldWidget).scrollBehavior is null))))
        {
            return true;
        }
        if ((((((Scrollable)this.widget).scrollBehavior is not null) && (((Scrollable)oldWidget).scrollBehavior is not null)) && ((Scrollable)this.widget).scrollBehavior!.shouldNotify(((Scrollable)oldWidget).scrollBehavior!)))
        {
            return true;
        }
        ScrollPhysics? newPhysics = ((((Scrollable)this.widget).physics ?? (ScrollPhysics)((Scrollable)this.widget).scrollBehavior?.getScrollPhysics(this.context)));
        ScrollPhysics? oldPhysics = ((((Scrollable)oldWidget).physics ?? (ScrollPhysics)((Scrollable)oldWidget).scrollBehavior?.getScrollPhysics(this.context)));
        do
        {
            if ((!object.Equals(DartRuntimePrimitives.RuntimeType(newPhysics), DartRuntimePrimitives.RuntimeType(oldPhysics))))
            {
                return true;
            }
            newPhysics = newPhysics?.parent;
            oldPhysics = oldPhysics?.parent;
        }
        while (((newPhysics is not null) || (oldPhysics is not null)));
        return (!object.Equals(DartRuntimePrimitives.RuntimeType(((Scrollable)this.widget).controller), DartRuntimePrimitives.RuntimeType(((Scrollable)oldWidget).controller)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(Scrollable oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        didUpdateRestorationId();
        if ((!object.Equals(((Scrollable)this.widget).controller, ((Scrollable)oldWidget).controller)))
        {
            if ((((Scrollable)oldWidget).controller is null))
            {
                DartRuntimePrimitives.Assert(() => (this._fallbackScrollController is not null));
                DartRuntimePrimitives.Assert(() => (((Scrollable)this.widget).controller is not null));
                this._fallbackScrollController!.detach(this.position);
                this._fallbackScrollController!.dispose();
                _fallbackScrollController = null;
            }
            else
            {
                ((Scrollable)oldWidget).controller?.detach(this.position);
                if ((((Scrollable)this.widget).controller is null))
                {
                    _fallbackScrollController = new ScrollController();
                }
            }
            this._effectiveScrollController.attach(this.position);
        }
        if (_shouldUpdatePosition(oldWidget))
        {
            _updatePosition();
        }
    }

    public override void dispose()
    {
        if ((((Scrollable)this.widget).controller is not null))
        {
            ((Scrollable)this.widget).controller!.detach(this.position);
        }
        else
        {
            this._fallbackScrollController?.detach(this.position);
            this._fallbackScrollController?.dispose();
        }
        this.position.dispose();
        this._persistedScrollOffset.dispose();
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) =>
        {
            if (!((dynamic)property)._disposed)
            {
                property.removeListener(listener);
            }
        })));
        this._bucket?.dispose();
        _bucket = null;
        base.dispose();
    }

    public virtual void setSemanticsActions(HashSet<SemanticsAction> actions)
    {
        if ((((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentState is not null))
        {
            ((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentState!.replaceSemanticsActions(actions);
        }
    }

    public virtual void setCanDrag(bool value)
    {
        if (((value == this._lastCanDrag) && ((!value || (object.Equals(((Scrollable)this.widget).axis, this._lastAxisDirection))))))
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
            switch (((Scrollable)this.widget).axis)
            {
                case global::Doroti.Framework.Painting.Axis.vertical:
                    {
                        _gestureRecognizers = new DartMap<Type, dynamic>
                        {
                            [typeof(global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer(supportedDevices: ((ScrollBehavior)this._configuration).dragDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)((instance) =>
                            {
                                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.VerticalDragGestureRecognizer>)(() =>
                                {
                                    var __cascade = instance;
                                    __cascade.onDown = this._handleDragDown;
                                    __cascade.onStart = this._handleDragStart;
                                    __cascade.onUpdate = this._handleDragUpdate;
                                    __cascade.onEnd = this._handleDragEnd;
                                    __cascade.onCancel = this._handleDragCancel;
                                    __cascade.minFlingDistance = this._physics?.minFlingDistance;
                                    __cascade.minFlingVelocity = this._physics?.minFlingVelocity;
                                    __cascade.maxFlingVelocity = this._physics?.maxFlingVelocity;
                                    __cascade.velocityTrackerBuilder = this._configuration.velocityTrackerBuilder(this.context);
                                    __cascade.dragStartBehavior = ((Scrollable)this.widget).dragStartBehavior;
                                    __cascade.multitouchDragStrategy = this._configuration.getMultitouchDragStrategy(this.context);
                                    __cascade.gestureSettings = this._mediaQueryGestureSettings;
                                    __cascade.supportedDevices = ((ScrollBehavior)this._configuration).dragDevices;
                                    return __cascade;
                                }))());
                            })))
                        };
                        break;
                    }
                case global::Doroti.Framework.Painting.Axis.horizontal:
                    {
                        _gestureRecognizers = new DartMap<Type, dynamic>
                        {
                            [typeof(global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer(supportedDevices: ((ScrollBehavior)this._configuration).dragDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)((instance) =>
                            {
                                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
                                {
                                    var __cascade = instance;
                                    __cascade.onDown = this._handleDragDown;
                                    __cascade.onStart = this._handleDragStart;
                                    __cascade.onUpdate = this._handleDragUpdate;
                                    __cascade.onEnd = this._handleDragEnd;
                                    __cascade.onCancel = this._handleDragCancel;
                                    __cascade.minFlingDistance = this._physics?.minFlingDistance;
                                    __cascade.minFlingVelocity = this._physics?.minFlingVelocity;
                                    __cascade.maxFlingVelocity = this._physics?.maxFlingVelocity;
                                    __cascade.velocityTrackerBuilder = this._configuration.velocityTrackerBuilder(this.context);
                                    __cascade.dragStartBehavior = ((Scrollable)this.widget).dragStartBehavior;
                                    __cascade.multitouchDragStrategy = this._configuration.getMultitouchDragStrategy(this.context);
                                    __cascade.gestureSettings = this._mediaQueryGestureSettings;
                                    __cascade.supportedDevices = ((ScrollBehavior)this._configuration).dragDevices;
                                    return __cascade;
                                }))());
                            })))
                        };
                        break;
                    }
            }
        }
        _lastCanDrag = value;
        _lastAxisDirection = ((Scrollable)this.widget).axis;
        if ((((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentState is not null))
        {
            ((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentState!.replaceGestureRecognizers(this._gestureRecognizers);
        }
    }

    public virtual void setIgnorePointer(bool value)
    {
        if ((this._shouldIgnorePointer == value))
        {
            return;
        }
        _shouldIgnorePointer = value;
        if ((((GlobalKey<IState>)this._ignorePointerKey).currentContext is not null))
        {
            var renderBox = ((global::Doroti.Framework.Rendering.RenderIgnorePointer?)(object?)((GlobalKey<IState>)this._ignorePointerKey).currentContext!.findRenderObject()!)!;
            renderBox.ignoring = this._shouldIgnorePointer;
        }
    }

    internal virtual void _handleDragDown(global::Doroti.Framework.Gestures.DragDownDetails details)
    {
        DartRuntimePrimitives.Assert(() => (this._drag is null));
        DartRuntimePrimitives.Assert(() => (this._hold is null));
        _hold = this.position.hold(() => this._disposeHold());
    }

    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => (this._drag is null));
        _drag = this.position.drag(details, () => this._disposeDrag());
        DartRuntimePrimitives.Assert(() => (this._drag is not null));
        if ((this._hold is not null))
        {
            _disposeHold();
        }
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => ((this._hold is null) || (this._drag is null)));
        this._drag?.update(details);
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => ((this._hold is null) || (this._drag is null)));
        this._drag?.end(details);
        DartRuntimePrimitives.Assert(() => (this._drag is null));
    }

    internal virtual void _handleDragCancel()
    {
        if ((((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentContext is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => ((this._hold is null) || (this._drag is null)));
        this._hold?.cancel();
        this._drag?.cancel();
        DartRuntimePrimitives.Assert(() => (this._hold is null));
        DartRuntimePrimitives.Assert(() => (this._drag is null));
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
        return Math.Min(Math.Max((((ScrollPosition)this.position).pixels + delta), ((ScrollPosition)this.position).minScrollExtent), ((ScrollPosition)this.position).maxScrollExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _pointerSignalEventDelta(global::Doroti.Framework.Gestures.PointerScrollEvent @event)
    {
        HashSet<global::Doroti.Framework.Services.LogicalKeyboardKey> pressed = global::Doroti.Framework.Services.HardwareKeyboard.instance.logicalKeysPressed;
        bool flipAxes = (pressed.any(__item => ((ScrollBehavior)this._configuration).pointerAxisModifiers.Contains(__item)) && (object.Equals(@event.kind, PointerDeviceKind.mouse)));
        global::Doroti.Framework.Painting.Axis axisLocal = (flipAxes ? global::Doroti.Framework.Painting.Basic_typesLibrary.flipAxis(((Scrollable)this.widget).axis) : ((Scrollable)this.widget).axis);
        double delta = (axisLocal switch { global::Doroti.Framework.Painting.Axis.horizontal => ((global::Doroti.Framework.Gestures.PointerScrollEvent)@event).scrollDelta.dx, global::Doroti.Framework.Painting.Axis.vertical => ((global::Doroti.Framework.Gestures.PointerScrollEvent)@event).scrollDelta.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((Scrollable)this.widget).axisDirection) ? -delta : delta);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _receivedPointerSignal(global::Doroti.Framework.Gestures.PointerSignalEvent @event)
    {
        if (((@event is global::Doroti.Framework.Gestures.PointerScrollEvent) && (this._position is not null)))
        {
            global::Doroti.Framework.Gestures.PointerScrollEvent @event__as37801 = (global::Doroti.Framework.Gestures.PointerScrollEvent)@event;
            if (((this._physics is not null) && !this._physics!.shouldAcceptUserOffset(this.position)))
            {
                return;
            }
            double delta = _pointerSignalEventDelta(((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as37801));
            double targetScrollOffset = _targetScrollOffsetForPointerScroll(delta);
            if (((delta != 0.0) && (targetScrollOffset != ((ScrollPosition)this.position).pixels)))
            {
                global::Doroti.Framework.Gestures.GestureBinding.instance.pointerSignalResolver.register(((global::Doroti.Framework.Gestures.PointerScrollEvent)@event__as37801), (__arg0) => ((global::System.Action<global::Doroti.Framework.Gestures.PointerEvent>)this._handlePointerScroll)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Gestures.PointerEvent>(__arg0)));
                return;
            }
        }
        else
        {
            if ((@event is global::Doroti.Framework.Gestures.PointerScrollInertiaCancelEvent))
            {
                global::Doroti.Framework.Gestures.PointerScrollInertiaCancelEvent @event__as38382 = (global::Doroti.Framework.Gestures.PointerScrollInertiaCancelEvent)@event;
                this.position.pointerScroll(0);
            }
        }
    }

    internal virtual void _handlePointerScroll(global::Doroti.Framework.Gestures.PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (@event is global::Doroti.Framework.Gestures.PointerScrollEvent));
        var scrollEvent = ((global::Doroti.Framework.Gestures.PointerScrollEvent?)(object?)@event)!;
        double delta = _pointerSignalEventDelta(scrollEvent);
        double targetScrollOffset = _targetScrollOffsetForPointerScroll(delta);
        if (((delta != 0.0) && (targetScrollOffset != ((ScrollPosition)this.position).pixels)))
        {
            this.position.pointerScroll(delta);
            scrollEvent.respond(allowPlatformDefault: false);
        }
    }

    internal virtual bool _handleScrollMetricsNotification(ScrollMetricsNotification notification)
    {
        if ((notification.depth == 0L))
        {
            global::Doroti.Framework.Rendering.RenderObject? scrollSemanticsRenderObject = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)((GlobalKey<IState>)this._scrollSemanticsKey).currentContext?.findRenderObject());
            if ((scrollSemanticsRenderObject is not null))
            {
                ((dynamic)scrollSemanticsRenderObject).markNeedsSemanticsUpdate();
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildChrome(BuildContext context, Widget child)
    {
        var details = new ScrollableDetails(direction: ((Scrollable)this.widget).axisDirection, controller: this._effectiveScrollController, decorationClipBehavior: ((Scrollable)this.widget).clipBehavior);
        return ((Widget)(object?)this._configuration.buildScrollbar(context, this._configuration.buildOverscrollIndicator(context, child, details), details));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (this._position is not null));
        Widget result = ((Widget)(object?)new _ScrollableScope__scrollable(scrollable: this, position: this.position, child: new Listener(onPointerSignal: (global::System.Action<global::Doroti.Framework.Gestures.PointerSignalEvent>)this._receivedPointerSignal, child: new RawGestureDetector(key: this._gestureDetectorKey, gestures: this._gestureRecognizers, behavior: ((Scrollable)this.widget).hitTestBehavior, excludeFromSemantics: ((Scrollable)this.widget).excludeFromSemantics, child: new Semantics(explicitChildNodes: !((Scrollable)this.widget).excludeFromSemantics, child: new IgnorePointer(key: this._ignorePointerKey, ignoring: this._shouldIgnorePointer, child: this.widget.viewportBuilder(context, this.position)))))));
        if (!((Scrollable)this.widget).excludeFromSemantics)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new NotificationListener<ScrollMetricsNotification>(onNotification: (global::System.Func<ScrollMetricsNotification, bool>)this._handleScrollMetricsNotification, child: new _ScrollSemantics__scrollable(key: this._scrollSemanticsKey, position: this.position, allowImplicitScrolling: this._physics!.allowImplicitScrolling, axis: ((Scrollable)this.widget).axis, semanticChildCount: ((Scrollable)this.widget).semanticChildCount, child: result)));
        }
        result = _buildChrome(context, result);
        global::Doroti.Framework.Rendering.SelectionRegistrar? registrarLocal = ((global::Doroti.Framework.Rendering.SelectionRegistrar?)(object?)SelectionContainer.maybeOf(context));
        if ((registrarLocal is not null))
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new _ScrollableSelectionHandler__scrollable(state: this, position: this.position, registrar: registrarLocal, child: result));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (List<Future>, ScrollableState) _performEnsureVisible(global::Doroti.Framework.Rendering.RenderObject @object, double alignment = 0.0, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, global::Doroti.Framework.Rendering.RenderObject? targetRenderObject = null)
    {
        Future ensureVisibleFuture = ((Future)(object?)this.position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy, targetRenderObject: targetRenderObject));
        return (new List<Future> { ensureVisibleFuture }, this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollPosition>("position", this._position));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("effective physics", this._physics));
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public virtual global::Doroti.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue = (hasSerializedValue ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
        if (!((dynamic)property).isRegistered)
        {
            property._register(restorationId, this);
            void listener()
            {
                if ((this.bucket is null))
                {
                    return;
                }
                _updateProperty(property);
            }
            property.addListener((global::System.Action)listener);
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue);
        if (((!hasSerializedValue && ((dynamic)property).enabled) && (this.bucket is not null)))
        {
            _updateProperty(property);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual void unregisterFromRestoration(dynamic property)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((dynamic)property)._owner, this)));
        this._bucket?.remove<object?>(((dynamic)property)._restorationId!);
        _unregister(property);
    }

    public virtual void didUpdateRestorationId()
    {
        if ((((this._currentParent is null) || (this._bucket?.restorationId == this.restorationId)) || this.restorePending))
        {
            return;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket is null)));
            oldBucket?.dispose();
        }
    }

    public virtual bool restorePending
    {
        get
        {
            if (this._firstRestorePending)
            {
                return true;
            }
            if ((this.restorationId is null))
            {
                return false;
            }
            global::Doroti.Framework.Services.RestorationBucket? potentialNewParent = ((global::Doroti.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent, this._currentParent)) && ((potentialNewParent?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration = this._properties.Keys.ToList();
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        restoreState(oldBucket, this._firstRestorePending);
        this._firstRestorePending = false;
        DartRuntimePrimitives.Assert(() =>
            {
                if (System.Linq.Enumerable.Any(this._debugPropertiesWaitingForReregistration!))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\"."), new global::Doroti.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:") }));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace = _setNewBucketIfNecessary(newBucket: ((global::Doroti.Framework.Services.RestorationBucket)(object)null), restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Framework.Services.RestorationBucket newBucketLocal = ((global::Doroti.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplaceLocal = _setNewBucketIfNecessary(newBucket: newBucketLocal, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucketLocal)));
            return didReplaceLocal;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Framework.Services.RestorationBucket? oldBucket = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket);
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _updateProperty(dynamic property)
    {
        if (((dynamic)property).enabled)
        {
            this._bucket?.write(((dynamic)property)._restorationId!, property.toPrimitives());
        }
        else
        {
            this._bucket?.remove<object>(((dynamic)property)._restorationId!);
        }
    }

    public virtual void _unregister(dynamic property)
    {
        global::System.Action listener = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
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
    public virtual global::Doroti.Framework.Rendering.SelectionRegistrar registrar { get; private set; } = default!;

    internal _ScrollableSelectionHandler__scrollable(ScrollableState state, ScrollPosition position, global::Doroti.Framework.Rendering.SelectionRegistrar registrar, Widget child)
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
        _selectionDelegate = new _ScrollableSelectionContainerDelegate__scrollable(state: ((_ScrollableSelectionHandler__scrollable)this.widget).state, position: ((_ScrollableSelectionHandler__scrollable)this.widget).position);
    }

    public override void didUpdateWidget(_ScrollableSelectionHandler__scrollable oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((_ScrollableSelectionHandler__scrollable)oldWidget).position, ((_ScrollableSelectionHandler__scrollable)this.widget).position)))
        {
            this._selectionDelegate.position = ((_ScrollableSelectionHandler__scrollable)this.widget).position;
        }
    }

    public override void dispose()
    {
        this._selectionDelegate.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new SelectionContainer(registrar: ((_ScrollableSelectionHandler__scrollable)this.widget).registrar, @delegate: this._selectionDelegate, child: ((_ScrollableSelectionHandler__scrollable)this.widget).child));
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
    internal virtual DartMap<global::Doroti.Framework.Rendering.Selectable, double> _selectableStartEdgeUpdateRecords { get; private set; } = new DartMap<global::Doroti.Framework.Rendering.Selectable, double>();
    internal virtual DartMap<global::Doroti.Framework.Rendering.Selectable, double> _selectableEndEdgeUpdateRecords { get; private set; } = new DartMap<global::Doroti.Framework.Rendering.Selectable, double>();

    internal _ScrollableSelectionContainerDelegate__scrollable(ScrollableState state, ScrollPosition position)
    {
        this.state = state;
        this._position = position;
        this._autoScroller = new EdgeDraggingAutoScroller(state, velocityScalar: _kDefaultSelectToScrollVelocityScalar);
        this._position.addListener(this._scheduleLayoutChange);
    }

    public virtual ScrollPosition position
    {
        get => this._position;
        set
        {
            var other = value;
            if ((object.Equals(other, this._position)))
            {
                return;
            }
            this._position.removeListener(this._scheduleLayoutChange);
            _position = other;
            this._position.addListener(this._scheduleLayoutChange);
        }
    }
    internal virtual void _scheduleLayoutChange()
    {
        if (this._scheduledLayoutChange)
        {
            return;
        }
        _scheduledLayoutChange = true;
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) =>
        {
            if (!this._scheduledLayoutChange)
            {
                return;
            }
            _scheduledLayoutChange = false;
            layoutDidChange();
        })), debugLabel: "ScrollableSelectionContainer.layoutDidChange");
    }

    public override void didChangeSelectables()
    {
        HashSet<global::Doroti.Framework.Rendering.Selectable> selectableSet = this.selectables.toSet();
        this._selectableStartEdgeUpdateRecords.removeWhere(((key, value) => !selectableSet.Contains(key)));
        this._selectableEndEdgeUpdateRecords.removeWhere(((key, value) => !selectableSet.Contains(key)));
        base.didChangeSelectables();
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleClearSelection(global::Doroti.Framework.Rendering.ClearSelectionEvent @event)
    {
        this._selectableStartEdgeUpdateRecords.Clear();
        this._selectableEndEdgeUpdateRecords.Clear();
        _currentDragStartRelatedToOrigin = null;
        _currentDragEndRelatedToOrigin = null;
        _selectionStartsInScrollable = false;
        return base.handleClearSelection(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectionEdgeUpdate(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent @event)
    {
        if (((this._currentDragEndRelatedToOrigin is null) && (this._currentDragStartRelatedToOrigin is null)))
        {
            DartRuntimePrimitives.Assert(() => !this._selectionStartsInScrollable);
            _selectionStartsInScrollable = _globalPositionInScrollable(((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition);
        }
        global::Doroti.Ui.Offset deltaToOrigin = ((global::Doroti.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
        if ((object.Equals(@event.type, global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate)))
        {
            _currentDragEndRelatedToOrigin = _inferPositionRelatedToOrigin(((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition);
            global::Doroti.Ui.Offset endOffset = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this._currentDragEndRelatedToOrigin).translate(-deltaToOrigin.dx, -deltaToOrigin.dy));
            @event = global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: endOffset, granularity: ((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).granularity);
        }
        else
        {
            _currentDragStartRelatedToOrigin = _inferPositionRelatedToOrigin(((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition);
            global::Doroti.Ui.Offset startOffset = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this._currentDragStartRelatedToOrigin).translate(-deltaToOrigin.dx, -deltaToOrigin.dy));
            @event = new global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: startOffset, granularity: ((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).granularity);
        }
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleSelectionEdgeUpdate(@event);
        if ((object.Equals(result, global::Doroti.Framework.Rendering.SelectionResult.pending)))
        {
            this._autoScroller.stopAutoScroll();
            return result;
        }
        if (this._selectionStartsInScrollable)
        {
            this._autoScroller.startAutoScrollIfNecessary(_dragTargetFromEvent(@event));
            if (((EdgeDraggingAutoScroller)this._autoScroller).scrolling)
            {
                return global::Doroti.Framework.Rendering.SelectionResult.pending;
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _inferPositionRelatedToOrigin(Offset globalPosition)
    {
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.state.context.findRenderObject()!)!;
        global::Doroti.Ui.Offset localPosition = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)box).globalToLocal(globalPosition)));
        if (!this._selectionStartsInScrollable)
        {
            if (((localPosition.dy < 0L) || (localPosition.dx < 0L)))
            {
                return ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)box).localToGlobal(Offset.zero)));
            }
            if (((localPosition.dy > ((global::Doroti.Framework.Rendering.RenderBox)box).size.height) || (localPosition.dx > ((global::Doroti.Framework.Rendering.RenderBox)box).size.width)))
            {
                return Offset.infinite;
            }
        }
        global::Doroti.Ui.Offset deltaToOrigin = ((global::Doroti.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
        return ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)box).localToGlobal(localPosition.translate(deltaToOrigin.dx, deltaToOrigin.dy))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateDragLocationsFromGeometries(bool forceUpdateStart = true, bool forceUpdateEnd = true)
    {
        global::Doroti.Ui.Offset deltaToOrigin = ((global::Doroti.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.state.context.findRenderObject()!)!;
        Matrix4 transform = ((Matrix4)(object?)box.getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null)));
        if (((this.currentSelectionStartIndex != -1L) && (((this._currentDragStartRelatedToOrigin is null) || forceUpdateStart))))
        {
            global::Doroti.Framework.Rendering.SelectionGeometry geometry = this.selectables[(int)(this.currentSelectionStartIndex)].value;
            DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.SelectionGeometry)geometry).hasSelection);
            global::Doroti.Framework.Rendering.SelectionPoint start = ((global::Doroti.Framework.Rendering.SelectionGeometry)geometry).startSelectionPoint!;
            Matrix4 childTransform = ((Matrix4)(object?)this.selectables[(int)(this.currentSelectionStartIndex)].getTransformTo(box));
            global::Doroti.Ui.Offset localDragStart = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(childTransform, (((global::Doroti.Framework.Rendering.SelectionPoint)start).localPosition + new global::Doroti.Ui.Offset(0, (-((global::Doroti.Framework.Rendering.SelectionPoint)start).lineHeight / 2L)))));
            _currentDragStartRelatedToOrigin = MatrixUtils.transformPoint(transform, (localDragStart + deltaToOrigin));
        }
        if (((this.currentSelectionEndIndex != -1L) && (((this._currentDragEndRelatedToOrigin is null) || forceUpdateEnd))))
        {
            global::Doroti.Framework.Rendering.SelectionGeometry geometryLocal = this.selectables[(int)(this.currentSelectionEndIndex)].value;
            DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.SelectionGeometry)geometryLocal).hasSelection);
            global::Doroti.Framework.Rendering.SelectionPoint end = ((global::Doroti.Framework.Rendering.SelectionGeometry)geometryLocal).endSelectionPoint!;
            Matrix4 childTransformLocal = ((Matrix4)(object?)this.selectables[(int)(this.currentSelectionEndIndex)].getTransformTo(box));
            global::Doroti.Ui.Offset localDragEnd = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(childTransformLocal, (((global::Doroti.Framework.Rendering.SelectionPoint)end).localPosition + new global::Doroti.Ui.Offset(0, (-((global::Doroti.Framework.Rendering.SelectionPoint)end).lineHeight / 2L)))));
            _currentDragEndRelatedToOrigin = MatrixUtils.transformPoint(transform, (localDragEnd + deltaToOrigin));
        }
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectAll(global::Doroti.Framework.Rendering.SelectAllSelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !this._selectionStartsInScrollable);
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleSelectAll(@event);
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionStartIndex == -1L)) == ((this.currentSelectionEndIndex == -1L))));
        if ((this.currentSelectionStartIndex != -1L))
        {
            _updateDragLocationsFromGeometries();
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleSelectWord(global::Doroti.Framework.Rendering.SelectWordSelectionEvent @event)
    {
        _selectionStartsInScrollable = _globalPositionInScrollable(((global::Doroti.Framework.Rendering.SelectWordSelectionEvent)@event).globalPosition);
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleSelectWord(@event);
        _updateDragLocationsFromGeometries();
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleGranularlyExtendSelection(global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleGranularlyExtendSelection(@event);
        _updateDragLocationsFromGeometries(forceUpdateStart: !((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd, forceUpdateEnd: ((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd);
        if (this._selectionStartsInScrollable)
        {
            _jumpToEdge(((global::Doroti.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult handleDirectionallyExtendSelection(global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent @event)
    {
        global::Doroti.Framework.Rendering.SelectionResult result = base.handleDirectionallyExtendSelection(@event);
        _updateDragLocationsFromGeometries(forceUpdateStart: !((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd, forceUpdateEnd: ((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd);
        if (this._selectionStartsInScrollable)
        {
            _jumpToEdge(((global::Doroti.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _jumpToEdge(bool isExtent)
    {
        global::Doroti.Framework.Rendering.Selectable selectable = default!;
        double? lineHeightLocal = default!;
        global::Doroti.Framework.Rendering.SelectionPoint? edge = default!;
        if (isExtent)
        {
            selectable = this.selectables[(int)(this.currentSelectionEndIndex)];
            edge = selectable.value.endSelectionPoint;
            lineHeightLocal = selectable.value.endSelectionPoint!.lineHeight;
        }
        else
        {
            selectable = this.selectables[(int)(this.currentSelectionStartIndex)];
            edge = selectable.value.startSelectionPoint;
            lineHeightLocal = selectable.value.startSelectionPoint?.lineHeight;
        }
        if (((lineHeightLocal is null) || (edge is null)))
        {
            return;
        }
        var scrollableBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.state.context.findRenderObject()!)!;
        Matrix4 transform = ((Matrix4)(object?)selectable.getTransformTo(scrollableBox));
        global::Doroti.Ui.Offset edgeOffsetInScrollableCoordinates = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(transform, ((global::Doroti.Framework.Rendering.SelectionPoint)edge).localPosition));
        var scrollableRect = global::Doroti.Ui.Rect.fromLTRB(0, 0, ((global::Doroti.Framework.Rendering.RenderBox)scrollableBox).size.width, ((global::Doroti.Framework.Rendering.RenderBox)scrollableBox).size.height);
        switch (((ScrollableState)this.state).axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    double edgeBottom = edgeOffsetInScrollableCoordinates.dy;
                    double edgeTop = (edgeOffsetInScrollableCoordinates.dy - DartRuntimePrimitives.RequireValue(lineHeightLocal));
                    if (((edgeBottom >= scrollableRect.bottom) && (edgeTop <= scrollableRect.top)))
                    {
                        return;
                    }
                    if ((edgeBottom > scrollableRect.bottom))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + scrollableRect.bottom) - edgeBottom));
                        return;
                    }
                    if ((edgeTop < scrollableRect.top))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + scrollableRect.top) - edgeTop));
                    }
                    return;
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    double edgeLocal = edgeOffsetInScrollableCoordinates.dx;
                    if (((edgeLocal >= scrollableRect.right) && (edgeLocal <= scrollableRect.left)))
                    {
                        return;
                    }
                    if ((edgeLocal > scrollableRect.right))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + edgeLocal) - scrollableRect.right));
                        return;
                    }
                    if ((edgeLocal < scrollableRect.left))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + edgeLocal) - scrollableRect.left));
                    }
                    return;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    double edgeBottomLocal = edgeOffsetInScrollableCoordinates.dy;
                    double edgeTopLocal = (edgeOffsetInScrollableCoordinates.dy - DartRuntimePrimitives.RequireValue(lineHeightLocal));
                    if (((edgeBottomLocal >= scrollableRect.bottom) && (edgeTopLocal <= scrollableRect.top)))
                    {
                        return;
                    }
                    if ((edgeBottomLocal > scrollableRect.bottom))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + edgeBottomLocal) - scrollableRect.bottom));
                        return;
                    }
                    if ((edgeTopLocal < scrollableRect.top))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + edgeTopLocal) - scrollableRect.top));
                    }
                    return;
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    double edgeAlternate = edgeOffsetInScrollableCoordinates.dx;
                    if (((edgeAlternate >= scrollableRect.right) && (edgeAlternate <= scrollableRect.left)))
                    {
                        return;
                    }
                    if ((edgeAlternate > scrollableRect.right))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + scrollableRect.right) - edgeAlternate));
                        return;
                    }
                    if ((edgeAlternate < scrollableRect.left))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + scrollableRect.left) - edgeAlternate));
                    }
                    return;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    internal virtual bool _globalPositionInScrollable(Offset globalPosition)
    {
        var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this.state.context.findRenderObject()!)!;
        global::Doroti.Ui.Offset localPosition = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)box).globalToLocal(globalPosition)));
        var rect = global::Doroti.Ui.Rect.fromLTWH(0, 0, ((global::Doroti.Framework.Rendering.RenderBox)box).size.width, ((global::Doroti.Framework.Rendering.RenderBox)box).size.height);
        return rect.contains(localPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Rect _dragTargetFromEvent(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent @event)
    {
        return global::Doroti.Ui.Rect.fromCenter(center: ((global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition, width: _kDefaultDragTargetSize, height: _kDefaultDragTargetSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.SelectionResult dispatchSelectionEventToChild(global::Doroti.Framework.Rendering.Selectable selectable, global::Doroti.Framework.Rendering.SelectionEvent @event)
    {
        switch (((global::Doroti.Framework.Rendering.SelectionEvent)@event).type)
        {
            case global::Doroti.Framework.Rendering.SelectionEventType.startEdgeUpdate:
                {
                    this._selectableStartEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    ensureChildUpdated(selectable);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.endEdgeUpdate:
                {
                    this._selectableEndEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    ensureChildUpdated(selectable);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.granularlyExtendSelection:
            case global::Doroti.Framework.Rendering.SelectionEventType.directionallyExtendSelection:
                {
                    ensureChildUpdated(selectable);
                    this._selectableStartEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    this._selectableEndEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.clear:
                {
                    this._selectableEndEdgeUpdateRecords.remove(selectable);
                    this._selectableStartEdgeUpdateRecords.remove(selectable);
                    break;
                }
            case global::Doroti.Framework.Rendering.SelectionEventType.selectAll:
            case global::Doroti.Framework.Rendering.SelectionEventType.selectWord:
            case global::Doroti.Framework.Rendering.SelectionEventType.selectParagraph:
                {
                    this._selectableEndEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    this._selectableStartEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    break;
                }
        }
        return base.dispatchSelectionEventToChild(selectable, @event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void ensureChildUpdated(global::Doroti.Framework.Rendering.Selectable selectable)
    {
        double newRecord = ((ScrollableState)this.state).position.pixels;
        double? previousStartRecord = DartCollectionRuntime.NullableMapValue<double>(this._selectableStartEdgeUpdateRecords, selectable);
        if (((this._currentDragStartRelatedToOrigin is not null) && (((previousStartRecord is null) || (((newRecord - DartRuntimePrimitives.RequireValue(previousStartRecord))).abs() > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))))
        {
            global::Doroti.Ui.Offset deltaToOrigin = ((global::Doroti.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
            global::Doroti.Ui.Offset startOffset = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this._currentDragStartRelatedToOrigin).translate(-deltaToOrigin.dx, -deltaToOrigin.dy));
            selectable.dispatchSelectionEvent(new global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: startOffset));
            this._selectableStartEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
        }
        double? previousEndRecord = DartCollectionRuntime.NullableMapValue<double>(this._selectableEndEdgeUpdateRecords, selectable);
        if (((this._currentDragEndRelatedToOrigin is not null) && (((previousEndRecord is null) || (((newRecord - DartRuntimePrimitives.RequireValue(previousEndRecord))).abs() > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))))
        {
            global::Doroti.Ui.Offset deltaToOriginLocal = ((global::Doroti.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
            global::Doroti.Ui.Offset endOffset = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this._currentDragEndRelatedToOrigin).translate(-deltaToOriginLocal.dx, -deltaToOriginLocal.dy));
            selectable.dispatchSelectionEvent(global::Doroti.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: endOffset));
            this._selectableEndEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
        }
    }

    public override void dispose()
    {
        this._selectableStartEdgeUpdateRecords.Clear();
        this._selectableEndEdgeUpdateRecords.Clear();
        _scheduledLayoutChange = false;
        this._autoScroller.stopAutoScroll();
        base.dispose();
    }

}

public static partial class ScrollableLibrary
{
    internal static Offset _getDeltaToScrollOrigin(ScrollableState scrollableState)
    {
        return (((ScrollableState)scrollableState).axisDirection switch { global::Doroti.Framework.Painting.AxisDirection.up => new global::Doroti.Ui.Offset(0, -((ScrollableState)scrollableState).position.pixels), global::Doroti.Framework.Painting.AxisDirection.down => new global::Doroti.Ui.Offset(0, ((ScrollableState)scrollableState).position.pixels), global::Doroti.Framework.Painting.AxisDirection.left => new global::Doroti.Ui.Offset(-((ScrollableState)scrollableState).position.pixels, 0), global::Doroti.Framework.Painting.AxisDirection.right => new global::Doroti.Ui.Offset(((ScrollableState)scrollableState).position.pixels, 0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ScrollSemantics__scrollable : SingleChildRenderObjectWidget
{
    public virtual ScrollPosition position { get; private set; } = default!;
    public virtual bool allowImplicitScrolling { get; private set; } = default!;
    public virtual long? semanticChildCount { get; private set; }
    public virtual global::Doroti.Framework.Painting.Axis axis { get; private set; } = default!;

    internal _ScrollSemantics__scrollable(global::Doroti.Framework.Foundation.Key? key = null, ScrollPosition position = default!, bool allowImplicitScrolling = default!, global::Doroti.Framework.Painting.Axis axis = default!, long? semanticChildCount = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.position = position;
        this.allowImplicitScrolling = allowImplicitScrolling;
        this.axis = axis;
        this.semanticChildCount = semanticChildCount;
        System.Diagnostics.Debug.Assert(((semanticChildCount is null) || (semanticChildCount >= 0L)));
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderScrollSemantics__scrollable(position: this.position, allowImplicitScrolling: this.allowImplicitScrolling, semanticChildCount: this.semanticChildCount, axis: this.axis));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderScrollSemantics__scrollable)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderScrollSemantics__scrollable>)(() =>
{
    var __cascade = __renderObject;
    __cascade.allowImplicitScrolling = this.allowImplicitScrolling;
    __cascade.axis = this.axis;
    __cascade.position = this.position;
    __cascade.semanticChildCount = this.semanticChildCount;
    return __cascade;
}))());
    }

}

public class _RenderScrollSemantics__scrollable : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual ScrollPosition _position { get; set; } = default!;
    internal virtual bool _allowImplicitScrolling { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis axis { get; set; } = default!;
    internal virtual long? _semanticChildCount { get; set; } = default;
    internal virtual global::Doroti.Framework.Semantics.SemanticsNode? _innerNode { get; set; } = default;

    internal _RenderScrollSemantics__scrollable(ScrollPosition position, bool allowImplicitScrolling, global::Doroti.Framework.Painting.Axis axis, long? semanticChildCount, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this.axis = axis;
        this._position = position;
        this._allowImplicitScrolling = allowImplicitScrolling;
        this._semanticChildCount = semanticChildCount;
        this._position.addListener(this.markNeedsSemanticsUpdate);
    }

    public virtual ScrollPosition position
    {
        get => this._position;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._position)))
            {
                return;
            }
            this._position.removeListener(this.markNeedsSemanticsUpdate);
            _position = __value;
            this._position.addListener(this.markNeedsSemanticsUpdate);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual bool allowImplicitScrolling
    {
        get => this._allowImplicitScrolling;
        set
        {
            var __value = value;
            if ((DartRuntimePrimitives.RequireValue(__value) == this._allowImplicitScrolling))
            {
                return;
            }
            _allowImplicitScrolling = DartRuntimePrimitives.RequireValue(__value);
            markNeedsSemanticsUpdate();
        }
    }
    public virtual long? semanticChildCount
    {
        get => this._semanticChildCount;
        set
        {
            var __value = value;
            if ((__value == this.semanticChildCount))
            {
                return;
            }
            _semanticChildCount = __value;
            markNeedsSemanticsUpdate();
        }
    }
    internal virtual void _onScrollToOffset(Offset targetOffset)
    {
        double offset = (this.axis switch { global::Doroti.Framework.Painting.Axis.horizontal => targetOffset.dx, global::Doroti.Framework.Painting.Axis.vertical => targetOffset.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        this._position.jumpTo(offset);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.isSemanticBoundary = true;
    __cascade.hasImplicitScrolling = this.allowImplicitScrolling;
    return __cascade;
}))());
        if (((ScrollPosition)this.position).haveDimensions)
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = config;
    __cascade.scrollPosition = ((ScrollPosition)this._position).pixels;
    __cascade.scrollExtentMax = ((ScrollPosition)this._position).maxScrollExtent;
    __cascade.scrollExtentMin = ((ScrollPosition)this._position).minScrollExtent;
    __cascade.scrollChildCount = this.semanticChildCount;
    return __cascade;
}))());
            if (((((ScrollPosition)this.position).maxScrollExtent > ((ScrollPosition)this.position).minScrollExtent) && this.allowImplicitScrolling))
            {
                config.onScrollToOffset = (global::System.Action<Offset>)this._onScrollToOffset;
            }
        }
    }

    public override void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        if ((!System.Linq.Enumerable.Any(children) || !children.First().isTagged(global::Doroti.Framework.Rendering.RenderViewport.useTwoPaneSemantics)))
        {
            _innerNode = null;
            base.assembleSemanticsNode(node, config, children.Cast<global::Doroti.Framework.Semantics.SemanticsNode>());
            return;
        }
        (_innerNode ??= new global::Doroti.Framework.Semantics.SemanticsNode(showOnScreen: () => this.showOnScreen())).rect = ((global::Doroti.Framework.Semantics.SemanticsNode)node).rect;
        long? firstVisibleIndex = default!;
        var excluded = new List<global::Doroti.Framework.Semantics.SemanticsNode> { this._innerNode! };
        var included = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
        foreach (var child in children)
        {
            DartRuntimePrimitives.Assert(() => child.isTagged(global::Doroti.Framework.Rendering.RenderViewport.useTwoPaneSemantics));
            if (child.isTagged(global::Doroti.Framework.Rendering.RenderViewport.excludeFromScrolling))
            {
                excluded.Add(child);
            }
            else
            {
                if (!((global::Doroti.Framework.Semantics.SemanticsNode)child).flagsCollection.isHidden)
                {
                    firstVisibleIndex ??= ((global::Doroti.Framework.Semantics.SemanticsNode)child).indexInParent;
                }
                included.Add(child);
            }
        }
        config.scrollIndex = firstVisibleIndex;
        node.updateWith(config: ((global::Doroti.Framework.Semantics.SemanticsConfiguration)(object)null), childrenInInversePaintOrder: excluded);
        this._innerNode!.updateWith(config: config, childrenInInversePaintOrder: included);
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
        return ((double)data!);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override object? toPrimitives()
    {
        return this.value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool enabled => DartRuntimePrimitives.ConvertValue<bool>((this.value is not null));
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
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, global::Doroti.Framework.Rendering.ViewportOffset, Widget> viewportBuilder { get; private set; } = default!;
    public virtual global::System.Func<ScrollIncrementDetails, double>? incrementCalculator { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;

    public TwoDimensionalScrollable(global::Doroti.Framework.Foundation.Key? key = null, ScrollableDetails horizontalDetails = default!, ScrollableDetails verticalDetails = default!, global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, global::Doroti.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::System.Func<ScrollIncrementDetails, double>? incrementCalculator = null, string? restorationId = null, bool excludeFromSemantics = false, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque) : base(key: key)
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
        _TwoDimensionalScrollableScope__scrollable? widget = ((_TwoDimensionalScrollableScope__scrollable?)(object?)context.dependOnInheritedWidgetOfExactType<_TwoDimensionalScrollableScope__scrollable>());
        return widget?.twoDimensionalScrollable;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TwoDimensionalScrollableState of(BuildContext context)
    {
        TwoDimensionalScrollableState? scrollableState = ((TwoDimensionalScrollableState?)(object?)TwoDimensionalScrollable.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((scrollableState is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("TwoDimensionalScrollable.of() was called with a context that does " + "not contain a TwoDimensionalScrollable widget.\n"), new global::Doroti.Framework.Foundation.ErrorDescription("No TwoDimensionalScrollable widget ancestor could be found starting " + "from the context that was passed to TwoDimensionalScrollable.of(). " + "This can happen because you are using a widget that looks for a " + "TwoDimensionalScrollable ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}") }));
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
            DartRuntimePrimitives.Assert(() => (((GlobalKey<ScrollableState>)this._verticalOuterScrollableKey).currentState is not null));
            return ((GlobalKey<ScrollableState>)this._verticalOuterScrollableKey).currentState!;
            return default!;
        }
    }
    public virtual ScrollableState horizontalScrollable
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (((GlobalKey<ScrollableState>)this._horizontalInnerScrollableKey).currentState is not null));
            return ((GlobalKey<ScrollableState>)this._horizontalInnerScrollableKey).currentState!;
            return default!;
        }
    }
    public override void initState()
    {
        if ((((TwoDimensionalScrollable)this.widget).verticalDetails.controller is null))
        {
            _verticalFallbackController = new ScrollController();
        }
        if ((((TwoDimensionalScrollable)this.widget).horizontalDetails.controller is null))
        {
            _horizontalFallbackController = new ScrollController();
        }
        base.initState();
    }

    public override void didUpdateWidget(TwoDimensionalScrollable oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((TwoDimensionalScrollable)oldWidget).verticalDetails.controller, ((TwoDimensionalScrollable)this.widget).verticalDetails.controller)))
        {
            if ((((TwoDimensionalScrollable)oldWidget).verticalDetails.controller is null))
            {
                DartRuntimePrimitives.Assert(() => (this._verticalFallbackController is not null));
                DartRuntimePrimitives.Assert(() => (((TwoDimensionalScrollable)this.widget).verticalDetails.controller is not null));
                this._verticalFallbackController!.dispose();
                _verticalFallbackController = null;
            }
            else
            {
                if ((((TwoDimensionalScrollable)this.widget).verticalDetails.controller is null))
                {
                    DartRuntimePrimitives.Assert(() => (this._verticalFallbackController is null));
                    _verticalFallbackController = new ScrollController();
                }
            }
        }
        if ((!object.Equals(((TwoDimensionalScrollable)oldWidget).horizontalDetails.controller, ((TwoDimensionalScrollable)this.widget).horizontalDetails.controller)))
        {
            if ((((TwoDimensionalScrollable)oldWidget).horizontalDetails.controller is null))
            {
                DartRuntimePrimitives.Assert(() => (this._horizontalFallbackController is not null));
                DartRuntimePrimitives.Assert(() => (((TwoDimensionalScrollable)this.widget).horizontalDetails.controller is not null));
                this._horizontalFallbackController!.dispose();
                _horizontalFallbackController = null;
            }
            else
            {
                if ((((TwoDimensionalScrollable)this.widget).horizontalDetails.controller is null))
                {
                    DartRuntimePrimitives.Assert(() => (this._horizontalFallbackController is null));
                    _horizontalFallbackController = new ScrollController();
                }
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((TwoDimensionalScrollable)this.widget).verticalDetails.direction), global::Doroti.Framework.Painting.Axis.vertical)), () => (object?)"TwoDimensionalScrollable.verticalDetails are not Axis.vertical.");
        DartRuntimePrimitives.Assert(() => (object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((TwoDimensionalScrollable)this.widget).horizontalDetails.direction), global::Doroti.Framework.Painting.Axis.horizontal)), () => (object?)"TwoDimensionalScrollable.horizontalDetails are not Axis.horizontal.");
        Widget result = ((Widget)(object?)new RestorationScope(restorationId: ((TwoDimensionalScrollable)this.widget).restorationId, child: new _VerticalOuterDimension__scrollable(key: this._verticalOuterScrollableKey, horizontalKey: this._horizontalInnerScrollableKey, axisDirection: ((TwoDimensionalScrollable)this.widget).verticalDetails.direction, controller: (((TwoDimensionalScrollable)this.widget).verticalDetails.controller ?? this._verticalFallbackController!), physics: ((TwoDimensionalScrollable)this.widget).verticalDetails.physics, clipBehavior: ((((TwoDimensionalScrollable)this.widget).verticalDetails.clipBehavior ?? ((TwoDimensionalScrollable)this.widget).verticalDetails.decorationClipBehavior) ?? Clip.hardEdge), incrementCalculator: (global::System.Func<ScrollIncrementDetails, double>?)((TwoDimensionalScrollable)this.widget).incrementCalculator, excludeFromSemantics: ((TwoDimensionalScrollable)this.widget).excludeFromSemantics, restorationId: "OuterVerticalTwoDimensionalScrollable", dragStartBehavior: ((TwoDimensionalScrollable)this.widget).dragStartBehavior, diagonalDragBehavior: ((TwoDimensionalScrollable)this.widget).diagonalDragBehavior, hitTestBehavior: ((TwoDimensionalScrollable)this.widget).hitTestBehavior, viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget>)((context, verticalOffset) =>
        {
            return ((Widget)(object?)new _HorizontalInnerDimension__scrollable(key: this._horizontalInnerScrollableKey, verticalOuterKey: this._verticalOuterScrollableKey, axisDirection: ((TwoDimensionalScrollable)this.widget).horizontalDetails.direction, controller: (((TwoDimensionalScrollable)this.widget).horizontalDetails.controller ?? this._horizontalFallbackController!), physics: ((TwoDimensionalScrollable)this.widget).horizontalDetails.physics, clipBehavior: ((((TwoDimensionalScrollable)this.widget).horizontalDetails.clipBehavior ?? ((TwoDimensionalScrollable)this.widget).horizontalDetails.decorationClipBehavior) ?? Clip.hardEdge), incrementCalculator: (global::System.Func<ScrollIncrementDetails, double>?)((TwoDimensionalScrollable)this.widget).incrementCalculator, excludeFromSemantics: ((TwoDimensionalScrollable)this.widget).excludeFromSemantics, restorationId: "InnerHorizontalTwoDimensionalScrollable", dragStartBehavior: ((TwoDimensionalScrollable)this.widget).dragStartBehavior, diagonalDragBehavior: ((TwoDimensionalScrollable)this.widget).diagonalDragBehavior, hitTestBehavior: ((TwoDimensionalScrollable)this.widget).hitTestBehavior, viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget>)((context, horizontalOffset) =>
            {
                return this.widget.viewportBuilder(context, verticalOffset, horizontalOffset);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        return ((Widget)(object?)new _TwoDimensionalScrollableScope__scrollable(twoDimensionalScrollable: this, child: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._verticalFallbackController?.dispose();
        this._horizontalFallbackController?.dispose();
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

    internal _VerticalOuterDimension__scrollable(global::Doroti.Framework.Foundation.Key? key = null, GlobalKey<ScrollableState> horizontalKey = default!, global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::Doroti.Framework.Painting.AxisDirection axisDirection = default!, ScrollController? controller = null, ScrollPhysics? physics = null, Clip clipBehavior = Clip.hardEdge, global::System.Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, string? restorationId = null, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none) : base(key: key, viewportBuilder: viewportBuilder, axisDirection: axisDirection, controller: controller, physics: physics, clipBehavior: clipBehavior, incrementCalculator: incrementCalculator, excludeFromSemantics: excludeFromSemantics, dragStartBehavior: dragStartBehavior, restorationId: restorationId, hitTestBehavior: hitTestBehavior)
    {
        this.horizontalKey = horizontalKey;
        this.diagonalDragBehavior = diagonalDragBehavior;
        System.Diagnostics.Debug.Assert(((object.Equals(axisDirection, global::Doroti.Framework.Painting.AxisDirection.up)) || (object.Equals(axisDirection, global::Doroti.Framework.Painting.AxisDirection.down))));
    }

    public override _VerticalOuterDimensionState__scrollable createState() => new _VerticalOuterDimensionState__scrollable();
}

internal class _VerticalOuterDimensionState__scrollable : ScrollableState
{
    public virtual global::Doroti.Framework.Painting.Axis? lockedAxis { get; set; } = default;
    public virtual Offset? lastDragOffset { get; set; } = default;

    public virtual DiagonalDragBehavior diagonalDragBehavior => (((_VerticalOuterDimension__scrollable?)(object?)this.widget)!).diagonalDragBehavior;
    public virtual ScrollableState horizontalScrollable => DartRuntimePrimitives.ConvertValue<ScrollableState>((((_VerticalOuterDimension__scrollable?)(object?)this.widget)!).horizontalKey.currentState!);
    internal override (List<Future>, ScrollableState) _performEnsureVisible(global::Doroti.Framework.Rendering.RenderObject @object, double alignment = 0.0, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, global::Doroti.Framework.Rendering.RenderObject? targetRenderObject = null)
    {
        DartRuntimePrimitives.Assert(() => false, () => (object?)"The _performEnsureVisible method was called for the vertical scrollable " + "of a TwoDimensionalScrollable. This should not happen as the horizontal " + "scrollable handles both axes.");
        return (new List<Future>(), this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _evaluateLockedAxis(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => (this.lastDragOffset is not null));
        global::Doroti.Ui.Offset offsetDelta = ((global::Doroti.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(this.lastDragOffset) - offset));
        double axisDifferential = (offsetDelta.dx.abs() - offsetDelta.dy.abs());
        if ((axisDifferential.abs() >= global::Doroti.Framework.Gestures.ConstantsLibrary.kTouchSlop))
        {
            lockedAxis = ((axisDifferential > 0.0) ? global::Doroti.Framework.Painting.Axis.horizontal : global::Doroti.Framework.Painting.Axis.vertical);
        }
        else
        {
            lockedAxis = null;
        }
    }

    internal override void _handleDragDown(global::Doroti.Framework.Gestures.DragDownDetails details)
    {
        switch (this.diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
            case DiagonalDragBehavior.free:
                {
                    this.horizontalScrollable._handleDragDown(details);
                    break;
                }
        }
        base._handleDragDown(details);
    }

    internal override void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        lastDragOffset = ((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition;
        switch (this.diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    break;
                }
            case DiagonalDragBehavior.free:
                {
                    this.horizontalScrollable._handleDragStart(details);
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
                {
                    _evaluateLockedAxis(((global::Doroti.Framework.Gestures.DragStartDetails)details).globalPosition);
                    switch (this.lockedAxis)
                    {
                        case null:
                            {
                                this.horizontalScrollable._handleDragStart(details);
                                break;
                            }
                        case global::Doroti.Framework.Painting.Axis.horizontal:
                            {
                                this.horizontalScrollable._handleDragStart(details);
                                return;
                            }
                        case global::Doroti.Framework.Painting.Axis.vertical:
                            break;
                    }
                    break;
                }
        }
        base._handleDragStart(details);
    }

    internal override void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        var verticalDragDetails = new global::Doroti.Framework.Gestures.DragUpdateDetails(sourceTimeStamp: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp, delta: new global::Doroti.Ui.Offset(0.0, ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta.dy), primaryDelta: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta.dy, globalPosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, localPosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).localPosition);
        var horizontalDragDetails = new global::Doroti.Framework.Gestures.DragUpdateDetails(sourceTimeStamp: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp, delta: new global::Doroti.Ui.Offset(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta.dx, 0.0), primaryDelta: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).delta.dx, globalPosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition, localPosition: ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).localPosition);
        switch (this.diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    base._handleDragUpdate(verticalDragDetails);
                    return;
                }
            case DiagonalDragBehavior.free:
                {
                    this.horizontalScrollable._handleDragUpdate(horizontalDragDetails);
                    base._handleDragUpdate(verticalDragDetails);
                    return;
                }
            case DiagonalDragBehavior.weightedContinuous:
                {
                    _evaluateLockedAxis(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition);
                    lastDragOffset = ((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition;
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
                {
                    if (((this.lockedAxis is null) && (this.lastDragOffset is not null)))
                    {
                        _evaluateLockedAxis(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).globalPosition);
                    }
                    break;
                }
        }
        switch (this.lockedAxis)
        {
            case null:
                {
                    this.horizontalScrollable._handleDragUpdate(horizontalDragDetails);
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    this.horizontalScrollable._handleDragUpdate(horizontalDragDetails);
                    return;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                break;
        }
        base._handleDragUpdate(verticalDragDetails);
    }

    internal override void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        lastDragOffset = null;
        lockedAxis = null;
        double dxLocal = ((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dx;
        double dyLocal = ((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy;
        var verticalDragDetails = new global::Doroti.Framework.Gestures.DragEndDetails(velocity: new global::Doroti.Framework.Gestures.Velocity(pixelsPerSecond: new global::Doroti.Ui.Offset(0.0, dyLocal)), primaryVelocity: dyLocal);
        var horizontalDragDetails = new global::Doroti.Framework.Gestures.DragEndDetails(velocity: new global::Doroti.Framework.Gestures.Velocity(pixelsPerSecond: new global::Doroti.Ui.Offset(dxLocal, 0.0)), primaryVelocity: dxLocal);
        switch (this.diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
            case DiagonalDragBehavior.free:
                {
                    this.horizontalScrollable._handleDragEnd(horizontalDragDetails);
                    break;
                }
        }
        base._handleDragEnd(verticalDragDetails);
    }

    internal override void _handleDragCancel()
    {
        lastDragOffset = null;
        lockedAxis = null;
        switch (this.diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
            case DiagonalDragBehavior.weightedContinuous:
            case DiagonalDragBehavior.free:
                {
                    this.horizontalScrollable._handleDragCancel();
                    break;
                }
        }
        base._handleDragCancel();
    }

    public override void setCanDrag(bool value)
    {
        switch (this.diagonalDragBehavior)
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
                            [typeof(global::Doroti.Framework.Gestures.PanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Framework.Gestures.PanGestureRecognizer>(((global::System.Func<global::Doroti.Framework.Gestures.PanGestureRecognizer>)(() => new global::Doroti.Framework.Gestures.PanGestureRecognizer(supportedDevices: ((ScrollBehavior)this._configuration).dragDevices))), ((global::System.Action<global::Doroti.Framework.Gestures.PanGestureRecognizer>)((instance) =>
                            {
                                DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Gestures.PanGestureRecognizer>)(() =>
                                {
                                    var __cascade = instance;
                                    __cascade.onDown = this._handleDragDown;
                                    __cascade.onStart = this._handleDragStart;
                                    __cascade.onUpdate = this._handleDragUpdate;
                                    __cascade.onEnd = this._handleDragEnd;
                                    __cascade.onCancel = this._handleDragCancel;
                                    __cascade.minFlingDistance = this._physics?.minFlingDistance;
                                    __cascade.minFlingVelocity = this._physics?.minFlingVelocity;
                                    __cascade.maxFlingVelocity = this._physics?.maxFlingVelocity;
                                    __cascade.velocityTrackerBuilder = this._configuration.velocityTrackerBuilder(this.context);
                                    __cascade.dragStartBehavior = this.widget.dragStartBehavior;
                                    __cascade.gestureSettings = this._mediaQueryGestureSettings;
                                    return __cascade;
                                }))());
                            })))
                        };
                        _handleDragCancel();
                        _lastCanDrag = value;
                        _lastAxisDirection = this.widget.axis;
                        if ((((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentState is not null))
                        {
                            ((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentState!.replaceGestureRecognizers(this._gestureRecognizers);
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
        var details = new ScrollableDetails(direction: this.widget.axisDirection, controller: this._effectiveScrollController, clipBehavior: this.widget.clipBehavior);
        return ((Widget)(object?)this._configuration.buildOverscrollIndicator(context, child, details));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HorizontalInnerDimension__scrollable : Scrollable
{
    public virtual GlobalKey<ScrollableState> verticalOuterKey { get; private set; } = default!;
    public virtual DiagonalDragBehavior diagonalDragBehavior { get; private set; } = default!;

    internal _HorizontalInnerDimension__scrollable(global::Doroti.Framework.Foundation.Key? key = null, GlobalKey<ScrollableState> verticalOuterKey = default!, global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::Doroti.Framework.Painting.AxisDirection axisDirection = default!, ScrollController? controller = null, ScrollPhysics? physics = null, Clip clipBehavior = Clip.hardEdge, global::System.Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, string? restorationId = null, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none) : base(key: key, viewportBuilder: viewportBuilder, axisDirection: axisDirection, controller: controller, physics: physics, clipBehavior: clipBehavior, incrementCalculator: incrementCalculator, excludeFromSemantics: excludeFromSemantics, dragStartBehavior: dragStartBehavior, restorationId: restorationId, hitTestBehavior: hitTestBehavior)
    {
        this.verticalOuterKey = verticalOuterKey;
        this.diagonalDragBehavior = diagonalDragBehavior;
        System.Diagnostics.Debug.Assert(((object.Equals(axisDirection, global::Doroti.Framework.Painting.AxisDirection.left)) || (object.Equals(axisDirection, global::Doroti.Framework.Painting.AxisDirection.right))));
    }

    public override _HorizontalInnerDimensionState__scrollable createState() => new _HorizontalInnerDimensionState__scrollable();
}

internal class _HorizontalInnerDimensionState__scrollable : ScrollableState
{
    public virtual ScrollableState verticalScrollable { get; set; } = default!;

    public virtual GlobalKey<ScrollableState> verticalOuterKey => (((_HorizontalInnerDimension__scrollable?)(object?)this.widget)!).verticalOuterKey;
    public virtual DiagonalDragBehavior diagonalDragBehavior => (((_HorizontalInnerDimension__scrollable?)(object?)this.widget)!).diagonalDragBehavior;
    public override void didChangeDependencies()
    {
        verticalScrollable = Scrollable.of(this.context);
        DartRuntimePrimitives.Assert(() => (object.Equals(global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableState)this.verticalScrollable).axisDirection), global::Doroti.Framework.Painting.Axis.vertical)));
        base.didChangeDependencies();
    }

    internal override (List<Future>, ScrollableState) _performEnsureVisible(global::Doroti.Framework.Rendering.RenderObject @object, double alignment = 0.0, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, global::Doroti.Framework.Rendering.RenderObject? targetRenderObject = null)
    {
        var newFutures = new List<Future> { this.position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy), ((ScrollableState)this.verticalScrollable).position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy) };
        return (newFutures, this.verticalScrollable);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setCanDrag(bool value)
    {
        switch (this.diagonalDragBehavior)
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
                        ((GlobalKey<ScrollableState>)this.verticalOuterKey).currentState!.setCanDrag(value);
                        _handleDragCancel();
                        _lastCanDrag = value;
                        _lastAxisDirection = this.widget.axis;
                        if ((((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentState is not null))
                        {
                            ((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentState!.replaceGestureRecognizers(this._gestureRecognizers);
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
        var details = new ScrollableDetails(direction: this.widget.axisDirection, controller: this._effectiveScrollController, clipBehavior: this.widget.clipBehavior);
        return ((Widget)(object?)this._configuration.buildOverscrollIndicator(context, child, details));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
