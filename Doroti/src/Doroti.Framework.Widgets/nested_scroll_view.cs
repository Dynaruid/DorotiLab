// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/nested_scroll_view.dart
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

public delegate List<Widget> NestedScrollViewHeaderSliversBuilder(BuildContext context, bool innerBoxIsScrolled);

public class NestedScrollView : StatefulWidget
{
    public virtual ScrollController? controller { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual global::System.Func<BuildContext, bool, List<Widget>> headerSliverBuilder { get; private set; } = default!;
    public virtual Widget body { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual bool floatHeaderSlivers { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }

    public NestedScrollView(global::Doroti.Generated.Framework.Foundation.Key? key = null, ScrollController? controller = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollPhysics? physics = null, global::System.Func<BuildContext, bool, List<Widget>> headerSliverBuilder = default!, Widget body = default!, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, bool floatHeaderSlivers = false, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, string? restorationId = null, ScrollBehavior? scrollBehavior = null) : base(key: key)
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
        _InheritedNestedScrollView__nested_scroll_view? target__15340 = ((_InheritedNestedScrollView__nested_scroll_view?)(object?)context.dependOnInheritedWidgetOfExactType<_InheritedNestedScrollView__nested_scroll_view>());
        DartRuntimePrimitives.Assert(() => (target__15340 is not null), () => (object?)"NestedScrollView.sliverOverlapAbsorberHandleFor must be called with a context that contains a NestedScrollView.");
        return target__15340!.state._absorberHandle;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<Widget> _buildSlivers(BuildContext context, ScrollController innerController, bool bodyIsScrolled)
    {
        return new List<Widget> { new SliverFillRemaining(child: new PrimaryScrollController(automaticallyInheritForPlatforms: System.Enum.GetValues<global::Doroti.Generated.Framework.Foundation.TargetPlatform>().ToList().toSet(), controller: innerController, child: this.body)) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new NestedScrollViewState());
}

public class NestedScrollViewState : State<NestedScrollView>
{
    internal virtual SliverOverlapAbsorberHandle _absorberHandle { get; private set; } = new SliverOverlapAbsorberHandle();
    internal virtual _NestedScrollCoordinator__nested_scroll_view? _coordinator { get; set; } = default;
    internal virtual bool? _lastHasScrolledBody { get; set; } = default;

    public virtual ScrollController innerController => DartRuntimePrimitives.ConvertValue<ScrollController>(this._coordinator!._innerController);
    public virtual ScrollController outerController => DartRuntimePrimitives.ConvertValue<ScrollController>(this._coordinator!._outerController);
    public override void initState()
    {
        base.initState();
        _coordinator = new _NestedScrollCoordinator__nested_scroll_view(this, ((NestedScrollView)this.widget).controller, () => this._handleHasScrolledBodyChanged(), ((NestedScrollView)this.widget).floatHeaderSlivers);
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        this._coordinator!.setParent(((NestedScrollView)this.widget).controller);
    }

    public override void didUpdateWidget(NestedScrollView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((NestedScrollView)oldWidget).controller, ((NestedScrollView)this.widget).controller)))
        {
            this._coordinator!.setParent(((NestedScrollView)this.widget).controller);
        }
    }

    public override void dispose()
    {
        this._coordinator!.dispose();
        _coordinator = null;
        this._absorberHandle.dispose();
        base.dispose();
    }

    internal virtual void _handleHasScrolledBodyChanged()
    {
        if (!this.mounted)
        {
            return;
        }
        bool newHasScrolledBody__20173 = this._coordinator!.hasScrolledBody;
        if ((this._lastHasScrolledBody != newHasScrolledBody__20173))
        {
            setState(((global::System.Action)(() => {
})));
        }
    }

    public override Widget build(BuildContext context)
    {
        ScrollPhysics scrollPhysics__20789 = (((((NestedScrollView)this.widget).physics?.applyTo(new ClampingScrollPhysics()) ?? (ScrollPhysics)((NestedScrollView)this.widget).scrollBehavior?.getScrollPhysics(context).applyTo(new ClampingScrollPhysics()))) ?? new ClampingScrollPhysics());
        return ((Widget)(object?)new _InheritedNestedScrollView__nested_scroll_view(state: this, child: new Builder(builder: ((global::System.Func<BuildContext, Widget>)((context) => {
_lastHasScrolledBody = this._coordinator!.hasScrolledBody;
return ((Widget)(object?)new _NestedScrollViewCustomScrollView__nested_scroll_view(dragStartBehavior: ((NestedScrollView)this.widget).dragStartBehavior, scrollDirection: ((NestedScrollView)this.widget).scrollDirection, reverse: ((NestedScrollView)this.widget).reverse, physics: scrollPhysics__20789, scrollBehavior: ((((NestedScrollView)this.widget).scrollBehavior ?? (ScrollBehavior)ScrollConfiguration.of(context).copyWith(scrollbars: false))), controller: this._coordinator!._outerController, slivers: this.widget._buildSlivers(context, this._coordinator!._innerController, DartRuntimePrimitives.RequireValue(this._lastHasScrolledBody)), handle: this._absorberHandle, clipBehavior: ((NestedScrollView)this.widget).clipBehavior, restorationId: ((NestedScrollView)this.widget).restorationId, hitTestBehavior: ((NestedScrollView)this.widget).hitTestBehavior));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NestedScrollViewCustomScrollView__nested_scroll_view : CustomScrollView
{
    public virtual SliverOverlapAbsorberHandle handle { get; private set; } = default!;

    internal _NestedScrollViewCustomScrollView__nested_scroll_view(global::Doroti.Generated.Framework.Painting.Axis scrollDirection, bool reverse, ScrollPhysics physics, ScrollBehavior scrollBehavior, ScrollController controller, List<Widget> slivers, SliverOverlapAbsorberHandle handle, Clip clipBehavior, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, string? restorationId = null) : base(scrollDirection: scrollDirection, reverse: reverse, physics: physics, scrollBehavior: scrollBehavior, controller: controller, slivers: slivers, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior, dragStartBehavior: dragStartBehavior, restorationId: restorationId)
    {
        this.handle = handle;
    }

    public override Widget buildViewport(BuildContext context, global::Doroti.Generated.Framework.Rendering.ViewportOffset offset, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection, List<Widget> slivers)
    {
        DartRuntimePrimitives.Assert(() => !this.shrinkWrap);
        return ((Widget)(object?)new NestedScrollViewViewport(axisDirection: axisDirection, offset: offset, slivers: slivers, handle: this.handle, clipBehavior: this.clipBehavior));
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

    public override bool updateShouldNotify(InheritedWidget oldWidget) => (!object.Equals(this.state, ((_InheritedNestedScrollView__nested_scroll_view)oldWidget).state));
}

public class _NestedScrollMetrics__nested_scroll_view : FixedScrollMetrics
{
    public virtual double minRange { get; private set; } = default!;
    public virtual double maxRange { get; private set; } = default!;
    public virtual double correctionOffset { get; private set; } = default!;

    internal _NestedScrollMetrics__nested_scroll_view(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection, double devicePixelRatio, double minRange, double maxRange, double correctionOffset) : base(minScrollExtent: DartRuntimePrimitives.RequireValue(minScrollExtent), maxScrollExtent: DartRuntimePrimitives.RequireValue(maxScrollExtent), pixels: DartRuntimePrimitives.RequireValue(pixels), viewportDimension: DartRuntimePrimitives.RequireValue(viewportDimension), axisDirection: axisDirection, devicePixelRatio: devicePixelRatio)
    {
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.correctionOffset = correctionOffset;
    }

    public virtual _NestedScrollMetrics__nested_scroll_view copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Generated.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new _NestedScrollMetrics__nested_scroll_view(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio), minRange: (minRange ?? this.minRange), maxRange: (maxRange ?? this.maxRange), correctionOffset: (correctionOffset ?? this.correctionOffset));
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
    internal virtual global::Doroti.Generated.Framework.Rendering.ScrollDirection _userScrollDirection { get; set; } = global::Doroti.Generated.Framework.Rendering.ScrollDirection.idle;
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
            return (((this._outerPosition?.outOfRange ?? false)) || this._innerPositions.any(((position) => position.outOfRange)));
            return default!;
        }
    }
    internal virtual _NestedScrollPosition__nested_scroll_view? _outerPosition
    {
        get
        {
            if (!this._outerController.hasClients)
            {
                return ((_NestedScrollPosition__nested_scroll_view)(object)null);
            }
            return ((_NestedScrollController__nested_scroll_view)this._outerController).nestedPositions.Single();
            return default!;
        }
    }
    internal virtual IEnumerable<_NestedScrollPosition__nested_scroll_view> _innerPositions
    {
        get
        {
            return ((_NestedScrollController__nested_scroll_view)this._innerController).nestedPositions;
            return default!;
        }
    }
    public virtual bool canScrollBody
    {
        get
        {
            _NestedScrollPosition__nested_scroll_view? outer__26121 = this._outerPosition;
            if ((outer__26121 is null))
            {
                return true;
            }
            return (outer__26121.haveDimensions && (outer__26121.extentAfter == 0.0));
            return default!;
        }
    }
    public virtual bool hasScrolledBody
    {
        get
        {
            foreach (_NestedScrollPosition__nested_scroll_view position__26327 in this._innerPositions)
            {
                if ((!position__26327.hasContentDimensions || !position__26327.hasPixels))
                {
                    continue;
                }
                else
                {
                    if ((position__26327.pixels > position__26327.minScrollExtent))
                    {
                        return true;
                    }
                }
            }
            return false;
            return default!;
        }
    }
    public virtual void updateShadow()
    {
        this._onHasScrolledBodyChanged();
    }

    public virtual global::Doroti.Generated.Framework.Rendering.ScrollDirection userScrollDirection => this._userScrollDirection;
    public virtual void updateUserScrollDirection(global::Doroti.Generated.Framework.Rendering.ScrollDirection value)
    {
        if ((object.Equals(this.userScrollDirection, DartRuntimePrimitives.RequireValue(value))))
        {
            return;
        }
        _userScrollDirection = DartRuntimePrimitives.RequireValue(value);
        this._outerPosition!.didUpdateScrollDirection(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value)));
        foreach (_NestedScrollPosition__nested_scroll_view position__27296 in this._innerPositions)
        {
            position__27296.didUpdateScrollDirection(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value)));
        }
    }

    public virtual void beginActivity(ScrollActivity newOuterActivity, global::System.Func<_NestedScrollPosition__nested_scroll_view, ScrollActivity> innerActivityGetter)
    {
        ((dynamic)this._outerPosition!).beginActivity(newOuterActivity);
        bool scrolling__27605 = ((ScrollActivity)newOuterActivity).isScrolling;
        foreach (_NestedScrollPosition__nested_scroll_view position__27684 in this._innerPositions)
        {
            ScrollActivity newInnerActivity__27742 = innerActivityGetter(position__27684);
            ((dynamic)position__27684).beginActivity(newInnerActivity__27742);
            scrolling__27605 = (scrolling__27605 && ((ScrollActivity)newInnerActivity__27742).isScrolling);
        }
        this._currentDrag?.dispose();
        _currentDrag = null;
        if (!scrolling__27605)
        {
            updateUserScrollDirection(global::Doroti.Generated.Framework.Rendering.ScrollDirection.idle);
        }
    }

    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection => this._outerPosition!.axisDirection;
    internal static IdleScrollActivity _createIdleScrollActivity(_NestedScrollPosition__nested_scroll_view position)
    {
        return new IdleScrollActivity(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void goIdle()
    {
        beginActivity(_NestedScrollCoordinator__nested_scroll_view._createIdleScrollActivity(this._outerPosition!), (global::System.Func<_NestedScrollPosition__nested_scroll_view, IdleScrollActivity>)_createIdleScrollActivity);
    }

    public virtual void goBallistic(double velocity)
    {
        beginActivity(createOuterBallisticScrollActivity(velocity), ((global::System.Func<_NestedScrollPosition__nested_scroll_view, ScrollActivity>)((position) => {
return ((ScrollActivity)(object?)createInnerBallisticScrollActivity(position, velocity));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
    }

    public virtual ScrollActivity createOuterBallisticScrollActivity(double velocity)
    {
        _NestedScrollPosition__nested_scroll_view? innerPosition__29317 = default!;
        if ((velocity != 0.0))
        {
            foreach (_NestedScrollPosition__nested_scroll_view position__29398 in this._innerPositions)
            {
                if ((innerPosition__29317 is not null))
                {
                    if ((velocity > 0.0))
                    {
                        if ((innerPosition__29317.pixels < position__29398.pixels))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => (velocity < 0.0));
                        if ((innerPosition__29317.pixels > position__29398.pixels))
                        {
                            continue;
                        }
                    }
                }
                innerPosition__29317 = position__29398;
            }
        }
        if ((innerPosition__29317 is null))
        {
            return ((ScrollActivity)(object?)this._outerPosition!.createBallisticScrollActivity(this._outerPosition!.physics.createBallisticSimulation(this._outerPosition!, velocity), mode: _NestedBallisticScrollActivityMode__nested_scroll_view.independent));
        }
        _NestedScrollMetrics__nested_scroll_view metrics__30160 = ((_NestedScrollMetrics__nested_scroll_view)(object?)_getMetrics(innerPosition__29317, velocity));
        return ((ScrollActivity)(object?)this._outerPosition!.createBallisticScrollActivity(this._outerPosition!.physics.createBallisticSimulation(metrics__30160, velocity), mode: _NestedBallisticScrollActivityMode__nested_scroll_view.outer, metrics: metrics__30160));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ScrollActivity createInnerBallisticScrollActivity(_NestedScrollPosition__nested_scroll_view position, double velocity)
    {
        return ((ScrollActivity)(object?)position.createBallisticScrollActivity(position.physics.createBallisticSimulation(_getMetrics(position, velocity), velocity), mode: _NestedBallisticScrollActivityMode__nested_scroll_view.inner));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _NestedScrollMetrics__nested_scroll_view _getMetrics(_NestedScrollPosition__nested_scroll_view innerPosition, double velocity)
    {
        double pixels__30874 = default!;
        double minRange__30882 = default!;
        double maxRange__30892 = default!;
        double correctionOffset__30902 = default!;
        var extra__30928 = 0.0;
        if ((innerPosition.pixels == innerPosition.minScrollExtent))
        {
            pixels__30874 = Dart_uiLibrary.clampDouble(this._outerPosition!.pixels, this._outerPosition!.minScrollExtent, this._outerPosition!.maxScrollExtent);
            minRange__30882 = this._outerPosition!.minScrollExtent;
            maxRange__30892 = this._outerPosition!.maxScrollExtent;
            DartRuntimePrimitives.Assert(() => (minRange__30882 <= maxRange__30892));
            correctionOffset__30902 = 0.0;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (innerPosition.pixels != innerPosition.minScrollExtent));
            if ((innerPosition.pixels < innerPosition.minScrollExtent))
            {
                pixels__30874 = ((innerPosition.pixels - innerPosition.minScrollExtent) + this._outerPosition!.minScrollExtent);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => (innerPosition.pixels > innerPosition.minScrollExtent));
                pixels__30874 = ((innerPosition.pixels - innerPosition.minScrollExtent) + this._outerPosition!.maxScrollExtent);
            }
            if ((((velocity > 0.0)) && ((innerPosition.pixels > innerPosition.minScrollExtent))))
            {
                extra__30928 = (this._outerPosition!.maxScrollExtent - this._outerPosition!.pixels);
                DartRuntimePrimitives.Assert(() => (extra__30928 >= 0.0));
                minRange__30882 = pixels__30874;
                maxRange__30892 = (pixels__30874 + extra__30928);
                DartRuntimePrimitives.Assert(() => (minRange__30882 <= maxRange__30892));
                correctionOffset__30902 = (this._outerPosition!.pixels - pixels__30874);
            }
            else
            {
                if ((((velocity < 0.0)) && ((innerPosition.pixels < innerPosition.minScrollExtent))))
                {
                    extra__30928 = (this._outerPosition!.pixels - this._outerPosition!.minScrollExtent);
                    DartRuntimePrimitives.Assert(() => (extra__30928 >= 0.0));
                    minRange__30882 = (pixels__30874 - extra__30928);
                    maxRange__30892 = pixels__30874;
                    DartRuntimePrimitives.Assert(() => (minRange__30882 <= maxRange__30892));
                    correctionOffset__30902 = (this._outerPosition!.pixels - pixels__30874);
                }
                else
                {
                    if ((velocity > 0.0))
                    {
                        extra__30928 = (this._outerPosition!.minScrollExtent - this._outerPosition!.pixels);
                    }
                    else
                    {
                        if ((velocity < 0.0))
                        {
                            extra__30928 = (this._outerPosition!.pixels - ((this._outerPosition!.maxScrollExtent - this._outerPosition!.minScrollExtent)));
                        }
                    }
                    DartRuntimePrimitives.Assert(() => (extra__30928 <= 0.0));
                    minRange__30882 = this._outerPosition!.minScrollExtent;
                    maxRange__30892 = (this._outerPosition!.maxScrollExtent + extra__30928);
                    DartRuntimePrimitives.Assert(() => (minRange__30882 <= maxRange__30892));
                    correctionOffset__30902 = 0.0;
                }
            }
        }
        return new _NestedScrollMetrics__nested_scroll_view(minScrollExtent: this._outerPosition!.minScrollExtent, maxScrollExtent: (((this._outerPosition!.maxScrollExtent + innerPosition.maxScrollExtent) - innerPosition.minScrollExtent) + extra__30928), pixels: pixels__30874, viewportDimension: this._outerPosition!.viewportDimension, axisDirection: this._outerPosition!.axisDirection, minRange: minRange__30882, maxRange: maxRange__30892, correctionOffset: correctionOffset__30902, devicePixelRatio: this._outerPosition!.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double unnestOffset(double value, _NestedScrollPosition__nested_scroll_view source)
    {
        if ((object.Equals(source, this._outerPosition)))
        {
            return Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), this._outerPosition!.minScrollExtent, this._outerPosition!.maxScrollExtent);
        }
        if ((DartRuntimePrimitives.RequireValue(value) < source.minScrollExtent))
        {
            return ((DartRuntimePrimitives.RequireValue(value) - source.minScrollExtent) + this._outerPosition!.minScrollExtent);
        }
        return ((DartRuntimePrimitives.RequireValue(value) - source.minScrollExtent) + this._outerPosition!.maxScrollExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double nestOffset(double value, _NestedScrollPosition__nested_scroll_view target)
    {
        if ((object.Equals(target, this._outerPosition)))
        {
            return Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(value), this._outerPosition!.minScrollExtent, this._outerPosition!.maxScrollExtent);
        }
        if ((DartRuntimePrimitives.RequireValue(value) < this._outerPosition!.minScrollExtent))
        {
            return ((DartRuntimePrimitives.RequireValue(value) - this._outerPosition!.minScrollExtent) + target.minScrollExtent);
        }
        if ((DartRuntimePrimitives.RequireValue(value) > this._outerPosition!.maxScrollExtent))
        {
            return ((DartRuntimePrimitives.RequireValue(value) - this._outerPosition!.maxScrollExtent) + target.minScrollExtent);
        }
        return target.minScrollExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateCanDrag()
    {
        if (!this._outerPosition!.haveDimensions)
        {
            return;
        }
        var innerCanDrag__35291 = false;
        foreach (_NestedScrollPosition__nested_scroll_view position__35350 in this._innerPositions)
        {
            if (!position__35350.haveDimensions)
            {
                return;
            }
            innerCanDrag__35291 = (innerCanDrag__35291 || position__35350.physics.shouldAcceptUserOffset(position__35350));
        }
        this._outerPosition!.updateCanDrag(innerCanDrag__35291);
    }

    public async virtual Future animateTo(double to, Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve)
    {
        DrivenScrollActivity outerActivity__35971 = ((DrivenScrollActivity)(object?)this._outerPosition!.createDrivenScrollActivity(nestOffset(to, this._outerPosition!), duration, curve));
        var resultFutures__36116 = new List<Future> { ((DrivenScrollActivity)outerActivity__35971).done };
        beginActivity(outerActivity__35971, ((global::System.Func<_NestedScrollPosition__nested_scroll_view, ScrollActivity>)((position) => {
DrivenScrollActivity innerActivity__36269 = ((DrivenScrollActivity)(object?)position.createDrivenScrollActivity(nestOffset(to, position), duration, curve));
resultFutures__36116.Add(((DrivenScrollActivity)innerActivity__36269).done);
return ((ScrollActivity)(object?)innerActivity__36269);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        await global::Doroti.Runtime.DartAsyncRuntime.wait<object?>(resultFutures__36116);
    }

    public virtual void jumpTo(double to)
    {
        goIdle();
        this._outerPosition!.localJumpTo(nestOffset(to, this._outerPosition!));
        foreach (_NestedScrollPosition__nested_scroll_view position__36672 in this._innerPositions)
        {
            position__36672.localJumpTo(nestOffset(to, position__36672));
        }
        goBallistic(0.0);
    }

    public virtual void pointerScroll(double delta)
    {
        if ((delta == 0.0))
        {
            goBallistic(0.0);
            return;
        }
        goIdle();
        updateUserScrollDirection(((delta < 0.0) ? global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward : global::Doroti.Generated.Framework.Rendering.ScrollDirection.reverse));
        this._outerPosition!.isScrollingNotifier.value = true;
        this._outerPosition!.didStartScroll();
        foreach (_NestedScrollPosition__nested_scroll_view position__37487 in this._innerPositions)
        {
            position__37487.isScrollingNotifier.value = true;
            position__37487.didStartScroll();
        }
        if (!System.Linq.Enumerable.Any(this._innerPositions))
        {
            this._outerPosition!.applyClampedPointerSignalUpdate(delta);
        }
        else
        {
            if ((delta > 0.0))
            {
                var outerDelta__37967 = delta;
                foreach (_NestedScrollPosition__nested_scroll_view position__38026 in this._innerPositions)
                {
                    if ((position__38026.pixels < 0.0))
                    {
                        double potentialOuterDelta__38168 = position__38026.applyClampedPointerSignalUpdate(delta);
                        outerDelta__37967 = Math.Max(outerDelta__37967, potentialOuterDelta__38168);
                    }
                }
                if ((outerDelta__37967 != 0.0))
                {
                    double innerDelta__38544 = this._outerPosition!.applyClampedPointerSignalUpdate(outerDelta__37967);
                    if ((innerDelta__38544 != 0.0))
                    {
                        foreach (_NestedScrollPosition__nested_scroll_view position__38694 in this._innerPositions)
                        {
                            position__38694.applyClampedPointerSignalUpdate(innerDelta__38544);
                        }
                    }
                }
            }
            else
            {
                var innerDelta__38889 = delta;
                if (this._floatHeaderSlivers)
                {
                    innerDelta__38889 = this._outerPosition!.applyClampedPointerSignalUpdate(delta);
                }
                if ((innerDelta__38889 != 0.0))
                {
                    var outerDelta__39342 = 0.0;
                    foreach (_NestedScrollPosition__nested_scroll_view position__39438 in this._innerPositions)
                    {
                        double overscroll__39492 = position__39438.applyClampedPointerSignalUpdate(innerDelta__38889);
                        outerDelta__39342 = Math.Min(outerDelta__39342, overscroll__39492);
                    }
                    if ((outerDelta__39342 != 0.0))
                    {
                        this._outerPosition!.applyClampedPointerSignalUpdate(outerDelta__39342);
                    }
                }
            }
        }
        this._outerPosition!.didEndScroll();
        foreach (_NestedScrollPosition__nested_scroll_view position__39828 in this._innerPositions)
        {
            position__39828.didEndScroll();
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
        beginActivity(new HoldScrollActivity(@delegate: this._outerPosition!, onHoldCanceled: () => holdCancelCallback()), ((global::System.Func<_NestedScrollPosition__nested_scroll_view, ScrollActivity>)((position) => new HoldScrollActivity(@delegate: position))));
        return ((ScrollHoldController)(object?)this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void cancel()
    {
        goBallistic(0.0);
    }

    public virtual global::Doroti.Generated.Framework.Gestures.Drag drag(global::Doroti.Generated.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback)
    {
        var drag__40436 = new ScrollDragController(@delegate: this, details: details, onDragCanceled: () => dragCancelCallback());
        beginActivity(new DragScrollActivity(this._outerPosition!, drag__40436), ((global::System.Func<_NestedScrollPosition__nested_scroll_view, ScrollActivity>)((position) => new DragScrollActivity(position, drag__40436))));
        DartRuntimePrimitives.Assert(() => (this._currentDrag is null));
        _currentDrag = drag__40436;
        return ((global::Doroti.Generated.Framework.Gestures.Drag)(object?)drag__40436);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyUserOffset(double delta)
    {
        updateUserScrollDirection(((delta > 0.0) ? global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward : global::Doroti.Generated.Framework.Rendering.ScrollDirection.reverse));
        DartRuntimePrimitives.Assert(() => (delta != 0.0));
        if (!System.Linq.Enumerable.Any(this._innerPositions))
        {
            this._outerPosition!.applyFullDragUpdate(delta);
        }
        else
        {
            if ((delta < 0.0))
            {
                var outerDelta__41259 = delta;
                foreach (_NestedScrollPosition__nested_scroll_view position__41318 in this._innerPositions)
                {
                    if ((position__41318.pixels < 0.0))
                    {
                        double potentialOuterDelta__41460 = position__41318.applyClampedDragUpdate(delta);
                        outerDelta__41259 = Math.Max(outerDelta__41259, potentialOuterDelta__41460);
                    }
                }
                if ((outerDelta__41259.abs() > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    double innerDelta__41852 = this._outerPosition!.applyClampedDragUpdate(outerDelta__41259);
                    if ((innerDelta__41852 != 0.0))
                    {
                        foreach (_NestedScrollPosition__nested_scroll_view position__41993 in this._innerPositions)
                        {
                            position__41993.applyFullDragUpdate(innerDelta__41852);
                        }
                    }
                }
            }
            else
            {
                var innerDelta__42176 = delta;
                if (this._floatHeaderSlivers)
                {
                    innerDelta__42176 = this._outerPosition!.applyClampedDragUpdate(delta);
                }
                if ((innerDelta__42176 != 0.0))
                {
                    var outerDelta__42620 = 0.0;
                    var overscrolls__42689 = new List<double>();
                    List<_NestedScrollPosition__nested_scroll_view> innerPositions__42757 = this._innerPositions.ToList().ToList();
                    foreach (var position__42819 in innerPositions__42757)
                    {
                        double overscroll__42872 = position__42819.applyClampedDragUpdate(innerDelta__42176);
                        outerDelta__42620 = Math.Max(outerDelta__42620, overscroll__42872);
                        overscrolls__42689.Add(overscroll__42872);
                    }
                    if ((outerDelta__42620 != 0.0))
                    {
                        outerDelta__42620 -= this._outerPosition!.applyClampedDragUpdate(outerDelta__42620);
                    }
                    for (var i__43213 = 0L; (i__43213 < checked((long)(innerPositions__42757.Count))); ++i__43213)
                    {
                        double remainingDelta__43277 = (overscrolls__42689[(int)(i__43213)] - outerDelta__42620);
                        if ((remainingDelta__43277 > 0.0))
                        {
                            innerPositions__42757[(int)(i__43213)].applyFullDragUpdate(remainingDelta__43277);
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
        this._outerPosition?.setParent(((this._parent ?? (ScrollController)PrimaryScrollController.maybeOf(this._state.context))));
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._currentDrag?.dispose();
        _currentDrag = null;
        this._outerController.dispose();
        this._innerController.dispose();
    }

    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_NestedScrollCoordinator"))}(outer={this._outerController}; inner={this._innerController})";
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
        return ((ScrollPosition)(object?)new _NestedScrollPosition__nested_scroll_view(coordinator: this.coordinator, physics: physics, context: context, initialPixels: this.initialScrollOffset, oldPosition: oldPosition, debugLabel: this.debugLabel));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(ScrollPosition position)
    {
        DartRuntimePrimitives.Assert(() => (position is _NestedScrollPosition__nested_scroll_view));
        base.attach(position);
        this.coordinator.updateParent();
        this.coordinator.updateCanDrag();
        position.addListener(() => this._scheduleUpdateShadow());
        _scheduleUpdateShadow();
    }

    public override void detach(ScrollPosition position)
    {
        DartRuntimePrimitives.Assert(() => (position is _NestedScrollPosition__nested_scroll_view));
        (((_NestedScrollPosition__nested_scroll_view?)(object?)position)!).setParent(((ScrollController)(object)null));
        ((_NestedScrollPosition__nested_scroll_view)position).removeListener(() => this._scheduleUpdateShadow());
        base.detach(((_NestedScrollPosition__nested_scroll_view)position));
        _scheduleUpdateShadow();
    }

    internal virtual void _scheduleUpdateShadow()
    {
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timeStamp) => {
this.coordinator.updateShadow();
})), debugLabel: "NestedScrollController.updateShadow");
    }

    public virtual IEnumerable<_NestedScrollPosition__nested_scroll_view> nestedPositions
    {
        get
        {
            return this.positions.cast<_NestedScrollPosition__nested_scroll_view>();
            return default!;
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

    public virtual global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync => ((ScrollContext)this.context).vsync;
    public virtual void setParent(ScrollController? value)
    {
        this._parent?.detach(this);
        _parent = value;
        this._parent?.attach(this);
    }

    public override global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection => ((ScrollContext)this.context).axisDirection;
    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        this.activity!.updateDelegate(this);
    }

    public override void restoreScrollOffset()
    {
        if (((_NestedScrollCoordinator__nested_scroll_view)this.coordinator).canScrollBody)
        {
            base.restoreScrollOffset();
        }
    }

    public virtual double applyClampedDragUpdate(double delta)
    {
        DartRuntimePrimitives.Assert(() => (delta != 0.0));
        double min__48547 = ((delta < 0.0) ? -double.PositiveInfinity : Math.Min(this.minScrollExtent, this.pixels));
        double max__48700 = ((delta > 0.0) ? double.PositiveInfinity : ((this.pixels < 0.0) ? 0.0 : Math.Max(this.maxScrollExtent, this.pixels)));
        double oldPixels__48992 = this.pixels;
        double newPixels__49029 = Dart_uiLibrary.clampDouble((this.pixels - delta), min__48547, max__48700);
        double clampedDelta__49097 = (newPixels__49029 - this.pixels);
        if ((clampedDelta__49097 == 0.0))
        {
            return delta;
        }
        double overscroll__49206 = this.physics.applyBoundaryConditions(this, newPixels__49029);
        double actualNewPixels__49286 = (newPixels__49029 - overscroll__49206);
        double offset__49345 = (actualNewPixels__49286 - oldPixels__48992);
        if ((offset__49345 != 0.0))
        {
            forcePixels(actualNewPixels__49286);
            didUpdateScrollPositionBy(offset__49345);
        }
        double result__49509 = (delta + offset__49345);
        if ((result__49509.abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return 0.0;
        }
        return result__49509;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double applyFullDragUpdate(double delta)
    {
        DartRuntimePrimitives.Assert(() => (delta != 0.0));
        double oldPixels__49749 = this.pixels;
        double newPixels__49809 = (this.pixels - this.physics.applyPhysicsToUserOffset(this, delta));
        if ((((oldPixels__49749 - newPixels__49809)).abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return 0.0;
        }
        double overscroll__50057 = this.physics.applyBoundaryConditions(this, newPixels__49809);
        double actualNewPixels__50137 = (newPixels__49809 - overscroll__50057);
        if ((actualNewPixels__50137 != oldPixels__49749))
        {
            forcePixels(actualNewPixels__50137);
            didUpdateScrollPositionBy((actualNewPixels__50137 - oldPixels__49749));
        }
        if ((overscroll__50057 != 0.0))
        {
            didOverscrollBy(overscroll__50057);
            return overscroll__50057;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double applyClampedPointerSignalUpdate(double delta)
    {
        DartRuntimePrimitives.Assert(() => (delta != 0.0));
        double min__50794 = ((delta > 0.0) ? -double.PositiveInfinity : Math.Min(this.minScrollExtent, this.pixels));
        double max__50947 = ((delta < 0.0) ? double.PositiveInfinity : Math.Max(this.maxScrollExtent, this.pixels));
        double newPixels__51037 = Dart_uiLibrary.clampDouble((this.pixels + delta), min__50794, max__50947);
        double clampedDelta__51105 = (newPixels__51037 - this.pixels);
        if ((clampedDelta__51105 == 0.0))
        {
            return delta;
        }
        forcePixels(newPixels__51037);
        didUpdateScrollPositionBy(clampedDelta__51105);
        return (delta - clampedDelta__51105);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Rendering.ScrollDirection userScrollDirection => ((_NestedScrollCoordinator__nested_scroll_view)this.coordinator).userScrollDirection;
    public virtual DrivenScrollActivity createDrivenScrollActivity(double to, Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve)
    {
        return new DrivenScrollActivity(this, from: this.pixels, to: to, duration: duration, curve: curve, vsync: this.vsync);
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
        this.coordinator.updateUserScrollDirection(global::Doroti.Generated.Framework.Rendering.ScrollDirection.idle);
    }

    public virtual void goBallistic(double velocity)
    {
        global::Doroti.Generated.Framework.Physics.Simulation? simulation__52111 = default!;
        if (((velocity != 0.0) || this.outOfRange))
        {
            simulation__52111 = this.physics.createBallisticSimulation(this, velocity);
        }
        beginActivity(createBallisticScrollActivity(simulation__52111, mode: _NestedBallisticScrollActivityMode__nested_scroll_view.independent));
    }

    public virtual ScrollActivity createBallisticScrollActivity(global::Doroti.Generated.Framework.Physics.Simulation? simulation, _NestedBallisticScrollActivityMode__nested_scroll_view mode, _NestedScrollMetrics__nested_scroll_view? metrics = null)
    {
        if ((simulation is null))
        {
            return ((ScrollActivity)(object?)new IdleScrollActivity(this));
        }
        switch (mode)
        {
            case _NestedBallisticScrollActivityMode__nested_scroll_view.outer:
                {
                    DartRuntimePrimitives.Assert(() => (metrics is not null));
                    if ((metrics!.minRange == ((_NestedScrollMetrics__nested_scroll_view)metrics).maxRange))
                    {
                        return ((ScrollActivity)(object?)new IdleScrollActivity(this));
                    }
                    return ((ScrollActivity)(object?)new _NestedOuterBallisticScrollActivity__nested_scroll_view(this.coordinator, this, metrics, simulation, ((ScrollContext)this.context).vsync, this.shouldIgnorePointer));
                }
            case _NestedBallisticScrollActivityMode__nested_scroll_view.inner:
                {
                    return ((ScrollActivity)(object?)new _NestedInnerBallisticScrollActivity__nested_scroll_view(this.coordinator, this, simulation, ((ScrollContext)this.context).vsync, this.shouldIgnorePointer));
                }
            case _NestedBallisticScrollActivityMode__nested_scroll_view.independent:
                {
                    return ((ScrollActivity)(object?)new BallisticScrollActivity(this, simulation, ((ScrollContext)this.context).vsync, this.shouldIgnorePointer));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Future animateTo(double to, Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve)
    {
        return ((Future)(object?)this.coordinator.animateTo(this.coordinator.unnestOffset(to, this), duration: duration, curve: curve));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void jumpTo(double pixels)
    {
        this.coordinator.jumpTo(this.coordinator.unnestOffset(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(pixels)), this));
        return;
    }

    public override void pointerScroll(double delta)
    {
        this.coordinator.pointerScroll(delta);
        return;
    }

    public override void jumpToWithoutSettling(double value)
    {
        DartRuntimePrimitives.Assert(() => false);
    }

    public virtual void localJumpTo(double value)
    {
        if ((this.pixels != DartRuntimePrimitives.RequireValue(value)))
        {
            double oldPixels__54071 = this.pixels;
            forcePixels(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value)));
            didStartScroll();
            didUpdateScrollPositionBy((this.pixels - oldPixels__54071));
            didEndScroll();
        }
    }

    public override void applyNewDimensions()
    {
        base.applyNewDimensions();
        this.coordinator.updateCanDrag();
    }

    public virtual void updateCanDrag(bool innerCanDrag)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((_NestedScrollCoordinator__nested_scroll_view)this.coordinator)._outerPosition, this)));
        this.context.setCanDrag((this.physics.shouldAcceptUserOffset(this) || innerCanDrag));
    }

    public override ScrollHoldController hold(global::System.Action holdCancelCallback)
    {
        return ((ScrollHoldController)(object?)this.coordinator.hold(() => holdCancelCallback()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Gestures.Drag drag(global::Doroti.Generated.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback)
    {
        return ((global::Doroti.Generated.Framework.Gestures.Drag)(object?)this.coordinator.drag(details, () => dragCancelCallback()));
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

    internal _NestedInnerBallisticScrollActivity__nested_scroll_view(_NestedScrollCoordinator__nested_scroll_view coordinator, _NestedScrollPosition__nested_scroll_view position, global::Doroti.Generated.Framework.Physics.Simulation simulation, global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync, bool shouldIgnorePointer) : base(position, simulation, vsync, shouldIgnorePointer)
    {
        this.coordinator = coordinator;
    }

    public override ScrollActivityDelegate @delegate => DartRuntimePrimitives.ConvertValue<ScrollActivityDelegate>(((_NestedScrollPosition__nested_scroll_view?)(object?)base.@delegate)!);
    public override void resetActivity()
    {
        ((dynamic)this.@delegate).beginActivity(this.coordinator.createInnerBallisticScrollActivity(DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(this.@delegate), this.velocity));
    }

    public override void applyNewDimensions()
    {
        ((dynamic)this.@delegate).beginActivity(this.coordinator.createInnerBallisticScrollActivity(DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(this.@delegate), this.velocity));
    }

    public override bool applyMoveTo(double value)
    {
        return base.applyMoveTo(this.coordinator.nestOffset(value, DartRuntimePrimitives.ConvertValue<_NestedScrollPosition__nested_scroll_view>(this.@delegate)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NestedOuterBallisticScrollActivity__nested_scroll_view : BallisticScrollActivity
{
    public virtual _NestedScrollCoordinator__nested_scroll_view coordinator { get; private set; } = default!;
    public virtual _NestedScrollMetrics__nested_scroll_view metrics { get; private set; } = default!;

    internal _NestedOuterBallisticScrollActivity__nested_scroll_view(_NestedScrollCoordinator__nested_scroll_view coordinator, _NestedScrollPosition__nested_scroll_view position, _NestedScrollMetrics__nested_scroll_view metrics, global::Doroti.Generated.Framework.Physics.Simulation simulation, global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync, bool shouldIgnorePointer) : base(position, simulation, vsync, shouldIgnorePointer)
    {
        this.coordinator = coordinator;
        this.metrics = metrics;
        System.Diagnostics.Debug.Assert((((_NestedScrollMetrics__nested_scroll_view)metrics).minRange != ((_NestedScrollMetrics__nested_scroll_view)metrics).maxRange));
        System.Diagnostics.Debug.Assert((((_NestedScrollMetrics__nested_scroll_view)metrics).maxRange > ((_NestedScrollMetrics__nested_scroll_view)metrics).minRange));
    }

    public override ScrollActivityDelegate @delegate => DartRuntimePrimitives.ConvertValue<ScrollActivityDelegate>(((_NestedScrollPosition__nested_scroll_view?)(object?)base.@delegate)!);
    public override void resetActivity()
    {
        ((dynamic)this.@delegate).beginActivity(this.coordinator.createOuterBallisticScrollActivity(this.velocity));
    }

    public override void applyNewDimensions()
    {
        ((dynamic)this.@delegate).beginActivity(this.coordinator.createOuterBallisticScrollActivity(this.velocity));
    }

    public override bool applyMoveTo(double value)
    {
        var done__56939 = false;
        if ((this.velocity > 0.0))
        {
            if ((value < ((_NestedScrollMetrics__nested_scroll_view)this.metrics).minRange))
            {
                return true;
            }
            if ((value > ((_NestedScrollMetrics__nested_scroll_view)this.metrics).maxRange))
            {
                value = ((_NestedScrollMetrics__nested_scroll_view)this.metrics).maxRange;
                done__56939 = true;
            }
        }
        else
        {
            if ((this.velocity < 0.0))
            {
                if ((value > ((_NestedScrollMetrics__nested_scroll_view)this.metrics).maxRange))
                {
                    return true;
                }
                if ((value < ((_NestedScrollMetrics__nested_scroll_view)this.metrics).minRange))
                {
                    value = ((_NestedScrollMetrics__nested_scroll_view)this.metrics).minRange;
                    done__56939 = true;
                }
            }
            else
            {
                value = Dart_uiLibrary.clampDouble(value, ((_NestedScrollMetrics__nested_scroll_view)this.metrics).minRange, ((_NestedScrollMetrics__nested_scroll_view)this.metrics).maxRange);
                done__56939 = true;
            }
        }
        bool result__57471 = base.applyMoveTo((value + ((_NestedScrollMetrics__nested_scroll_view)this.metrics).correctionOffset));
        DartRuntimePrimitives.Assert(() => result__57471);
        return !done__56939;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "_NestedOuterBallisticScrollActivity"))}({((_NestedScrollMetrics__nested_scroll_view)this.metrics).minRange} .. {((_NestedScrollMetrics__nested_scroll_view)this.metrics).maxRange}; correcting by {((_NestedScrollMetrics__nested_scroll_view)this.metrics).correctionOffset})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverOverlapAbsorberHandle : global::Doroti.Generated.Framework.Foundation.ChangeNotifier
{
    internal virtual long _writers { get; set; } = 0L;
    internal virtual double? _layoutExtent { get; set; } = default;
    internal virtual double? _scrollExtent { get; set; } = default;

    public SliverOverlapAbsorberHandle()
    {
    }

    public virtual double? layoutExtent => this._layoutExtent;
    public virtual double? scrollExtent => this._scrollExtent;
    internal virtual void _setExtents(double? layoutValue, double? scrollValue)
    {
        DartRuntimePrimitives.Assert(() => (this._writers == 1L), () => (object?)"Multiple RenderSliverOverlapAbsorbers have been provided the same SliverOverlapAbsorberHandle.");
        _layoutExtent = layoutValue;
        _scrollExtent = scrollValue;
    }

    internal virtual void _markNeedsLayout() => notifyListeners();
    public override string ToString()
    {
        string? extra__61418 = (this._writers switch { 0L => ", orphan", 1L => DartRuntimePrimitives.ConvertValue<string>(null), _ => $", {this._writers} WRITERS ASSIGNED" });
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "SliverOverlapAbsorberHandle"))}({this.layoutExtent}{extra__61418})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverOverlapAbsorber : SingleChildRenderObjectWidget
{
    public virtual SliverOverlapAbsorberHandle handle { get; private set; } = default!;

    public SliverOverlapAbsorber(global::Doroti.Generated.Framework.Foundation.Key? key = null, SliverOverlapAbsorberHandle handle = default!, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.handle = handle;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new RenderSliverOverlapAbsorber(handle: this.handle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (RenderSliverOverlapAbsorber)(object)renderObject;
        __renderObject.handle = this.handle;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", this.handle));
    }

}

public class RenderSliverOverlapAbsorber : global::Doroti.Generated.Framework.Rendering.RenderSliver, global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<global::Doroti.Generated.Framework.Rendering.RenderSliver>
{
    internal virtual SliverOverlapAbsorberHandle _handle { get; set; } = default!;
    public virtual RenderSliver? _child { get; set; } = default;

    public RenderSliverOverlapAbsorber(SliverOverlapAbsorberHandle handle, global::Doroti.Generated.Framework.Rendering.RenderSliver? sliver = null)
    {
        this._handle = handle;
    }

    public virtual SliverOverlapAbsorberHandle handle
    {
        get => this._handle;
        set
        {
            var __value = value;
            if ((object.Equals(this.handle, __value)))
            {
                return;
            }
            if (this.attached)
            {
                this.handle._writers -= 1L;
                __value._writers += 1L;
                __value._setExtents(((SliverOverlapAbsorberHandle)this.handle).layoutExtent, ((SliverOverlapAbsorberHandle)this.handle).scrollExtent);
            }
            _handle = __value;
        }
    }
    public override void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        this._child?.attach(owner);
        this.handle._writers += 1L;
    }

    public override void detach()
    {
        this.handle._writers -= 1L;
        base.detach();
        this._child?.detach();
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => (((SliverOverlapAbsorberHandle)this.handle)._writers == 1L), () => (object?)"A SliverOverlapAbsorberHandle cannot be passed to multiple RenderSliverOverlapAbsorber objects at the same time.");
        if ((this.child is null))
        {
            geometry = global::Doroti.Generated.Framework.Rendering.SliverGeometry.zero;
            return;
        }
        this.child!.layout(this.constraints, parentUsesSize: true);
        global::Doroti.Generated.Framework.Rendering.SliverGeometry childLayoutGeometry__65029 = this.child!.geometry!;
        geometry = childLayoutGeometry__65029.copyWith(scrollExtent: (((global::Doroti.Generated.Framework.Rendering.SliverGeometry)childLayoutGeometry__65029).scrollExtent - ((global::Doroti.Generated.Framework.Rendering.SliverGeometry)childLayoutGeometry__65029).maxScrollObstructionExtent), layoutExtent: Math.Max(0, (((global::Doroti.Generated.Framework.Rendering.SliverGeometry)childLayoutGeometry__65029).paintExtent - ((global::Doroti.Generated.Framework.Rendering.SliverGeometry)childLayoutGeometry__65029).maxScrollObstructionExtent)));
        this.handle._setExtents(((global::Doroti.Generated.Framework.Rendering.SliverGeometry)childLayoutGeometry__65029).maxScrollObstructionExtent, ((global::Doroti.Generated.Framework.Rendering.SliverGeometry)childLayoutGeometry__65029).maxScrollObstructionExtent);
    }

    public override void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        if ((this.child is not null))
        {
            return this.child!.hitTest(result, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((this.child is not null))
        {
            context.paintChild(this.child!, offset);
        }
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", this.handle));
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderSliver))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderSliver)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((dynamic)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? child
    {
        get => this._child;
        set
        {
            var __value = value;
            if ((this._child is not null))
            {
                dropChild(this._child!);
            }
            this._child = __value;
            if ((this._child is not null))
            {
                adoptChild(this._child!);
            }
        }
    }
    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverOverlapInjector : SingleChildRenderObjectWidget
{
    public virtual SliverOverlapAbsorberHandle handle { get; private set; } = default!;

    public SliverOverlapInjector(global::Doroti.Generated.Framework.Foundation.Key? key = null, SliverOverlapAbsorberHandle handle = default!, Widget? sliver = null) : base(key: key, child: sliver)
    {
        this.handle = handle;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new RenderSliverOverlapInjector(handle: this.handle));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (RenderSliverOverlapInjector)(object)renderObject;
        __renderObject.handle = this.handle;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", this.handle));
    }

}

public class RenderSliverOverlapInjector : global::Doroti.Generated.Framework.Rendering.RenderSliver
{
    internal virtual double? _currentLayoutExtent { get; set; } = default;
    internal virtual double? _currentMaxExtent { get; set; } = default;
    internal virtual SliverOverlapAbsorberHandle _handle { get; set; } = default!;

    public RenderSliverOverlapInjector(SliverOverlapAbsorberHandle handle)
    {
        this._handle = handle;
    }

    public virtual SliverOverlapAbsorberHandle handle
    {
        get => this._handle;
        set
        {
            var __value = value;
            if ((object.Equals(this.handle, __value)))
            {
                return;
            }
            if (this.attached)
            {
                this.handle.removeListener(() => this.markNeedsLayout());
            }
            _handle = __value;
            if (this.attached)
            {
                this.handle.addListener(() => this.markNeedsLayout());
                if (((((SliverOverlapAbsorberHandle)this.handle).layoutExtent != this._currentLayoutExtent) || (((SliverOverlapAbsorberHandle)this.handle).scrollExtent != this._currentMaxExtent)))
                {
                    markNeedsLayout();
                }
            }
        }
    }
    public override void attach(global::Doroti.Generated.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        this.handle.addListener(() => this.markNeedsLayout());
        if (((((SliverOverlapAbsorberHandle)this.handle).layoutExtent != this._currentLayoutExtent) || (((SliverOverlapAbsorberHandle)this.handle).scrollExtent != this._currentMaxExtent)))
        {
            markNeedsLayout();
        }
    }

    public override void detach()
    {
        this.handle.removeListener(() => this.markNeedsLayout());
        base.detach();
    }

    public override void performLayout()
    {
        _currentLayoutExtent = ((SliverOverlapAbsorberHandle)this.handle).layoutExtent;
        _currentMaxExtent = ((SliverOverlapAbsorberHandle)this.handle).layoutExtent;
        DartRuntimePrimitives.Assert(() => ((this._currentLayoutExtent is not null) && (this._currentMaxExtent is not null)), () => (object?)"SliverOverlapInjector has found no absorbed extent to inject.\n " + "The SliverOverlapAbsorber must be an earlier descendant of a common " + "ancestor Viewport, so that it will always be laid out before the " + "SliverOverlapInjector during a particular frame.\n " + "The SliverOverlapAbsorber is typically contained in the list of slivers " + "provided by NestedScrollView.headerSliverBuilder.\n");
        double clampedPaintExtent__70242 = Math.Min(DartRuntimePrimitives.RequireValue(this._currentLayoutExtent), ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent);
        double clampedLayoutExtent__70366 = Math.Min((DartRuntimePrimitives.RequireValue(this._currentLayoutExtent) - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset), ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent);
        geometry = new global::Doroti.Generated.Framework.Rendering.SliverGeometry(scrollExtent: DartRuntimePrimitives.RequireValue(this._currentLayoutExtent), paintExtent: Math.Max(0.0, clampedPaintExtent__70242), layoutExtent: Math.Max(0.0, clampedLayoutExtent__70366), maxPaintExtent: DartRuntimePrimitives.RequireValue(this._currentMaxExtent));
    }

    public override void debugPaint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled)
                {
                    var paint__70876 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.color = new global::Doroti.Ui.Color(4291598643L);
            __cascade.strokeWidth = 3.0;
            __cascade.style = PaintingStyle.stroke;
            return __cascade;        }))();
                    global::Doroti.Ui.Offset start__71023 = default!;
                    global::Doroti.Ui.Offset end__71030 = default!;
                    global::Doroti.Ui.Offset delta__71035 = default!;
                    switch (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).axis)
                    {
                        case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                            {
                                double x__71133 = (offset.dx + (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).crossAxisExtent / 2.0));
                                start__71023 = new global::Doroti.Ui.Offset(x__71133, offset.dy);
                                end__71030 = new global::Doroti.Ui.Offset(x__71133, (offset.dy + this.geometry!.paintExtent));
                                delta__71035 = new global::Doroti.Ui.Offset((((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).crossAxisExtent / 5.0), 0.0);
                                break;
                            }
                        case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                            {
                                double y__71415 = (offset.dy + (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).crossAxisExtent / 2.0));
                                start__71023 = new global::Doroti.Ui.Offset(offset.dx, y__71415);
                                end__71030 = new global::Doroti.Ui.Offset((offset.dy + this.geometry!.paintExtent), y__71415);
                                delta__71035 = new global::Doroti.Ui.Offset(0.0, (((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).crossAxisExtent / 5.0));
                                break;
                            }
                    }
                    for (var index__71667 = -2L; (index__71667 <= 2L); index__71667 += 1L)
                    {
                        global::Doroti.Generated.Framework.Painting.Paint_utilitiesLibrary.paintZigZag(((global::Doroti.Generated.Framework.Rendering.PaintingContext)context).canvas, paint__70876, (start__71023 - (delta__71035 * index__71667.toDouble())), (end__71030 - (delta__71035 * index__71667.toDouble())), 10L, 10.0);
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", this.handle));
    }

}

public class NestedScrollViewViewport : Viewport
{
    public virtual SliverOverlapAbsorberHandle handle { get; private set; } = default!;

    public NestedScrollViewViewport(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection = global::Doroti.Generated.Framework.Painting.AxisDirection.down, global::Doroti.Generated.Framework.Painting.AxisDirection? crossAxisDirection = null, double anchor = 0.0, global::Doroti.Generated.Framework.Rendering.ViewportOffset offset = default!, global::Doroti.Generated.Framework.Foundation.Key? center = null, List<Widget> slivers = default!, SliverOverlapAbsorberHandle handle = default!, Clip clipBehavior = Clip.hardEdge) : base(key: key, axisDirection: axisDirection, crossAxisDirection: DartRuntimePrimitives.RequireValue(crossAxisDirection), anchor: anchor, offset: offset, center: center, slivers: slivers ?? new List<Widget>(), clipBehavior: clipBehavior)
    {
        this.handle = handle;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new RenderNestedScrollViewViewport(axisDirection: this.axisDirection, crossAxisDirection: ((this.crossAxisDirection ?? (global::Doroti.Generated.Framework.Painting.AxisDirection)Viewport.getDefaultCrossAxisDirection(context, this.axisDirection))), anchor: this.anchor, offset: this.offset, handle: this.handle, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (RenderNestedScrollViewViewport)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderNestedScrollViewViewport>)(() =>
{            var __cascade = __renderObject;
            __cascade.axisDirection = this.axisDirection;
            __cascade.crossAxisDirection = ((this.crossAxisDirection ?? (global::Doroti.Generated.Framework.Painting.AxisDirection)Viewport.getDefaultCrossAxisDirection(context, this.axisDirection)));
            __cascade.anchor = this.anchor;
            __cascade.offset = this.offset;
            __cascade.handle = this.handle;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", this.handle));
    }

}

public class RenderNestedScrollViewViewport : global::Doroti.Generated.Framework.Rendering.RenderViewport
{
    internal virtual SliverOverlapAbsorberHandle _handle { get; set; } = default!;

    public RenderNestedScrollViewViewport(global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection = global::Doroti.Generated.Framework.Painting.AxisDirection.down, global::Doroti.Generated.Framework.Painting.AxisDirection crossAxisDirection = default!, global::Doroti.Generated.Framework.Rendering.ViewportOffset offset = default!, double anchor = 0.0, List<global::Doroti.Generated.Framework.Rendering.RenderSliver>? children = null, global::Doroti.Generated.Framework.Rendering.RenderSliver? center = null, SliverOverlapAbsorberHandle handle = default!, Clip clipBehavior = Clip.hardEdge) : base(axisDirection: axisDirection, crossAxisDirection: crossAxisDirection, offset: offset, anchor: anchor, children: children, center: center, clipBehavior: clipBehavior)
    {
        this._handle = handle;
    }

    public virtual SliverOverlapAbsorberHandle handle
    {
        get => this._handle;
        set
        {
            var __value = value;
            if ((object.Equals(this.handle, __value)))
            {
                return;
            }
            _handle = __value;
            this.handle._markNeedsLayout();
        }
    }
    public override void markNeedsLayout()
    {
        this.handle._markNeedsLayout();
        base.markNeedsLayout();
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<SliverOverlapAbsorberHandle>("handle", this.handle));
    }

}

