// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/list_wheel_scroll_view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum ChangeReportingBehavior
{
    onScrollEnd,
    onScrollUpdate
}

public abstract class ListWheelChildDelegate
{
    public abstract Widget? build(BuildContext context, long index);
    public abstract long? estimatedChildCount { get; }
    public virtual long trueIndexOf(long index) => index;
    public abstract bool shouldRebuild(ListWheelChildDelegate oldDelegate);
}

public class ListWheelChildListDelegate : ListWheelChildDelegate
{
    public virtual List<Widget> children { get; private set; } = default!;

    public ListWheelChildListDelegate(List<Widget> children)
    {
        this.children = children;
    }

    public override long? estimatedChildCount => checked(children.Count);
    public override Widget? build(BuildContext context, long index)
    {
        if ((index < 0L) || (index >= checked(children.Count)))
        {
            return null;
        }
        return (Widget?)new IndexedSemantics(index: index, child: children[(int)index]);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(ListWheelChildDelegate oldDelegate)
    {
        var __oldDelegate = (ListWheelChildListDelegate)oldDelegate;
        return !Equals(children, __oldDelegate.children);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ListWheelChildLoopingListDelegate : ListWheelChildDelegate
{
    public virtual List<Widget> children { get; private set; } = default!;

    public ListWheelChildLoopingListDelegate(List<Widget> children)
    {
        this.children = children;
    }

    public override long? estimatedChildCount => DartRuntimePrimitives.ConvertValue<long>(null);
    public override long trueIndexOf(long index) => DartRuntimePrimitives.ConvertValue<long>(index % checked(children.Count));
    public override Widget? build(BuildContext context, long index)
    {
        if (!Enumerable.Any(children))
        {
            return null;
        }
        return (Widget?)new IndexedSemantics(index: index, child: children[(int)(index % checked(children.Count))]);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(ListWheelChildDelegate oldDelegate)
    {
        var __oldDelegate = (ListWheelChildLoopingListDelegate)oldDelegate;
        return !Equals(children, __oldDelegate.children);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ListWheelChildBuilderDelegate : ListWheelChildDelegate
{
    public virtual Func<BuildContext, long, Widget?> builder { get; private set; } = default!;
    public virtual long? childCount { get; private set; }

    public ListWheelChildBuilderDelegate(Func<BuildContext, long, Widget?> builder, long? childCount = null)
    {
        this.builder = builder;
        this.childCount = childCount;
    }

    public override long? estimatedChildCount => childCount;
    public override Widget? build(BuildContext context, long index)
    {
        if (childCount is null)
        {
            Widget? childLocal = builder(context, index);
            return (childLocal is null) ? null : new IndexedSemantics(index: index, child: childLocal);
        }
        if ((index < 0L) || (index >= DartRuntimePrimitives.RequireValue(childCount)))
        {
            return null;
        }
        return (Widget?)new IndexedSemantics(index: index, child: builder(context, index));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(ListWheelChildDelegate oldDelegate)
    {
        var __oldDelegate = (ListWheelChildBuilderDelegate)oldDelegate;
        return (!Equals(builder, __oldDelegate.builder)) || (childCount != __oldDelegate.childCount);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FixedExtentScrollController : ScrollController
{
    public virtual long initialItem { get; private set; } = default!;

    public FixedExtentScrollController(long initialItem = 0, bool keepScrollOffset = true, string? debugLabel = null, System.Action<ScrollPosition>? onAttach = null, System.Action<ScrollPosition>? onDetach = null) : base(keepScrollOffset: keepScrollOffset, debugLabel: debugLabel, onAttach: onAttach, onDetach: onDetach)
    {
        this.initialItem = initialItem;
    }

    public virtual long selectedItem
    {
        get
        {
            DartRuntimePrimitives.Assert(() => Enumerable.Any(positions), () => (object?)"FixedExtentScrollController.selectedItem cannot be accessed before a " + "scroll view is built with it.");
            DartRuntimePrimitives.Assert(() => positions.Count() == 1L, () => (object?)"The selectedItem property cannot be read when multiple scroll views are " + "attached to the same FixedExtentScrollController.");
            var positionLocal = ((_FixedExtentScrollPosition__list_wheel_scroll_view?)position)!;
            return positionLocal.itemIndex;
        }
    }
    public async virtual Future animateToItem(long itemIndex, Duration duration, Curve curve)
    {
        if (!hasClients)
        {
            return;
        }
        await DartAsyncRuntime.wait<object?>(new List<Future>());
    }

    public virtual void jumpToItem(long itemIndex)
    {
        foreach (_FixedExtentScrollPosition__list_wheel_scroll_view position in positions.cast<_FixedExtentScrollPosition__list_wheel_scroll_view>())
        {
            position.jumpTo(itemIndex * position.itemExtent);
        }
    }

    public override ScrollPosition createScrollPosition(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition)
    {
        return new _FixedExtentScrollPosition__list_wheel_scroll_view(physics: physics, context: context, initialItem: initialItem, oldPosition: oldPosition, keepScrollOffset: keepScrollOffset, debugLabel: debugLabel);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FixedExtentMetrics : FixedScrollMetrics
{
    public virtual long itemIndex { get; private set; } = default!;
    public FixedExtentMetrics() : base(default!, default!, default!, default!, default!, default!) { }


    public FixedExtentMetrics(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, AxisDirection axisDirection, long itemIndex, double devicePixelRatio) : base(minScrollExtent: DartRuntimePrimitives.RequireValue(minScrollExtent), maxScrollExtent: DartRuntimePrimitives.RequireValue(maxScrollExtent), pixels: DartRuntimePrimitives.RequireValue(pixels), viewportDimension: DartRuntimePrimitives.RequireValue(viewportDimension), axisDirection: axisDirection, devicePixelRatio: devicePixelRatio)
    {
        this.itemIndex = itemIndex;
    }

    public override FixedExtentMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new FixedExtentMetrics(minScrollExtent: minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : null), maxScrollExtent: maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : null), pixels: pixels ?? (hasPixels ? this.pixels : null), viewportDimension: viewportDimension ?? (hasViewportDimension ? this.viewportDimension : null), axisDirection: axisDirection ?? this.axisDirection, itemIndex: itemIndex ?? this.itemIndex, devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class List_wheel_scroll_viewLibrary
{
    internal static long _getItemFromOffset(double offset, double itemExtent, double minScrollExtent, double maxScrollExtent)
    {
        return (_clipOffsetToScrollableRange(offset, minScrollExtent, maxScrollExtent) / itemExtent).round();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class List_wheel_scroll_viewLibrary
{
    internal static double _clipOffsetToScrollableRange(double offset, double minScrollExtent, double maxScrollExtent)
    {
        return Math.Min(Math.Max(offset, minScrollExtent), maxScrollExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _FixedExtentScrollPosition__list_wheel_scroll_view : ScrollPositionWithSingleContext
{
    internal _FixedExtentScrollPosition__list_wheel_scroll_view(ScrollPhysics physics, ScrollContext context, long initialItem, ScrollPosition? oldPosition = null, bool keepScrollOffset = true, string? debugLabel = null) : base(physics: physics, context: context, oldPosition: oldPosition, keepScrollOffset: keepScrollOffset, debugLabel: debugLabel, initialPixels: _getItemExtentFromScrollContext(context) * initialItem)
    {
        System.Diagnostics.Debug.Assert(context is _FixedExtentScrollableState__list_wheel_scroll_view);
    }

    internal static double _getItemExtentFromScrollContext(ScrollContext context)
    {
        var scrollable = ((_FixedExtentScrollableState__list_wheel_scroll_view?)context)!;
        return scrollable.itemExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double itemExtent => _getItemExtentFromScrollContext(context);
    public virtual long itemIndex
    {
        get
        {
            return List_wheel_scroll_viewLibrary._getItemFromOffset(offset: DartRuntimePrimitives.RequireValue(pixels), itemExtent: itemExtent, minScrollExtent: DartRuntimePrimitives.RequireValue(minScrollExtent), maxScrollExtent: DartRuntimePrimitives.RequireValue(maxScrollExtent));
        }
    }
    public override FixedExtentMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new FixedExtentMetrics(minScrollExtent: minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : null), maxScrollExtent: maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : null), pixels: pixels ?? (hasPixels ? this.pixels : null), viewportDimension: viewportDimension ?? (hasViewportDimension ? this.viewportDimension : null), axisDirection: axisDirection ?? this.axisDirection, itemIndex: itemIndex ?? this.itemIndex, devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FixedExtentScrollable__list_wheel_scroll_view : Scrollable
{
    public virtual double itemExtent { get; private set; } = default!;

    internal _FixedExtentScrollable__list_wheel_scroll_view(ScrollController? controller = null, ScrollPhysics? physics = null, double itemExtent = default!, Func<BuildContext, ViewportOffset, Widget> viewportBuilder = default!, DragStartBehavior dragStartBehavior = default!, string? restorationId = null, ScrollBehavior? scrollBehavior = null, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque) : base(controller: controller, physics: physics, viewportBuilder: viewportBuilder, dragStartBehavior: dragStartBehavior, restorationId: restorationId, scrollBehavior: scrollBehavior, hitTestBehavior: hitTestBehavior)
    {
        this.itemExtent = itemExtent;
    }

    public override _FixedExtentScrollableState__list_wheel_scroll_view createState() => new _FixedExtentScrollableState__list_wheel_scroll_view();
}

internal class _FixedExtentScrollableState__list_wheel_scroll_view : ScrollableState
{
    public virtual double itemExtent
    {
        get
        {
            var actualWidget = ((_FixedExtentScrollable__list_wheel_scroll_view?)widget)!;
            return actualWidget.itemExtent;
        }
    }
}

public class FixedExtentScrollPhysics : ScrollPhysics
{
    public FixedExtentScrollPhysics(ScrollPhysics? parent = null) : base(parent: parent)
    {
    }

    public override FixedExtentScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new FixedExtentScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        DartRuntimePrimitives.Assert(() => position is _FixedExtentScrollPosition__list_wheel_scroll_view, () => (object?)"FixedExtentScrollPhysics can only be used with Scrollables that uses " + "the FixedExtentScrollController");
        var metrics = ((_FixedExtentScrollPosition__list_wheel_scroll_view?)position)!;
        if ((velocity <= 0.0) && (metrics.pixels <= metrics.minScrollExtent) || (velocity >= 0.0) && (metrics.pixels >= metrics.maxScrollExtent))
        {
            return base.createBallisticSimulation(metrics, velocity);
        }
        Physics.Simulation? testFrictionSimulation = base.createBallisticSimulation(metrics, velocity);
        if ((testFrictionSimulation is not null) && ((testFrictionSimulation.x(double.PositiveInfinity) == metrics.minScrollExtent) || (testFrictionSimulation.x(double.PositiveInfinity) == metrics.maxScrollExtent)))
        {
            return base.createBallisticSimulation(metrics, velocity);
        }
        long settlingItemIndex = List_wheel_scroll_viewLibrary._getItemFromOffset(offset: testFrictionSimulation?.x(double.PositiveInfinity) ?? (double)metrics.pixels, itemExtent: metrics.itemExtent, minScrollExtent: metrics.minScrollExtent, maxScrollExtent: metrics.maxScrollExtent);
        double settlingPixels = settlingItemIndex * metrics.itemExtent;
        if ((velocity.abs() < toleranceFor((_FixedExtentScrollPosition__list_wheel_scroll_view)position).velocity) && ((settlingPixels - metrics.pixels).abs() < toleranceFor((_FixedExtentScrollPosition__list_wheel_scroll_view)position).distance))
        {
            return null;
        }
        if (settlingItemIndex == metrics.itemIndex)
        {
            return (Physics.Simulation?)new Physics.SpringSimulation(spring, metrics.pixels, settlingPixels, velocity, tolerance: toleranceFor((_FixedExtentScrollPosition__list_wheel_scroll_view)position));
        }
        return (Physics.Simulation?)Physics.FrictionSimulation.CreateThrough(metrics.pixels, settlingPixels, velocity, toleranceFor((_FixedExtentScrollPosition__list_wheel_scroll_view)position).velocity * Math.Sign(velocity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ListWheelScrollView : StatefulWidget
{
    public virtual ScrollController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual double diameterRatio { get; private set; } = default!;
    public virtual double perspective { get; private set; } = default!;
    public virtual double offAxisFraction { get; private set; } = default!;
    public virtual bool useMagnifier { get; private set; } = default!;
    public virtual double magnification { get; private set; } = default!;
    public virtual double overAndUnderCenterOpacity { get; private set; } = default!;
    public virtual double itemExtent { get; private set; } = default!;
    public virtual double squeeze { get; private set; } = default!;
    public virtual System.Action<long>? onSelectedItemChanged { get; private set; }
    public virtual bool renderChildrenOutsideViewport { get; private set; } = default!;
    public virtual ListWheelChildDelegate childDelegate { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;

    public ListWheelScrollView(Key? key = null, ScrollController? controller = null, ScrollPhysics? physics = null, double? diameterRatio = null, double? perspective = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, double overAndUnderCenterOpacity = 1.0, double itemExtent = default!, double squeeze = 1.0, System.Action<long>? onSelectedItemChanged = null, bool renderChildrenOutsideViewport = false, Clip clipBehavior = Clip.hardEdge, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, string? restorationId = null, ScrollBehavior? scrollBehavior = null, DragStartBehavior dragStartBehavior = DragStartBehavior.start, ChangeReportingBehavior changeReportingBehavior = ChangeReportingBehavior.onScrollUpdate, List<Widget> children = default!) : base(key: key)
    {
        double __diameterRatio = diameterRatio ?? RenderListWheelViewport.defaultDiameterRatio;
        double __perspective = perspective ?? RenderListWheelViewport.defaultPerspective;
        this.controller = controller;
        this.physics = physics;
        this.diameterRatio = __diameterRatio;
        this.perspective = __perspective;
        this.offAxisFraction = offAxisFraction;
        this.useMagnifier = useMagnifier;
        this.magnification = magnification;
        this.overAndUnderCenterOpacity = overAndUnderCenterOpacity;
        this.itemExtent = itemExtent;
        this.squeeze = squeeze;
        this.onSelectedItemChanged = onSelectedItemChanged;
        this.renderChildrenOutsideViewport = renderChildrenOutsideViewport;
        this.clipBehavior = clipBehavior;
        this.hitTestBehavior = hitTestBehavior;
        this.restorationId = restorationId;
        this.scrollBehavior = scrollBehavior;
        this.dragStartBehavior = dragStartBehavior;
        this.changeReportingBehavior = changeReportingBehavior;
        childDelegate = new ListWheelChildListDelegate(children: children);
        System.Diagnostics.Debug.Assert(__diameterRatio > 0.0);
        System.Diagnostics.Debug.Assert(__perspective > 0L);
        System.Diagnostics.Debug.Assert(__perspective <= 0.01);
        System.Diagnostics.Debug.Assert(magnification > 0L);
        System.Diagnostics.Debug.Assert((overAndUnderCenterOpacity >= 0L) && (overAndUnderCenterOpacity <= 1L));
        System.Diagnostics.Debug.Assert(itemExtent > 0L);
        System.Diagnostics.Debug.Assert(squeeze > 0L);
        System.Diagnostics.Debug.Assert(!renderChildrenOutsideViewport || Equals(clipBehavior, Clip.none));
    }

    public static ListWheelScrollView CreateUseDelegate(Key? key = null, ScrollController? controller = null, ScrollPhysics? physics = null, double? diameterRatio = null, double? perspective = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, double overAndUnderCenterOpacity = 1.0, double itemExtent = default!, double squeeze = 1.0, System.Action<long>? onSelectedItemChanged = null, bool renderChildrenOutsideViewport = false, Clip clipBehavior = Clip.hardEdge, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, string? restorationId = null, ScrollBehavior? scrollBehavior = null, DragStartBehavior dragStartBehavior = DragStartBehavior.start, ChangeReportingBehavior changeReportingBehavior = ChangeReportingBehavior.onScrollUpdate, ListWheelChildDelegate childDelegate = default!)
    {
        var __instance = new ListWheelScrollView(key, controller, physics, diameterRatio, perspective, offAxisFraction, useMagnifier, magnification, overAndUnderCenterOpacity, itemExtent, squeeze, onSelectedItemChanged, renderChildrenOutsideViewport, clipBehavior, hitTestBehavior, restorationId, scrollBehavior, dragStartBehavior, changeReportingBehavior, default!);
        double __diameterRatio = diameterRatio ?? RenderListWheelViewport.defaultDiameterRatio;
        double __perspective = perspective ?? RenderListWheelViewport.defaultPerspective;
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.diameterRatio = __diameterRatio;
        __instance.perspective = __perspective;
        __instance.offAxisFraction = offAxisFraction;
        __instance.useMagnifier = useMagnifier;
        __instance.magnification = magnification;
        __instance.overAndUnderCenterOpacity = overAndUnderCenterOpacity;
        __instance.itemExtent = itemExtent;
        __instance.squeeze = squeeze;
        __instance.onSelectedItemChanged = onSelectedItemChanged;
        __instance.renderChildrenOutsideViewport = renderChildrenOutsideViewport;
        __instance.clipBehavior = clipBehavior;
        __instance.hitTestBehavior = hitTestBehavior;
        __instance.restorationId = restorationId;
        __instance.scrollBehavior = scrollBehavior;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.changeReportingBehavior = changeReportingBehavior;
        __instance.childDelegate = childDelegate;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ListWheelScrollViewState__list_wheel_scroll_view());
}

internal class _ListWheelScrollViewState__list_wheel_scroll_view : State<ListWheelScrollView>
{
    internal virtual long _lastReportedItemIndex { get; set; } = 0L;
    internal virtual ScrollController? _backupController { get; set; } = default;

    internal virtual ScrollController _effectiveController => DartRuntimePrimitives.ConvertValue<ScrollController>(widget.controller ?? (_backupController ??= new FixedExtentScrollController()));
    public override void initState()
    {
        base.initState();
        if (widget.controller is FixedExtentScrollController)
        {
            var controllerLocal = ((FixedExtentScrollController?)widget.controller!)!;
            _lastReportedItemIndex = controllerLocal.initialItem;
        }
    }

    public override void dispose()
    {
        _backupController?.dispose();
        base.dispose();
    }

    internal virtual void _reportSelectedItemChanged(ScrollNotification notification)
    {
        var metricsLocal = ((FixedExtentMetrics?)notification.metrics)!;
        long currentItemIndex = metricsLocal.itemIndex;
        if (currentItemIndex != _lastReportedItemIndex)
        {
            _lastReportedItemIndex = currentItemIndex;
            long trueIndex = widget.childDelegate.trueIndexOf(currentItemIndex);
            widget.onSelectedItemChanged!(trueIndex);
        }
    }

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if ((widget.onSelectedItemChanged is null) || (notification.depth != 0L) || (notification.metrics is not FixedExtentMetrics))
        {
            return false;
        }
        switch (widget.changeReportingBehavior)
        {
            case ChangeReportingBehavior.onScrollEnd:
                {
                    if (notification is ScrollEndNotification)
                    {
                        ScrollEndNotification notification__as29356 = (ScrollEndNotification)notification;
                        _reportSelectedItemChanged(notification__as29356);
                    }
                    break;
                }
            case ChangeReportingBehavior.onScrollUpdate:
                {
                    if (notification is ScrollUpdateNotification)
                    {
                        ScrollUpdateNotification notification__as29522 = (ScrollUpdateNotification)notification;
                        _reportSelectedItemChanged(notification__as29522);
                    }
                    break;
                }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new NotificationListener<ScrollNotification>(onNotification: _handleScrollNotification, child: new _FixedExtentScrollable__list_wheel_scroll_view(controller: _effectiveController, physics: widget.physics, itemExtent: widget.itemExtent, restorationId: widget.restorationId, hitTestBehavior: widget.hitTestBehavior, scrollBehavior: widget.scrollBehavior ?? ScrollConfiguration.of(context).copyWith(scrollbars: false), dragStartBehavior: widget.dragStartBehavior, viewportBuilder: (context, offset) =>
        {
            return new ListWheelViewport(diameterRatio: widget.diameterRatio, perspective: widget.perspective, offAxisFraction: widget.offAxisFraction, useMagnifier: widget.useMagnifier, magnification: widget.magnification, overAndUnderCenterOpacity: widget.overAndUnderCenterOpacity, itemExtent: widget.itemExtent, squeeze: widget.squeeze, renderChildrenOutsideViewport: widget.renderChildrenOutsideViewport, offset: offset, childDelegate: widget.childDelegate, clipBehavior: widget.clipBehavior);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ListWheelElement : RenderObjectElement, ListWheelChildManager
{
    internal virtual DartMap<long, Widget?> _childWidgets { get; private set; } = new DartMap<long, Widget?>();
    internal virtual SortedDictionary<long, Element> _childElements { get; private set; } = new SortedDictionary<long, Element>();

    public ListWheelElement(ListWheelViewport widget) : base(widget)
    {
    }

    public override RenderListWheelViewport renderObject => (RenderListWheelViewport)base.renderObject;
    public override void update(Widget newWidget)
    {
        var __newWidget = (ListWheelViewport)newWidget;
        var oldWidget = ((ListWheelViewport?)widget)!;
        base.update(__newWidget);
        ListWheelChildDelegate newDelegate = __newWidget.childDelegate;
        ListWheelChildDelegate oldDelegate = oldWidget.childDelegate;
        if ((!Equals(newDelegate, oldDelegate)) && ((!Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || newDelegate.shouldRebuild(oldDelegate)))
        {
            performRebuild();
            renderObject.markNeedsLayout();
        }
    }

    public virtual long? childCount => ((ListWheelViewport?)widget)!.childDelegate.estimatedChildCount;
    public override void performRebuild()
    {
        _childWidgets.Clear();
        base.performRebuild();
        if (!Enumerable.Any(_childElements))
        {
            return;
        }
        long firstIndex = DartRuntimePrimitives.RequireValue(DartCollectionRuntime.FirstKeyOrNull(_childElements));
        long lastIndex = DartRuntimePrimitives.RequireValue(DartCollectionRuntime.LastKeyOrNull(_childElements));
        for (var index = firstIndex; index <= lastIndex; ++index)
        {
            Element? newChild = updateChild(_childElements.GetValueOrDefault(index), retrieveWidget(index), index);
            if (newChild is not null)
            {
                _childElements[index] = newChild;
            }
            else
            {
                _childElements.Remove(index);
            }
        }
    }

    public virtual Widget? retrieveWidget(long index)
    {
        return _childWidgets.putIfAbsent(index, () => ((ListWheelViewport?)widget)!.childDelegate.build(this, index));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool childExistsAt(long index) => DartRuntimePrimitives.ConvertValue<bool>(retrieveWidget(index) is not null);
    public virtual void createChild(long index, RenderBox? after)
    {
        owner!.buildScope(this, () =>
        {
            var insertFirst = after is null;
            DartRuntimePrimitives.Assert(() => insertFirst || _childElements.ContainsKey(index - 1L));
            Element? newChild = updateChild(_childElements.GetValueOrDefault(index), retrieveWidget(index), index);
            if (newChild is not null)
            {
                _childElements[index] = newChild;
            }
            else
            {
                _childElements.Remove(index);
            }
        });
    }

    public virtual void removeChild(RenderBox child)
    {
        long index = DartRuntimePrimitives.ConvertValue<long>(renderObject.indexOf(child));
        owner!.buildScope(this, () =>
        {
            DartRuntimePrimitives.Assert(() => _childElements.ContainsKey(index));
            Element? result = updateChild(_childElements.GetValueOrDefault(index), null, index);
            DartRuntimePrimitives.Assert(() => result is null);
            _childElements.Remove(index);
            DartRuntimePrimitives.Assert(() => !_childElements.ContainsKey(index));
        });
    }

    public override Element? updateChild(Element? child, Widget? newWidget, object? newSlot)
    {
        var oldParentData = ((ListWheelParentData?)((child?.renderObject)?.parentData))!;
        Element? newChild = base.updateChild(child, newWidget, newSlot);
        var newParentData = ((ListWheelParentData?)((newChild?.renderObject)?.parentData))!;
        if (newParentData is not null)
        {
            newParentData.index = (long)newSlot!;
            if (oldParentData is not null)
            {
                newParentData.offset = oldParentData.offset;
            }
        }
        return newChild;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void insertRenderObjectChild(RenderObject child, object? slot)
    {
        long __slot = DartRuntimePrimitives.ConvertValue<long>(slot);
        RenderListWheelViewport renderObjectLocal = DartRuntimePrimitives.ConvertValue<RenderListWheelViewport>(renderObject);
        DartRuntimePrimitives.Assert(() => renderObjectLocal.debugValidateChild(child));
        renderObjectLocal.insert(((RenderBox?)child)!, after: ((RenderBox?)_childElements.GetValueOrDefault(__slot - 1L)?.renderObject)!);
        DartRuntimePrimitives.Assert(() => Equals(renderObjectLocal, renderObject));
    }

    public override void moveRenderObjectChild(RenderObject child, object? oldSlot, object? newSlot)
    {
        long __oldSlot = DartRuntimePrimitives.ConvertValue<long>(oldSlot);
        long __newSlot = DartRuntimePrimitives.ConvertValue<long>(newSlot);
        var moveChildRenderObjectErrorMessage = "Currently we maintain the list in contiguous increasing order, so " + "moving children around is not allowed.";
        DartRuntimePrimitives.Assert(() => false, () => (object?)moveChildRenderObjectErrorMessage);
    }

    public override void removeRenderObjectChild(RenderObject child, object? slot)
    {
        long __slot = DartRuntimePrimitives.ConvertValue<long>(slot);
        DartRuntimePrimitives.Assert(() => Equals(child.parent, renderObject));
        renderObject.remove(((RenderBox?)child)!);
    }

    public override void visitChildren(System.Action<Element> visitor)
    {
        _childElements.forEach((key, child) =>
        {
            visitor(child);
        });
    }

    public override void forgetChild(Element child)
    {
        _childElements.Remove(DartRuntimePrimitives.ConvertValue<long>(child.slot));
        base.forgetChild(child);
    }

}

public class ListWheelViewport : RenderObjectWidget
{
    public virtual double diameterRatio { get; private set; } = default!;
    public virtual double perspective { get; private set; } = default!;
    public virtual double offAxisFraction { get; private set; } = default!;
    public virtual bool useMagnifier { get; private set; } = default!;
    public virtual double magnification { get; private set; } = default!;
    public virtual double overAndUnderCenterOpacity { get; private set; } = default!;
    public virtual double itemExtent { get; private set; } = default!;
    public virtual double squeeze { get; private set; } = default!;
    public virtual bool renderChildrenOutsideViewport { get; private set; } = default!;
    public virtual ViewportOffset offset { get; private set; } = default!;
    public virtual ListWheelChildDelegate childDelegate { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ListWheelViewport(Key? key = null, double? diameterRatio = null, double? perspective = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, double overAndUnderCenterOpacity = 1.0, double itemExtent = default!, double squeeze = 1.0, bool renderChildrenOutsideViewport = false, ViewportOffset offset = default!, ListWheelChildDelegate childDelegate = default!, Clip clipBehavior = Clip.hardEdge) : base(key: key)
    {
        double __diameterRatio = diameterRatio ?? RenderListWheelViewport.defaultDiameterRatio;
        double __perspective = perspective ?? RenderListWheelViewport.defaultPerspective;
        this.diameterRatio = __diameterRatio;
        this.perspective = __perspective;
        this.offAxisFraction = offAxisFraction;
        this.useMagnifier = useMagnifier;
        this.magnification = magnification;
        this.overAndUnderCenterOpacity = overAndUnderCenterOpacity;
        this.itemExtent = itemExtent;
        this.squeeze = squeeze;
        this.renderChildrenOutsideViewport = renderChildrenOutsideViewport;
        this.offset = offset;
        this.childDelegate = childDelegate;
        this.clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(__diameterRatio > 0L);
        System.Diagnostics.Debug.Assert(__perspective > 0L);
        System.Diagnostics.Debug.Assert(__perspective <= 0.01);
        System.Diagnostics.Debug.Assert((overAndUnderCenterOpacity >= 0L) && (overAndUnderCenterOpacity <= 1L));
        System.Diagnostics.Debug.Assert(itemExtent > 0L);
        System.Diagnostics.Debug.Assert(squeeze > 0L);
        System.Diagnostics.Debug.Assert(!renderChildrenOutsideViewport || Equals(clipBehavior, Clip.none));
    }

    public override ListWheelElement createElement() => new ListWheelElement(this);
    public override RenderObject createRenderObject(BuildContext context)
    {
        var childManagerLocal = ((ListWheelElement?)context)!;
        return new RenderListWheelViewport(childManager: childManagerLocal, offset: offset, diameterRatio: DartRuntimePrimitives.RequireValue(diameterRatio), perspective: DartRuntimePrimitives.RequireValue(perspective), offAxisFraction: offAxisFraction, useMagnifier: useMagnifier, magnification: magnification, overAndUnderCenterOpacity: overAndUnderCenterOpacity, itemExtent: itemExtent, squeeze: squeeze, renderChildrenOutsideViewport: renderChildrenOutsideViewport, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (RenderListWheelViewport)renderObject;
        DartRuntimePrimitives.Ignore(((Func<RenderListWheelViewport>)(() =>
{
    var __cascade = __renderObject;
    __cascade.offset = offset;
    __cascade.diameterRatio = diameterRatio;
    __cascade.perspective = perspective;
    __cascade.offAxisFraction = offAxisFraction;
    __cascade.useMagnifier = useMagnifier;
    __cascade.magnification = magnification;
    __cascade.overAndUnderCenterOpacity = overAndUnderCenterOpacity;
    __cascade.itemExtent = itemExtent;
    __cascade.squeeze = squeeze;
    __cascade.renderChildrenOutsideViewport = renderChildrenOutsideViewport;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

}

