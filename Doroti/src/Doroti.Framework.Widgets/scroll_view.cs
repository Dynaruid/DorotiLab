// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum ScrollViewKeyboardDismissBehavior
{
    manual,
    onDrag
}

public abstract class ScrollView : StatelessWidget
{
    public virtual global::Doroti.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual bool shrinkWrap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Foundation.Key? center { get; private set; }
    public virtual double anchor { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual long? semanticChildCount { get; private set; }
    public virtual global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;

    protected ScrollView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, ScrollBehavior? scrollBehavior = null, bool shrinkWrap = false, global::Doroti.Framework.Foundation.Key? center = null, double anchor = 0.0, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque) : base(key: key)
    {
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.controller = controller;
        this.primary = primary;
        this.scrollBehavior = scrollBehavior;
        this.shrinkWrap = shrinkWrap;
        this.center = center;
        this.anchor = anchor;
        this.cacheExtent = cacheExtent;
        this.scrollCacheExtent = scrollCacheExtent;
        this.semanticChildCount = semanticChildCount;
        this.paintOrder = paintOrder;
        this.dragStartBehavior = dragStartBehavior;
        this.keyboardDismissBehavior = keyboardDismissBehavior;
        this.restorationId = restorationId;
        this.clipBehavior = clipBehavior;
        this.hitTestBehavior = hitTestBehavior;
        this.physics = physics ?? (((primary ?? false) || (primary is null) && (controller is null) && DartRuntimePrimitives.Identical(scrollDirection, Axis.vertical)) ? new AlwaysScrollableScrollPhysics() : null);
        System.Diagnostics.Debug.Assert(!((controller is not null) && (primary ?? false)));
        System.Diagnostics.Debug.Assert(!shrinkWrap || (center is null));
        System.Diagnostics.Debug.Assert((anchor >= 0.0) && (anchor <= 1.0));
        System.Diagnostics.Debug.Assert((semanticChildCount is null) || (semanticChildCount >= 0L));
    }

    public virtual global::Doroti.Framework.Painting.AxisDirection getDirection(BuildContext context)
    {
        return BasicLibrary.getAxisDirectionFromAxisReverseAndDirectionality(context, scrollDirection, reverse);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract List<Widget> buildSlivers(BuildContext context);
    public virtual Widget buildViewport(BuildContext context, global::Doroti.Framework.Rendering.ViewportOffset offset, global::Doroti.Framework.Painting.AxisDirection axisDirection, List<Widget> slivers)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                switch (axisDirection)
                {
                    case AxisDirection.up:
                    case AxisDirection.down:
                        {
                            return DebugLibrary.debugCheckHasDirectionality(context, why: "to determine the cross-axis direction of the scroll view", hint: "Vertical scroll views create Viewport widgets that try to determine their cross axis direction " + "from the ambient Directionality.");
                        }
                    case AxisDirection.left:
                    case AxisDirection.right:
                        {
                            return true;
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        global::Doroti.Framework.Rendering.ScrollCacheExtent? effectiveScrollCacheExtent = scrollCacheExtent ?? ((cacheExtent is not null) ? ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(cacheExtent)) : null);
        if (shrinkWrap)
        {
            return new ShrinkWrappingViewport(axisDirection: axisDirection, offset: offset, slivers: slivers, paintOrder: paintOrder, clipBehavior: clipBehavior, scrollCacheExtent: effectiveScrollCacheExtent);
        }
        return new Viewport(axisDirection: axisDirection, offset: offset, slivers: slivers, scrollCacheExtent: effectiveScrollCacheExtent, center: center, anchor: anchor, paintOrder: paintOrder, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        List<Widget> slivers = buildSlivers(context);
        global::Doroti.Framework.Painting.AxisDirection axisDirectionLocal = getDirection(context);
        bool effectivePrimary = primary ?? ((controller is null) && PrimaryScrollController.shouldInherit(context, scrollDirection));
        ScrollController? scrollController = effectivePrimary ? PrimaryScrollController.maybeOf(context) : controller;
        var scrollable = new Scrollable(dragStartBehavior: dragStartBehavior, axisDirection: axisDirectionLocal, controller: scrollController, physics: physics, scrollBehavior: scrollBehavior, semanticChildCount: semanticChildCount, restorationId: restorationId, hitTestBehavior: hitTestBehavior, viewportBuilder: (context, offset) =>
        {
            return buildViewport(context, offset, axisDirectionLocal, slivers);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, clipBehavior: clipBehavior);
        Widget scrollableResult = (effectivePrimary && (scrollController is not null)) ? PrimaryScrollController.CreateNone(child: scrollable) : scrollable;
        ScrollViewKeyboardDismissBehavior effectiveKeyboardDismissBehavior = keyboardDismissBehavior ?? ScrollViewKeyboardDismissBehavior.manual;
        if (Equals(effectiveKeyboardDismissBehavior, ScrollViewKeyboardDismissBehavior.onDrag))
        {
            return new NotificationListener<ScrollUpdateNotification>(child: scrollableResult, onNotification: (notification) =>
            {
                FocusScopeNode currentScope = FocusScope.of(context);
                if ((notification.dragDetails is not null) && !currentScope.hasPrimaryFocus && currentScope.hasFocus)
                {
                    FocusManager.instance.primaryFocus?.unfocus();
                }
                return false;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
        else
        {
            return scrollableResult;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.Axis>("scrollDirection", scrollDirection));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("reverse", value: reverse, ifTrue: "reversed", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollController>("controller", controller, showName: false, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("primary", value: primary, ifTrue: "using primary controller", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("physics", physics, showName: false, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.FlagProperty("shrinkWrap", value: shrinkWrap, ifTrue: "shrink-wrapping", showName: true));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.ScrollCacheExtent>("scrollCacheExtent", scrollCacheExtent, defaultValue: null));
    }

}

public class CustomScrollView : ScrollView
{
    public virtual List<Widget> slivers { get; private set; } = default!;

    public CustomScrollView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, ScrollBehavior? scrollBehavior = null, bool shrinkWrap = false, global::Doroti.Framework.Foundation.Key? center = null, double anchor = 0.0, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Framework.Rendering.SliverPaintOrder paintOrder = SliverPaintOrder.firstIsTop, List<Widget> slivers = default!, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque) : base(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, scrollBehavior: scrollBehavior, shrinkWrap: shrinkWrap, center: center, anchor: anchor, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, paintOrder: paintOrder, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior)
    {
        List<Widget> __slivers = slivers ?? new List<Widget>();
        this.slivers = __slivers;
    }

    public override List<Widget> buildSlivers(BuildContext context) => slivers;
}

public abstract class BoxScrollView : ScrollView
{
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }

    protected BoxScrollView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque) : base(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior)
    {
        this.padding = padding;
    }

    public override List<Widget> buildSlivers(BuildContext context)
    {
        Widget sliverLocal = buildChildLayout(context);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding = padding;
        if (padding is null)
        {
            MediaQueryData? mediaQuery = MediaQuery.maybeOf(context);
            if (mediaQuery is not null)
            {
                global::Doroti.Framework.Painting.EdgeInsets mediaQueryHorizontalPadding = mediaQuery.padding.copyWith(top: 0.0, bottom: 0.0);
                global::Doroti.Framework.Painting.EdgeInsets mediaQueryVerticalPadding = mediaQuery.padding.copyWith(left: 0.0, right: 0.0);
                effectivePadding = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(Equals(scrollDirection, Axis.vertical) ? mediaQueryVerticalPadding : mediaQueryHorizontalPadding);
                sliverLocal = DartRuntimePrimitives.ConvertValue<Widget>(new MediaQuery(data: mediaQuery.copyWith(padding: Equals(scrollDirection, Axis.vertical) ? mediaQueryHorizontalPadding : mediaQueryVerticalPadding), child: sliverLocal));
            }
        }
        if (effectivePadding is not null)
        {
            sliverLocal = DartRuntimePrimitives.ConvertValue<Widget>(new SliverPadding(padding: effectivePadding, sliver: sliverLocal));
        }
        return new List<Widget> { sliverLocal };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Widget buildChildLayout(BuildContext context);
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", padding, defaultValue: null));
    }

}

public class ListView : BoxScrollView
{
    public virtual double? itemExtent { get; private set; }
    public virtual ItemExtentBuilder? itemExtentBuilder { get; private set; }
    public virtual Widget? prototypeItem { get; private set; }
    public virtual SliverChildDelegate childrenDelegate { get; private set; } = default!;

    public ListView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, Widget? prototypeItem = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> children = default!, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque) : base(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior, semanticChildCount: semanticChildCount ?? checked((children ?? new List<Widget>()).Count))
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.itemExtent = itemExtent;
        this.itemExtentBuilder = itemExtentBuilder;
        this.prototypeItem = prototypeItem;
        childrenDelegate = new SliverChildListDelegate(children ?? new List<Widget>(), addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        System.Diagnostics.Debug.Assert((itemExtent is null) && (prototypeItem is null) || (itemExtent is null) && (itemExtentBuilder is null) || (prototypeItem is null) && (itemExtentBuilder is null));
    }

    public static ListView CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, Widget? prototypeItem = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque)
    {
        var __instance = new ListView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, itemExtent: itemExtent, itemExtentBuilder: itemExtentBuilder, prototypeItem: prototypeItem, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, children: new List<Widget>(), semanticChildCount: semanticChildCount ?? itemCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        __instance.itemExtent = itemExtent;
        __instance.itemExtentBuilder = itemExtentBuilder;
        __instance.prototypeItem = prototypeItem;
        __instance.childrenDelegate = new SliverChildBuilderDelegate(itemBuilder, findChildIndexCallback: findChildIndexCallback, childCount: itemCount, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        return __instance;
    }

    public static ListView CreateSeparated(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findItemIndexCallback = null, global::System.Func<BuildContext, long, Widget> separatorBuilder = default!, long itemCount = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque)
    {
        var __instance = new ListView(key, scrollDirection, reverse, controller, primary, physics, shrinkWrap, padding, default!, default!, default!, addAutomaticKeepAlives, addRepaintBoundaries, addSemanticIndexes, cacheExtent, scrollCacheExtent, default!, default!, dragStartBehavior, keyboardDismissBehavior, restorationId, clipBehavior, hitTestBehavior);
        __instance.itemExtent = null;
        __instance.itemExtentBuilder = null;
        __instance.prototypeItem = null;
        __instance.childrenDelegate = new SliverChildBuilderDelegate((context, index) =>
        {
            long itemIndex = checked(index / 2L);
            if ((checked(index) & 1L) == 0L)
            {
                return itemBuilder(context, itemIndex);
            }
            return separatorBuilder(context, itemIndex);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, findChildIndexCallback: (findItemIndexCallback is not null) ? ((key) =>
        {
            long? itemIndexLocal = findItemIndexCallback(key);
            return (itemIndexLocal is null) ? null : (DartRuntimePrimitives.RequireValue(itemIndexLocal) * 2L);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }) : findChildIndexCallback, childCount: _computeActualChildCount(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(itemCount))), addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes, semanticIndexCallback: (widget, index) =>
        {
            return ((checked(index) & 1L) == 0L) ? checked(index / 2L) : null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return __instance;
    }

    public static ListView CreateCustom(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, double? itemExtent = null, Widget? prototypeItem = null, ItemExtentBuilder? itemExtentBuilder = null, SliverChildDelegate childrenDelegate = default!, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque)
    {
        var __instance = new ListView(key, scrollDirection, reverse, controller, primary, physics, shrinkWrap, padding, itemExtent, itemExtentBuilder, prototypeItem, default!, default!, default!, cacheExtent, scrollCacheExtent, default!, semanticChildCount, dragStartBehavior, keyboardDismissBehavior, restorationId, clipBehavior, hitTestBehavior);
        __instance.itemExtent = itemExtent;
        __instance.prototypeItem = prototypeItem;
        __instance.itemExtentBuilder = itemExtentBuilder;
        __instance.childrenDelegate = childrenDelegate;
        return __instance;
    }

    public override Widget buildChildLayout(BuildContext context)
    {
        if (itemExtent is not null)
        {
            double itemExtent__value70683 = DartRuntimePrimitives.RequireValue(itemExtent);
            return new SliverFixedExtentList(@delegate: childrenDelegate, itemExtent: DartRuntimePrimitives.RequireValue(itemExtent));
        }
        else
        {
            if (itemExtentBuilder is not null)
            {
                return new SliverVariedExtentList(@delegate: childrenDelegate, itemExtentBuilder: itemExtentBuilder!);
            }
            else
            {
                if (prototypeItem is not null)
                {
                    return new SliverPrototypeExtentList(@delegate: childrenDelegate, prototypeItem: prototypeItem!);
                }
            }
        }
        return new SliverList(@delegate: childrenDelegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("itemExtent", itemExtent, defaultValue: null));
    }

    internal static long _computeActualChildCount(long itemCount)
    {
        return Math.Max(0L, (DartRuntimePrimitives.RequireValue(itemCount) * 2L) - 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class GridView : BoxScrollView
{
    public virtual global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate { get; private set; } = default!;
    public virtual SliverChildDelegate childrenDelegate { get; private set; } = default!;

    public GridView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> children = default!, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, Clip clipBehavior = Clip.hardEdge, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque) : base(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, dragStartBehavior: dragStartBehavior, clipBehavior: clipBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, hitTestBehavior: hitTestBehavior, semanticChildCount: semanticChildCount ?? checked((children ?? new List<Widget>()).Count))
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.gridDelegate = gridDelegate;
        childrenDelegate = new SliverChildListDelegate(children ?? new List<Widget>(), addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
    }

    public static GridView CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate = default!, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque)
    {
        var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        __instance.gridDelegate = gridDelegate;
        __instance.childrenDelegate = new SliverChildBuilderDelegate(itemBuilder, findChildIndexCallback: findChildIndexCallback, childCount: itemCount, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        return __instance;
    }

    public static GridView CreateCustom(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate = default!, SliverChildDelegate childrenDelegate = default!, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque)
    {
        var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        __instance.gridDelegate = gridDelegate;
        __instance.childrenDelegate = childrenDelegate;
        return __instance;
    }

    public static GridView CreateCount(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, long crossAxisCount = default!, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, double? mainAxisExtent = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> children = default!, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque)
    {
        var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        List<Widget> __children = children ?? new List<Widget>();
        __instance.gridDelegate = new global::Doroti.Framework.Rendering.SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: crossAxisCount, mainAxisSpacing: mainAxisSpacing, crossAxisSpacing: crossAxisSpacing, childAspectRatio: childAspectRatio, mainAxisExtent: mainAxisExtent);
        __instance.childrenDelegate = new SliverChildListDelegate(children ?? new List<Widget>(), addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        return __instance;
    }

    public static GridView CreateExtent(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, double maxCrossAxisExtent = default!, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, double? mainAxisExtent = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> children = default!, long? semanticChildCount = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque)
    {
        var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        List<Widget> __children = children ?? new List<Widget>();
        __instance.gridDelegate = new global::Doroti.Framework.Rendering.SliverGridDelegateWithMaxCrossAxisExtent(maxCrossAxisExtent: maxCrossAxisExtent, mainAxisSpacing: mainAxisSpacing, crossAxisSpacing: crossAxisSpacing, childAspectRatio: childAspectRatio, mainAxisExtent: mainAxisExtent);
        __instance.childrenDelegate = new SliverChildListDelegate(children ?? new List<Widget>(), addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        return __instance;
    }

    public override Widget buildChildLayout(BuildContext context)
    {
        return new SliverGrid(@delegate: childrenDelegate, gridDelegate: gridDelegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
