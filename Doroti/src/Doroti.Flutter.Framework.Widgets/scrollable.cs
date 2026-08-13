// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/scrollable.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public delegate Widget ViewportBuilder(BuildContext context, global::Doroti.Generated.Framework.Rendering.ViewportOffset position);

public delegate Widget TwoDimensionalViewportBuilder(BuildContext context, global::Doroti.Generated.Framework.Rendering.ViewportOffset verticalPosition, global::Doroti.Generated.Framework.Rendering.ViewportOffset horizontalPosition);

internal delegate void _EnsureVisibleResults__scrollable();

public class Scrollable : StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget> viewportBuilder { get; private set; } = default!;
    public virtual global::System.Func<ScrollIncrementDetails, double>? incrementCalculator { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual long? semanticChildCount { get; private set; }
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;

    public Scrollable(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection = global::Doroti.Generated.Framework.Painting.AxisDirection.down, ScrollController? controller = null, ScrollPhysics? physics = null, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::System.Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, string? restorationId = null, ScrollBehavior? scrollBehavior = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque) : base(key: key)
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

    public virtual global::Doroti.Generated.Framework.Painting.Axis axis => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this.axisDirection);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new ScrollableState());
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("physics", this.physics));
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("restorationId", this.restorationId));
    }

    public static ScrollableState? maybeOf(BuildContext context, global::Doroti.Generated.Framework.Painting.Axis? axis = null)
    {
        var originalContext__15978 = context;
        InheritedElement? element__16027 = ((InheritedElement?)(object?)context.getElementForInheritedWidgetOfExactType<_ScrollableScope__scrollable>());
        while ((element__16027 is not null))
        {
            ScrollableState scrollable__16164 = (((_ScrollableScope__scrollable?)(object?)element__16027.widget)!).scrollable;
            if (((axis is null) || (object.Equals(global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableState)scrollable__16164).axisDirection), DartRuntimePrimitives.RequireValue(axis)))))
            {
                originalContext__15978.dependOnInheritedElement(element__16027);
                return scrollable__16164;
            }
            context = scrollable__16164.context;
            element__16027 = context.getElementForInheritedWidgetOfExactType<_ScrollableScope__scrollable>();
        }
        return ((ScrollableState)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ScrollableState of(BuildContext context, global::Doroti.Generated.Framework.Painting.Axis? axis = null)
    {
        ScrollableState? scrollableState__17918 = ((ScrollableState?)(object?)Scrollable.maybeOf(context, axis: axis));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((scrollableState__17918 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Scrollable.of() was called with a context that does not contain a " + "Scrollable widget."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("No Scrollable widget ancestor could be found " + $"{((axis is null) ? "" : $"for the provided Axis: {DartRuntimePrimitives.RequireValue(axis)} ")}" + "starting from the context that was passed to Scrollable.of(). This " + "can happen because you are using a widget that looks for a Scrollable " + "ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return scrollableState__17918!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static bool recommendDeferredLoadingForContext(BuildContext context, global::Doroti.Generated.Framework.Painting.Axis? axis = null)
    {
        _ScrollableScope__scrollable? widget__20075 = ((_ScrollableScope__scrollable?)(object?)context.getInheritedWidgetOfExactType<_ScrollableScope__scrollable>());
        while ((widget__20075 is not null))
        {
            if (((axis is null) || (object.Equals(global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((_ScrollableScope__scrollable)widget__20075).scrollable.axisDirection), DartRuntimePrimitives.RequireValue(axis)))))
            {
                return ((_ScrollableScope__scrollable)widget__20075).position.recommendDeferredLoading(context);
            }
            context = ((_ScrollableScope__scrollable)widget__20075).scrollable.context;
            widget__20075 = context.getInheritedWidgetOfExactType<_ScrollableScope__scrollable>();
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future ensureVisible(BuildContext context, double alignment = 0.0, Duration duration = default, global::Doroti.Generated.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit)
    {
        var futures__21069 = new List<Future>();
        global::Doroti.Generated.Framework.Rendering.RenderObject? targetRenderObject__21540 = default!;
        ScrollableState? scrollable__21581 = ((ScrollableState?)(object?)Scrollable.maybeOf(context));
        while ((scrollable__21581 is not null))
        {
            List<Future> newFutures__21687 = default!;
            DartRuntimePrimitives.Ignore((newFutures__21687, scrollable__21581) = scrollable__21581._performEnsureVisible(context.findRenderObject()!, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy, targetRenderObject: targetRenderObject__21540));
            futures__21069.AddRange(newFutures__21687.Cast<Future>());
            targetRenderObject__21540 ??= context.findRenderObject();
            context = scrollable__21581.context;
            scrollable__21581 = Scrollable.maybeOf(context);
        }
        if ((!System.Linq.Enumerable.Any(futures__21069) || (object.Equals(duration, Duration.zero))))
        {
            return Future.value();
        }
        if ((checked((long)(futures__21069.Count)) == 1L))
        {
            return futures__21069.Single();
        }
        return global::Doroti.Flutter.Runtime.DartAsyncRuntime.wait<object?>(futures__21069).then((global::System.Action<List<object?>>)((_) => { }));
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
    internal virtual global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings? _mediaQueryGestureSettings { get; set; } = default;
    internal virtual GlobalKey<IState> _scrollSemanticsKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual GlobalKey<RawGestureDetectorState> _gestureDetectorKey { get; private set; } = GlobalKey<RawGestureDetectorState>.Create();
    internal virtual GlobalKey<IState> _ignorePointerKey { get; private set; } = GlobalKey<IState>.Create();
    internal virtual DartMap<Type, dynamic> _gestureRecognizers { get; set; } = new DartMap<Type, dynamic>();
    internal virtual bool _shouldIgnorePointer { get; set; } = false;
    internal virtual bool? _lastCanDrag { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.Axis? _lastAxisDirection { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Gestures.Drag? _drag { get; set; } = default;
    internal virtual ScrollHoldController? _hold { get; set; } = default;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _bucket { get; set; } = default;
    public virtual DartMap<dynamic, global::System.Action> _properties { get; set; } = new DartMap<dynamic, global::System.Action>();
    public virtual List<dynamic>? _debugPropertiesWaitingForReregistration { get; set; } = default;
    public virtual bool _firstRestorePending { get; set; } = true;
    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? _currentParent { get; set; } = default;

    public virtual ScrollPosition position => DartRuntimePrimitives.ConvertValue<ScrollPosition>(this._position!);
    public virtual ScrollPhysics? resolvedPhysics => this._physics;
    public virtual global::Doroti.Flutter.Ui.Offset deltaToScrollOrigin => DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Offset>((this.axisDirection switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => new global::Doroti.Flutter.Ui.Offset(0, -((ScrollPosition)this.position).pixels), global::Doroti.Generated.Framework.Painting.AxisDirection.down => new global::Doroti.Flutter.Ui.Offset(0, ((ScrollPosition)this.position).pixels), global::Doroti.Generated.Framework.Painting.AxisDirection.left => new global::Doroti.Flutter.Ui.Offset(-((ScrollPosition)this.position).pixels, 0), global::Doroti.Generated.Framework.Painting.AxisDirection.right => new global::Doroti.Flutter.Ui.Offset(((ScrollPosition)this.position).pixels, 0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    internal virtual ScrollController _effectiveScrollController => DartRuntimePrimitives.ConvertValue<ScrollController>((((Scrollable)this.widget).controller ?? this._fallbackScrollController!));
    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection => ((Scrollable)this.widget).axisDirection;
    public virtual global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Scheduler.TickerProvider>(this);
    public virtual double devicePixelRatio => this._devicePixelRatio;
    public virtual BuildContext? notificationContext => ((GlobalKey<RawGestureDetectorState>)this._gestureDetectorKey).currentContext;
    public virtual BuildContext storageContext => this.context;
    public virtual string? restorationId => ((Scrollable)this.widget).restorationId;
    internal virtual void _updatePosition()
    {
        _configuration = ((((Scrollable)this.widget).scrollBehavior ?? (ScrollBehavior)ScrollConfiguration.of(this.context)));
        ScrollPhysics? physicsFromWidget__25542 = ((((Scrollable)this.widget).physics ?? (ScrollPhysics)((Scrollable)this.widget).scrollBehavior?.getScrollPhysics(this.context)));
        _physics = this._configuration.getScrollPhysics(this.context);
        _physics = (physicsFromWidget__25542?.applyTo(this._physics) ?? this._physics);
        ScrollPosition? oldPosition__25787 = this._position;
        if ((oldPosition__25787 is not null))
        {
            this._effectiveScrollController.detach(oldPosition__25787);
            DartAsyncRuntime.scheduleMicrotask(((ScrollPosition)oldPosition__25787).dispose);
        }
        _position = this._effectiveScrollController.createScrollPosition(this._physics!, this, oldPosition__25787);
        DartRuntimePrimitives.Assert(() => (this._position is not null));
        this._effectiveScrollController.attach(this.position);
    }

    public virtual void restoreState(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket, bool initialRestore)
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Services.RestorationLibrary.debugIsSerializableForRestoration(offset));
        this._persistedScrollOffset.value = offset;
        global::Doroti.Generated.Framework.Services.ServicesBinding.instance.restorationManager.flushData();
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
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__41020 = this._bucket;
        bool needsRestore__41056 = this.restorePending;
        _currentParent = RestorationScope.maybeOf(this.context);
        bool didReplaceBucket__41159 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: needsRestore__41056);
        if (needsRestore__41056)
        {
            _doRestore(oldBucket__41020);
        }
        if (didReplaceBucket__41159)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__41020, this._bucket)));
            oldBucket__41020?.dispose();
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
        ScrollPhysics? newPhysics__27896 = ((((Scrollable)this.widget).physics ?? (ScrollPhysics)((Scrollable)this.widget).scrollBehavior?.getScrollPhysics(this.context)));
        ScrollPhysics? oldPhysics__27996 = ((((Scrollable)oldWidget).physics ?? (ScrollPhysics)((Scrollable)oldWidget).scrollBehavior?.getScrollPhysics(this.context)));
        do
        {
            if ((!object.Equals(DartRuntimePrimitives.RuntimeType(newPhysics__27896), DartRuntimePrimitives.RuntimeType(oldPhysics__27996))))
            {
                return true;
            }
            newPhysics__27896 = newPhysics__27896?.parent;
            oldPhysics__27996 = oldPhysics__27996?.parent;
        }
        while (((newPhysics__27896 is not null) || (oldPhysics__27996 is not null)));
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
        this._properties.forEach(((global::System.Action<dynamic, global::System.Action>)((property, listener) => {
if (!((dynamic)property)._disposed)
{
    property.removeListener((global::System.Action)(() => listener()));
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
                case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                    {
                        _gestureRecognizers = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Gestures.VerticalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Generated.Framework.Gestures.VerticalDragGestureRecognizer>(((global::System.Func<global::Doroti.Generated.Framework.Gestures.VerticalDragGestureRecognizer>)(() => new global::Doroti.Generated.Framework.Gestures.VerticalDragGestureRecognizer(supportedDevices: ((ScrollBehavior)this._configuration).dragDevices))), ((global::System.Action<global::Doroti.Generated.Framework.Gestures.VerticalDragGestureRecognizer>)((instance) => {
DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Gestures.VerticalDragGestureRecognizer>)(() =>
{            var __cascade = instance;
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
            return __cascade;        }))());
}))) };
                        break;
                    }
                case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                    {
                        _gestureRecognizers = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Gestures.HorizontalDragGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Generated.Framework.Gestures.HorizontalDragGestureRecognizer>(((global::System.Func<global::Doroti.Generated.Framework.Gestures.HorizontalDragGestureRecognizer>)(() => new global::Doroti.Generated.Framework.Gestures.HorizontalDragGestureRecognizer(supportedDevices: ((ScrollBehavior)this._configuration).dragDevices))), ((global::System.Action<global::Doroti.Generated.Framework.Gestures.HorizontalDragGestureRecognizer>)((instance) => {
DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Gestures.HorizontalDragGestureRecognizer>)(() =>
{            var __cascade = instance;
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
            return __cascade;        }))());
}))) };
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
            var renderBox__34287 = ((global::Doroti.Generated.Framework.Rendering.RenderIgnorePointer?)(object?)((GlobalKey<IState>)this._ignorePointerKey).currentContext!.findRenderObject()!)!;
            renderBox__34287.ignoring = this._shouldIgnorePointer;
        }
    }

    internal virtual void _handleDragDown(global::Doroti.Generated.Framework.Gestures.DragDownDetails details)
    {
        DartRuntimePrimitives.Assert(() => (this._drag is null));
        DartRuntimePrimitives.Assert(() => (this._hold is null));
        _hold = this.position.hold(() => this._disposeHold());
    }

    internal virtual void _handleDragStart(global::Doroti.Generated.Framework.Gestures.DragStartDetails details)
    {
        DartRuntimePrimitives.Assert(() => (this._drag is null));
        _drag = this.position.drag(details, () => this._disposeDrag());
        DartRuntimePrimitives.Assert(() => (this._drag is not null));
        if ((this._hold is not null))
        {
            _disposeHold();
        }
    }

    internal virtual void _handleDragUpdate(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => ((this._hold is null) || (this._drag is null)));
        this._drag?.update(details);
    }

    internal virtual void _handleDragEnd(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
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

    internal virtual double _pointerSignalEventDelta(global::Doroti.Generated.Framework.Gestures.PointerScrollEvent @event)
    {
        HashSet<global::Doroti.Generated.Framework.Services.LogicalKeyboardKey> pressed__36842 = global::Doroti.Generated.Framework.Services.HardwareKeyboard.instance.logicalKeysPressed;
        bool flipAxes__36913 = (pressed__36842.any(__item => ((ScrollBehavior)this._configuration).pointerAxisModifiers.Contains(__item)) && (object.Equals(@event.kind, PointerDeviceKind.mouse)));
        global::Doroti.Generated.Framework.Painting.Axis axis__37459 = (flipAxes__36913 ? global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.flipAxis(((Scrollable)this.widget).axis) : ((Scrollable)this.widget).axis);
        double delta__37531 = (axis__37459 switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => ((global::Doroti.Generated.Framework.Gestures.PointerScrollEvent)@event).scrollDelta.dx, global::Doroti.Generated.Framework.Painting.Axis.vertical => ((global::Doroti.Generated.Framework.Gestures.PointerScrollEvent)@event).scrollDelta.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((Scrollable)this.widget).axisDirection) ? -delta__37531 : delta__37531);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _receivedPointerSignal(global::Doroti.Generated.Framework.Gestures.PointerSignalEvent @event)
    {
        if (((@event is global::Doroti.Generated.Framework.Gestures.PointerScrollEvent) && (this._position is not null)))
        {
            global::Doroti.Generated.Framework.Gestures.PointerScrollEvent @event__as37801 = (global::Doroti.Generated.Framework.Gestures.PointerScrollEvent)@event;
            if (((this._physics is not null) && !this._physics!.shouldAcceptUserOffset(this.position)))
            {
                return;
            }
            double delta__37973 = _pointerSignalEventDelta(((global::Doroti.Generated.Framework.Gestures.PointerScrollEvent)@event__as37801));
            double targetScrollOffset__38033 = _targetScrollOffsetForPointerScroll(delta__37973);
            if (((delta__37973 != 0.0) && (targetScrollOffset__38033 != ((ScrollPosition)this.position).pixels)))
            {
                global::Doroti.Generated.Framework.Gestures.GestureBinding.instance.pointerSignalResolver.register(((global::Doroti.Generated.Framework.Gestures.PointerScrollEvent)@event__as37801), (__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEvent>)this._handlePointerScroll)(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Gestures.PointerEvent>(__arg0)));
                return;
            }
        }
        else
        {
            if ((@event is global::Doroti.Generated.Framework.Gestures.PointerScrollInertiaCancelEvent))
            {
                global::Doroti.Generated.Framework.Gestures.PointerScrollInertiaCancelEvent @event__as38382 = (global::Doroti.Generated.Framework.Gestures.PointerScrollInertiaCancelEvent)@event;
                this.position.pointerScroll(0);
            }
        }
    }

    internal virtual void _handlePointerScroll(global::Doroti.Generated.Framework.Gestures.PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (@event is global::Doroti.Generated.Framework.Gestures.PointerScrollEvent));
        var scrollEvent__38659 = ((global::Doroti.Generated.Framework.Gestures.PointerScrollEvent?)(object?)@event)!;
        double delta__38719 = _pointerSignalEventDelta(scrollEvent__38659);
        double targetScrollOffset__38783 = _targetScrollOffsetForPointerScroll(delta__38719);
        if (((delta__38719 != 0.0) && (targetScrollOffset__38783 != ((ScrollPosition)this.position).pixels)))
        {
            this.position.pointerScroll(delta__38719);
            scrollEvent__38659.respond(allowPlatformDefault: false);
        }
    }

    internal virtual bool _handleScrollMetricsNotification(ScrollMetricsNotification notification)
    {
        if ((notification.depth == 0L))
        {
            global::Doroti.Generated.Framework.Rendering.RenderObject? scrollSemanticsRenderObject__39297 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)((GlobalKey<IState>)this._scrollSemanticsKey).currentContext?.findRenderObject());
            if ((scrollSemanticsRenderObject__39297 is not null))
            {
                ((dynamic)scrollSemanticsRenderObject__39297).markNeedsSemanticsUpdate();
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildChrome(BuildContext context, Widget child)
    {
        var details__39614 = new ScrollableDetails(direction: ((Scrollable)this.widget).axisDirection, controller: this._effectiveScrollController, decorationClipBehavior: ((Scrollable)this.widget).clipBehavior);
        return ((Widget)(object?)this._configuration.buildScrollbar(context, this._configuration.buildOverscrollIndicator(context, child, details__39614), details__39614));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (this._position is not null));
        Widget result__40491 = ((Widget)(object?)new _ScrollableScope__scrollable(scrollable: this, position: this.position, child: new Listener(onPointerSignal: (global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerSignalEvent>)this._receivedPointerSignal, child: new RawGestureDetector(key: this._gestureDetectorKey, gestures: this._gestureRecognizers, behavior: ((Scrollable)this.widget).hitTestBehavior, excludeFromSemantics: ((Scrollable)this.widget).excludeFromSemantics, child: new Semantics(explicitChildNodes: !((Scrollable)this.widget).excludeFromSemantics, child: new IgnorePointer(key: this._ignorePointerKey, ignoring: this._shouldIgnorePointer, child: this.widget.viewportBuilder(context, this.position)))))));
        if (!((Scrollable)this.widget).excludeFromSemantics)
        {
            result__40491 = DartRuntimePrimitives.ConvertValue<Widget>(new NotificationListener<ScrollMetricsNotification>(onNotification: (global::System.Func<ScrollMetricsNotification, bool>)this._handleScrollMetricsNotification, child: new _ScrollSemantics__scrollable(key: this._scrollSemanticsKey, position: this.position, allowImplicitScrolling: this._physics!.allowImplicitScrolling, axis: ((Scrollable)this.widget).axis, semanticChildCount: ((Scrollable)this.widget).semanticChildCount, child: result__40491)));
        }
        result__40491 = _buildChrome(context, result__40491);
        global::Doroti.Generated.Framework.Rendering.SelectionRegistrar? registrar__41794 = ((global::Doroti.Generated.Framework.Rendering.SelectionRegistrar?)(object?)SelectionContainer.maybeOf(context));
        if ((registrar__41794 is not null))
        {
            result__40491 = DartRuntimePrimitives.ConvertValue<Widget>(new _ScrollableSelectionHandler__scrollable(state: this, position: this.position, registrar: registrar__41794, child: result__40491));
        }
        return result__40491;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (List<Future>, ScrollableState) _performEnsureVisible(global::Doroti.Generated.Framework.Rendering.RenderObject @object, double alignment = 0.0, Duration duration = default, global::Doroti.Generated.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, global::Doroti.Generated.Framework.Rendering.RenderObject? targetRenderObject = null)
    {
        Future ensureVisibleFuture__42614 = ((Future)(object?)this.position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy, targetRenderObject: targetRenderObject));
        return (new List<Future> { ensureVisibleFuture__42614 }, this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ScrollPosition>("position", this._position));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("effective physics", this._physics));
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public virtual global::Doroti.Generated.Framework.Services.RestorationBucket? bucket => this._bucket;
    public virtual void didToggleBucket(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
    {
        DartRuntimePrimitives.Assert(() => (this._bucket?.isReplacing != true));
    }

    public virtual void registerForRestoration(dynamic property, string restorationId)
    {
        DartRuntimePrimitives.Assert(() => ((((dynamic)property)._restorationId is null) || ((this._debugDoingRestore && (((dynamic)property)._restorationId == restorationId)))), () => (object?)$"Property is already registered under {((dynamic)property)._restorationId}.");
        DartRuntimePrimitives.Assert(() => (this._debugDoingRestore || !this._properties.Keys.map<dynamic, string?>(((r) => ((dynamic)r)._restorationId)).contains(restorationId)), () => (object?)$"\"{restorationId}\" is already registered to another property.");
        bool hasSerializedValue__36723 = (this.bucket?.contains(restorationId) ?? false);
        object? initialValue__36804 = (hasSerializedValue__36723 ? property.fromPrimitives(this.bucket!.read<object>(restorationId)) : property.createDefaultValue());
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
            property.addListener((global::System.Action)(() => listener()));
            this._properties[property] = (global::System.Action)listener;
        }
        DartRuntimePrimitives.Assert(() => (((((dynamic)property)._restorationId == restorationId) && (object.Equals(((dynamic)property)._owner, this))) && this._properties.ContainsKey(property)));
        property.initWithValue((dynamic)initialValue__36804);
        if (((!hasSerializedValue__36723 && ((dynamic)property).enabled) && (this.bucket is not null)))
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
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__39230 = this._bucket;
        DartRuntimePrimitives.Assert(() => !this.restorePending);
        bool didReplaceBucket__39295 = _updateBucketIfNecessary(parent: this._currentParent, restorePending: false);
        if (didReplaceBucket__39295)
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(oldBucket__39230, this._bucket)));
            DartRuntimePrimitives.Assert(() => ((this._bucket is null) || (oldBucket__39230 is null)));
            oldBucket__39230?.dispose();
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
            global::Doroti.Generated.Framework.Services.RestorationBucket? potentialNewParent__40517 = ((global::Doroti.Generated.Framework.Services.RestorationBucket?)(object?)RestorationScope.maybeOf(this.context));
            return ((!object.Equals(potentialNewParent__40517, this._currentParent)) && ((potentialNewParent__40517?.isReplacing ?? false)));
            return default!;
        }
    }
    public virtual bool _debugDoingRestore => DartRuntimePrimitives.ConvertValue<bool>((this._debugPropertiesWaitingForReregistration is not null));
    public virtual void _doRestore(global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket)
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Previously registered RestorableProperties must be re-registered in \"restoreState\"."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The RestorableProperties with the following IDs were not re-registered to {this} when " + "\"restoreState\" was called:") }));
                }
                this._debugPropertiesWaitingForReregistration = null;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public virtual bool _updateBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? parent, bool restorePending)
    {
        if (((this.restorationId is null) || (parent is null)))
        {
            bool didReplace__42801 = _setNewBucketIfNecessary(newBucket: ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object)null), restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (this._bucket is null));
            return didReplace__42801;
        }
        DartRuntimePrimitives.Assert(() => (this.restorationId is not null));
        if ((restorePending || (this._bucket is null)))
        {
            global::Doroti.Generated.Framework.Services.RestorationBucket newBucket__43086 = ((global::Doroti.Generated.Framework.Services.RestorationBucket)(object?)parent.claimChild(this.restorationId!, debugOwner: this));
            bool didReplace__43168 = _setNewBucketIfNecessary(newBucket: newBucket__43086, restorePending: restorePending);
            DartRuntimePrimitives.Assert(() => (object.Equals(this._bucket, newBucket__43086)));
            return didReplace__43168;
        }
        DartRuntimePrimitives.Assert(() => (this._bucket is not null));
        DartRuntimePrimitives.Assert(() => !restorePending);
        this._bucket!.rename(this.restorationId!);
        parent.adoptChild(this._bucket!);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _setNewBucketIfNecessary(global::Doroti.Generated.Framework.Services.RestorationBucket? newBucket, bool restorePending)
    {
        if ((object.Equals(newBucket, this._bucket)))
        {
            return false;
        }
        global::Doroti.Generated.Framework.Services.RestorationBucket? oldBucket__43946 = this._bucket;
        this._bucket = newBucket;
        if (!restorePending)
        {
            if ((this._bucket is not null))
            {
                this._properties.Keys.forEach((__arg0) => ((global::System.Action<dynamic>)this._updateProperty)(__arg0));
            }
            didToggleBucket(oldBucket__43946);
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
        global::System.Action listener__44576 = this._properties.remove(property)!;
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugPropertiesWaitingForReregistration?.Remove(property);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        property.removeListener((global::System.Action)(() => listener__44576()));
        property._unregister();
    }

}

public class _ScrollableSelectionHandler__scrollable : StatefulWidget
{
    public virtual ScrollableState state { get; private set; } = default!;
    public virtual ScrollPosition position { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.SelectionRegistrar registrar { get; private set; } = default!;

    internal _ScrollableSelectionHandler__scrollable(ScrollableState state, ScrollPosition position, global::Doroti.Generated.Framework.Rendering.SelectionRegistrar registrar, Widget child)
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
    internal virtual DartMap<global::Doroti.Generated.Framework.Rendering.Selectable, double> _selectableStartEdgeUpdateRecords { get; private set; } = new DartMap<global::Doroti.Generated.Framework.Rendering.Selectable, double>();
    internal virtual DartMap<global::Doroti.Generated.Framework.Rendering.Selectable, double> _selectableEndEdgeUpdateRecords { get; private set; } = new DartMap<global::Doroti.Generated.Framework.Rendering.Selectable, double>();

    internal _ScrollableSelectionContainerDelegate__scrollable(ScrollableState state, ScrollPosition position)
    {
        this.state = state;
        this._position = position;
        this._autoScroller = new EdgeDraggingAutoScroller(state, velocityScalar: _kDefaultSelectToScrollVelocityScalar);
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
            this._position.removeListener(() => this._scheduleLayoutChange());
            _position = other;
            this._position.addListener(() => this._scheduleLayoutChange());
        }
    }
    internal virtual void _scheduleLayoutChange()
    {
        if (this._scheduledLayoutChange)
        {
            return;
        }
        _scheduledLayoutChange = true;
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
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
        HashSet<global::Doroti.Generated.Framework.Rendering.Selectable> selectableSet__47989 = this.selectables.toSet();
        this._selectableStartEdgeUpdateRecords.removeWhere(((key, value) => !selectableSet__47989.Contains(key)));
        this._selectableEndEdgeUpdateRecords.removeWhere(((key, value) => !selectableSet__47989.Contains(key)));
        base.didChangeSelectables();
    }

    public override global::Doroti.Generated.Framework.Rendering.SelectionResult handleClearSelection(global::Doroti.Generated.Framework.Rendering.ClearSelectionEvent @event)
    {
        this._selectableStartEdgeUpdateRecords.Clear();
        this._selectableEndEdgeUpdateRecords.Clear();
        _currentDragStartRelatedToOrigin = null;
        _currentDragEndRelatedToOrigin = null;
        _selectionStartsInScrollable = false;
        return base.handleClearSelection(@event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.SelectionResult handleSelectionEdgeUpdate(global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent @event)
    {
        if (((this._currentDragEndRelatedToOrigin is null) && (this._currentDragStartRelatedToOrigin is null)))
        {
            DartRuntimePrimitives.Assert(() => !this._selectionStartsInScrollable);
            _selectionStartsInScrollable = _globalPositionInScrollable(((global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition);
        }
        global::Doroti.Flutter.Ui.Offset deltaToOrigin__49012 = ((global::Doroti.Flutter.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
        if ((object.Equals(@event.type, global::Doroti.Generated.Framework.Rendering.SelectionEventType.endEdgeUpdate)))
        {
            _currentDragEndRelatedToOrigin = _inferPositionRelatedToOrigin(((global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition);
            global::Doroti.Flutter.Ui.Offset endOffset__49229 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this._currentDragEndRelatedToOrigin).translate(-deltaToOrigin__49012.dx, -deltaToOrigin__49012.dy));
            @event = global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: endOffset__49229, granularity: ((global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent)@event).granularity);
        }
        else
        {
            _currentDragStartRelatedToOrigin = _inferPositionRelatedToOrigin(((global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition);
            global::Doroti.Flutter.Ui.Offset startOffset__49604 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this._currentDragStartRelatedToOrigin).translate(-deltaToOrigin__49012.dx, -deltaToOrigin__49012.dy));
            @event = new global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: startOffset__49604, granularity: ((global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent)@event).granularity);
        }
        global::Doroti.Generated.Framework.Rendering.SelectionResult result__49893 = base.handleSelectionEdgeUpdate(@event);
        if ((object.Equals(result__49893, global::Doroti.Generated.Framework.Rendering.SelectionResult.pending)))
        {
            this._autoScroller.stopAutoScroll();
            return result__49893;
        }
        if (this._selectionStartsInScrollable)
        {
            this._autoScroller.startAutoScrollIfNecessary(_dragTargetFromEvent(@event));
            if (((EdgeDraggingAutoScroller)this._autoScroller).scrolling)
            {
                return global::Doroti.Generated.Framework.Rendering.SelectionResult.pending;
            }
        }
        return result__49893;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Offset _inferPositionRelatedToOrigin(Offset globalPosition)
    {
        var box__50540 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.state.context.findRenderObject()!)!;
        global::Doroti.Flutter.Ui.Offset localPosition__50611 = ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)box__50540).globalToLocal(globalPosition)));
        if (!this._selectionStartsInScrollable)
        {
            if (((localPosition__50611.dy < 0L) || (localPosition__50611.dx < 0L)))
            {
                return ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)box__50540).localToGlobal(Offset.zero)));
            }
            if (((localPosition__50611.dy > ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__50540).size.height) || (localPosition__50611.dx > ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__50540).size.width)))
            {
                return Offset.infinite;
            }
        }
        global::Doroti.Flutter.Ui.Offset deltaToOrigin__51282 = ((global::Doroti.Flutter.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
        return ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)box__50540).localToGlobal(localPosition__50611.translate(deltaToOrigin__51282.dx, deltaToOrigin__51282.dy))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateDragLocationsFromGeometries(bool forceUpdateStart = true, bool forceUpdateEnd = true)
    {
        global::Doroti.Flutter.Ui.Offset deltaToOrigin__51957 = ((global::Doroti.Flutter.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
        var box__52015 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.state.context.findRenderObject()!)!;
        Matrix4 transform__52087 = ((Matrix4)(object?)box__52015.getTransformTo(((global::Doroti.Generated.Framework.Rendering.RenderObject)(object)null)));
        if (((this.currentSelectionStartIndex != -1L) && (((this._currentDragStartRelatedToOrigin is null) || forceUpdateStart))))
        {
            global::Doroti.Generated.Framework.Rendering.SelectionGeometry geometry__52273 = this.selectables[(int)(this.currentSelectionStartIndex)].value;
            DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Rendering.SelectionGeometry)geometry__52273).hasSelection);
            global::Doroti.Generated.Framework.Rendering.SelectionPoint start__52395 = ((global::Doroti.Generated.Framework.Rendering.SelectionGeometry)geometry__52273).startSelectionPoint!;
            Matrix4 childTransform__52454 = ((Matrix4)(object?)this.selectables[(int)(this.currentSelectionStartIndex)].getTransformTo(box__52015));
            global::Doroti.Flutter.Ui.Offset localDragStart__52551 = ((global::Doroti.Flutter.Ui.Offset)(object?)MatrixUtils.transformPoint(childTransform__52454, (((global::Doroti.Generated.Framework.Rendering.SelectionPoint)start__52395).localPosition + new global::Doroti.Flutter.Ui.Offset(0, (-((global::Doroti.Generated.Framework.Rendering.SelectionPoint)start__52395).lineHeight / 2L)))));
            _currentDragStartRelatedToOrigin = MatrixUtils.transformPoint(transform__52087, (localDragStart__52551 + deltaToOrigin__51957));
        }
        if (((this.currentSelectionEndIndex != -1L) && (((this._currentDragEndRelatedToOrigin is null) || forceUpdateEnd))))
        {
            global::Doroti.Generated.Framework.Rendering.SelectionGeometry geometry__52978 = this.selectables[(int)(this.currentSelectionEndIndex)].value;
            DartRuntimePrimitives.Assert(() => ((global::Doroti.Generated.Framework.Rendering.SelectionGeometry)geometry__52978).hasSelection);
            global::Doroti.Generated.Framework.Rendering.SelectionPoint end__53098 = ((global::Doroti.Generated.Framework.Rendering.SelectionGeometry)geometry__52978).endSelectionPoint!;
            Matrix4 childTransform__53153 = ((Matrix4)(object?)this.selectables[(int)(this.currentSelectionEndIndex)].getTransformTo(box__52015));
            global::Doroti.Flutter.Ui.Offset localDragEnd__53248 = ((global::Doroti.Flutter.Ui.Offset)(object?)MatrixUtils.transformPoint(childTransform__53153, (((global::Doroti.Generated.Framework.Rendering.SelectionPoint)end__53098).localPosition + new global::Doroti.Flutter.Ui.Offset(0, (-((global::Doroti.Generated.Framework.Rendering.SelectionPoint)end__53098).lineHeight / 2L)))));
            _currentDragEndRelatedToOrigin = MatrixUtils.transformPoint(transform__52087, (localDragEnd__53248 + deltaToOrigin__51957));
        }
    }

    public override global::Doroti.Generated.Framework.Rendering.SelectionResult handleSelectAll(global::Doroti.Generated.Framework.Rendering.SelectAllSelectionEvent @event)
    {
        DartRuntimePrimitives.Assert(() => !this._selectionStartsInScrollable);
        global::Doroti.Generated.Framework.Rendering.SelectionResult result__53676 = base.handleSelectAll(@event);
        DartRuntimePrimitives.Assert(() => (((this.currentSelectionStartIndex == -1L)) == ((this.currentSelectionEndIndex == -1L))));
        if ((this.currentSelectionStartIndex != -1L))
        {
            _updateDragLocationsFromGeometries();
        }
        return result__53676;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.SelectionResult handleSelectWord(global::Doroti.Generated.Framework.Rendering.SelectWordSelectionEvent @event)
    {
        _selectionStartsInScrollable = _globalPositionInScrollable(((global::Doroti.Generated.Framework.Rendering.SelectWordSelectionEvent)@event).globalPosition);
        global::Doroti.Generated.Framework.Rendering.SelectionResult result__54110 = base.handleSelectWord(@event);
        _updateDragLocationsFromGeometries();
        return result__54110;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.SelectionResult handleGranularlyExtendSelection(global::Doroti.Generated.Framework.Rendering.GranularlyExtendSelectionEvent @event)
    {
        global::Doroti.Generated.Framework.Rendering.SelectionResult result__54344 = base.handleGranularlyExtendSelection(@event);
        _updateDragLocationsFromGeometries(forceUpdateStart: !((global::Doroti.Generated.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd, forceUpdateEnd: ((global::Doroti.Generated.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd);
        if (this._selectionStartsInScrollable)
        {
            _jumpToEdge(((global::Doroti.Generated.Framework.Rendering.GranularlyExtendSelectionEvent)@event).isEnd);
        }
        return result__54344;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.SelectionResult handleDirectionallyExtendSelection(global::Doroti.Generated.Framework.Rendering.DirectionallyExtendSelectionEvent @event)
    {
        global::Doroti.Generated.Framework.Rendering.SelectionResult result__54955 = base.handleDirectionallyExtendSelection(@event);
        _updateDragLocationsFromGeometries(forceUpdateStart: !((global::Doroti.Generated.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd, forceUpdateEnd: ((global::Doroti.Generated.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd);
        if (this._selectionStartsInScrollable)
        {
            _jumpToEdge(((global::Doroti.Generated.Framework.Rendering.DirectionallyExtendSelectionEvent)@event).isEnd);
        }
        return result__54955;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _jumpToEdge(bool isExtent)
    {
        global::Doroti.Generated.Framework.Rendering.Selectable selectable__55492 = default!;
        double? lineHeight__55522 = default!;
        global::Doroti.Generated.Framework.Rendering.SelectionPoint? edge__55560 = default!;
        if (isExtent)
        {
            selectable__55492 = this.selectables[(int)(this.currentSelectionEndIndex)];
            edge__55560 = selectable__55492.value.endSelectionPoint;
            lineHeight__55522 = selectable__55492.value.endSelectionPoint!.lineHeight;
        }
        else
        {
            selectable__55492 = this.selectables[(int)(this.currentSelectionStartIndex)];
            edge__55560 = selectable__55492.value.startSelectionPoint;
            lineHeight__55522 = selectable__55492.value.startSelectionPoint?.lineHeight;
        }
        if (((lineHeight__55522 is null) || (edge__55560 is null)))
        {
            return;
        }
        var scrollableBox__56035 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.state.context.findRenderObject()!)!;
        Matrix4 transform__56117 = ((Matrix4)(object?)selectable__55492.getTransformTo(scrollableBox__56035));
        global::Doroti.Flutter.Ui.Offset edgeOffsetInScrollableCoordinates__56188 = ((global::Doroti.Flutter.Ui.Offset)(object?)MatrixUtils.transformPoint(transform__56117, ((global::Doroti.Generated.Framework.Rendering.SelectionPoint)edge__55560).localPosition));
        var scrollableRect__56312 = global::Doroti.Flutter.Ui.Rect.fromLTRB(0, 0, ((global::Doroti.Generated.Framework.Rendering.RenderBox)scrollableBox__56035).size.width, ((global::Doroti.Generated.Framework.Rendering.RenderBox)scrollableBox__56035).size.height);
        switch (((ScrollableState)this.state).axisDirection)
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
                {
                    double edgeBottom__56488 = edgeOffsetInScrollableCoordinates__56188.dy;
                    double edgeTop__56560 = (edgeOffsetInScrollableCoordinates__56188.dy - DartRuntimePrimitives.RequireValue(lineHeight__55522));
                    if (((edgeBottom__56488 >= scrollableRect__56312.bottom) && (edgeTop__56560 <= scrollableRect__56312.top)))
                    {
                        return;
                    }
                    if ((edgeBottom__56488 > scrollableRect__56312.bottom))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + scrollableRect__56312.bottom) - edgeBottom__56488));
                        return;
                    }
                    if ((edgeTop__56560 < scrollableRect__56312.top))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + scrollableRect__56312.top) - edgeTop__56560));
                    }
                    return;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
                {
                    double edge__57090 = edgeOffsetInScrollableCoordinates__56188.dx;
                    if (((edge__57090 >= scrollableRect__56312.right) && (edge__57090 <= scrollableRect__56312.left)))
                    {
                        return;
                    }
                    if ((edge__57090 > scrollableRect__56312.right))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + edge__57090) - scrollableRect__56312.right));
                        return;
                    }
                    if ((edge__57090 < scrollableRect__56312.left))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + edge__57090) - scrollableRect__56312.left));
                    }
                    return;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                {
                    double edgeBottom__57576 = edgeOffsetInScrollableCoordinates__56188.dy;
                    double edgeTop__57648 = (edgeOffsetInScrollableCoordinates__56188.dy - DartRuntimePrimitives.RequireValue(lineHeight__55522));
                    if (((edgeBottom__57576 >= scrollableRect__56312.bottom) && (edgeTop__57648 <= scrollableRect__56312.top)))
                    {
                        return;
                    }
                    if ((edgeBottom__57576 > scrollableRect__56312.bottom))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + edgeBottom__57576) - scrollableRect__56312.bottom));
                        return;
                    }
                    if ((edgeTop__57648 < scrollableRect__56312.top))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + edgeTop__57648) - scrollableRect__56312.top));
                    }
                    return;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                {
                    double edge__58177 = edgeOffsetInScrollableCoordinates__56188.dx;
                    if (((edge__58177 >= scrollableRect__56312.right) && (edge__58177 <= scrollableRect__56312.left)))
                    {
                        return;
                    }
                    if ((edge__58177 > scrollableRect__56312.right))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + scrollableRect__56312.right) - edge__58177));
                        return;
                    }
                    if ((edge__58177 < scrollableRect__56312.left))
                    {
                        this.position.jumpTo(((((ScrollPosition)this.position).pixels + scrollableRect__56312.left) - edge__58177));
                    }
                    return;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    internal virtual bool _globalPositionInScrollable(Offset globalPosition)
    {
        var box__58692 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.state.context.findRenderObject()!)!;
        global::Doroti.Flutter.Ui.Offset localPosition__58763 = ((global::Doroti.Flutter.Ui.Offset)(object?)((Offset)((dynamic)box__58692).globalToLocal(globalPosition)));
        var rect__58824 = global::Doroti.Flutter.Ui.Rect.fromLTWH(0, 0, ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__58692).size.width, ((global::Doroti.Generated.Framework.Rendering.RenderBox)box__58692).size.height);
        return rect__58824.contains(localPosition__58763);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Flutter.Ui.Rect _dragTargetFromEvent(global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent @event)
    {
        return global::Doroti.Flutter.Ui.Rect.fromCenter(center: ((global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent)@event).globalPosition, width: _kDefaultDragTargetSize, height: _kDefaultDragTargetSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.SelectionResult dispatchSelectionEventToChild(global::Doroti.Generated.Framework.Rendering.Selectable selectable, global::Doroti.Generated.Framework.Rendering.SelectionEvent @event)
    {
        switch (((global::Doroti.Generated.Framework.Rendering.SelectionEvent)@event).type)
        {
            case global::Doroti.Generated.Framework.Rendering.SelectionEventType.startEdgeUpdate:
                {
                    this._selectableStartEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    ensureChildUpdated(selectable);
                    break;
                }
            case global::Doroti.Generated.Framework.Rendering.SelectionEventType.endEdgeUpdate:
                {
                    this._selectableEndEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    ensureChildUpdated(selectable);
                    break;
                }
            case global::Doroti.Generated.Framework.Rendering.SelectionEventType.granularlyExtendSelection:
            case global::Doroti.Generated.Framework.Rendering.SelectionEventType.directionallyExtendSelection:
                {
                    ensureChildUpdated(selectable);
                    this._selectableStartEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    this._selectableEndEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    break;
                }
            case global::Doroti.Generated.Framework.Rendering.SelectionEventType.clear:
                {
                    this._selectableEndEdgeUpdateRecords.remove(selectable);
                    this._selectableStartEdgeUpdateRecords.remove(selectable);
                    break;
                }
            case global::Doroti.Generated.Framework.Rendering.SelectionEventType.selectAll:
            case global::Doroti.Generated.Framework.Rendering.SelectionEventType.selectWord:
            case global::Doroti.Generated.Framework.Rendering.SelectionEventType.selectParagraph:
                {
                    this._selectableEndEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    this._selectableStartEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
                    break;
                }
        }
        return base.dispatchSelectionEventToChild(selectable, @event);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void ensureChildUpdated(global::Doroti.Generated.Framework.Rendering.Selectable selectable)
    {
        double newRecord__60523 = ((ScrollableState)this.state).position.pixels;
        double? previousStartRecord__60576 = DartCollectionRuntime.NullableMapValue<double>(this._selectableStartEdgeUpdateRecords, selectable);
        if (((this._currentDragStartRelatedToOrigin is not null) && (((previousStartRecord__60576 is null) || (((newRecord__60523 - DartRuntimePrimitives.RequireValue(previousStartRecord__60576))).abs() > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))))
        {
            global::Doroti.Flutter.Ui.Offset deltaToOrigin__60895 = ((global::Doroti.Flutter.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
            global::Doroti.Flutter.Ui.Offset startOffset__60962 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this._currentDragStartRelatedToOrigin).translate(-deltaToOrigin__60895.dx, -deltaToOrigin__60895.dy));
            selectable.dispatchSelectionEvent(new global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent(globalPosition: startOffset__60962));
            this._selectableStartEdgeUpdateRecords[selectable] = ((ScrollableState)this.state).position.pixels;
        }
        double? previousEndRecord__61449 = DartCollectionRuntime.NullableMapValue<double>(this._selectableEndEdgeUpdateRecords, selectable);
        if (((this._currentDragEndRelatedToOrigin is not null) && (((previousEndRecord__61449 is null) || (((newRecord__60523 - DartRuntimePrimitives.RequireValue(previousEndRecord__61449))).abs() > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))))
        {
            global::Doroti.Flutter.Ui.Offset deltaToOrigin__61758 = ((global::Doroti.Flutter.Ui.Offset)(object?)ScrollableLibrary._getDeltaToScrollOrigin(this.state));
            global::Doroti.Flutter.Ui.Offset endOffset__61825 = ((global::Doroti.Flutter.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(this._currentDragEndRelatedToOrigin).translate(-deltaToOrigin__61758.dx, -deltaToOrigin__61758.dy));
            selectable.dispatchSelectionEvent(global::Doroti.Generated.Framework.Rendering.SelectionEdgeUpdateEvent.CreateForEnd(globalPosition: endOffset__61825));
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
        return (((ScrollableState)scrollableState).axisDirection switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => new global::Doroti.Flutter.Ui.Offset(0, -((ScrollableState)scrollableState).position.pixels), global::Doroti.Generated.Framework.Painting.AxisDirection.down => new global::Doroti.Flutter.Ui.Offset(0, ((ScrollableState)scrollableState).position.pixels), global::Doroti.Generated.Framework.Painting.AxisDirection.left => new global::Doroti.Flutter.Ui.Offset(-((ScrollableState)scrollableState).position.pixels, 0), global::Doroti.Generated.Framework.Painting.AxisDirection.right => new global::Doroti.Flutter.Ui.Offset(((ScrollableState)scrollableState).position.pixels, 0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ScrollSemantics__scrollable : SingleChildRenderObjectWidget
{
    public virtual ScrollPosition position { get; private set; } = default!;
    public virtual bool allowImplicitScrolling { get; private set; } = default!;
    public virtual long? semanticChildCount { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Axis axis { get; private set; } = default!;

    internal _ScrollSemantics__scrollable(global::Doroti.Generated.Framework.Foundation.Key? key = null, ScrollPosition position = default!, bool allowImplicitScrolling = default!, global::Doroti.Generated.Framework.Painting.Axis axis = default!, long? semanticChildCount = default!, Widget? child = null) : base(key: key, child: child)
    {
        this.position = position;
        this.allowImplicitScrolling = allowImplicitScrolling;
        this.axis = axis;
        this.semanticChildCount = semanticChildCount;
        System.Diagnostics.Debug.Assert(((semanticChildCount is null) || (semanticChildCount >= 0L)));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderScrollSemantics__scrollable(position: this.position, allowImplicitScrolling: this.allowImplicitScrolling, semanticChildCount: this.semanticChildCount, axis: this.axis));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderScrollSemantics__scrollable)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderScrollSemantics__scrollable>)(() =>
{            var __cascade = __renderObject;
            __cascade.allowImplicitScrolling = this.allowImplicitScrolling;
            __cascade.axis = this.axis;
            __cascade.position = this.position;
            __cascade.semanticChildCount = this.semanticChildCount;
            return __cascade;        }))());
    }

}

public class _RenderScrollSemantics__scrollable : global::Doroti.Generated.Framework.Rendering.RenderProxyBox
{
    internal virtual ScrollPosition _position { get; set; } = default!;
    internal virtual bool _allowImplicitScrolling { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.Axis axis { get; set; } = default!;
    internal virtual long? _semanticChildCount { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Semantics.SemanticsNode? _innerNode { get; set; } = default;

    internal _RenderScrollSemantics__scrollable(ScrollPosition position, bool allowImplicitScrolling, global::Doroti.Generated.Framework.Painting.Axis axis, long? semanticChildCount, global::Doroti.Generated.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this.axis = axis;
        this._position = position;
        this._allowImplicitScrolling = allowImplicitScrolling;
        this._semanticChildCount = semanticChildCount;
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
            this._position.removeListener(() => this.markNeedsSemanticsUpdate());
            _position = __value;
            this._position.addListener(() => this.markNeedsSemanticsUpdate());
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
        double offset__66181 = (this.axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => targetOffset.dx, global::Doroti.Generated.Framework.Painting.Axis.vertical => targetOffset.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        this._position.jumpTo(offset__66181);
    }

    public override void describeSemanticsConfiguration(global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)(() =>
{            var __cascade = config;
            __cascade.isSemanticBoundary = true;
            __cascade.hasImplicitScrolling = this.allowImplicitScrolling;
            return __cascade;        }))());
        if (((ScrollPosition)this.position).haveDimensions)
        {
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration>)(() =>
{            var __cascade = config;
            __cascade.scrollPosition = ((ScrollPosition)this._position).pixels;
            __cascade.scrollExtentMax = ((ScrollPosition)this._position).maxScrollExtent;
            __cascade.scrollExtentMin = ((ScrollPosition)this._position).minScrollExtent;
            __cascade.scrollChildCount = this.semanticChildCount;
            return __cascade;        }))());
            if (((((ScrollPosition)this.position).maxScrollExtent > ((ScrollPosition)this.position).minScrollExtent) && this.allowImplicitScrolling))
            {
                config.onScrollToOffset = (global::System.Action<Offset>)this._onScrollToOffset;
            }
        }
    }

    public override void assembleSemanticsNode(global::Doroti.Generated.Framework.Semantics.SemanticsNode node, global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Generated.Framework.Semantics.SemanticsNode> children)
    {
        if ((!System.Linq.Enumerable.Any(children) || !children.First().isTagged(global::Doroti.Generated.Framework.Rendering.RenderViewport.useTwoPaneSemantics)))
        {
            _innerNode = null;
            base.assembleSemanticsNode(node, config, children.Cast<global::Doroti.Generated.Framework.Semantics.SemanticsNode>());
            return;
        }
        (_innerNode ??= new global::Doroti.Generated.Framework.Semantics.SemanticsNode(showOnScreen: () => this.showOnScreen())).rect = ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)node).rect;
        long? firstVisibleIndex__67438 = default!;
        var excluded__67467 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode> { this._innerNode! };
        var included__67518 = new List<global::Doroti.Generated.Framework.Semantics.SemanticsNode>();
        foreach (var child__67563 in children)
        {
            DartRuntimePrimitives.Assert(() => child__67563.isTagged(global::Doroti.Generated.Framework.Rendering.RenderViewport.useTwoPaneSemantics));
            if (child__67563.isTagged(global::Doroti.Generated.Framework.Rendering.RenderViewport.excludeFromScrolling))
            {
                excluded__67467.Add(child__67563);
            }
            else
            {
                if (!((global::Doroti.Generated.Framework.Semantics.SemanticsNode)child__67563).flagsCollection.isHidden)
                {
                    firstVisibleIndex__67438 ??= ((global::Doroti.Generated.Framework.Semantics.SemanticsNode)child__67563).indexInParent;
                }
                included__67518.Add(child__67563);
            }
        }
        config.scrollIndex = firstVisibleIndex__67438;
        node.updateWith(config: ((global::Doroti.Generated.Framework.Semantics.SemanticsConfiguration)(object)null), childrenInInversePaintOrder: excluded__67467);
        this._innerNode!.updateWith(config: config, childrenInInversePaintOrder: included__67518);
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
    public virtual global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget> viewportBuilder { get; private set; } = default!;
    public virtual global::System.Func<ScrollIncrementDetails, double>? incrementCalculator { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual bool excludeFromSemantics { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;

    public TwoDimensionalScrollable(global::Doroti.Generated.Framework.Foundation.Key? key = null, ScrollableDetails horizontalDetails = default!, ScrollableDetails verticalDetails = default!, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::System.Func<ScrollIncrementDetails, double>? incrementCalculator = null, string? restorationId = null, bool excludeFromSemantics = false, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque) : base(key: key)
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
        _TwoDimensionalScrollableScope__scrollable? widget__74462 = ((_TwoDimensionalScrollableScope__scrollable?)(object?)context.dependOnInheritedWidgetOfExactType<_TwoDimensionalScrollableScope__scrollable>());
        return widget__74462?.twoDimensionalScrollable;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TwoDimensionalScrollableState of(BuildContext context)
    {
        TwoDimensionalScrollableState? scrollableState__75717 = ((TwoDimensionalScrollableState?)(object?)TwoDimensionalScrollable.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((scrollableState__75717 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("TwoDimensionalScrollable.of() was called with a context that does " + "not contain a TwoDimensionalScrollable widget.\n"), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("No TwoDimensionalScrollable widget ancestor could be found starting " + "from the context that was passed to TwoDimensionalScrollable.of(). " + "This can happen because you are using a widget that looks for a " + "TwoDimensionalScrollable ancestor, but no such ancestor exists.\n" + "The context used was:\n" + $"  {context}") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return scrollableState__75717!;
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
        DartRuntimePrimitives.Assert(() => (object.Equals(global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((TwoDimensionalScrollable)this.widget).verticalDetails.direction), global::Doroti.Generated.Framework.Painting.Axis.vertical)), () => (object?)"TwoDimensionalScrollable.verticalDetails are not Axis.vertical.");
        DartRuntimePrimitives.Assert(() => (object.Equals(global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((TwoDimensionalScrollable)this.widget).horizontalDetails.direction), global::Doroti.Generated.Framework.Painting.Axis.horizontal)), () => (object?)"TwoDimensionalScrollable.horizontalDetails are not Axis.horizontal.");
        Widget result__80791 = ((Widget)(object?)new RestorationScope(restorationId: ((TwoDimensionalScrollable)this.widget).restorationId, child: new _VerticalOuterDimension__scrollable(key: this._verticalOuterScrollableKey, horizontalKey: this._horizontalInnerScrollableKey, axisDirection: ((TwoDimensionalScrollable)this.widget).verticalDetails.direction, controller: (((TwoDimensionalScrollable)this.widget).verticalDetails.controller ?? this._verticalFallbackController!), physics: ((TwoDimensionalScrollable)this.widget).verticalDetails.physics, clipBehavior: ((((TwoDimensionalScrollable)this.widget).verticalDetails.clipBehavior ?? ((TwoDimensionalScrollable)this.widget).verticalDetails.decorationClipBehavior) ?? Clip.hardEdge), incrementCalculator: (global::System.Func<ScrollIncrementDetails, double>?)((TwoDimensionalScrollable)this.widget).incrementCalculator, excludeFromSemantics: ((TwoDimensionalScrollable)this.widget).excludeFromSemantics, restorationId: "OuterVerticalTwoDimensionalScrollable", dragStartBehavior: ((TwoDimensionalScrollable)this.widget).dragStartBehavior, diagonalDragBehavior: ((TwoDimensionalScrollable)this.widget).diagonalDragBehavior, hitTestBehavior: ((TwoDimensionalScrollable)this.widget).hitTestBehavior, viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget>)((context, verticalOffset) => {
return ((Widget)(object?)new _HorizontalInnerDimension__scrollable(key: this._horizontalInnerScrollableKey, verticalOuterKey: this._verticalOuterScrollableKey, axisDirection: ((TwoDimensionalScrollable)this.widget).horizontalDetails.direction, controller: (((TwoDimensionalScrollable)this.widget).horizontalDetails.controller ?? this._horizontalFallbackController!), physics: ((TwoDimensionalScrollable)this.widget).horizontalDetails.physics, clipBehavior: ((((TwoDimensionalScrollable)this.widget).horizontalDetails.clipBehavior ?? ((TwoDimensionalScrollable)this.widget).horizontalDetails.decorationClipBehavior) ?? Clip.hardEdge), incrementCalculator: (global::System.Func<ScrollIncrementDetails, double>?)((TwoDimensionalScrollable)this.widget).incrementCalculator, excludeFromSemantics: ((TwoDimensionalScrollable)this.widget).excludeFromSemantics, restorationId: "InnerHorizontalTwoDimensionalScrollable", dragStartBehavior: ((TwoDimensionalScrollable)this.widget).dragStartBehavior, diagonalDragBehavior: ((TwoDimensionalScrollable)this.widget).diagonalDragBehavior, hitTestBehavior: ((TwoDimensionalScrollable)this.widget).hitTestBehavior, viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget>)((context, horizontalOffset) => {
return this.widget.viewportBuilder(context, verticalOffset, horizontalOffset);
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        return ((Widget)(object?)new _TwoDimensionalScrollableScope__scrollable(twoDimensionalScrollable: this, child: result__80791));
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

    internal _VerticalOuterDimension__scrollable(global::Doroti.Generated.Framework.Foundation.Key? key = null, GlobalKey<ScrollableState> horizontalKey = default!, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection = default!, ScrollController? controller = null, ScrollPhysics? physics = null, Clip clipBehavior = Clip.hardEdge, global::System.Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, string? restorationId = null, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none) : base(key: key, viewportBuilder: viewportBuilder, axisDirection: axisDirection, controller: controller, physics: physics, clipBehavior: clipBehavior, incrementCalculator: incrementCalculator, excludeFromSemantics: excludeFromSemantics, dragStartBehavior: dragStartBehavior, restorationId: restorationId, hitTestBehavior: hitTestBehavior)
    {
        this.horizontalKey = horizontalKey;
        this.diagonalDragBehavior = diagonalDragBehavior;
        System.Diagnostics.Debug.Assert(((object.Equals(axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.up)) || (object.Equals(axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.down))));
    }

    public override _VerticalOuterDimensionState__scrollable createState() => new _VerticalOuterDimensionState__scrollable();
}

internal class _VerticalOuterDimensionState__scrollable : ScrollableState
{
    public virtual global::Doroti.Generated.Framework.Painting.Axis? lockedAxis { get; set; } = default;
    public virtual Offset? lastDragOffset { get; set; } = default;

    public virtual DiagonalDragBehavior diagonalDragBehavior => (((_VerticalOuterDimension__scrollable?)(object?)this.widget)!).diagonalDragBehavior;
    public virtual ScrollableState horizontalScrollable => DartRuntimePrimitives.ConvertValue<ScrollableState>((((_VerticalOuterDimension__scrollable?)(object?)this.widget)!).horizontalKey.currentState!);
    internal override (List<Future>, ScrollableState) _performEnsureVisible(global::Doroti.Generated.Framework.Rendering.RenderObject @object, double alignment = 0.0, Duration duration = default, global::Doroti.Generated.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, global::Doroti.Generated.Framework.Rendering.RenderObject? targetRenderObject = null)
    {
        DartRuntimePrimitives.Assert(() => false, () => (object?)"The _performEnsureVisible method was called for the vertical scrollable " + "of a TwoDimensionalScrollable. This should not happen as the horizontal " + "scrollable handles both axes.");
        return (new List<Future>(), this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _evaluateLockedAxis(Offset offset)
    {
        DartRuntimePrimitives.Assert(() => (this.lastDragOffset is not null));
        global::Doroti.Flutter.Ui.Offset offsetDelta__85769 = ((global::Doroti.Flutter.Ui.Offset)(object?)(DartRuntimePrimitives.RequireValue(this.lastDragOffset) - offset));
        double axisDifferential__85826 = (offsetDelta__85769.dx.abs() - offsetDelta__85769.dy.abs());
        if ((axisDifferential__85826.abs() >= global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kTouchSlop))
        {
            lockedAxis = ((axisDifferential__85826 > 0.0) ? global::Doroti.Generated.Framework.Painting.Axis.horizontal : global::Doroti.Generated.Framework.Painting.Axis.vertical);
        }
        else
        {
            lockedAxis = null;
        }
    }

    internal override void _handleDragDown(global::Doroti.Generated.Framework.Gestures.DragDownDetails details)
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

    internal override void _handleDragStart(global::Doroti.Generated.Framework.Gestures.DragStartDetails details)
    {
        lastDragOffset = ((global::Doroti.Generated.Framework.Gestures.DragStartDetails)details).globalPosition;
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
                    _evaluateLockedAxis(((global::Doroti.Generated.Framework.Gestures.DragStartDetails)details).globalPosition);
                    switch (this.lockedAxis)
                    {
                        case null:
                            {
                                this.horizontalScrollable._handleDragStart(details);
                                break;
                            }
                        case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                            {
                                this.horizontalScrollable._handleDragStart(details);
                                return;
                            }
                        case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                            break;
                    }
                    break;
                }
        }
        base._handleDragStart(details);
    }

    internal override void _handleDragUpdate(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details)
    {
        var verticalDragDetails__87812 = new global::Doroti.Generated.Framework.Gestures.DragUpdateDetails(sourceTimeStamp: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp, delta: new global::Doroti.Flutter.Ui.Offset(0.0, ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta.dy), primaryDelta: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta.dy, globalPosition: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).globalPosition, localPosition: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).localPosition);
        var horizontalDragDetails__88090 = new global::Doroti.Generated.Framework.Gestures.DragUpdateDetails(sourceTimeStamp: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp, delta: new global::Doroti.Flutter.Ui.Offset(((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta.dx, 0.0), primaryDelta: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta.dx, globalPosition: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).globalPosition, localPosition: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).localPosition);
        switch (this.diagonalDragBehavior)
        {
            case DiagonalDragBehavior.none:
                {
                    base._handleDragUpdate(verticalDragDetails__87812);
                    return;
                }
            case DiagonalDragBehavior.free:
                {
                    this.horizontalScrollable._handleDragUpdate(horizontalDragDetails__88090);
                    base._handleDragUpdate(verticalDragDetails__87812);
                    return;
                }
            case DiagonalDragBehavior.weightedContinuous:
                {
                    _evaluateLockedAxis(((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).globalPosition);
                    lastDragOffset = ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).globalPosition;
                    break;
                }
            case DiagonalDragBehavior.weightedEvent:
                {
                    if (((this.lockedAxis is null) && (this.lastDragOffset is not null)))
                    {
                        _evaluateLockedAxis(((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).globalPosition);
                    }
                    break;
                }
        }
        switch (this.lockedAxis)
        {
            case null:
                {
                    this.horizontalScrollable._handleDragUpdate(horizontalDragDetails__88090);
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    this.horizontalScrollable._handleDragUpdate(horizontalDragDetails__88090);
                    return;
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                break;
        }
        base._handleDragUpdate(verticalDragDetails__87812);
    }

    internal override void _handleDragEnd(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        lastDragOffset = null;
        lockedAxis = null;
        double dx__89854 = ((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dx;
        double dy__89913 = ((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy;
        var verticalDragDetails__89965 = new global::Doroti.Generated.Framework.Gestures.DragEndDetails(velocity: new global::Doroti.Generated.Framework.Gestures.Velocity(pixelsPerSecond: new global::Doroti.Flutter.Ui.Offset(0.0, dy__89913)), primaryVelocity: dy__89913);
        var horizontalDragDetails__90107 = new global::Doroti.Generated.Framework.Gestures.DragEndDetails(velocity: new global::Doroti.Generated.Framework.Gestures.Velocity(pixelsPerSecond: new global::Doroti.Flutter.Ui.Offset(dx__89854, 0.0)), primaryVelocity: dx__89854);
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
                    this.horizontalScrollable._handleDragEnd(horizontalDragDetails__90107);
                    break;
                }
        }
        base._handleDragEnd(verticalDragDetails__89965);
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
                        _gestureRecognizers = new DartMap<Type, dynamic> { [typeof(global::Doroti.Generated.Framework.Gestures.PanGestureRecognizer)] = new GestureRecognizerFactoryWithHandlers<global::Doroti.Generated.Framework.Gestures.PanGestureRecognizer>(((global::System.Func<global::Doroti.Generated.Framework.Gestures.PanGestureRecognizer>)(() => new global::Doroti.Generated.Framework.Gestures.PanGestureRecognizer(supportedDevices: ((ScrollBehavior)this._configuration).dragDevices))), ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PanGestureRecognizer>)((instance) => {
DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Gestures.PanGestureRecognizer>)(() =>
{            var __cascade = instance;
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
            return __cascade;        }))());
}))) };
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
        var details__93415 = new ScrollableDetails(direction: this.widget.axisDirection, controller: this._effectiveScrollController, clipBehavior: this.widget.clipBehavior);
        return ((Widget)(object?)this._configuration.buildOverscrollIndicator(context, child, details__93415));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _HorizontalInnerDimension__scrollable : Scrollable
{
    public virtual GlobalKey<ScrollableState> verticalOuterKey { get; private set; } = default!;
    public virtual DiagonalDragBehavior diagonalDragBehavior { get; private set; } = default!;

    internal _HorizontalInnerDimension__scrollable(global::Doroti.Generated.Framework.Foundation.Key? key = null, GlobalKey<ScrollableState> verticalOuterKey = default!, global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection = default!, ScrollController? controller = null, ScrollPhysics? physics = null, Clip clipBehavior = Clip.hardEdge, global::System.Func<ScrollIncrementDetails, double>? incrementCalculator = null, bool excludeFromSemantics = false, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, string? restorationId = null, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, DiagonalDragBehavior diagonalDragBehavior = DiagonalDragBehavior.none) : base(key: key, viewportBuilder: viewportBuilder, axisDirection: axisDirection, controller: controller, physics: physics, clipBehavior: clipBehavior, incrementCalculator: incrementCalculator, excludeFromSemantics: excludeFromSemantics, dragStartBehavior: dragStartBehavior, restorationId: restorationId, hitTestBehavior: hitTestBehavior)
    {
        this.verticalOuterKey = verticalOuterKey;
        this.diagonalDragBehavior = diagonalDragBehavior;
        System.Diagnostics.Debug.Assert(((object.Equals(axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.left)) || (object.Equals(axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.right))));
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
        DartRuntimePrimitives.Assert(() => (object.Equals(global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableState)this.verticalScrollable).axisDirection), global::Doroti.Generated.Framework.Painting.Axis.vertical)));
        base.didChangeDependencies();
    }

    internal override (List<Future>, ScrollableState) _performEnsureVisible(global::Doroti.Generated.Framework.Rendering.RenderObject @object, double alignment = 0.0, Duration duration = default, global::Doroti.Generated.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, global::Doroti.Generated.Framework.Rendering.RenderObject? targetRenderObject = null)
    {
        var newFutures__95712 = new List<Future> { this.position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy), ((ScrollableState)this.verticalScrollable).position.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy) };
        return (newFutures__95712, this.verticalScrollable);
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
        var details__97740 = new ScrollableDetails(direction: this.widget.axisDirection, controller: this._effectiveScrollController, clipBehavior: this.widget.clipBehavior);
        return ((Widget)(object?)this._configuration.buildOverscrollIndicator(context, child, details__97740));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
