// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/scroll_view.dart
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

public enum ScrollViewKeyboardDismissBehavior
{
    manual,
    onDrag
}

public abstract class ScrollView : StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual bool shrinkWrap { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Foundation.Key? center { get; private set; }
    public virtual double anchor { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual long? semanticChildCount { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.SliverPaintOrder paintOrder { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;

    protected ScrollView(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, ScrollBehavior? scrollBehavior = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Foundation.Key? center = null, double anchor = 0.0, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Generated.Framework.Rendering.SliverPaintOrder paintOrder = global::Doroti.Generated.Framework.Rendering.SliverPaintOrder.firstIsTop, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque) : base(key: key)
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
        this.physics = (physics ?? (((((primary ?? false)) || ((((primary is null) && (controller is null)) && DartRuntimePrimitives.Identical(scrollDirection, global::Doroti.Generated.Framework.Painting.Axis.vertical)))) ? new AlwaysScrollableScrollPhysics() : null)));
        System.Diagnostics.Debug.Assert(!(((controller is not null) && ((primary ?? false)))));
        System.Diagnostics.Debug.Assert((!shrinkWrap || (center is null)));
        System.Diagnostics.Debug.Assert(((anchor >= 0.0) && (anchor <= 1.0)));
        System.Diagnostics.Debug.Assert(((semanticChildCount is null) || (semanticChildCount >= 0L)));
    }

    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection getDirection(BuildContext context)
    {
        return global::Doroti.Generated.Framework.Widgets.BasicLibrary.getAxisDirectionFromAxisReverseAndDirectionality(context, this.scrollDirection, this.reverse);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract List<Widget> buildSlivers(BuildContext context);
    public virtual Widget buildViewport(BuildContext context, global::Doroti.Generated.Framework.Rendering.ViewportOffset offset, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection, List<Widget> slivers)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                switch (axisDirection)
                {
                    case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
                    case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                        {
                            return global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context, why: "to determine the cross-axis direction of the scroll view", hint: "Vertical scroll views create Viewport widgets that try to determine their cross axis direction " + "from the ambient Directionality.");
                        }
                    case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                    case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
                        {
                            return true;
                        }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? effectiveScrollCacheExtent__20063 = (this.scrollCacheExtent ?? (((this.cacheExtent is not null) ? global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(this.cacheExtent)) : null)));
        if (this.shrinkWrap)
        {
            return ((Widget)(object?)new ShrinkWrappingViewport(axisDirection: axisDirection, offset: offset, slivers: slivers, paintOrder: this.paintOrder, clipBehavior: this.clipBehavior, scrollCacheExtent: effectiveScrollCacheExtent__20063));
        }
        return ((Widget)(object?)new Viewport(axisDirection: axisDirection, offset: offset, slivers: slivers, scrollCacheExtent: effectiveScrollCacheExtent__20063, center: this.center, anchor: this.anchor, paintOrder: this.paintOrder, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        List<Widget> slivers__20827 = ((List<Widget>)(object?)buildSlivers(context));
        global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection__20884 = getDirection(context);
        bool effectivePrimary__20939 = (this.primary ?? ((this.controller is null) && PrimaryScrollController.shouldInherit(context, this.scrollDirection)));
        ScrollController? scrollController__21101 = (effectivePrimary__20939 ? PrimaryScrollController.maybeOf(context) : this.controller);
        var scrollable__21221 = new Scrollable(dragStartBehavior: this.dragStartBehavior, axisDirection: axisDirection__20884, controller: scrollController__21101, physics: this.physics, scrollBehavior: this.scrollBehavior, semanticChildCount: this.semanticChildCount, restorationId: this.restorationId, hitTestBehavior: this.hitTestBehavior, viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget>)((context, offset) => {
return ((Widget)(object?)buildViewport(context, offset, axisDirection__20884, slivers__20827));
throw new InvalidOperationException("Dart closure completed without a value.");
})), clipBehavior: this.clipBehavior);
        Widget scrollableResult__21756 = ((effectivePrimary__20939 && (scrollController__21101 is not null)) ? PrimaryScrollController.CreateNone(child: scrollable__21221) : scrollable__21221);
        ScrollViewKeyboardDismissBehavior effectiveKeyboardDismissBehavior__22037 = this.keyboardDismissBehavior ?? ScrollViewKeyboardDismissBehavior.manual;
        if ((object.Equals(effectiveKeyboardDismissBehavior__22037, ScrollViewKeyboardDismissBehavior.onDrag)))
        {
            return ((Widget)(object?)new NotificationListener<ScrollUpdateNotification>(child: scrollableResult__21756, onNotification: ((global::System.Func<ScrollUpdateNotification, bool>?)((notification) => {
FocusScopeNode currentScope__22527 = ((FocusScopeNode)(object?)FocusScope.of(context));
if ((((((ScrollUpdateNotification)notification).dragDetails is not null) && !currentScope__22527.hasPrimaryFocus) && currentScope__22527.hasFocus))
{
    FocusManager.instance.primaryFocus?.unfocus();
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        }
        else
        {
            return scrollableResult__21756;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.Axis>("scrollDirection", this.scrollDirection));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("reverse", value: this.reverse, ifTrue: "reversed", showName: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ScrollController>("controller", this.controller, showName: false, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("primary", value: this.primary, ifTrue: "using primary controller", showName: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("physics", this.physics, showName: false, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("shrinkWrap", value: this.shrinkWrap, ifTrue: "shrink-wrapping", showName: true));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent>("scrollCacheExtent", this.scrollCacheExtent, defaultValue: null));
    }

}

public class CustomScrollView : ScrollView
{
    public virtual List<Widget> slivers { get; private set; } = default!;

    public CustomScrollView(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, ScrollBehavior? scrollBehavior = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Foundation.Key? center = null, double anchor = 0.0, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Generated.Framework.Rendering.SliverPaintOrder paintOrder = global::Doroti.Generated.Framework.Rendering.SliverPaintOrder.firstIsTop, List<Widget> slivers = default!, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque) : base(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, scrollBehavior: scrollBehavior, shrinkWrap: shrinkWrap, center: center, anchor: anchor, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, paintOrder: paintOrder, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior)
    {
        List<Widget> __slivers = slivers ?? new List<Widget>();
        this.slivers = __slivers;
    }

    public override List<Widget> buildSlivers(BuildContext context) => this.slivers;
}

public abstract class BoxScrollView : ScrollView
{
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }

    protected BoxScrollView(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque) : base(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior)
    {
        this.padding = padding;
    }

    public override List<Widget> buildSlivers(BuildContext context)
    {
        Widget sliver__37242 = ((Widget)(object?)buildChildLayout(context));
        global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? effectivePadding__37302 = this.padding;
        if ((this.padding is null))
        {
            MediaQueryData? mediaQuery__37385 = ((MediaQueryData?)(object?)MediaQuery.maybeOf(context));
            if ((mediaQuery__37385 is not null))
            {
                global::Doroti.Generated.Framework.Painting.EdgeInsets mediaQueryHorizontalPadding__37550 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)((MediaQueryData)mediaQuery__37385).padding.copyWith(top: 0.0, bottom: 0.0));
                global::Doroti.Generated.Framework.Painting.EdgeInsets mediaQueryVerticalPadding__37688 = ((global::Doroti.Generated.Framework.Painting.EdgeInsets)(object?)((MediaQueryData)mediaQuery__37385).padding.copyWith(left: 0.0, right: 0.0));
                effectivePadding__37302 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>(((object.Equals(this.scrollDirection, global::Doroti.Generated.Framework.Painting.Axis.vertical)) ? mediaQueryVerticalPadding__37688 : mediaQueryHorizontalPadding__37550));
                sliver__37242 = DartRuntimePrimitives.ConvertValue<Widget>(new MediaQuery(data: mediaQuery__37385.copyWith(padding: ((object.Equals(this.scrollDirection, global::Doroti.Generated.Framework.Painting.Axis.vertical)) ? mediaQueryHorizontalPadding__37550 : mediaQueryVerticalPadding__37688)), child: sliver__37242));
            }
        }
        if ((effectivePadding__37302 is not null))
        {
            sliver__37242 = DartRuntimePrimitives.ConvertValue<Widget>(new SliverPadding(padding: effectivePadding__37302, sliver: sliver__37242));
        }
        return new List<Widget> { sliver__37242 };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract Widget buildChildLayout(BuildContext context);
    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
    }

}

public class ListView : BoxScrollView
{
    public virtual double? itemExtent { get; private set; }
    public virtual ItemExtentBuilder? itemExtentBuilder { get; private set; }
    public virtual Widget? prototypeItem { get; private set; }
    public virtual SliverChildDelegate childrenDelegate { get; private set; } = default!;

    public ListView(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, Widget? prototypeItem = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> children = default!, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque) : base(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior, semanticChildCount: (semanticChildCount ?? checked((long)((children ?? new List<Widget>()).Count))))
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.itemExtent = itemExtent;
        this.itemExtentBuilder = itemExtentBuilder;
        this.prototypeItem = prototypeItem;
        this.childrenDelegate = new SliverChildListDelegate(children, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        System.Diagnostics.Debug.Assert((((((itemExtent is null) && (prototypeItem is null))) || (((itemExtent is null) && (itemExtentBuilder is null)))) || (((prototypeItem is null) && (itemExtentBuilder is null)))));
    }

    public static ListView CreateBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, Widget? prototypeItem = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        var __instance = new ListView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, itemExtent: itemExtent, itemExtentBuilder: itemExtentBuilder, prototypeItem: prototypeItem, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, children: new List<Widget>(), semanticChildCount: semanticChildCount ?? itemCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        __instance.itemExtent = itemExtent;
        __instance.itemExtentBuilder = itemExtentBuilder;
        __instance.prototypeItem = prototypeItem;
        __instance.childrenDelegate = new SliverChildBuilderDelegate((global::System.Func<BuildContext, long, Widget?>)itemBuilder, findChildIndexCallback: (global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>?)findChildIndexCallback, childCount: itemCount, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        return __instance;
    }

    public static ListView CreateSeparated(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findItemIndexCallback = null, global::System.Func<BuildContext, long, Widget> separatorBuilder = default!, long itemCount = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        var __instance = new ListView(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.itemExtent = null;
        __instance.itemExtentBuilder = null;
        __instance.prototypeItem = null;
        __instance.childrenDelegate = new SliverChildBuilderDelegate(((global::System.Func<BuildContext, long, Widget?>)((context, index) => {
long itemIndex__64623 = (checked((long)(index / 2L)));
if (((checked((long)(index)) & 1L) == 0L))
{
    return itemBuilder(context, itemIndex__64623);
}
return separatorBuilder(context, itemIndex__64623);
throw new InvalidOperationException("Dart closure completed without a value.");
})), findChildIndexCallback: ((global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>)((findItemIndexCallback is not null) ? ((key) => {
long? itemIndex__64930 = findItemIndexCallback(key);
return ((itemIndex__64930 is null) ? null : (DartRuntimePrimitives.RequireValue(itemIndex__64930) * 2L));
throw new InvalidOperationException("Dart closure completed without a value.");
}) : findChildIndexCallback)), childCount: ListView._computeActualChildCount(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(itemCount))), addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes, semanticIndexCallback: ((global::System.Func<Widget, long, long?>)((widget, index) => {
return (((checked((long)(index)) & 1L) == 0L) ? (checked((long)(index / 2L))) : null);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        return __instance;
    }

    public static ListView CreateCustom(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, double? itemExtent = null, Widget? prototypeItem = null, ItemExtentBuilder? itemExtentBuilder = null, SliverChildDelegate childrenDelegate = default!, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        var __instance = new ListView(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.itemExtent = itemExtent;
        __instance.prototypeItem = prototypeItem;
        __instance.itemExtentBuilder = itemExtentBuilder;
        __instance.childrenDelegate = childrenDelegate;
        return __instance;
    }

    public override Widget buildChildLayout(BuildContext context)
    {
        if ((this.itemExtent is not null))
        {
            double itemExtent__value70683 = DartRuntimePrimitives.RequireValue(itemExtent);
            return ((Widget)(object?)new SliverFixedExtentList(@delegate: this.childrenDelegate, itemExtent: DartRuntimePrimitives.RequireValue(this.itemExtent)));
        }
        else
        {
            if ((this.itemExtentBuilder is not null))
            {
                return ((Widget)(object?)new SliverVariedExtentList(@delegate: this.childrenDelegate, itemExtentBuilder: this.itemExtentBuilder!));
            }
            else
            {
                if ((this.prototypeItem is not null))
                {
                    return ((Widget)(object?)new SliverPrototypeExtentList(@delegate: this.childrenDelegate, prototypeItem: this.prototypeItem!));
                }
            }
        }
        return ((Widget)(object?)new SliverList(@delegate: this.childrenDelegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("itemExtent", this.itemExtent, defaultValue: null));
    }

    internal static long _computeActualChildCount(long itemCount)
    {
        return Math.Max(0L, ((DartRuntimePrimitives.RequireValue(itemCount) * 2L) - 1L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class GridView : BoxScrollView
{
    public virtual global::Doroti.Generated.Framework.Rendering.SliverGridDelegate gridDelegate { get; private set; } = default!;
    public virtual SliverChildDelegate childrenDelegate { get; private set; } = default!;

    public GridView(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Rendering.SliverGridDelegate gridDelegate = default!, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> children = default!, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, Clip clipBehavior = Clip.hardEdge, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque) : base(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, dragStartBehavior: dragStartBehavior, clipBehavior: clipBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, hitTestBehavior: hitTestBehavior, semanticChildCount: (semanticChildCount ?? checked((long)((children ?? new List<Widget>()).Count))))
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.gridDelegate = gridDelegate;
        this.childrenDelegate = new SliverChildListDelegate(children, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
    }

    public static GridView CreateBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Rendering.SliverGridDelegate gridDelegate = default!, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        __instance.gridDelegate = gridDelegate;
        __instance.childrenDelegate = new SliverChildBuilderDelegate((global::System.Func<BuildContext, long, Widget?>)itemBuilder, findChildIndexCallback: (global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>?)findChildIndexCallback, childCount: itemCount, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        return __instance;
    }

    public static GridView CreateCustom(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Rendering.SliverGridDelegate gridDelegate = default!, SliverChildDelegate childrenDelegate = default!, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        __instance.gridDelegate = gridDelegate;
        __instance.childrenDelegate = childrenDelegate;
        return __instance;
    }

    public static GridView CreateCount(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, long crossAxisCount = default!, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, double? mainAxisExtent = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> children = default!, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        List<Widget> __children = children ?? new List<Widget>();
        __instance.gridDelegate = new global::Doroti.Generated.Framework.Rendering.SliverGridDelegateWithFixedCrossAxisCount(crossAxisCount: crossAxisCount, mainAxisSpacing: mainAxisSpacing, crossAxisSpacing: crossAxisSpacing, childAspectRatio: childAspectRatio, mainAxisExtent: mainAxisExtent);
        __instance.childrenDelegate = new SliverChildListDelegate(children, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        return __instance;
    }

    public static GridView CreateExtent(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, double maxCrossAxisExtent = default!, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, double? mainAxisExtent = null, bool addAutomaticKeepAlives = true, bool addRepaintBoundaries = true, bool addSemanticIndexes = true, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, List<Widget> children = default!, long? semanticChildCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque)
    {
        var __instance = new GridView(key: key, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: primary, physics: physics, shrinkWrap: shrinkWrap, padding: padding, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, semanticChildCount: semanticChildCount, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior);
        List<Widget> __children = children ?? new List<Widget>();
        __instance.gridDelegate = new global::Doroti.Generated.Framework.Rendering.SliverGridDelegateWithMaxCrossAxisExtent(maxCrossAxisExtent: maxCrossAxisExtent, mainAxisSpacing: mainAxisSpacing, crossAxisSpacing: crossAxisSpacing, childAspectRatio: childAspectRatio, mainAxisExtent: mainAxisExtent);
        __instance.childrenDelegate = new SliverChildListDelegate(children, addAutomaticKeepAlives: addAutomaticKeepAlives, addRepaintBoundaries: addRepaintBoundaries, addSemanticIndexes: addSemanticIndexes);
        return __instance;
    }

    public override Widget buildChildLayout(BuildContext context)
    {
        return ((Widget)(object?)new SliverGrid(@delegate: this.childrenDelegate, gridDelegate: this.gridDelegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
