// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/list_wheel_scroll_view.dart
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

    public override long? estimatedChildCount => checked((long)(this.children.Count));
    public override Widget? build(BuildContext context, long index)
    {
        if (((index < 0L) || (index >= checked((long)(this.children.Count)))))
        {
            return ((Widget)(object)null);
        }
        return ((Widget?)(object?)new IndexedSemantics(index: index, child: this.children[(int)(index)]));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(ListWheelChildDelegate oldDelegate)
    {
        var __oldDelegate = (ListWheelChildListDelegate)(object)oldDelegate;
        return (!object.Equals(this.children, ((ListWheelChildListDelegate)__oldDelegate).children));
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
    public override long trueIndexOf(long index) => DartRuntimePrimitives.ConvertValue<long>((index % checked((long)(this.children.Count))));
    public override Widget? build(BuildContext context, long index)
    {
        if (!System.Linq.Enumerable.Any(this.children))
        {
            return ((Widget)(object)null);
        }
        return ((Widget?)(object?)new IndexedSemantics(index: index, child: this.children[(int)((index % checked((long)(this.children.Count))))]));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(ListWheelChildDelegate oldDelegate)
    {
        var __oldDelegate = (ListWheelChildLoopingListDelegate)(object)oldDelegate;
        return (!object.Equals(this.children, ((ListWheelChildLoopingListDelegate)__oldDelegate).children));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ListWheelChildBuilderDelegate : ListWheelChildDelegate
{
    public virtual global::System.Func<BuildContext, long, Widget?> builder { get; private set; } = default!;
    public virtual long? childCount { get; private set; }

    public ListWheelChildBuilderDelegate(global::System.Func<BuildContext, long, Widget?> builder, long? childCount = null)
    {
        this.builder = builder;
        this.childCount = childCount;
    }

    public override long? estimatedChildCount => this.childCount;
    public override Widget? build(BuildContext context, long index)
    {
        if ((this.childCount is null))
        {
            Widget? child__7876 = this.builder(context, index);
            return ((Widget?)(object?)((child__7876 is null) ? null : new IndexedSemantics(index: index, child: child__7876)));
        }
        if (((index < 0L) || (index >= DartRuntimePrimitives.RequireValue(this.childCount))))
        {
            return ((Widget)(object)null);
        }
        return ((Widget?)(object?)new IndexedSemantics(index: index, child: this.builder(context, index)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(ListWheelChildDelegate oldDelegate)
    {
        var __oldDelegate = (ListWheelChildBuilderDelegate)(object)oldDelegate;
        return ((!object.Equals((global::System.Func<BuildContext, long, Widget?>)this.builder, (global::System.Func<BuildContext, long, Widget?>)((ListWheelChildBuilderDelegate)__oldDelegate).builder)) || (this.childCount != ((ListWheelChildBuilderDelegate)__oldDelegate).childCount));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FixedExtentScrollController : ScrollController
{
    public virtual long initialItem { get; private set; } = default!;

    public FixedExtentScrollController(long initialItem = 0, bool keepScrollOffset = true, string? debugLabel = null, global::System.Action<ScrollPosition>? onAttach = null, global::System.Action<ScrollPosition>? onDetach = null) : base(keepScrollOffset: keepScrollOffset, debugLabel: debugLabel, onAttach: onAttach, onDetach: onDetach)
    {
        this.initialItem = initialItem;
    }

    public virtual long selectedItem
    {
        get
        {
            DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.positions), () => (object?)"FixedExtentScrollController.selectedItem cannot be accessed before a " + "scroll view is built with it.");
            DartRuntimePrimitives.Assert(() => (this.positions.Count() == 1L), () => (object?)"The selectedItem property cannot be read when multiple scroll views are " + "attached to the same FixedExtentScrollController.");
            var position__10372 = ((_FixedExtentScrollPosition__list_wheel_scroll_view?)(object?)this.position)!;
            return ((_FixedExtentScrollPosition__list_wheel_scroll_view)position__10372).itemIndex;
            return default!;
        }
    }
    public async virtual Future animateToItem(long itemIndex, Duration duration, global::Doroti.Framework.Animation.Curve curve)
    {
        if (!this.hasClients)
        {
            return;
        }
        await global::Doroti.Runtime.DartAsyncRuntime.wait<object?>(new List<Future>());
    }

    public virtual void jumpToItem(long itemIndex)
    {
        foreach (_FixedExtentScrollPosition__list_wheel_scroll_view position__11444 in this.positions.cast<_FixedExtentScrollPosition__list_wheel_scroll_view>())
        {
            position__11444.jumpTo((itemIndex * ((_FixedExtentScrollPosition__list_wheel_scroll_view)position__11444).itemExtent));
        }
    }

    public override ScrollPosition createScrollPosition(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition)
    {
        return ((ScrollPosition)(object?)new _FixedExtentScrollPosition__list_wheel_scroll_view(physics: physics, context: context, initialItem: this.initialItem, oldPosition: oldPosition, keepScrollOffset: this.keepScrollOffset, debugLabel: this.debugLabel));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class FixedExtentMetrics : FixedScrollMetrics
{
    public virtual long itemIndex { get; private set; } = default!;
    public FixedExtentMetrics() : base(default!, default!, default!, default!, default!, default!) { }


    public FixedExtentMetrics(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, global::Doroti.Framework.Painting.AxisDirection axisDirection, long itemIndex, double devicePixelRatio) : base(minScrollExtent: DartRuntimePrimitives.RequireValue(minScrollExtent), maxScrollExtent: DartRuntimePrimitives.RequireValue(maxScrollExtent), pixels: DartRuntimePrimitives.RequireValue(pixels), viewportDimension: DartRuntimePrimitives.RequireValue(viewportDimension), axisDirection: axisDirection, devicePixelRatio: devicePixelRatio)
    {
        this.itemIndex = itemIndex;
    }

    public virtual FixedExtentMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new FixedExtentMetrics(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), itemIndex: (itemIndex ?? this.itemIndex), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class List_wheel_scroll_viewLibrary
{
    internal static long _getItemFromOffset(double offset, double itemExtent, double minScrollExtent, double maxScrollExtent)
    {
        return ((List_wheel_scroll_viewLibrary._clipOffsetToScrollableRange(offset, minScrollExtent, maxScrollExtent) / itemExtent)).round();
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
    internal _FixedExtentScrollPosition__list_wheel_scroll_view(ScrollPhysics physics, ScrollContext context, long initialItem, ScrollPosition? oldPosition = null, bool keepScrollOffset = true, string? debugLabel = null) : base(physics: physics, context: context, oldPosition: oldPosition, keepScrollOffset: keepScrollOffset, debugLabel: debugLabel, initialPixels: (_FixedExtentScrollPosition__list_wheel_scroll_view._getItemExtentFromScrollContext(context) * initialItem))
    {
        System.Diagnostics.Debug.Assert((context is _FixedExtentScrollableState__list_wheel_scroll_view));
    }

    internal static double _getItemExtentFromScrollContext(ScrollContext context)
    {
        var scrollable__15017 = ((_FixedExtentScrollableState__list_wheel_scroll_view?)(object?)context)!;
        return ((_FixedExtentScrollableState__list_wheel_scroll_view)scrollable__15017).itemExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double itemExtent => _FixedExtentScrollPosition__list_wheel_scroll_view._getItemExtentFromScrollContext(this.context);
    public virtual long itemIndex
    {
        get
        {
            return List_wheel_scroll_viewLibrary._getItemFromOffset(offset: DartRuntimePrimitives.RequireValue(this.pixels), itemExtent: this.itemExtent, minScrollExtent: DartRuntimePrimitives.RequireValue(this.minScrollExtent), maxScrollExtent: DartRuntimePrimitives.RequireValue(this.maxScrollExtent));
            return default!;
        }
    }
    public virtual FixedExtentMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new FixedExtentMetrics(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), itemIndex: (itemIndex ?? this.itemIndex), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _FixedExtentScrollable__list_wheel_scroll_view : Scrollable
{
    public virtual double itemExtent { get; private set; } = default!;

    internal _FixedExtentScrollable__list_wheel_scroll_view(ScrollController? controller = null, ScrollPhysics? physics = null, double itemExtent = default!, global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget> viewportBuilder = default!, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = default!, string? restorationId = null, ScrollBehavior? scrollBehavior = null, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque) : base(controller: controller, physics: physics, viewportBuilder: viewportBuilder, dragStartBehavior: dragStartBehavior, restorationId: restorationId, scrollBehavior: scrollBehavior, hitTestBehavior: hitTestBehavior)
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
            var actualWidget__17090 = ((_FixedExtentScrollable__list_wheel_scroll_view?)(object?)this.widget)!;
            return ((_FixedExtentScrollable__list_wheel_scroll_view)actualWidget__17090).itemExtent;
            return default!;
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

    public override global::Doroti.Framework.Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        DartRuntimePrimitives.Assert(() => (position is _FixedExtentScrollPosition__list_wheel_scroll_view), () => (object?)"FixedExtentScrollPhysics can only be used with Scrollables that uses " + "the FixedExtentScrollController");
        var metrics__18212 = ((_FixedExtentScrollPosition__list_wheel_scroll_view?)(object?)position)!;
        if (((((velocity <= 0.0) && (metrics__18212.pixels <= metrics__18212.minScrollExtent))) || (((velocity >= 0.0) && (metrics__18212.pixels >= metrics__18212.maxScrollExtent)))))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)(object?)base.createBallisticSimulation(metrics__18212, velocity));
        }
        global::Doroti.Framework.Physics.Simulation? testFrictionSimulation__18812 = ((global::Doroti.Framework.Physics.Simulation?)(object?)base.createBallisticSimulation(metrics__18212, velocity));
        if (((testFrictionSimulation__18812 is not null) && (((testFrictionSimulation__18812.x(double.PositiveInfinity) == metrics__18212.minScrollExtent) || (testFrictionSimulation__18812.x(double.PositiveInfinity) == metrics__18212.maxScrollExtent)))))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)(object?)base.createBallisticSimulation(metrics__18212, velocity));
        }
        long settlingItemIndex__19471 = List_wheel_scroll_viewLibrary._getItemFromOffset(offset: ((testFrictionSimulation__18812?.x(double.PositiveInfinity) ?? (double)metrics__18212.pixels)), itemExtent: ((_FixedExtentScrollPosition__list_wheel_scroll_view)metrics__18212).itemExtent, minScrollExtent: metrics__18212.minScrollExtent, maxScrollExtent: metrics__18212.maxScrollExtent);
        double settlingPixels__19746 = (settlingItemIndex__19471 * ((_FixedExtentScrollPosition__list_wheel_scroll_view)metrics__18212).itemExtent);
        if (((velocity.abs() < toleranceFor(((_FixedExtentScrollPosition__list_wheel_scroll_view)position)).velocity) && (((settlingPixels__19746 - metrics__18212.pixels)).abs() < toleranceFor(((_FixedExtentScrollPosition__list_wheel_scroll_view)position)).distance)))
        {
            return ((global::Doroti.Framework.Physics.Simulation)(object)null);
        }
        if ((settlingItemIndex__19471 == ((_FixedExtentScrollPosition__list_wheel_scroll_view)metrics__18212).itemIndex))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)(object?)new global::Doroti.Framework.Physics.SpringSimulation(this.spring, metrics__18212.pixels, settlingPixels__19746, velocity, tolerance: toleranceFor(((_FixedExtentScrollPosition__list_wheel_scroll_view)position))));
        }
        return ((global::Doroti.Framework.Physics.Simulation?)(object?)global::Doroti.Framework.Physics.FrictionSimulation.CreateThrough(metrics__18212.pixels, settlingPixels__19746, velocity, (toleranceFor(((_FixedExtentScrollPosition__list_wheel_scroll_view)position)).velocity * Math.Sign(velocity))));
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
    public virtual global::System.Action<long>? onSelectedItemChanged { get; private set; }
    public virtual bool renderChildrenOutsideViewport { get; private set; } = default!;
    public virtual ListWheelChildDelegate childDelegate { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ChangeReportingBehavior changeReportingBehavior { get; private set; } = default!;

    public ListWheelScrollView(global::Doroti.Framework.Foundation.Key? key = null, ScrollController? controller = null, ScrollPhysics? physics = null, double? diameterRatio = null, double? perspective = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, double overAndUnderCenterOpacity = 1.0, double itemExtent = default!, double squeeze = 1.0, global::System.Action<long>? onSelectedItemChanged = null, bool renderChildrenOutsideViewport = false, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque, string? restorationId = null, ScrollBehavior? scrollBehavior = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, ChangeReportingBehavior changeReportingBehavior = ChangeReportingBehavior.onScrollUpdate, List<Widget> children = default!) : base(key: key)
    {
        double __diameterRatio = diameterRatio ?? global::Doroti.Framework.Rendering.RenderListWheelViewport.defaultDiameterRatio;
        double __perspective = perspective ?? global::Doroti.Framework.Rendering.RenderListWheelViewport.defaultPerspective;
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
        this.childDelegate = new ListWheelChildListDelegate(children: children);
        System.Diagnostics.Debug.Assert((__diameterRatio > 0.0));
        System.Diagnostics.Debug.Assert((__perspective > 0L));
        System.Diagnostics.Debug.Assert((__perspective <= 0.01));
        System.Diagnostics.Debug.Assert((magnification > 0L));
        System.Diagnostics.Debug.Assert(((overAndUnderCenterOpacity >= 0L) && (overAndUnderCenterOpacity <= 1L)));
        System.Diagnostics.Debug.Assert((itemExtent > 0L));
        System.Diagnostics.Debug.Assert((squeeze > 0L));
        System.Diagnostics.Debug.Assert((!renderChildrenOutsideViewport || (object.Equals(clipBehavior, Clip.none))));
    }

    public static ListWheelScrollView CreateUseDelegate(global::Doroti.Framework.Foundation.Key? key = null, ScrollController? controller = null, ScrollPhysics? physics = null, double? diameterRatio = null, double? perspective = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, double overAndUnderCenterOpacity = 1.0, double itemExtent = default!, double squeeze = 1.0, global::System.Action<long>? onSelectedItemChanged = null, bool renderChildrenOutsideViewport = false, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque, string? restorationId = null, ScrollBehavior? scrollBehavior = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, ChangeReportingBehavior changeReportingBehavior = ChangeReportingBehavior.onScrollUpdate, ListWheelChildDelegate childDelegate = default!)
    {
        var __instance = new ListWheelScrollView(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        double __diameterRatio = diameterRatio ?? global::Doroti.Framework.Rendering.RenderListWheelViewport.defaultDiameterRatio;
        double __perspective = perspective ?? global::Doroti.Framework.Rendering.RenderListWheelViewport.defaultPerspective;
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

    internal virtual ScrollController _effectiveController => DartRuntimePrimitives.ConvertValue<ScrollController>((((ListWheelScrollView)this.widget).controller ?? (_backupController ??= new FixedExtentScrollController())));
    public override void initState()
    {
        base.initState();
        if ((((ListWheelScrollView)this.widget).controller is FixedExtentScrollController))
        {
            var controller__28369 = ((FixedExtentScrollController?)(object?)((ListWheelScrollView)this.widget).controller!)!;
            _lastReportedItemIndex = ((FixedExtentScrollController)controller__28369).initialItem;
        }
    }

    public override void dispose()
    {
        this._backupController?.dispose();
        base.dispose();
    }

    internal virtual void _reportSelectedItemChanged(ScrollNotification notification)
    {
        var metrics__28669 = ((FixedExtentMetrics?)(object?)((ScrollNotification)notification).metrics)!;
        long currentItemIndex__28737 = ((FixedExtentMetrics)metrics__28669).itemIndex;
        if ((currentItemIndex__28737 != this._lastReportedItemIndex))
        {
            _lastReportedItemIndex = currentItemIndex__28737;
            long trueIndex__28894 = ((ListWheelScrollView)this.widget).childDelegate.trueIndexOf(currentItemIndex__28737);
            ((ListWheelScrollView)this.widget).onSelectedItemChanged!(trueIndex__28894);
        }
    }

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if ((((((ListWheelScrollView)this.widget).onSelectedItemChanged is null) || (notification.depth != 0L)) || (((ScrollNotification)notification).metrics is not FixedExtentMetrics)))
        {
            return false;
        }
        switch (((ListWheelScrollView)this.widget).changeReportingBehavior)
        {
            case ChangeReportingBehavior.onScrollEnd:
                {
                    if ((notification is ScrollEndNotification))
                    {
                        ScrollEndNotification notification__as29356 = (ScrollEndNotification)notification;
                        _reportSelectedItemChanged(((ScrollEndNotification)notification__as29356));
                    }
                    break;
                }
            case ChangeReportingBehavior.onScrollUpdate:
                {
                    if ((notification is ScrollUpdateNotification))
                    {
                        ScrollUpdateNotification notification__as29522 = (ScrollUpdateNotification)notification;
                        _reportSelectedItemChanged(((ScrollUpdateNotification)notification__as29522));
                    }
                    break;
                }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new NotificationListener<ScrollNotification>(onNotification: (global::System.Func<ScrollNotification, bool>)this._handleScrollNotification, child: new _FixedExtentScrollable__list_wheel_scroll_view(controller: this._effectiveController, physics: ((ListWheelScrollView)this.widget).physics, itemExtent: ((ListWheelScrollView)this.widget).itemExtent, restorationId: ((ListWheelScrollView)this.widget).restorationId, hitTestBehavior: ((ListWheelScrollView)this.widget).hitTestBehavior, scrollBehavior: ((((ListWheelScrollView)this.widget).scrollBehavior ?? (ScrollBehavior)ScrollConfiguration.of(context).copyWith(scrollbars: false))), dragStartBehavior: ((ListWheelScrollView)this.widget).dragStartBehavior, viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget>)((context, offset) => {
return ((Widget)(object?)new ListWheelViewport(diameterRatio: ((ListWheelScrollView)this.widget).diameterRatio, perspective: ((ListWheelScrollView)this.widget).perspective, offAxisFraction: ((ListWheelScrollView)this.widget).offAxisFraction, useMagnifier: ((ListWheelScrollView)this.widget).useMagnifier, magnification: ((ListWheelScrollView)this.widget).magnification, overAndUnderCenterOpacity: ((ListWheelScrollView)this.widget).overAndUnderCenterOpacity, itemExtent: ((ListWheelScrollView)this.widget).itemExtent, squeeze: ((ListWheelScrollView)this.widget).squeeze, renderChildrenOutsideViewport: ((ListWheelScrollView)this.widget).renderChildrenOutsideViewport, offset: offset, childDelegate: ((ListWheelScrollView)this.widget).childDelegate, clipBehavior: ((ListWheelScrollView)this.widget).clipBehavior));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ListWheelElement : RenderObjectElement, global::Doroti.Framework.Rendering.ListWheelChildManager
{
    internal virtual DartMap<long, Widget?> _childWidgets { get; private set; } = new DartMap<long, Widget?>();
    internal virtual SortedDictionary<long, Element> _childElements { get; private set; } = new SortedDictionary<long, Element>();

    public ListWheelElement(ListWheelViewport widget) : base(widget)
    {
    }

    public override global::Doroti.Framework.Rendering.RenderObject renderObject => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(((global::Doroti.Framework.Rendering.RenderListWheelViewport?)(object?)base.renderObject)!);
    public override void update(Widget newWidget)
    {
        var __newWidget = (ListWheelViewport)(object)newWidget;
        var oldWidget__32216 = ((ListWheelViewport?)(object?)this.widget)!;
        base.update(__newWidget);
        ListWheelChildDelegate newDelegate__32319 = ((ListWheelViewport)__newWidget).childDelegate;
        ListWheelChildDelegate oldDelegate__32391 = ((ListWheelViewport)oldWidget__32216).childDelegate;
        if (((!object.Equals(newDelegate__32319, oldDelegate__32391)) && (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate__32319), DartRuntimePrimitives.RuntimeType(oldDelegate__32391))) || newDelegate__32319.shouldRebuild(oldDelegate__32391)))))
        {
            performRebuild();
            this.renderObject.markNeedsLayout();
        }
    }

    public virtual long? childCount => (((ListWheelViewport?)(object?)this.widget)!).childDelegate.estimatedChildCount;
    public override void performRebuild()
    {
        this._childWidgets.Clear();
        base.performRebuild();
        if (!System.Linq.Enumerable.Any(this._childElements))
        {
            return;
        }
        long firstIndex__32924 = DartRuntimePrimitives.RequireValue(DartCollectionRuntime.FirstKeyOrNull<long, Element>(this._childElements));
        long lastIndex__32979 = DartRuntimePrimitives.RequireValue(DartCollectionRuntime.LastKeyOrNull<long, Element>(this._childElements));
        for (var index__33032 = firstIndex__32924; (index__33032 <= lastIndex__32979); ++index__33032)
        {
            Element? newChild__33104 = ((Element?)(object?)updateChild(this._childElements.GetValueOrDefault(index__33032), retrieveWidget(index__33032), index__33032));
            if ((newChild__33104 is not null))
            {
                this._childElements[index__33032] = newChild__33104;
            }
            else
            {
                this._childElements.Remove(index__33032);
            }
        }
    }

    public virtual Widget? retrieveWidget(long index)
    {
        return this._childWidgets.putIfAbsent(index, (() => (((ListWheelViewport?)(object?)this.widget)!).childDelegate.build(this, index)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool childExistsAt(long index) => DartRuntimePrimitives.ConvertValue<bool>((retrieveWidget(index) is not null));
    public virtual void createChild(long index, global::Doroti.Framework.Rendering.RenderBox? after)
    {
        this.owner!.buildScope(this, ((global::System.Action)(() => {
var insertFirst__33942 = (after is null);
DartRuntimePrimitives.Assert(() => (insertFirst__33942 || (this._childElements.ContainsKey((index - 1L)))));
Element? newChild__34056 = ((Element?)(object?)updateChild(this._childElements.GetValueOrDefault(index), retrieveWidget(index), index));
if ((newChild__34056 is not null))
{
    this._childElements[index] = newChild__34056;
}
else
{
    this._childElements.Remove(index);
}
})));
    }

    public virtual void removeChild(global::Doroti.Framework.Rendering.RenderBox child)
    {
        long index__34343 = DartRuntimePrimitives.ConvertValue<long>(((long)((dynamic)this.renderObject).indexOf(child)));
        this.owner!.buildScope(this, ((global::System.Action)(() => {
DartRuntimePrimitives.Assert(() => this._childElements.ContainsKey(index__34343));
Element? result__34483 = ((Element?)(object?)updateChild(this._childElements.GetValueOrDefault(index__34343), ((Widget)(object)null), index__34343));
DartRuntimePrimitives.Assert(() => (result__34483 is null));
this._childElements.Remove(index__34343);
DartRuntimePrimitives.Assert(() => !this._childElements.ContainsKey(index__34343));
})));
    }

    public override Element? updateChild(Element? child, Widget? newWidget, object? newSlot)
    {
        var oldParentData__34769 = ((global::Doroti.Framework.Rendering.ListWheelParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)child?.renderObject)?.parentData))!;
        Element? newChild__34861 = ((Element?)(object?)base.updateChild(child, newWidget, newSlot));
        var newParentData__34928 = ((global::Doroti.Framework.Rendering.ListWheelParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)newChild__34861?.renderObject)?.parentData))!;
        if ((newParentData__34928 is not null))
        {
            newParentData__34928.index = ((long)newSlot!);
            if ((oldParentData__34769 is not null))
            {
                newParentData__34928.offset = oldParentData__34769.offset;
            }
        }
        return newChild__34861;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void insertRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        long __slot = DartRuntimePrimitives.ConvertValue<long>(slot);
        global::Doroti.Framework.Rendering.RenderListWheelViewport renderObject__35320 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderListWheelViewport>(this.renderObject);
        DartRuntimePrimitives.Assert(() => ((bool)((dynamic)renderObject__35320).debugValidateChild(child)));
        renderObject__35320.insert(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!, after: ((global::Doroti.Framework.Rendering.RenderBox?)(object?)this._childElements.GetValueOrDefault((__slot - 1L))?.renderObject)!);
        DartRuntimePrimitives.Assert(() => (object.Equals(renderObject__35320, this.renderObject)));
    }

    public override void moveRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? oldSlot, object? newSlot)
    {
        long __oldSlot = DartRuntimePrimitives.ConvertValue<long>(oldSlot);
        long __newSlot = DartRuntimePrimitives.ConvertValue<long>(newSlot);
        var moveChildRenderObjectErrorMessage__35682 = "Currently we maintain the list in contiguous increasing order, so " + "moving children around is not allowed.";
        DartRuntimePrimitives.Assert(() => false, () => (object?)moveChildRenderObjectErrorMessage__35682);
    }

    public override void removeRenderObjectChild(global::Doroti.Framework.Rendering.RenderObject child, object? slot)
    {
        long __slot = DartRuntimePrimitives.ConvertValue<long>(slot);
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Framework.Rendering.RenderObject)child).parent, this.renderObject)));
        ((dynamic)this.renderObject).remove(((global::Doroti.Framework.Rendering.RenderBox?)(object?)child)!);
    }

    public override void visitChildren(global::System.Action<Element> visitor)
    {
        this._childElements.forEach(((global::System.Action<long, Element>)((key, child) => {
visitor(child);
})));
    }

    public override void forgetChild(Element child)
    {
        this._childElements.Remove(DartRuntimePrimitives.ConvertValue<long>(((Element)child).slot));
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
    public virtual global::Doroti.Framework.Rendering.ViewportOffset offset { get; private set; } = default!;
    public virtual ListWheelChildDelegate childDelegate { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    public ListWheelViewport(global::Doroti.Framework.Foundation.Key? key = null, double? diameterRatio = null, double? perspective = null, double offAxisFraction = 0.0, bool useMagnifier = false, double magnification = 1.0, double overAndUnderCenterOpacity = 1.0, double itemExtent = default!, double squeeze = 1.0, bool renderChildrenOutsideViewport = false, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, ListWheelChildDelegate childDelegate = default!, Clip clipBehavior = Clip.hardEdge) : base(key: key)
    {
        double __diameterRatio = diameterRatio ?? global::Doroti.Framework.Rendering.RenderListWheelViewport.defaultDiameterRatio;
        double __perspective = perspective ?? global::Doroti.Framework.Rendering.RenderListWheelViewport.defaultPerspective;
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
        System.Diagnostics.Debug.Assert((__diameterRatio > 0L));
        System.Diagnostics.Debug.Assert((__perspective > 0L));
        System.Diagnostics.Debug.Assert((__perspective <= 0.01));
        System.Diagnostics.Debug.Assert(((overAndUnderCenterOpacity >= 0L) && (overAndUnderCenterOpacity <= 1L)));
        System.Diagnostics.Debug.Assert((itemExtent > 0L));
        System.Diagnostics.Debug.Assert((squeeze > 0L));
        System.Diagnostics.Debug.Assert((!renderChildrenOutsideViewport || (object.Equals(clipBehavior, Clip.none))));
    }

    public override ListWheelElement createElement() => new ListWheelElement(this);
    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        var childManager__39973 = ((ListWheelElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new global::Doroti.Framework.Rendering.RenderListWheelViewport(childManager: childManager__39973, offset: this.offset, diameterRatio: DartRuntimePrimitives.RequireValue(this.diameterRatio), perspective: DartRuntimePrimitives.RequireValue(this.perspective), offAxisFraction: this.offAxisFraction, useMagnifier: this.useMagnifier, magnification: this.magnification, overAndUnderCenterOpacity: this.overAndUnderCenterOpacity, itemExtent: this.itemExtent, squeeze: this.squeeze, renderChildrenOutsideViewport: this.renderChildrenOutsideViewport, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (global::Doroti.Framework.Rendering.RenderListWheelViewport)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.RenderListWheelViewport>)(() =>
{            var __cascade = __renderObject;
            __cascade.offset = this.offset;
            __cascade.diameterRatio = this.diameterRatio;
            __cascade.perspective = this.perspective;
            __cascade.offAxisFraction = this.offAxisFraction;
            __cascade.useMagnifier = this.useMagnifier;
            __cascade.magnification = this.magnification;
            __cascade.overAndUnderCenterOpacity = this.overAndUnderCenterOpacity;
            __cascade.itemExtent = this.itemExtent;
            __cascade.squeeze = this.squeeze;
            __cascade.renderChildrenOutsideViewport = this.renderChildrenOutsideViewport;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

}

