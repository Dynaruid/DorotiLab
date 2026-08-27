// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/reorderable_list.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;

public class ReorderableListView : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget> itemBuilder { get; private set; } = default!;
    public virtual long itemCount { get; private set; } = default!;
    public virtual global::System.Action<long, long>? onReorder { get; private set; }
    public virtual global::System.Action<long, long>? onReorderItem { get; private set; }
    public virtual global::System.Action<long>? onReorderStart { get; private set; }
    public virtual global::System.Action<long>? onReorderEnd { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.Widget, long, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>? proxyDecorator { get; private set; }
    public virtual bool buildDefaultDragHandles { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? header { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? footer { get; private set; }
    public virtual global::Doroti.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ScrollController? scrollController { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual global::Doroti.Framework.Widgets.ScrollPhysics? physics { get; private set; }
    public virtual bool shrinkWrap { get; private set; } = default!;
    public virtual double anchor { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double? itemExtent { get; private set; }
    public virtual ItemExtentBuilder? itemExtentBuilder { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? prototypeItem { get; private set; }
    public virtual double? autoScrollerVelocityScalar { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.DragBoundaryDelegate<Rect>?>? dragBoundaryProvider { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    public ReorderableListView(global::Doroti.Framework.Foundation.Key? key = null, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::System.Action<long, long>? onReorder = null, global::System.Action<long, long>? onReorderItem = null, global::System.Action<long>? onReorderStart = null, global::System.Action<long>? onReorderEnd = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, global::Doroti.Framework.Widgets.Widget? prototypeItem = null, global::System.Func<global::Doroti.Framework.Widgets.Widget, long, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>? proxyDecorator = null, bool buildDefaultDragHandles = true, global::Doroti.Framework.Painting.EdgeInsets? padding = null, global::Doroti.Framework.Widgets.Widget? header = null, global::Doroti.Framework.Widgets.Widget? footer = null, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.vertical, bool reverse = false, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, bool? primary = null, global::Doroti.Framework.Widgets.ScrollPhysics? physics = null, bool shrinkWrap = false, double anchor = 0.0, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::Doroti.Framework.Widgets.ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, double? autoScrollerVelocityScalar = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.DragBoundaryDelegate<Rect>?>? dragBoundaryProvider = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
    {
        this.onReorder = onReorder;
        this.onReorderItem = onReorderItem;
        this.onReorderStart = onReorderStart;
        this.onReorderEnd = onReorderEnd;
        this.itemExtent = itemExtent;
        this.itemExtentBuilder = itemExtentBuilder;
        this.prototypeItem = prototypeItem;
        this.proxyDecorator = proxyDecorator;
        this.buildDefaultDragHandles = buildDefaultDragHandles;
        this.padding = padding;
        this.header = header;
        this.footer = footer;
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.scrollController = scrollController;
        this.primary = primary;
        this.physics = physics;
        this.shrinkWrap = shrinkWrap;
        this.anchor = anchor;
        this.cacheExtent = cacheExtent;
        this.scrollCacheExtent = scrollCacheExtent;
        this.dragStartBehavior = dragStartBehavior;
        this.keyboardDismissBehavior = keyboardDismissBehavior;
        this.restorationId = restorationId;
        this.clipBehavior = clipBehavior;
        this.autoScrollerVelocityScalar = autoScrollerVelocityScalar;
        this.dragBoundaryProvider = dragBoundaryProvider;
        this.mouseCursor = mouseCursor;
        this.itemBuilder = (((context, index) => children[(int)(index)]));
        this.itemCount = checked((long)(children.Count));
        System.Diagnostics.Debug.Assert((((((itemExtent is null) && (prototypeItem is null))) || (((itemExtent is null) && (itemExtentBuilder is null)))) || (((prototypeItem is null) && (itemExtentBuilder is null)))));
        System.Diagnostics.Debug.Assert(children.All(((w) => (((global::Doroti.Framework.Widgets.Widget)w).key is not null))));
        System.Diagnostics.Debug.Assert(((((onReorderItem is not null) && (onReorder is null))) || (((onReorderItem is null) && (onReorder is not null)))));
    }

    public static ReorderableListView CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget> itemBuilder = default!, long itemCount = default!, global::System.Action<long, long>? onReorder = null, global::System.Action<long, long>? onReorderItem = null, global::System.Action<long>? onReorderStart = null, global::System.Action<long>? onReorderEnd = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, global::Doroti.Framework.Widgets.Widget? prototypeItem = null, global::System.Func<global::Doroti.Framework.Widgets.Widget, long, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>? proxyDecorator = null, bool buildDefaultDragHandles = true, global::Doroti.Framework.Painting.EdgeInsets? padding = null, global::Doroti.Framework.Widgets.Widget? header = null, global::Doroti.Framework.Widgets.Widget? footer = null, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.vertical, bool reverse = false, global::Doroti.Framework.Widgets.ScrollController? scrollController = null, bool? primary = null, global::Doroti.Framework.Widgets.ScrollPhysics? physics = null, bool shrinkWrap = false, double anchor = 0.0, double? cacheExtent = null, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::Doroti.Framework.Widgets.ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, double? autoScrollerVelocityScalar = null, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.DragBoundaryDelegate<Rect>?>? dragBoundaryProvider = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null)
    {
        var __instance = new ReorderableListView(key: key, children: default!, onReorder: onReorder, onReorderItem: onReorderItem, onReorderStart: onReorderStart, onReorderEnd: onReorderEnd, itemExtent: itemExtent, itemExtentBuilder: itemExtentBuilder, prototypeItem: prototypeItem, proxyDecorator: proxyDecorator, buildDefaultDragHandles: buildDefaultDragHandles, padding: padding, header: header, footer: footer, scrollDirection: scrollDirection, reverse: reverse, scrollController: scrollController, primary: primary, physics: physics, shrinkWrap: shrinkWrap, anchor: anchor, cacheExtent: cacheExtent, scrollCacheExtent: scrollCacheExtent, dragStartBehavior: dragStartBehavior, keyboardDismissBehavior: keyboardDismissBehavior, restorationId: restorationId, clipBehavior: clipBehavior, autoScrollerVelocityScalar: autoScrollerVelocityScalar, dragBoundaryProvider: dragBoundaryProvider, mouseCursor: mouseCursor);
        __instance.itemBuilder = itemBuilder;
        __instance.itemCount = itemCount;
        __instance.onReorder = onReorder;
        __instance.onReorderItem = onReorderItem;
        __instance.onReorderStart = onReorderStart;
        __instance.onReorderEnd = onReorderEnd;
        __instance.itemExtent = itemExtent;
        __instance.itemExtentBuilder = itemExtentBuilder;
        __instance.prototypeItem = prototypeItem;
        __instance.proxyDecorator = proxyDecorator;
        __instance.buildDefaultDragHandles = buildDefaultDragHandles;
        __instance.padding = padding;
        __instance.header = header;
        __instance.footer = footer;
        __instance.scrollDirection = scrollDirection;
        __instance.reverse = reverse;
        __instance.scrollController = scrollController;
        __instance.primary = primary;
        __instance.physics = physics;
        __instance.shrinkWrap = shrinkWrap;
        __instance.anchor = anchor;
        __instance.cacheExtent = cacheExtent;
        __instance.scrollCacheExtent = scrollCacheExtent;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.keyboardDismissBehavior = keyboardDismissBehavior;
        __instance.restorationId = restorationId;
        __instance.clipBehavior = clipBehavior;
        __instance.autoScrollerVelocityScalar = autoScrollerVelocityScalar;
        __instance.dragBoundaryProvider = dragBoundaryProvider;
        __instance.mouseCursor = mouseCursor;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ReorderableListViewState__reorderable_list());
}

internal class _ReorderableListViewState__reorderable_list : global::Doroti.Framework.Widgets.State<ReorderableListView>
{
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _dragging { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<bool>(false);

    internal virtual global::Doroti.Framework.Widgets.Widget _itemBuilder(global::Doroti.Framework.Widgets.BuildContext context, long index)
    {
        global::Doroti.Framework.Widgets.Widget item = this.widget.itemBuilder(context, index);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((global::Doroti.Framework.Widgets.Widget)item).key is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("Every item of ReorderableListView must have a key."));
                }
                return true;
            });
        global::Doroti.Framework.Foundation.Key itemGlobalKey = ((global::Doroti.Framework.Foundation.Key)(object?)new _ReorderableListViewChildGlobalKey__reorderable_list(((global::Doroti.Framework.Widgets.Widget)item).key!, this));
        if (((ReorderableListView)this.widget).buildDefaultDragHandles)
        {
            switch (Theme.of(context).platform)
            {
                case global::Doroti.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Framework.Foundation.TargetPlatform.windows:
                case global::Doroti.Framework.Foundation.TargetPlatform.macOS:
                    {
                        var dragHandle = new global::Doroti.Framework.Widgets.ListenableBuilder(listenable: this._dragging, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
                        {
                            global::Doroti.Framework.Services.MouseCursor effectiveMouseCursor = ((global::Doroti.Framework.Services.MouseCursor)(object?)WidgetStateProperty.resolveAs<global::Doroti.Framework.Services.MouseCursor>((((ReorderableListView)this.widget).mouseCursor ?? global::Doroti.Framework.Widgets.WidgetStateMouseCursor.CreateFromMap(new DartMap<global::Doroti.Framework.Widgets.WidgetStatesConstraint, global::Doroti.Framework.Services.MouseCursor> { [global::Doroti.Framework.Widgets.WidgetState.dragged.asConstraint()] = ((global::Doroti.Framework.Services.MouseCursor)(object?)global::Doroti.Framework.Services.SystemMouseCursors.grabbing), [global::Doroti.Framework.Widgets.WidgetStateMembers.any] = ((global::Doroti.Framework.Services.MouseCursor)(object?)global::Doroti.Framework.Services.SystemMouseCursors.grab) })), ((Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>>)(() => { var __collection15120 = new HashSet<global::Doroti.Framework.Widgets.WidgetState>(); if (((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this._dragging).value) { __collection15120.Add(global::Doroti.Framework.Widgets.WidgetState.dragged); } return __collection15120; }))()));
                            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.MouseRegion(cursor: effectiveMouseCursor, child: child));
                            throw new InvalidOperationException("Dart closure completed without a value.");
                        })), child: new global::Doroti.Framework.Widgets.Icon(Icons.drag_handle));
                        switch (((ReorderableListView)this.widget).scrollDirection)
                        {
                            case global::Doroti.Framework.Painting.Axis.horizontal:
                                {
                                    return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.Stack.Create(key: itemGlobalKey, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(item), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Positioned.CreateDirectional(textDirection: Directionality.of(context), start: 0, end: 0, bottom: 8, child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.bottomCenter, child: new global::Doroti.Framework.Widgets.ReorderableDragStartListener(index: index, child: dragHandle)))) }));
                                }
                            case global::Doroti.Framework.Painting.Axis.vertical:
                                {
                                    return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.Stack.Create(key: itemGlobalKey, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(item), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(global::Doroti.Framework.Widgets.Positioned.CreateDirectional(textDirection: Directionality.of(context), top: 0, bottom: 0, end: 8, child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerEnd, child: new global::Doroti.Framework.Widgets.ReorderableDragStartListener(index: index, child: dragHandle)))) }));
                                }
                            default:
                                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                        }
                        break;
                    }
                case global::Doroti.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Framework.Foundation.TargetPlatform.fuchsia:
                    {
                        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ReorderableDelayedDragStartListener(key: itemGlobalKey, index: index, child: item));
                    }
            }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.KeyedSubtree(key: itemGlobalKey, child: item));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _proxyDecorator(global::Doroti.Framework.Widgets.Widget child, long index, global::Doroti.Framework.Animation.Animation<double> animation)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: animation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            double animValue = global::Doroti.Framework.Animation.Curves.easeInOut.transform(((global::Doroti.Framework.Animation.Animation<double>)animation).value);
            double elevationLocal = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(0L, 6L, animValue));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(elevation: elevationLocal, child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._dragging.dispose();
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasOverlay(context));
        global::Doroti.Framework.Painting.EdgeInsets paddingLocal = (((ReorderableListView)this.widget).padding ?? global::Doroti.Framework.Painting.EdgeInsets.zero);
        double? start = ((((ReorderableListView)this.widget).header is null) ? null : 0.0);
        double? end = ((((ReorderableListView)this.widget).footer is null) ? null : 0.0);
        if (((ReorderableListView)this.widget).reverse)
        {
            DartRuntimePrimitives.Ignore((start, end) = (end, start));
        }
        global::Doroti.Framework.Painting.EdgeInsets startPadding = default!;
        global::Doroti.Framework.Painting.EdgeInsets endPadding = default!;
        global::Doroti.Framework.Painting.EdgeInsets listPadding = default!;
        DartRuntimePrimitives.Ignore((startPadding, endPadding, listPadding) = (((ReorderableListView)this.widget).scrollDirection switch { global::Doroti.Framework.Painting.Axis.horizontal or global::Doroti.Framework.Painting.Axis.vertical when ((((start ?? end)) is null)) => (((global::Doroti.Framework.Painting.EdgeInsets, global::Doroti.Framework.Painting.EdgeInsets, global::Doroti.Framework.Painting.EdgeInsets))((global::Doroti.Framework.Painting.EdgeInsets.zero, global::Doroti.Framework.Painting.EdgeInsets.zero, paddingLocal))), global::Doroti.Framework.Painting.Axis.horizontal => (((global::Doroti.Framework.Painting.EdgeInsets, global::Doroti.Framework.Painting.EdgeInsets, global::Doroti.Framework.Painting.EdgeInsets))((paddingLocal.copyWith(left: 0), paddingLocal.copyWith(right: 0), paddingLocal.copyWith(left: start, right: end)))), global::Doroti.Framework.Painting.Axis.vertical => (((global::Doroti.Framework.Painting.EdgeInsets, global::Doroti.Framework.Painting.EdgeInsets, global::Doroti.Framework.Painting.EdgeInsets))((paddingLocal.copyWith(top: 0), paddingLocal.copyWith(bottom: 0), paddingLocal.copyWith(top: start, bottom: end)))), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        var (headerPadding, footerPadding) = (((ReorderableListView)this.widget).reverse ? (((global::Doroti.Framework.Painting.EdgeInsets, global::Doroti.Framework.Painting.EdgeInsets))(startPadding, endPadding)) : (((global::Doroti.Framework.Painting.EdgeInsets, global::Doroti.Framework.Painting.EdgeInsets))(endPadding, startPadding)));
        global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtentLocal = (((ReorderableListView)this.widget).scrollCacheExtent ?? (((((ReorderableListView)this.widget).cacheExtent is null) ? null : global::Doroti.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(((ReorderableListView)this.widget).cacheExtent)))));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CustomScrollView(scrollDirection: ((ReorderableListView)this.widget).scrollDirection, reverse: ((ReorderableListView)this.widget).reverse, controller: ((ReorderableListView)this.widget).scrollController, primary: ((ReorderableListView)this.widget).primary, physics: ((ReorderableListView)this.widget).physics, shrinkWrap: ((ReorderableListView)this.widget).shrinkWrap, anchor: ((ReorderableListView)this.widget).anchor, scrollCacheExtent: scrollCacheExtentLocal, dragStartBehavior: ((ReorderableListView)this.widget).dragStartBehavior, keyboardDismissBehavior: ((ReorderableListView)this.widget).keyboardDismissBehavior, restorationId: ((ReorderableListView)this.widget).restorationId, clipBehavior: ((ReorderableListView)this.widget).clipBehavior, slivers: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection19473 = new List<global::Doroti.Framework.Widgets.Widget>(); if ((((ReorderableListView)this.widget).header is not null)) { __collection19473.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SliverPadding(padding: headerPadding, sliver: new global::Doroti.Framework.Widgets.SliverToBoxAdapter(child: ((ReorderableListView)this.widget).header)))); }
            __collection19473.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SliverPadding(padding: listPadding, sliver: new global::Doroti.Framework.Widgets.SliverReorderableList(itemBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget>)this._itemBuilder, itemExtent: ((ReorderableListView)this.widget).itemExtent, itemExtentBuilder: (ItemExtentBuilder?)((ReorderableListView)this.widget).itemExtentBuilder, prototypeItem: ((ReorderableListView)this.widget).prototypeItem, itemCount: ((ReorderableListView)this.widget).itemCount, onReorder: (global::System.Action<long, long>?)((ReorderableListView)this.widget).onReorder, onReorderItem: (global::System.Action<long, long>?)((ReorderableListView)this.widget).onReorderItem, onReorderStart: ((global::System.Action<long>)((index) =>
            {
                this._dragging.value = true;
                ((ReorderableListView)this.widget).onReorderStart?.Invoke(index);
            })), onReorderEnd: ((global::System.Action<long>)((index) =>
            {
                this._dragging.value = false;
                ((ReorderableListView)this.widget).onReorderEnd?.Invoke(index);
            })), proxyDecorator: ((((ReorderableListView)this.widget).proxyDecorator ?? (global::System.Func<global::Doroti.Framework.Widgets.Widget, long, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.Widget>)this._proxyDecorator)), autoScrollerVelocityScalar: ((ReorderableListView)this.widget).autoScrollerVelocityScalar, dragBoundaryProvider: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.DragBoundaryDelegate<Rect>?>?)((ReorderableListView)this.widget).dragBoundaryProvider)))); if ((((ReorderableListView)this.widget).footer is not null)) { __collection19473.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SliverPadding(padding: footerPadding, sliver: new global::Doroti.Framework.Widgets.SliverToBoxAdapter(child: ((ReorderableListView)this.widget).footer)))); }
            return __collection19473;
        }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ReorderableListViewChildGlobalKey__reorderable_list : global::Doroti.Framework.Widgets.GlobalObjectKey<IState>
{
    public virtual global::Doroti.Framework.Foundation.Key subKey { get; private set; } = default!;
    public virtual IState state { get; private set; } = default!;

    internal _ReorderableListViewChildGlobalKey__reorderable_list(global::Doroti.Framework.Foundation.Key subKey, IState state) : base(subKey)
    {
        this.subKey = subKey;
        this.state = state;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ReorderableListViewChildGlobalKey__reorderable_list;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is _ReorderableListViewChildGlobalKey__reorderable_list) && (object.Equals(((_ReorderableListViewChildGlobalKey__reorderable_list)((_ReorderableListViewChildGlobalKey__reorderable_list)__other)).subKey, this.subKey))) && (object.Equals(((_ReorderableListViewChildGlobalKey__reorderable_list)((_ReorderableListViewChildGlobalKey__reorderable_list)__other)).state, this.state)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.subKey, this.state));
}
