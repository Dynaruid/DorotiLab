// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/nested_scroll_view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate List<Widget> NestedScrollViewHeaderSliversBuilder(BuildContext context, bool innerBoxIsScrolled);

public class NestedScrollView : StatefulWidget
{
    public virtual ScrollController? controller { get; private set; }
    public virtual global::Doroti.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual global::System.Func<BuildContext, bool, List<Widget>> headerSliverBuilder { get; private set; } = default!;
    public virtual Widget body { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual bool floatHeaderSlivers { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }

    public NestedScrollView(global::Doroti.Framework.Foundation.Key? key = null, ScrollController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollPhysics? physics = null, global::System.Func<BuildContext, bool, List<Widget>> headerSliverBuilder = default!, Widget body = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, bool floatHeaderSlivers = false, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, string? restorationId = null, ScrollBehavior? scrollBehavior = null) : base(key: key)
    {
        this.controller = controller;
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.physics = physics;
        this.headerSliverBuilder = headerSliverBuilder;
        this.body = body;
        this.dragStartBehavior = dragStartBehavior;
        this.floatHeaderSlivers = floatHeaderSlivers;
        this.clipBehavior = clipBehavior;
        this.hitTestBehavior = hitTestBehavior;
        this.restorationId = restorationId;
        this.scrollBehavior = scrollBehavior;
    }

    public static SliverOverlapAbsorberHandle sliverOverlapAbsorberHandleFor(BuildContext context)
    {
        _InheritedNestedScrollView__nested_scroll_view? target = context.dependOnInheritedWidgetOfExactType<_InheritedNestedScrollView__nested_scroll_view>();
        DartRuntimePrimitives.Assert(() => target is not null, () => (object?)"NestedScrollView.sliverOverlapAbsorberHandleFor must be called with a context that contains a NestedScrollView.");
        return target!.state._absorberHandle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<Widget> _buildSlivers(BuildContext context, ScrollController innerController, bool bodyIsScrolled)
    {
        return new List<Widget> { new SliverFillRemaining(child: new PrimaryScrollController(automaticallyInheritForPlatforms: Enum.GetValues<global::Doroti.Framework.Foundation.TargetPlatform>().ToList().toSet(), controller: innerController, child: body)) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new NestedScrollViewState());
}

public class NestedScrollViewState : State<NestedScrollView>
{
    internal virtual SliverOverlapAbsorberHandle _absorberHandle { get; private set; } = new SliverOverlapAbsorberHandle();
    internal virtual _NestedScrollCoordinator__nested_scroll_view? _coordinator { get; set; } = default;
    internal virtual bool? _lastHasScrolledBody { get; set; } = default;

    public virtual ScrollController innerController => DartRuntimePrimitives.ConvertValue<ScrollController>(_coordinator!._innerController);
    public virtual ScrollController outerController => DartRuntimePrimitives.ConvertValue<ScrollController>(_coordinator!._outerController);
    public override void initState()
    {
        base.initState();
        _coordinator = new _NestedScrollCoordinator__nested_scroll_view(this, widget.controller, () => _handleHasScrolledBodyChanged(), widget.floatHeaderSlivers);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _coordinator!.setParent(widget.controller);
    }

    public override void didUpdateWidget(NestedScrollView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.controller, widget.controller))
        {
            _coordinator!.setParent(widget.controller);
        }
    }

    public override void dispose()
    {
        _coordinator!.dispose();
        _coordinator = null;
        _absorberHandle.dispose();
        base.dispose();
    }

    internal virtual void _handleHasScrolledBodyChanged()
    {
        if (!mounted)
        {
            return;
        }
        bool newHasScrolledBody = _coordinator!.hasScrolledBody;
        if (_lastHasScrolledBody != newHasScrolledBody)
        {
            setState(() =>
            {
            });
        }
    }

    public override Widget build(BuildContext context)
    {
        ScrollPhysics scrollPhysics = (widget.physics?.applyTo(new ClampingScrollPhysics()) ?? (widget.scrollBehavior?.getScrollPhysics(context).applyTo(new ClampingScrollPhysics()))) ?? new ClampingScrollPhysics();
        return new _InheritedNestedScrollView__nested_scroll_view(state: this, child: new Builder(builder: (context) =>
        {
            _lastHasScrolledBody = _coordinator!.hasScrolledBody;
            return new _NestedScrollViewCustomScrollView__nested_scroll_view(dragStartBehavior: widget.dragStartBehavior, scrollDirection: widget.scrollDirection, reverse: widget.reverse, physics: scrollPhysics, scrollBehavior: widget.scrollBehavior ?? ScrollConfiguration.of(context).copyWith(scrollbars: false), controller: _coordinator!._outerController, slivers: widget._buildSlivers(context, _coordinator!._innerController, DartRuntimePrimitives.RequireValue(_lastHasScrolledBody)), handle: _absorberHandle, clipBehavior: widget.clipBehavior, restorationId: widget.restorationId, hitTestBehavior: widget.hitTestBehavior);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NestedScrollViewCustomScrollView__nested_scroll_view : CustomScrollView
{
    public virtual SliverOverlapAbsorberHandle handle { get; private set; } = default!;

    internal _NestedScrollViewCustomScrollView__nested_scroll_view(global::Doroti.Framework.Painting.Axis scrollDirection, bool reverse, ScrollPhysics physics, ScrollBehavior scrollBehavior, ScrollController controller, List<Widget> slivers, SliverOverlapAbsorberHandle handle, Clip clipBehavior, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, string? restorationId = null) : base(scrollDirection: scrollDirection, reverse: reverse, physics: physics, scrollBehavior: scrollBehavior, controller: controller, slivers: slivers, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior, dragStartBehavior: dragStartBehavior, restorationId: restorationId)
    {
        this.handle = handle;
    }

    public override Widget buildViewport(BuildContext context, global::Doroti.Framework.Rendering.ViewportOffset offset, global::Doroti.Framework.Painting.AxisDirection axisDirection, List<Widget> slivers)
    {
        DartRuntimePrimitives.Assert(() => !shrinkWrap);
        return new NestedScrollViewViewport(axisDirection: axisDirection, offset: offset, slivers: slivers, handle: handle, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InheritedNestedScrollView__nested_scroll_view : InheritedWidget
{
    public virtual NestedScrollViewState state { get; private set; } = default!;

    internal _InheritedNestedScrollView__nested_scroll_view(NestedScrollViewState state, Widget child) : base(child: child)
    {
        this.state = state;
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => !Equals(state, ((_InheritedNestedScrollView__nested_scroll_view)oldWidget).state);
}

public class _NestedScrollMetrics__nested_scroll_view : FixedScrollMetrics
{
    public virtual double minRange { get; private set; } = default!;
    public virtual double maxRange { get; private set; } = default!;
    public virtual double correctionOffset { get; private set; } = default!;

    internal _NestedScrollMetrics__nested_scroll_view(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, global::Doroti.Framework.Painting.AxisDirection axisDirection, double devicePixelRatio, double minRange, double maxRange, double correctionOffset) : base(minScrollExtent: DartRuntimePrimitives.RequireValue(minScrollExtent), maxScrollExtent: DartRuntimePrimitives.RequireValue(maxScrollExtent), pixels: DartRuntimePrimitives.RequireValue(pixels), viewportDimension: DartRuntimePrimitives.RequireValue(viewportDimension), axisDirection: axisDirection, devicePixelRatio: devicePixelRatio)
    {
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.correctionOffset = correctionOffset;
    }

    public override _NestedScrollMetrics__nested_scroll_view copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new _NestedScrollMetrics__nested_scroll_view(minScrollExtent: minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : null), maxScrollExtent: maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : null), pixels: pixels ?? (hasPixels ? this.pixels : null), viewportDimension: viewportDimension ?? (hasViewportDimension ? this.viewportDimension : null), axisDirection: axisDirection ?? this.axisDirection, devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio, minRange: minRange ?? this.minRange, maxRange: maxRange ?? this.maxRange, correctionOffset: correctionOffset ?? this.correctionOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate ScrollActivity _NestedScrollActivityGetter__nested_scroll_view(_NestedScrollPosition__nested_scroll_view position);

public class _NestedScrollCoordinator__nested_scroll_view : ScrollActivityDelegate, ScrollHoldController
{
    internal virtual NestedScrollViewState _state { get; private set; } = default!;
    internal virtual ScrollController? _parent { get; set; } = default;
    internal virtual global::System.Action _onHasScrolledBodyChanged { get; private set; } = default!;
    internal virtual bool _floatHeaderSlivers { get; private set; } = default!;
    internal virtual _NestedScrollController__nested_scroll_view _outerController { get; set; } = default!;
    internal virtual _NestedScrollController__nested_scroll_view _innerController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.ScrollDirection _userScrollDirection { get; set; } = ScrollDirection.idle;
    internal virtual ScrollDragController? _currentDrag { get; set; } = default;

    internal _NestedScrollCoordinator__nested_scroll_view(NestedScrollViewState _state, ScrollController? _parent, global::System.Action _onHasScrolledBodyChanged, bool _floatHeaderSlivers)
    {
        this._state = _state;
        this._parent = _parent;
        this._onHasScrolledBodyChanged = _onHasScrolledBodyChanged;
        this._floatHeaderSlivers = _floatHeaderSlivers;
    }

    public virtual bool outOfRange
    {
        get
        {
            return (_outerPosition?.outOfRange ?? false) || _innerPositions.any((position) => position.outOfRange);
        }
    }
    internal virtual _NestedScrollPosition__nested_scroll_view? _outerPosition
    {
        get
        {
            if (!_outerController.hasClients)
            {
                return null;
            }
            return _outerController.nestedPositions.Single();
        }
    }
    internal virtual IEnumerable<_NestedScrollPosition__nested_scroll_view> _innerPositions
    {
        get
        {
            return _innerController.nestedPositions;
        }
    }
    public virtual bool canScrollBody
    {
        get
        {
            _NestedScrollPosition__nested_scroll_view? outer = _outerPosition;
            if (outer is null)
            {
                return true;
            }
            return outer.haveDimensions && (outer.extentAfter == 0.0);
        }
    }
    public virtual bool hasScrolledBody
    {
        get
        {
            foreach (_NestedScrollPosition__nested_scroll_view position in _innerPositions)
            {
                if (!position.hasContentDimensions || !position.hasPixels)
                {
                    continue;
                }
                else
                {
                    if (position.pixels > position.minScrollExtent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public virtual void updateShadow()
    {
        _onHasScrolledBodyChanged();
    }

    public virtual global::Doroti.Framework.Rendering.ScrollDirection userScrollDirection => _userScrollDirection;
    public virtual void updateUserScrollDirection(global::Doroti.Framework.Rendering.ScrollDirection value)
    {
        if (Equals(userScrollDirection, DartRuntimePrimitives.RequireValue(value)))
        {
            return;
        }
        _userScrollDirection = DartRuntimePrimitives.RequireValue(value);
        _outerPosition!.didUpdateScrollDirection(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value)));
        foreach (_NestedScrollPosition__nested_scroll_view position in _innerPositions)
        {
            position.didUpdateScrollDirection(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value)));
        }
    }

    public virtual void beginActivity(ScrollActivity newOuterActivity, global::System.Func<_NestedScrollPosition__nested_scroll_view, ScrollActivity> innerActivityGetter)
    {
        _outerPosition!.beginActivity(newOuterActivity);
        bool scrolling = newOuterActivity.isScrolling;
        foreach (_NestedScrollPosition__nested_scroll_view position in _innerPositions)
        {
            ScrollActivity newInnerActivity = innerActivityGetter(position);
            position.beginActivity(newInnerActivity);
            scrolling = scrolling && newInnerActivity.isScrolling;
        }
        _currentDrag?.dispose();
        _currentDrag = null;
        if (!scrolling)
        {
            updateUserScrollDirection(ScrollDirection.idle);
        }
    }

    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection => _outerPosition!.axisDirection;
    internal static IdleScrollActivity _createIdleScrollActivity(_NestedScrollPosition__nested_scroll_view position)
    {
        return new IdleScrollActivity(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void goIdle()
    {
        beginActivity(_createIdleScrollActivity(_outerPosition!), (global::System.Func<_NestedScrollPosition__nested_scroll_view, IdleScrollActivity>)_createIdleScrollActivity);
    }

    public virtual void goBallistic(double velocity)
    {
        beginActivity(createOuterBallisticScrollActivity(velocity), (position) =>
        {
            return createInnerBallisticScrollActivity(position, velocity);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
    }

    public virtual ScrollActivity createOuterBallisticScrollActivity(double velocity)
    {
        _NestedScrollPosition__nested_scroll_view? innerPosition = default!;
        if (velocity != 0.0)
        {
            foreach (_NestedScrollPosition__nested_scroll_view position in _innerPositions)
            {
                if (innerPosition is not null)
                {
                    if (velocity > 0.0)
                    {
                        if (innerPosition.pixels < position.pixels)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => velocity < 0.0);
                        if (innerPosition.pixels > position.pixels)
                        {
                            continue;
                        }
                    }
                }
                innerPosition = position;
            }
        }
        if (innerPosition is null)
        {
            return _outerPosition!.createBallisticScrollActivity(_outerPosition!.physics.createBallisticSimulation(_outerPosition!, velocity), mode: _NestedBallisticScrollActivityMode__nested_scroll_view.independent);
        }
        _NestedScrollMetrics__nested_scroll_view metricsLocal = _getMetrics(innerPosition, velocity);
        return _outerPosition!.createBallisticScrollActivity(_outerPosition!.physics.createBallisticSimulation(metricsLocal, velocity), mode: _NestedBallisticScrollActivityMode__nested_scroll_view.outer, metrics: metricsLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ScrollActivity createInnerBallisticScrollActivity(_NestedScrollPosition__nested_scroll_view position, double velocity)
    {
        return position.createBallisticScrollActivity(position.physics.createBallisticSimulation(_getMetrics(position, velocity), velocity), mode: _NestedBallisticScrollActivityMode__nested_scroll_view.inner);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _NestedScrollMetrics__nested_scroll_view _getMetrics(_NestedScrollPosition__nested_scroll_view innerPosition, double velocity)
    {
        double pixelsLocal = default!;
        double minRangeLocal = default!;
        double maxRangeLocal = default!;
        double correctionOffsetLocal = default!;
        var extra = 0.0;
        if (innerPosition.pixels == innerPosition.minScrollExtent)
        {
            pixelsLocal = Dart_uiLibrary.clampDouble(_outerPosition!.pixels, _outerPosition!.minScrollExtent, _outerPosition!.maxScrollExtent);
            minRangeLocal = _outerPosition!.minScrollExtent;
            maxRangeLocal = _outerPosition!.maxScrollExtent;
            DartRuntimePrimitives.Assert(() => minRangeLocal <= maxRangeLocal);
            correctionOffsetLocal = 0.0;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => innerPosition.pixels != innerPosition.minScrollExtent);
            if (innerPosition.pixels < innerPosition.minScrollExtent)
            {
                pixelsLocal = innerPosition.pixels - innerPosition.minScrollExtent + _outerPosition!.minScrollExtent;
            }
            else
            {
                DartRuntimePrimitives.Assert(() => innerPosition.pixels > innerPosition.minScrollExtent);
                pixelsLocal = innerPosition.pixels - innerPosition.minScrollExtent + _outerPosition!.maxScrollExtent;
            }
            if (velocity > 0.0 && innerPosition.pixels > innerPosition.minScrollExtent)
            {
                extra = _outerPosition!.maxScrollExtent - _outerPosition!.pixels;
                DartRuntimePrimitives.Assert(() => extra >= 0.0);
                minRangeLocal = pixelsLocal;
                maxRangeLocal = pixelsLocal + extra;
                DartRuntimePrimitives.Assert(() => minRangeLocal <= maxRangeLocal);
                correctionOffsetLocal = _outerPosition!.pixels - pixelsLocal;
            }
            else
            {
                if (velocity < 0.0 && innerPosition.pixels < innerPosition.minScrollExtent)
                {
                    extra = _outerPosition!.pixels - _outerPosition!.minScrollExtent;
                    DartRuntimePrimitives.Assert(() => extra >= 0.0);
                    minRangeLocal = pixelsLocal - extra;
                    maxRangeLocal = pixelsLocal;
                    DartRuntimePrimitives.Assert(() => minRangeLocal <= maxRangeLocal);
                    correctionOffsetLocal = _outerPosition!.pixels - pixelsLocal;
                }
                else
                {
                    if (velocity > 0.0)
                    {
                        extra = _outerPosition!.minScrollExtent - _outerPosition!.pixels;
                    }
                    else
                    {
                        if (velocity < 0.0)
                        {
                            extra = _outerPosition!.pixels - (_outerPosition!.maxScrollExtent - _outerPosition!.minScrollExtent);
                        }
                    }
                    DartRuntimePrimitives.Assert(() => extra <= 0.0);
                    minRangeLocal = _outerPosition!.minScrollExtent;
                    maxRangeLocal = _outerPosition!.maxScrollExtent + extra;
                    DartRuntimePrimitives.Assert(() => minRangeLocal <= maxRangeLocal);
                    correctionOffsetLocal = 0.0;
                }
            }
        }
        return new _NestedScrollMetrics__nested_scroll_view(minScrollExtent: _outerPosition!.minScrollExtent, maxScrollExtent: _outerPosition!.maxScrollExtent + innerPosition.maxScrollExtent - innerPosition.minScrollExtent + extra, pixels: pixelsLocal, viewportDimension: _outerPosition!.viewportDimension, axisDirection: _outerPosition!.axisDirection, minRange: minRangeLocal, maxRange: maxRangeLocal, correctionOffset: correctionOffsetLocal, devicePixelRatio: _outerPosition!.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double unnestOffset(double value, _NestedScrollPosition__nested_scroll_view source)
    {
        if (Equals(source, _outerPosition))
        {
            return Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), _outerPosition!.minScrollExtent, _outerPosition!.maxScrollExtent);
        }
        if (DartRuntimePrimitives.RequireValue(value) < source.minScrollExtent)
        {
            return DartRuntimePrimitives.RequireValue(value) - source.minScrollExtent + _outerPosition!.minScrollExtent;
        }
        return DartRuntimePrimitives.RequireValue(value) - source.minScrollExtent + _outerPosition!.maxScrollExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double nestOffset(double value, _NestedScrollPosition__nested_scroll_view target)
    {
        if (Equals(target, _outerPosition))
        {
            return Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), _outerPosition!.minScrollExtent, _outerPosition!.maxScrollExtent);
        }
        if (DartRuntimePrimitives.RequireValue(value) < _outerPosition!.minScrollExtent)
        {
            return DartRuntimePrimitives.RequireValue(value) - _outerPosition!.minScrollExtent + target.minScrollExtent;
        }
        if (DartRuntimePrimitives.RequireValue(value) > _outerPosition!.maxScrollExtent)
        {
            return DartRuntimePrimitives.RequireValue(value) - _outerPosition!.maxScrollExtent + target.minScrollExtent;
        }
        return target.minScrollExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateCanDrag()
    {
        if (!_outerPosition!.haveDimensions)
        {
            return;
        }
        var innerCanDrag = false;
        foreach (_NestedScrollPosition__nested_scroll_view position in _innerPositions)
        {
            if (!position.haveDimensions)
            {
                return;
            }
            innerCanDrag = innerCanDrag || position.physics.shouldAcceptUserOffset(position);
        }
        _outerPosition!.updateCanDrag(innerCanDrag);
    }

    public async virtual Future animateTo(double to, Duration duration, global::Doroti.Framework.Animation.Curve curve)
    {
        DrivenScrollActivity outerActivity = _outerPosition!.createDrivenScrollActivity(nestOffset(to, _outerPosition!), duration, curve);
        var resultFutures = new List<Future> { outerActivity.done };
        beginActivity(outerActivity, (position) =>
        {
            DrivenScrollActivity innerActivity = position.createDrivenScrollActivity(nestOffset(to, position), duration, curve);
            resultFutures.Add(innerActivity.done);
            return innerActivity;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        await DartAsyncRuntime.wait<object?>(resultFutures);
    }

    public virtual void jumpTo(double to)
    {
        goIdle();
        _outerPosition!.localJumpTo(nestOffset(to, _outerPosition!));
        foreach (_NestedScrollPosition__nested_scroll_view position in _innerPositions)
        {
            position.localJumpTo(nestOffset(to, position));
        }
        goBallistic(0.0);
    }

    public virtual void pointerScroll(double delta)
    {
        if (delta == 0.0)
        {
            goBallistic(0.0);
            return;
        }
        goIdle();
        updateUserScrollDirection((delta < 0.0) ? ScrollDirection.forward : ScrollDirection.reverse);
        _outerPosition!.isScrollingNotifier.value = true;
        _outerPosition!.didStartScroll();
        foreach (_NestedScrollPosition__nested_scroll_view position in _innerPositions)
        {
            position.isScrollingNotifier.value = true;
            position.didStartScroll();
        }
        if (!Enumerable.Any(_innerPositions))
        {
            _outerPosition!.applyClampedPointerSignalUpdate(delta);
        }
        else
        {
            if (delta > 0.0)
            {
                var outerDelta = delta;
                foreach (_NestedScrollPosition__nested_scroll_view positionLocal in _innerPositions)
                {
                    if (positionLocal.pixels < 0.0)
                    {
                        double potentialOuterDelta = positionLocal.applyClampedPointerSignalUpdate(delta);
                        outerDelta = Math.Max(outerDelta, potentialOuterDelta);
                    }
                }
                if (outerDelta != 0.0)
                {
                    double innerDelta = _outerPosition!.applyClampedPointerSignalUpdate(outerDelta);
                    if (innerDelta != 0.0)
                    {
                        foreach (_NestedScrollPosition__nested_scroll_view positionAlternate in _innerPositions)
                        {
                            positionAlternate.applyClampedPointerSignalUpdate(innerDelta);
                        }
                    }
                }
            }
            else
            {
                var innerDeltaLocal = delta;
                if (_floatHeaderSlivers)
                {
                    innerDeltaLocal = _outerPosition!.applyClampedPointerSignalUpdate(delta);
                }
                if (innerDeltaLocal != 0.0)
                {
                    var outerDeltaLocal = 0.0;
                    foreach (_NestedScrollPosition__nested_scroll_view positionNested in _innerPositions)
                    {
                        double overscroll = positionNested.applyClampedPointerSignalUpdate(innerDeltaLocal);
                        outerDeltaLocal = Math.Min(outerDeltaLocal, overscroll);
                    }
                    if (outerDeltaLocal != 0.0)
                    {
                        _outerPosition!.applyClampedPointerSignalUpdate(outerDeltaLocal);
                    }
                }
            }
        }
        _outerPosition!.didEndScroll();
        foreach (_NestedScrollPosition__nested_scroll_view positionCurrent in _innerPositions)
        {
            positionCurrent.didEndScroll();
        }
        goBallistic(0.0);
    }

    public virtual double setPixels(double pixels)
    {
        DartRuntimePrimitives.Assert(() => false);
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ScrollHoldController hold(global::System.Action holdCancelCallback)
    {
        beginActivity(new HoldScrollActivity(@delegate: _outerPosition!, onHoldCanceled: () => holdCancelCallback()), (position) => new HoldScrollActivity(@delegate: position));
        return this;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void cancel()
    {
        goBallistic(0.0);
    }

    public virtual global::Doroti.Framework.Gestures.Drag drag(global::Doroti.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback)
    {
        var dragLocal = new ScrollDragController(@delegate: this, details: details, onDragCanceled: () => dragCancelCallback());
        beginActivity(new DragScrollActivity(_outerPosition!, dragLocal), (position) => new DragScrollActivity(position, dragLocal));
        DartRuntimePrimitives.Assert(() => _currentDrag is null);
        _currentDrag = dragLocal;
        return dragLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyUserOffset(double delta)
    {
        updateUserScrollDirection((delta > 0.0) ? ScrollDirection.forward : ScrollDirection.reverse);
        DartRuntimePrimitives.Assert(() => delta != 0.0);
        if (!Enumerable.Any(_innerPositions))
        {
            _outerPosition!.applyFullDragUpdate(delta);
        }
        else
        {
            if (delta < 0.0)
            {
                var outerDelta = delta;
                foreach (_NestedScrollPosition__nested_scroll_view position in _innerPositions)
                {
                    if (position.pixels < 0.0)
                    {
                        double potentialOuterDelta = position.applyClampedDragUpdate(delta);
                        outerDelta = Math.Max(outerDelta, potentialOuterDelta);
                    }
                }
                if (outerDelta.abs() > Foundation.ConstantsLibrary.precisionErrorTolerance)
                {
                    double innerDelta = _outerPosition!.applyClampedDragUpdate(outerDelta);
                    if (innerDelta != 0.0)
                    {
                        foreach (_NestedScrollPosition__nested_scroll_view positionLocal in _innerPositions)
                        {
                            positionLocal.applyFullDragUpdate(innerDelta);
                        }
                    }
                }
            }
            else
            {
                var innerDeltaLocal = delta;
                if (_floatHeaderSlivers)
                {
                    innerDeltaLocal = _outerPosition!.applyClampedDragUpdate(delta);
                }
                if (innerDeltaLocal != 0.0)
                {
                    var outerDeltaLocal = 0.0;
                    var overscrolls = new List<double>();
                    List<_NestedScrollPosition__nested_scroll_view> innerPositions = _innerPositions.ToList().ToList();
                    foreach (var positionAlternate in innerPositions)
                    {
                        double overscroll = positionAlternate.applyClampedDragUpdate(innerDeltaLocal);
                        outerDeltaLocal = Math.Max(outerDeltaLocal, overscroll);
                        overscrolls.Add(overscroll);
                    }
                    if (outerDeltaLocal != 0.0)
                    {
                        outerDeltaLocal -= _outerPosition!.applyClampedDragUpdate(outerDeltaLocal);
                    }
                    for (var i = 0L; i < checked(innerPositions.Count); ++i)
                    {
                        double remainingDelta = overscrolls[(int)i] - outerDeltaLocal;
                        if (remainingDelta > 0.0)
                        {
                            innerPositions[(int)i].applyFullDragUpdate(remainingDelta);
                        }
                    }
                }
            }
        }
    }

    public virtual void setParent(ScrollController? value)
    {
        _parent = value;
        updateParent();
    }

    public virtual void updateParent()
    {
        _outerPosition?.setParent(_parent ?? PrimaryScrollController.maybeOf(_state.context));
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _currentDrag?.dispose();
        _currentDrag = null;
        _outerController.dispose();
        _innerController.dispose();
    }

    public override string ToString() => $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "_NestedScrollCoordinator")}(outer={_outerController}; inner={_innerController})";
}

internal class _NestedScrollController__nested_scroll_view : ScrollController
{
    public virtual _NestedScrollCoordinator__nested_scroll_view coordinator { get; private set; } = default!;

    internal _NestedScrollController__nested_scroll_view(_NestedScrollCoordinator__nested_scroll_view coordinator, double initialScrollOffset = 0.0, string? debugLabel = null) : base(initialScrollOffset: initialScrollOffset, debugLabel: debugLabel)
    {
        this.coordinator = coordinator;
    }

    public override ScrollPosition createScrollPosition(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition)
    {
        return new _NestedScrollPosition__nested_scroll_view(coordinator: coordinator, physics: physics, context: context, initialPixels: initialScrollOffset, oldPosition: oldPosition, debugLabel: debugLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(ScrollPosition position)
    {
        DartRuntimePrimitives.Assert(() => position is _NestedScrollPosition__nested_scroll_view);
        base.attach(position);
        coordinator.updateParent();
        coordinator.updateCanDrag();
        position.addListener(_scheduleUpdateShadow);
        _scheduleUpdateShadow();
    }

    public override void detach(ScrollPosition position)
    {
        DartRuntimePrimitives.Assert(() => position is _NestedScrollPosition__nested_scroll_view);
        ((_NestedScrollPosition__nested_scroll_view?)position)!.setParent(null);
        ((_NestedScrollPosition__nested_scroll_view)position).removeListener(_scheduleUpdateShadow);
        base.detach((_NestedScrollPosition__nested_scroll_view)position);
        _scheduleUpdateShadow();
    }

    internal virtual void _scheduleUpdateShadow()
    {
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((timeStamp) =>
        {
            coordinator.updateShadow();
        }, debugLabel: "NestedScrollController.updateShadow");
    }

    public virtual IEnumerable<_NestedScrollPosition__nested_scroll_view> nestedPositions
    {
        get
        {
            return positions.cast<_NestedScrollPosition__nested_scroll_view>();
        }
    }
}

public class _NestedScrollPosition__nested_scroll_view : ScrollPosition, ScrollActivityDelegate
{
    public virtual _NestedScrollCoordinator__nested_scroll_view coordinator { get; private set; } = default!;
    internal virtual ScrollController? _parent { get; set; } = default;

    internal _NestedScrollPosition__nested_scroll_view(ScrollPhysics physics, ScrollContext context, double initialPixels = 0.0, ScrollPosition? oldPosition = null, string? debugLabel = null, _NestedScrollCoordinator__nested_scroll_view coordinator = default!) : base(physics: physics, context: context, oldPosition: oldPosition, debugLabel: debugLabel)
    {
        this.coordinator = coordinator;
    }

    public virtual global::Doroti.Framework.Scheduler.TickerProvider vsync => context.vsync;
    public virtual void setParent(ScrollController? value)
    {
        _parent?.detach(this);
        _parent = value;
        _parent?.attach(this);
    }

    public override global::Doroti.Framework.Painting.AxisDirection axisDirection => context.axisDirection;
    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        activity!.updateDelegate(this);
    }

    public override void restoreScrollOffset()
    {
        if (coordinator.canScrollBody)
        {
            base.restoreScrollOffset();
        }
    }

    public virtual double applyClampedDragUpdate(double delta)
    {
        DartRuntimePrimitives.Assert(() => delta != 0.0);
        double min = (delta < 0.0) ? -double.PositiveInfinity : Math.Min(minScrollExtent, pixels);
        double max = (delta > 0.0) ? double.PositiveInfinity : ((pixels < 0.0) ? 0.0 : Math.Max(maxScrollExtent, pixels));
        double oldPixels = pixels;
        double newPixels = Dart_uiLibrary.clampDouble(pixels - delta, min, max);
        double clampedDelta = newPixels - pixels;
        if (clampedDelta == 0.0)
        {
            return delta;
        }
        double overscroll = physics.applyBoundaryConditions(this, newPixels);
        double actualNewPixels = newPixels - overscroll;
        double offset = actualNewPixels - oldPixels;
        if (offset != 0.0)
        {
            forcePixels(actualNewPixels);
            didUpdateScrollPositionBy(offset);
        }
        double result = delta + offset;
        if (result.abs() < Foundation.ConstantsLibrary.precisionErrorTolerance)
        {
            return 0.0;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double applyFullDragUpdate(double delta)
    {
        DartRuntimePrimitives.Assert(() => delta != 0.0);
        double oldPixels = pixels;
        double newPixels = pixels - physics.applyPhysicsToUserOffset(this, delta);
        if ((oldPixels - newPixels).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance)
        {
            return 0.0;
        }
        double overscroll = physics.applyBoundaryConditions(this, newPixels);
        double actualNewPixels = newPixels - overscroll;
        if (actualNewPixels != oldPixels)
        {
            forcePixels(actualNewPixels);
            didUpdateScrollPositionBy(actualNewPixels - oldPixels);
        }
        if (overscroll != 0.0)
        {
            didOverscrollBy(overscroll);
            return overscroll;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double applyClampedPointerSignalUpdate(double delta)
    {
        DartRuntimePrimitives.Assert(() => delta != 0.0);
        double min = (delta > 0.0) ? -double.PositiveInfinity : Math.Min(minScrollExtent, pixels);
        double max = (delta < 0.0) ? double.PositiveInfinity : Math.Max(maxScrollExtent, pixels);
        double newPixels = Dart_uiLibrary.clampDouble(pixels + delta, min, max);
        double clampedDelta = newPixels - pixels;
        if (clampedDelta == 0.0)
        {
            return delta;
        }
        forcePixels(newPixels);
        didUpdateScrollPositionBy(clampedDelta);
        return delta - clampedDelta;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.ScrollDirection userScrollDirection => coordinator.userScrollDirection;
    public virtual DrivenScrollActivity createDrivenScrollActivity(double to, Duration duration, global::Doroti.Framework.Animation.Curve curve)
    {
        return new DrivenScrollActivity(this, from: pixels, to: to, duration: duration, curve: curve, vsync: vsync);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyUserOffset(double delta)
    {
        DartRuntimePrimitives.Assert(() => false);
        _ = 0.0;
        return;
    }

    public virtual void goIdle()
    {
        beginActivity(new IdleScrollActivity(this));
        coordinator.updateUserScrollDirection(ScrollDirection.idle);
    }

    public virtual void goBallistic(double velocity)
    {
        global::Doroti.Framework.Physics.Simulation? simulation = default!;
        if ((velocity != 0.0) || outOfRange)
        {
            simulation = physics.createBallisticSimulation(this, velocity);
        }
        beginActivity(createBallisticScrollActivity(simulation, mode: _NestedBallisticScrollActivityMode__nested_scroll_view.independent));
    }

    public virtual ScrollActivity createBallisticScrollActivity(global::Doroti.Framework.Physics.Simulation? simulation, _NestedBallisticScrollActivityMode__nested_scroll_view mode, _NestedScrollMetrics__nested_scroll_view? metrics = null)
    {
        if (simulation is null)
        {
            return new IdleScrollActivity(this);
        }
        switch (mode)
        {
            case _NestedBallisticScrollActivityMode__nested_scroll_view.outer:
                {
                    DartRuntimePrimitives.Assert(() => metrics is not null);
                    if (metrics!.minRange == metrics.maxRange)
                    {
                        return new IdleScrollActivity(this);
                    }
                    return new _NestedOuterBallisticScrollActivity__nested_scroll_view(coordinator, this, metrics, simulation, context.vsync, shouldIgnorePointer);
                }
            case _NestedBallisticScrollActivityMode__nested_scroll_view.inner:
                {
                    return new _NestedInnerBallisticScrollActivity__nested_scroll_view(coordinator, this, simulation, context.vsync, shouldIgnorePointer);
                }
            case _NestedBallisticScrollActivityMode__nested_scroll_view.independent:
                {
                    return new BallisticScrollActivity(this, simulation, context.vsync, shouldIgnorePointer);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Future animateTo(double to, Duration duration, global::Doroti.Framework.Animation.Curve curve)
    {
        return coordinator.animateTo(coordinator.unnestOffset(to, this), duration: duration, curve: curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void jumpTo(double pixels)
    {
        coordinator.jumpTo(coordinator.unnestOffset(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(pixels)), this));
        return;
    }

    public override void pointerScroll(double delta)
    {
        coordinator.pointerScroll(delta);
        return;
    }

    public override void jumpToWithoutSettling(double value)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual void localJumpTo(double value)
    {
        if (pixels != DartRuntimePrimitives.RequireValue(value))
        {
            double oldPixels = pixels;
            forcePixels(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value)));
            didStartScroll();
            didUpdateScrollPositionBy(pixels - oldPixels);
            didEndScroll();
        }
    }

    public override void applyNewDimensions()
    {
        base.applyNewDimensions();
        coordinator.updateCanDrag();
    }

    public virtual void updateCanDrag(bool innerCanDrag)
    {
        DartRuntimePrimitives.Assert(() => Equals(coordinator._outerPosition, this));
        context.setCanDrag(physics.shouldAcceptUserOffset(this) || innerCanDrag);
    }

    public override ScrollHoldController hold(global::System.Action holdCancelCallback)
    {
        return coordinator.hold(() => holdCancelCallback());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Gestures.Drag drag(global::Doroti.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback)
    {
        return coordinator.drag(details, () => dragCancelCallback());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _NestedBallisticScrollActivityMode__nested_scroll_view
{
    outer,
    inner,
    independent
}

internal class _NestedInnerBallisticScrollActivity__nested_scroll_view : BallisticScrollActivity
{
    public virtual _NestedScrollCoordinator__nested_scroll_view coordinator { get; private set; } = default!;

    internal _NestedInnerBallisticScrollActivity__nested_scroll_view(_NestedScrollCoordinator__nested_scroll_view coordinator, _NestedScrollPosition__nested_scroll_view position, global::Doroti.Framework.Physics.Simulation simulation, global::Doroti.Framework.Scheduler.TickerProvider vsync, bool shouldIgnorePointer) : base(position, simulation, vsync, shouldIgnorePointer)
    {
        this.coordinator = coordinator;
    }

    public override ScrollActivityDelegate @delegate => DartRuntimePrimitives.ConvertValue<ScrollActivityDelegate>(((_NestedScrollPosition__nested_scroll_view?)base.@delegate)!);
    public override void resetActivity()
    {
        ((_NestedScrollPosition__nested_scroll_view)@delegate).beginActivity(coordinator.createInnerBallisticScrollActivity(DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(@delegate), velocity));
    }

    public override void applyNewDimensions()
    {
        ((_NestedScrollPosition__nested_scroll_view)@delegate).beginActivity(coordinator.createInnerBallisticScrollActivity(DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(@delegate), velocity));
    }

    public override bool applyMoveTo(double value)
    {
        return base.applyMoveTo(coordinator.nestOffset(value, DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(@delegate)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NestedOuterBallisticScrollActivity__nested_scroll_view : BallisticScrollActivity
{
    public virtual _NestedScrollCoordinator__nested_scroll_view coordinator { get; private set; } = default!;
    public virtual _NestedScrollMetrics__nested_scroll_view metrics { get; private set; } = default!;

    internal _NestedOuterBallisticScrollActivity__nested_scroll_view(_NestedScrollCoordinator__nested_scroll_view coordinator, _NestedScrollPosition__nested_scroll_view position, _NestedScrollMetrics__nested_scroll_view metrics, global::Doroti.Framework.Physics.Simulation simulation, global::Doroti.Framework.Scheduler.TickerProvider vsync, bool shouldIgnorePointer) : base(position, simulation, vsync, shouldIgnorePointer)
    {
        this.coordinator = coordinator;
        this.metrics = metrics;
        System.Diagnostics.Debug.Assert(metrics.minRange != metrics.maxRange);
        System.Diagnostics.Debug.Assert(metrics.maxRange > metrics.minRange);
    }

    public override ScrollActivityDelegate @delegate => DartRuntimePrimitives.ConvertValue<ScrollActivityDelegate>(((_NestedScrollPosition__nested_scroll_view?)base.@delegate)!);
    public override void resetActivity()
    {
        ((_NestedScrollPosition__nested_scroll_view)@delegate).beginActivity(coordinator.createOuterBallisticScrollActivity(velocity));
    }

    public override void applyNewDimensions()
    {
        ((_NestedScrollPosition__nested_scroll_view)@delegate).beginActivity(coordinator.createOuterBallisticScrollActivity(velocity));
    }

    public override bool applyMoveTo(double value)
    {
        var done = false;
        if (velocity > 0.0)
        {
            if (value < metrics.minRange)
            {
                return true;
            }
            if (value > metrics.maxRange)
            {
                value = metrics.maxRange;
                done = true;
            }
        }
        else
        {
            if (velocity < 0.0)
            {
                if (value > metrics.maxRange)
                {
                    return true;
                }
                if (value < metrics.minRange)
                {
                    value = metrics.minRange;
                    done = true;
                }
            }
            else
            {
                value = Dart_uiLibrary.clampDouble(value, metrics.minRange, metrics.maxRange);
                done = true;
            }
        }
        bool result = base.applyMoveTo(value + metrics.correctionOffset);
        DartRuntimePrimitives.Assert(() => result);
        return !done;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "_NestedOuterBallisticScrollActivity")}({metrics.minRange} .. {metrics.maxRange}; correcting by {metrics.correctionOffset})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverOverlapAbsorberHandle : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual long _writers { get; set; } = 0L;
    internal virtual double? _layoutExtent { get; set; } = default;
    internal virtual double? _scrollExtent { get; set; } = default;

    public SliverOverlapAbsorberHandle()
    {
    }

    public virtual double? layoutExtent => _layoutExtent;
    public virtual double? scrollExtent => _scrollExtent;
    internal virtual void _setExtents(double? layoutValue, double? scrollValue)
    {
        DartRuntimePrimitives.Assert(() => _writers == 1L, () => (object?)"Multiple RenderSliverOverlapAbsorbers have been provided the same SliverOverlapAbsorberHandle.");
        _layoutExtent = layoutValue;
        _scrollExtent = scrollValue;
    }

    internal virtual void _markNeedsLayout() => notifyListeners();
    public override string ToString()
    {
        string? extra = _writers switch { 0L => ", orphan", 1L => DartRuntimePrimitives.ConvertValue<string>(null), _ => $", {_writers} WRITERS ASSIGNED" };
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "SliverOverlapAbsorberHandle")}({layoutExtent}{extra})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverOverlapAbsorber : SingleChildRenderObjectWidget
{
    public virtual SliverOverlapAbsorberHandle handle { get; private set; } = default!;

    public SliverOverlapAbsorber(global::Doroti.Framework.Foundation.Key? key = null, SliverOverlapAbsorberHandle handle = default!, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.handle = handle;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverOverlapAbsorber(handle: handle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (RenderSliverOverlapAbsorber)renderObject;
        __renderObject.handle = handle;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", handle));
    }

}

public class RenderSliverOverlapAbsorber : global::Doroti.Framework.Rendering.RenderSliver, global::Doroti.Framework.Rendering.RenderObjectWithChildMixin<global::Doroti.Framework.Rendering.RenderSliver>
{
    internal virtual SliverOverlapAbsorberHandle _handle { get; set; } = default!;
    public virtual RenderSliver? _child { get; set; } = default;

    public RenderSliverOverlapAbsorber(SliverOverlapAbsorberHandle handle, global::Doroti.Framework.Rendering.RenderSliver? sliver = null)
    {
        _handle = handle;
    }

    public virtual SliverOverlapAbsorberHandle handle
    {
        get => _handle;
        set
        {
            var __value = value;
            if (Equals(handle, __value))
            {
                return;
            }
            if (attached)
            {
                handle._writers -= 1L;
                __value._writers += 1L;
                __value._setExtents(handle.layoutExtent, handle.scrollExtent);
            }
            _handle = __value;
        }
    }
    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        _child?.attach(owner);
        handle._writers += 1L;
    }

    public override void detach()
    {
        handle._writers -= 1L;
        base.detach();
        _child?.detach();
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => handle._writers == 1L, () => (object?)"A SliverOverlapAbsorberHandle cannot be passed to multiple RenderSliverOverlapAbsorber objects at the same time.");
        if (child is null)
        {
            geometry = SliverGeometry.zero;
            return;
        }
        child!.layout(constraints, parentUsesSize: true);
        global::Doroti.Framework.Rendering.SliverGeometry childLayoutGeometry = child!.geometry!;
        geometry = childLayoutGeometry.copyWith(scrollExtent: childLayoutGeometry.scrollExtent - childLayoutGeometry.maxScrollObstructionExtent, layoutExtent: Math.Max(0, childLayoutGeometry.paintExtent - childLayoutGeometry.maxScrollObstructionExtent));
        handle._setExtents(childLayoutGeometry.maxScrollObstructionExtent, childLayoutGeometry.maxScrollObstructionExtent);
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        if (child is not null)
        {
            return child!.hitTest(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            context.paintChild(child!, offset);
        }
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", handle));
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderSliver)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? child
    {
        get => _child;
        set
        {
            var __value = value;
            if (_child is not null)
            {
                dropChild(_child!);
            }
            _child = __value;
            if (_child is not null)
            {
                adoptChild(_child!);
            }
        }
    }
    public override void redepthChildren()
    {
        if (_child is not null)
        {
            redepthChild(_child!);
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return (child is not null) ? new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { ((Diagnosticable)child!).toDiagnosticsNode(name: "child") } : new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverOverlapInjector : SingleChildRenderObjectWidget
{
    public virtual SliverOverlapAbsorberHandle handle { get; private set; } = default!;

    public SliverOverlapInjector(global::Doroti.Framework.Foundation.Key? key = null, SliverOverlapAbsorberHandle handle = default!, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.handle = handle;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new RenderSliverOverlapInjector(handle: handle);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (RenderSliverOverlapInjector)renderObject;
        __renderObject.handle = handle;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", handle));
    }

}

public class RenderSliverOverlapInjector : global::Doroti.Framework.Rendering.RenderSliver
{
    internal virtual double? _currentLayoutExtent { get; set; } = default;
    internal virtual double? _currentMaxExtent { get; set; } = default;
    internal virtual SliverOverlapAbsorberHandle _handle { get; set; } = default!;

    public RenderSliverOverlapInjector(SliverOverlapAbsorberHandle handle)
    {
        _handle = handle;
    }

    public virtual SliverOverlapAbsorberHandle handle
    {
        get => _handle;
        set
        {
            var __value = value;
            if (Equals(handle, __value))
            {
                return;
            }
            if (attached)
            {
                handle.removeListener(markNeedsLayout);
            }
            _handle = __value;
            if (attached)
            {
                handle.addListener(markNeedsLayout);
                if ((handle.layoutExtent != _currentLayoutExtent) || (handle.scrollExtent != _currentMaxExtent))
                {
                    markNeedsLayout();
                }
            }
        }
    }
    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        handle.addListener(markNeedsLayout);
        if ((handle.layoutExtent != _currentLayoutExtent) || (handle.scrollExtent != _currentMaxExtent))
        {
            markNeedsLayout();
        }
    }

    public override void detach()
    {
        handle.removeListener(markNeedsLayout);
        base.detach();
    }

    public override void performLayout()
    {
        _currentLayoutExtent = handle.layoutExtent;
        _currentMaxExtent = handle.layoutExtent;
        DartRuntimePrimitives.Assert(() => (_currentLayoutExtent is not null) && (_currentMaxExtent is not null), () => (object?)"SliverOverlapInjector has found no absorbed extent to inject.\n " + "The SliverOverlapAbsorber must be an earlier descendant of a common " + "ancestor Viewport, so that it will always be laid out before the " + "SliverOverlapInjector during a particular frame.\n " + "The SliverOverlapAbsorber is typically contained in the list of slivers " + "provided by NestedScrollView.headerSliverBuilder.\n");
        double clampedPaintExtent = Math.Min(DartRuntimePrimitives.RequireValue(_currentLayoutExtent), constraints.remainingPaintExtent);
        double clampedLayoutExtent = Math.Min(DartRuntimePrimitives.RequireValue(_currentLayoutExtent) - constraints.scrollOffset, constraints.remainingPaintExtent);
        geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: DartRuntimePrimitives.RequireValue(_currentLayoutExtent), paintExtent: Math.Max(0.0, clampedPaintExtent), layoutExtent: Math.Max(0.0, clampedLayoutExtent), maxPaintExtent: DartRuntimePrimitives.RequireValue(_currentMaxExtent));
    }

    public override void debugPaint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (Rendering.DebugLibrary.debugPaintSizeEnabled)
                {
                    var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = new global::Doroti.Ui.Color(4291598643L);
    __cascade.strokeWidth = 3.0;
    __cascade.style = PaintingStyle.stroke;
    return __cascade;
}))();
                    global::Doroti.Ui.Offset start = default!;
                    global::Doroti.Ui.Offset end = default!;
                    global::Doroti.Ui.Offset delta = default!;
                    switch (constraints.axis)
                    {
                        case Axis.vertical:
                            {
                                double x = offset.dx + (constraints.crossAxisExtent / 2.0);
                                start = new global::Doroti.Ui.Offset(x, offset.dy);
                                end = new global::Doroti.Ui.Offset(x, offset.dy + geometry!.paintExtent);
                                delta = new global::Doroti.Ui.Offset(constraints.crossAxisExtent / 5.0, 0.0);
                                break;
                            }
                        case Axis.horizontal:
                            {
                                double y = offset.dy + (constraints.crossAxisExtent / 2.0);
                                start = new global::Doroti.Ui.Offset(offset.dx, y);
                                end = new global::Doroti.Ui.Offset(offset.dy + geometry!.paintExtent, y);
                                delta = new global::Doroti.Ui.Offset(0.0, constraints.crossAxisExtent / 5.0);
                                break;
                            }
                    }
                    for (var index = -2L; index <= 2L; index += 1L)
                    {
                        Paint_utilitiesLibrary.paintZigZag(context.canvas, paint, start - (delta * index.toDouble()), end - (delta * index.toDouble()), 10L, 10.0);
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", handle));
    }

}

public class NestedScrollViewViewport : Viewport
{
    public virtual SliverOverlapAbsorberHandle handle { get; private set; } = default!;

    public NestedScrollViewViewport(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection? crossAxisDirection = null, double anchor = 0.0, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, global::Doroti.Framework.Foundation.Key? center = null, List<Widget> slivers = default!, SliverOverlapAbsorberHandle handle = default!, Clip clipBehavior = Clip.hardEdge) : base(key: key, axisDirection: axisDirection, crossAxisDirection: DartRuntimePrimitives.RequireValue(crossAxisDirection), anchor: anchor, offset: offset, center: center, slivers: slivers ?? new List<Widget>(), clipBehavior: clipBehavior)
    {
        this.handle = handle;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new RenderNestedScrollViewViewport(axisDirection: axisDirection, crossAxisDirection: crossAxisDirection ?? getDefaultCrossAxisDirection(context, axisDirection), anchor: anchor, offset: offset, handle: handle, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (RenderNestedScrollViewViewport)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderNestedScrollViewViewport>)(() =>
{
    var __cascade = __renderObject;
    __cascade.axisDirection = axisDirection;
    __cascade.crossAxisDirection = crossAxisDirection ?? getDefaultCrossAxisDirection(context, axisDirection);
    __cascade.anchor = anchor;
    __cascade.offset = offset;
    __cascade.handle = handle;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", handle));
    }

}

public class RenderNestedScrollViewViewport : global::Doroti.Framework.Rendering.RenderViewport
{
    internal virtual SliverOverlapAbsorberHandle _handle { get; set; } = default!;

    public RenderNestedScrollViewViewport(global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Painting.AxisDirection crossAxisDirection = default!, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, double anchor = 0.0, List<global::Doroti.Framework.Rendering.RenderSliver>? children = null, global::Doroti.Framework.Rendering.RenderSliver? center = null, SliverOverlapAbsorberHandle handle = default!, Clip clipBehavior = Clip.hardEdge) : base(axisDirection: axisDirection, crossAxisDirection: crossAxisDirection, offset: offset, anchor: anchor, children: children, center: center, clipBehavior: clipBehavior)
    {
        _handle = handle;
    }

    public virtual SliverOverlapAbsorberHandle handle
    {
        get => _handle;
        set
        {
            var __value = value;
            if (Equals(handle, __value))
            {
                return;
            }
            _handle = __value;
            handle._markNeedsLayout();
        }
    }
    public override void markNeedsLayout()
    {
        handle._markNeedsLayout();
        base.markNeedsLayout();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", handle));
    }

}
