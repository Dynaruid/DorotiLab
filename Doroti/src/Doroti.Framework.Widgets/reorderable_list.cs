// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/reorderable_list.dart
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

public delegate void ReorderCallback(long oldIndex, long newIndex);

public delegate Widget ReorderItemProxyDecorator(Widget child, long index, global::Doroti.Generated.Framework.Animation.Animation<double> animation);

public delegate DragBoundaryDelegate<Rect>? ReorderDragBoundaryProvider(BuildContext context);

public class ReorderableList : StatefulWidget
{
    public virtual global::System.Func<BuildContext, long, Widget> itemBuilder { get; private set; } = default!;
    public virtual long itemCount { get; private set; } = default!;
    public virtual global::System.Action<long, long>? onReorder { get; private set; }
    public virtual global::System.Action<long, long>? onReorderItem { get; private set; }
    public virtual global::System.Action<long>? onReorderStart { get; private set; }
    public virtual global::System.Action<long>? onReorderEnd { get; private set; }
    public virtual global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>? proxyDecorator { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual bool shrinkWrap { get; private set; } = default!;
    public virtual double anchor { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double? itemExtent { get; private set; }
    public virtual ItemExtentBuilder? itemExtentBuilder { get; private set; }
    public virtual Widget? prototypeItem { get; private set; }
    public virtual double? autoScrollerVelocityScalar { get; private set; }
    public virtual global::System.Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider { get; private set; }

    public ReorderableList(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, Widget> itemBuilder = default!, long itemCount = default!, global::System.Action<long, long>? onReorder = null, global::System.Action<long, long>? onReorderItem = null, global::System.Action<long>? onReorderStart = null, global::System.Action<long>? onReorderEnd = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, Widget? prototypeItem = null, global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>? proxyDecorator = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, double anchor = 0.0, double? cacheExtent = null, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, double? autoScrollerVelocityScalar = null, global::System.Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider = null) : base(key: key)
    {
        this.itemBuilder = itemBuilder;
        this.itemCount = itemCount;
        this.onReorder = onReorder;
        this.onReorderItem = onReorderItem;
        this.onReorderStart = onReorderStart;
        this.onReorderEnd = onReorderEnd;
        this.itemExtent = itemExtent;
        this.itemExtentBuilder = itemExtentBuilder;
        this.prototypeItem = prototypeItem;
        this.proxyDecorator = proxyDecorator;
        this.padding = padding;
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.controller = controller;
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
        System.Diagnostics.Debug.Assert((itemCount >= 0L));
        System.Diagnostics.Debug.Assert((((((itemExtent is null) && (prototypeItem is null))) || (((itemExtent is null) && (itemExtentBuilder is null)))) || (((prototypeItem is null) && (itemExtentBuilder is null)))));
        System.Diagnostics.Debug.Assert(((((onReorderItem is not null) && (onReorder is null))) || (((onReorderItem is null) && (onReorder is not null)))));
    }

    public static ReorderableListState of(BuildContext context)
    {
        ReorderableListState? result__13995 = ((ReorderableListState?)(object?)context.findAncestorStateOfType<ReorderableListState>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__13995 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("ReorderableList.of() called with a context that does not contain a ReorderableList."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("No ReorderableList ancestor could be found starting from the context that was passed to ReorderableList.of()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("This can happen when the context provided is from the same StatefulWidget that " + "built the ReorderableList. Please see the ReorderableList documentation for examples " + "of how to refer to an ReorderableListState object:\n" + "  https://api.flutter.dev/flutter/widgets/ReorderableListState-class.html"), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__13995!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ReorderableListState? maybeOf(BuildContext context)
    {
        return ((ReorderableListState?)(object?)context.findAncestorStateOfType<ReorderableListState>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new ReorderableListState());
}

public class ReorderableListState : State<ReorderableList>
{
    internal virtual GlobalKey<SliverReorderableListState> _sliverReorderableListKey { get; private set; } = GlobalKey<SliverReorderableListState>.Create();

    internal virtual global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? _effectiveScrollCacheExtent
    {
        get
        {
            if ((((ReorderableList)this.widget).scrollCacheExtent is not null))
            {
                return ((ReorderableList)this.widget).scrollCacheExtent;
            }
            if ((((ReorderableList)this.widget).cacheExtent is not null))
            {
                return global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(((ReorderableList)this.widget).cacheExtent));
            }
            return ((global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent)(object)null);
            return default!;
        }
    }
    public virtual void startItemDragReorder(long index, global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event, global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer recognizer)
    {
        ((GlobalKey<SliverReorderableListState>)this._sliverReorderableListKey).currentState!.startItemDragReorder(index: index, @event: @event, recognizer: recognizer);
    }

    public virtual void cancelReorder()
    {
        ((GlobalKey<SliverReorderableListState>)this._sliverReorderableListKey).currentState!.cancelReorder();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new CustomScrollView(scrollDirection: ((ReorderableList)this.widget).scrollDirection, reverse: ((ReorderableList)this.widget).reverse, controller: ((ReorderableList)this.widget).controller, primary: ((ReorderableList)this.widget).primary, physics: ((ReorderableList)this.widget).physics, shrinkWrap: ((ReorderableList)this.widget).shrinkWrap, anchor: ((ReorderableList)this.widget).anchor, scrollCacheExtent: this._effectiveScrollCacheExtent, dragStartBehavior: ((ReorderableList)this.widget).dragStartBehavior, keyboardDismissBehavior: ((ReorderableList)this.widget).keyboardDismissBehavior, restorationId: ((ReorderableList)this.widget).restorationId, clipBehavior: ((ReorderableList)this.widget).clipBehavior, slivers: new List<Widget> { new SliverPadding(padding: (((ReorderableList)this.widget).padding ?? global::Doroti.Generated.Framework.Painting.EdgeInsets.zero), sliver: new SliverReorderableList(key: this._sliverReorderableListKey, itemExtent: ((ReorderableList)this.widget).itemExtent, prototypeItem: ((ReorderableList)this.widget).prototypeItem, itemBuilder: (global::System.Func<BuildContext, long, Widget>)((ReorderableList)this.widget).itemBuilder, itemExtentBuilder: (ItemExtentBuilder?)((ReorderableList)this.widget).itemExtentBuilder, itemCount: ((ReorderableList)this.widget).itemCount, onReorder: (global::System.Action<long, long>?)((ReorderableList)this.widget).onReorder, onReorderItem: (global::System.Action<long, long>?)((ReorderableList)this.widget).onReorderItem, onReorderStart: (global::System.Action<long>?)((ReorderableList)this.widget).onReorderStart, onReorderEnd: (global::System.Action<long>?)((ReorderableList)this.widget).onReorderEnd, proxyDecorator: (global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>?)((ReorderableList)this.widget).proxyDecorator, autoScrollerVelocityScalar: ((ReorderableList)this.widget).autoScrollerVelocityScalar, dragBoundaryProvider: (global::System.Func<BuildContext, DragBoundaryDelegate<Rect>?>?)((ReorderableList)this.widget).dragBoundaryProvider)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverReorderableList : StatefulWidget
{
    internal const double _kDefaultAutoScrollVelocityScalar = 50;
    public virtual global::System.Func<BuildContext, long, Widget> itemBuilder { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback { get; private set; }
    public virtual long itemCount { get; private set; } = default!;
    public virtual global::System.Action<long, long>? onReorder { get; private set; }
    public virtual global::System.Action<long, long>? onReorderItem { get; private set; }
    public virtual global::System.Action<long>? onReorderStart { get; private set; }
    public virtual global::System.Action<long>? onReorderEnd { get; private set; }
    public virtual global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>? proxyDecorator { get; private set; }
    public virtual double? itemExtent { get; private set; }
    public virtual ItemExtentBuilder? itemExtentBuilder { get; private set; }
    public virtual Widget? prototypeItem { get; private set; }
    public virtual double autoScrollerVelocityScalar { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider { get; private set; }

    public SliverReorderableList(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, Widget> itemBuilder = default!, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long itemCount = default!, global::System.Action<long, long>? onReorder = null, global::System.Action<long, long>? onReorderItem = null, global::System.Action<long>? onReorderStart = null, global::System.Action<long>? onReorderEnd = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, Widget? prototypeItem = null, global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>? proxyDecorator = null, global::System.Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider = null, double? autoScrollerVelocityScalar = null) : base(key: key)
    {
        this.itemBuilder = itemBuilder;
        this.findChildIndexCallback = findChildIndexCallback;
        this.itemCount = itemCount;
        this.onReorder = onReorder;
        this.onReorderItem = onReorderItem;
        this.onReorderStart = onReorderStart;
        this.onReorderEnd = onReorderEnd;
        this.itemExtent = itemExtent;
        this.itemExtentBuilder = itemExtentBuilder;
        this.prototypeItem = prototypeItem;
        this.proxyDecorator = proxyDecorator;
        this.dragBoundaryProvider = dragBoundaryProvider;
        this.autoScrollerVelocityScalar = (autoScrollerVelocityScalar ?? _kDefaultAutoScrollVelocityScalar);
        System.Diagnostics.Debug.Assert((itemCount >= 0L));
        System.Diagnostics.Debug.Assert((((((itemExtent is null) && (prototypeItem is null))) || (((itemExtent is null) && (itemExtentBuilder is null)))) || (((prototypeItem is null) && (itemExtentBuilder is null)))));
        System.Diagnostics.Debug.Assert(((((onReorderItem is not null) && (onReorder is null))) || (((onReorderItem is null) && (onReorder is not null)))));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new SliverReorderableListState());
    public static SliverReorderableListState of(BuildContext context)
    {
        SliverReorderableListState? result__24973 = ((SliverReorderableListState?)(object?)context.findAncestorStateOfType<SliverReorderableListState>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__24973 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("SliverReorderableList.of() called with a context that does not contain a SliverReorderableList."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("No SliverReorderableList ancestor could be found starting from the context that was passed to SliverReorderableList.of()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("This can happen when the context provided is from the same StatefulWidget that " + "built the SliverReorderableList. Please see the SliverReorderableList documentation for examples " + "of how to refer to an SliverReorderableList object:\n" + "  https://api.flutter.dev/flutter/widgets/SliverReorderableListState-class.html"), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__24973!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SliverReorderableListState? maybeOf(BuildContext context)
    {
        return ((SliverReorderableListState?)(object?)context.findAncestorStateOfType<SliverReorderableListState>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverReorderableListState : State<SliverReorderableList>, TickerProviderStateMixin<SliverReorderableList>
{
    internal virtual DartMap<long, _ReorderableItemState__reorderable_list> _items { get; private set; } = new DartMap<long, _ReorderableItemState__reorderable_list>();
    internal virtual OverlayEntry? _overlayEntry { get; set; } = default;
    internal virtual long? _dragIndex { get; set; } = default;
    internal virtual _DragInfo__reorderable_list? _dragInfo { get; set; } = default;
    internal virtual long? _insertIndex { get; set; } = default;
    internal virtual Offset? _finalDropPosition { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer? _recognizer { get; set; } = default;
    internal virtual long? _recognizerPointer { get; set; } = default;
    internal virtual EdgeDraggingAutoScroller? _autoScroller { get; set; } = default;
    internal virtual ScrollableState _scrollable { get; set; } = default!;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual global::Doroti.Generated.Framework.Painting.Axis _scrollDirection => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((ScrollableState)this._scrollable).axisDirection);
    internal virtual bool _reverse => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((ScrollableState)this._scrollable).axisDirection);
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _scrollable = Scrollable.of(this.context);
        if ((!object.Equals(this._autoScroller?.scrollable, this._scrollable)))
        {
            this._autoScroller?.stopAutoScroll();
            _autoScroller = new EdgeDraggingAutoScroller(this._scrollable, onScrollViewScrolled: () => this._handleScrollableAutoScrolled(), velocityScalar: ((SliverReorderableList)this.widget).autoScrollerVelocityScalar);
        }
    }

    public override void didUpdateWidget(SliverReorderableList oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((SliverReorderableList)this.widget).itemCount != ((SliverReorderableList)oldWidget).itemCount))
        {
            cancelReorder();
        }
        if ((((SliverReorderableList)this.widget).autoScrollerVelocityScalar != ((SliverReorderableList)oldWidget).autoScrollerVelocityScalar))
        {
            this._autoScroller?.stopAutoScroll();
            _autoScroller = new EdgeDraggingAutoScroller(this._scrollable, onScrollViewScrolled: () => this._handleScrollableAutoScrolled(), velocityScalar: ((SliverReorderableList)this.widget).autoScrollerVelocityScalar);
        }
    }

    public override void dispose()
    {
        _dragReset();
        this._recognizer?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual void startItemDragReorder(long index, global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event, global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer recognizer)
    {
        DartRuntimePrimitives.Assert(() => ((0L <= index) && (index < ((SliverReorderableList)this.widget).itemCount)));
        setState(((global::System.Action)(() => {
if ((this._dragInfo is not null))
{
    cancelReorder();
}
else
{
    if (((this._recognizer is not null) && (this._recognizerPointer != @event.pointer)))
    {
        this._recognizer!.dispose();
        _recognizer = null;
        _recognizerPointer = null;
    }
}
if (this._items.ContainsKey(index))
{
    _dragIndex = index;
    _recognizer = ((Func<global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer>)(() =>
{            var __cascade = recognizer;
            __cascade.onStart = this._dragStart;
            __cascade.addPointer(@event);
            return __cascade;        }))();
    _recognizerPointer = @event.pointer;
}
else
{
    throw new Exception("Attempting to start a drag on a non-visible item");
}
})));
    }

    public virtual void cancelReorder()
    {
        setState(((global::System.Action)(() => {
_dragReset();
})));
    }

    internal virtual void _registerItem(_ReorderableItemState__reorderable_list item)
    {
        if (((this._dragInfo is not null) && (!object.Equals(this._items.GetValueOrDefault(((_ReorderableItemState__reorderable_list)item).index), item))))
        {
            item.updateForGap(this._dragInfo!.index, this._dragInfo!.index, this._dragInfo!.itemExtent, false, this._reverse);
        }
        this._items[((_ReorderableItemState__reorderable_list)item).index] = item;
        if ((((_ReorderableItemState__reorderable_list)item).index == this._dragInfo?.index))
        {
            item.dragging = true;
            item.rebuild();
        }
    }

    internal virtual void _unregisterItem(long index, _ReorderableItemState__reorderable_list item)
    {
        _ReorderableItemState__reorderable_list? currentItem__31893 = this._items.GetValueOrDefault(index);
        if ((object.Equals(currentItem__31893, item)))
        {
            this._items.remove(index);
        }
    }

    internal virtual global::Doroti.Generated.Framework.Gestures.Drag? _dragStart(Offset position)
    {
        DartRuntimePrimitives.Assert(() => (this._dragInfo is null));
        _ReorderableItemState__reorderable_list item__32093 = this._items.GetValueOrDefault(DartRuntimePrimitives.RequireValue(this._dragIndex))!;
        item__32093.dragging = true;
        ((SliverReorderableList)this.widget).onReorderStart?.Invoke(DartRuntimePrimitives.RequireValue(this._dragIndex));
        item__32093.rebuild();
        _insertIndex = ((_ReorderableItemState__reorderable_list)item__32093).index;
        _dragInfo = new _DragInfo__reorderable_list(item: item__32093, initialPosition: position, scrollDirection: this._scrollDirection, onUpdate: (global::System.Action<_DragInfo__reorderable_list, Offset, Offset>)this._dragUpdate, onCancel: (global::System.Action<_DragInfo__reorderable_list>)this._dragCancel, onEnd: (global::System.Action<_DragInfo__reorderable_list>)this._dragEnd, onDropCompleted: () => this._dropCompleted(), proxyDecorator: (global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>?)((SliverReorderableList)this.widget).proxyDecorator, tickerProvider: this);
        this._dragInfo!.startDrag();
        OverlayState overlay__32617 = ((OverlayState)(object?)Overlay.of(this.context, debugRequiredFor: this.widget));
        DartRuntimePrimitives.Assert(() => (this._overlayEntry is null));
        _overlayEntry = new OverlayEntry(builder: (global::System.Func<BuildContext, Widget>)this._dragInfo!.createProxy);
        overlay__32617.insert(this._overlayEntry!);
        foreach (_ReorderableItemState__reorderable_list childItem__32850 in this._items.Values)
        {
            if (((object.Equals(childItem__32850, item__32093)) || !childItem__32850.mounted))
            {
                continue;
            }
            childItem__32850.updateForGap(DartRuntimePrimitives.RequireValue(this._insertIndex), DartRuntimePrimitives.RequireValue(this._insertIndex), this._dragInfo!.itemExtent, false, this._reverse);
        }
        return ((global::Doroti.Generated.Framework.Gestures.Drag?)(object?)this._dragInfo);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dragUpdate(_DragInfo__reorderable_list item, Offset position, Offset delta)
    {
        setState(((global::System.Action)(() => {
this._overlayEntry?.markNeedsBuild();
_dragUpdateItems();
this._autoScroller?.startAutoScrollIfNecessary(this._dragTargetRect);
})));
    }

    internal virtual void _dragCancel(_DragInfo__reorderable_list item)
    {
        setState(((global::System.Action)(() => {
_dragReset();
})));
    }

    internal virtual void _dragEnd(_DragInfo__reorderable_list item)
    {
        setState(((global::System.Action)(() => {
if (((DartRuntimePrimitives.RequireValue(this._insertIndex) - ((_DragInfo__reorderable_list)item).index) == 1L))
{
    _finalDropPosition = _itemOffsetAt((DartRuntimePrimitives.RequireValue(this._insertIndex) - 1L));
}
else
{
    if ((this._insertIndex == ((_DragInfo__reorderable_list)item).index))
    {
        _finalDropPosition = _itemOffsetAt(DartRuntimePrimitives.RequireValue(this._insertIndex));
    }
    else
    {
        if (this._reverse)
        {
            if ((DartRuntimePrimitives.RequireValue(this._insertIndex) >= checked((long)(this._items.Count))))
            {
                _finalDropPosition = (_itemOffsetAt((checked((long)(this._items.Count)) - 1L)) - Reorderable_listLibrary._extentOffset(((_DragInfo__reorderable_list)item).itemExtent, this._scrollDirection));
            }
            else
            {
                _finalDropPosition = (_itemOffsetAt(DartRuntimePrimitives.RequireValue(this._insertIndex)) + Reorderable_listLibrary._extentOffset(_itemExtentAt(DartRuntimePrimitives.RequireValue(this._insertIndex)), this._scrollDirection));
            }
        }
        else
        {
            if ((DartRuntimePrimitives.RequireValue(this._insertIndex) == 0L))
            {
                _finalDropPosition = (_itemOffsetAt(0L) - Reorderable_listLibrary._extentOffset(((_DragInfo__reorderable_list)item).itemExtent, this._scrollDirection));
            }
            else
            {
                long atIndex__34881 = (DartRuntimePrimitives.RequireValue(this._insertIndex) - 1L);
                _finalDropPosition = (_itemOffsetAt(atIndex__34881) + Reorderable_listLibrary._extentOffset(_itemExtentAt(atIndex__34881), this._scrollDirection));
            }
        }
    }
}
})));
        ((SliverReorderableList)this.widget).onReorderEnd?.Invoke(DartRuntimePrimitives.RequireValue(this._insertIndex));
    }

    internal virtual void _dropCompleted()
    {
        long oldIndex__35154 = DartRuntimePrimitives.RequireValue(this._dragIndex);
        long newIndex__35192 = DartRuntimePrimitives.RequireValue(this._insertIndex);
        _handleReorderItem(oldIndex__35154, newIndex__35192);
        setState(((global::System.Action)(() => {
_dragReset();
})));
    }

    internal virtual void _dragReset()
    {
        if ((this._dragInfo is not null))
        {
            if (((this._dragIndex is not null) && this._items.ContainsKey(DartRuntimePrimitives.RequireValue(this._dragIndex))))
            {
                _ReorderableItemState__reorderable_list dragItem__35468 = this._items.GetValueOrDefault(DartRuntimePrimitives.RequireValue(this._dragIndex))!;
                dragItem__35468._dragging = false;
                dragItem__35468.rebuild();
                _dragIndex = null;
            }
            this._dragInfo?.dispose();
            _dragInfo = null;
            this._autoScroller?.stopAutoScroll();
            _resetItemGap();
            this._recognizer?.dispose();
            _recognizer = null;
            this._overlayEntry?.remove();
            this._overlayEntry?.dispose();
            _overlayEntry = null;
            _finalDropPosition = null;
        }
    }

    internal virtual void _resetItemGap()
    {
        foreach (_ReorderableItemState__reorderable_list item__35967 in this._items.Values)
        {
            item__35967.resetGap();
        }
    }

    internal virtual void _handleReorderItem(long oldIndex, long newIndex)
    {
        if (((((SliverReorderableList)this.widget).onReorder is not null) && (oldIndex != newIndex)))
        {
            ((SliverReorderableList)this.widget).onReorder?.Invoke(oldIndex, newIndex);
            return;
        }
        if ((newIndex > oldIndex))
        {
            newIndex -= 1L;
        }
        if ((oldIndex != newIndex))
        {
            ((SliverReorderableList)this.widget).onReorderItem?.Invoke(oldIndex, newIndex);
        }
    }

    internal virtual void _handleScrollableAutoScrolled()
    {
        if ((this._dragInfo is null))
        {
            return;
        }
        _dragUpdateItems();
        this._autoScroller?.startAutoScrollIfNecessary(this._dragTargetRect);
    }

    internal virtual void _dragUpdateItems()
    {
        DartRuntimePrimitives.Assert(() => (this._dragInfo is not null));
        double gapExtent__36755 = this._dragInfo!.itemExtent;
        double proxyItemStart__36807 = Reorderable_listLibrary._offsetExtent((this._dragInfo!.dragPosition - this._dragInfo!.dragOffset), this._scrollDirection);
        double proxyItemEnd__36942 = (proxyItemStart__36807 + gapExtent__36755);
        long newIndex__37058 = DartRuntimePrimitives.RequireValue(this._insertIndex);
        foreach (_ReorderableItemState__reorderable_list item__37121 in this._items.Values)
        {
            if ((((this._reverse && (((_ReorderableItemState__reorderable_list)item__37121).index == DartRuntimePrimitives.RequireValue(this._dragIndex)))) || !item__37121.mounted))
            {
                continue;
            }
            global::Doroti.Ui.Rect geometry__37260 = ((global::Doroti.Ui.Rect)(object?)item__37121.targetGeometry());
            double itemStart__37313 = ((object.Equals(this._scrollDirection, global::Doroti.Generated.Framework.Painting.Axis.vertical)) ? geometry__37260.top : geometry__37260.left);
            double itemExtent__37410 = ((object.Equals(this._scrollDirection, global::Doroti.Generated.Framework.Painting.Axis.vertical)) ? geometry__37260.height : geometry__37260.width);
            double itemEnd__37532 = (itemStart__37313 + itemExtent__37410);
            double itemMiddle__37585 = (itemStart__37313 + (itemExtent__37410 / 2L));
            if (this._reverse)
            {
                if (((itemEnd__37532 >= proxyItemEnd__36942) && (proxyItemEnd__36942 >= itemMiddle__37585)))
                {
                    newIndex__37058 = ((_ReorderableItemState__reorderable_list)item__37121).index;
                    break;
                }
                else
                {
                    if (((itemMiddle__37585 >= proxyItemStart__36807) && (proxyItemStart__36807 >= itemStart__37313)))
                    {
                        newIndex__37058 = (((_ReorderableItemState__reorderable_list)item__37121).index + 1L);
                        break;
                    }
                    else
                    {
                        if (((itemStart__37313 > proxyItemEnd__36942) && (newIndex__37058 < ((((_ReorderableItemState__reorderable_list)item__37121).index + 1L)))))
                        {
                            newIndex__37058 = (((_ReorderableItemState__reorderable_list)item__37121).index + 1L);
                        }
                        else
                        {
                            if (((proxyItemStart__36807 > itemEnd__37532) && (newIndex__37058 > ((_ReorderableItemState__reorderable_list)item__37121).index)))
                            {
                                newIndex__37058 = ((_ReorderableItemState__reorderable_list)item__37121).index;
                            }
                        }
                    }
                }
            }
            else
            {
                if ((((_ReorderableItemState__reorderable_list)item__37121).index == DartRuntimePrimitives.RequireValue(this._dragIndex)))
                {
                    if (((itemMiddle__37585 <= proxyItemEnd__36942) && (proxyItemEnd__36942 <= itemEnd__37532)))
                    {
                        newIndex__37058 = DartRuntimePrimitives.RequireValue(this._dragIndex);
                    }
                }
                else
                {
                    if (((itemStart__37313 <= proxyItemStart__36807) && (proxyItemStart__36807 <= itemMiddle__37585)))
                    {
                        newIndex__37058 = ((_ReorderableItemState__reorderable_list)item__37121).index;
                        break;
                    }
                    else
                    {
                        if (((itemMiddle__37585 <= proxyItemEnd__36942) && (proxyItemEnd__36942 <= itemEnd__37532)))
                        {
                            newIndex__37058 = (((_ReorderableItemState__reorderable_list)item__37121).index + 1L);
                            break;
                        }
                        else
                        {
                            if (((itemEnd__37532 < proxyItemStart__36807) && (newIndex__37058 < ((((_ReorderableItemState__reorderable_list)item__37121).index + 1L)))))
                            {
                                newIndex__37058 = (((_ReorderableItemState__reorderable_list)item__37121).index + 1L);
                            }
                            else
                            {
                                if (((proxyItemEnd__36942 < itemStart__37313) && (newIndex__37058 > ((_ReorderableItemState__reorderable_list)item__37121).index)))
                                {
                                    newIndex__37058 = ((_ReorderableItemState__reorderable_list)item__37121).index;
                                }
                            }
                        }
                    }
                }
            }
        }
        if ((newIndex__37058 != this._insertIndex))
        {
            _insertIndex = newIndex__37058;
            foreach (_ReorderableItemState__reorderable_list item__39775 in this._items.Values)
            {
                if (((((_ReorderableItemState__reorderable_list)item__39775).index == DartRuntimePrimitives.RequireValue(this._dragIndex)) || !item__39775.mounted))
                {
                    continue;
                }
                item__39775.updateForGap(DartRuntimePrimitives.RequireValue(this._dragIndex), newIndex__37058, gapExtent__36755, true, this._reverse);
            }
        }
    }

    internal virtual global::Doroti.Ui.Rect _dragTargetRect
    {
        get
        {
            global::Doroti.Ui.Offset origin__40030 = ((global::Doroti.Ui.Offset)(object?)(this._dragInfo!.dragPosition - this._dragInfo!.dragOffset));
            return global::Doroti.Ui.Rect.fromLTWH(origin__40030.dx, origin__40030.dy, this._dragInfo!.itemSize.width, this._dragInfo!.itemSize.height);
            return default!;
        }
    }
    internal virtual global::Doroti.Ui.Offset _itemOffsetAt(long index)
    {
        return this._items.GetValueOrDefault(index)!.targetGeometry().topLeft;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _itemExtentAt(long index)
    {
        return Reorderable_listLibrary._sizeExtent(this._items.GetValueOrDefault(index)!.targetGeometry().size, this._scrollDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _itemBuilder(BuildContext context, long index)
    {
        if (((this._dragInfo is not null) && (index >= ((SliverReorderableList)this.widget).itemCount)))
        {
            return ((Widget)(object?)(this._scrollDirection switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new SizedBox(width: this._dragInfo!.itemExtent), global::Doroti.Generated.Framework.Painting.Axis.vertical => new SizedBox(height: this._dragInfo!.itemExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        }
        Widget child__40762 = this.widget.itemBuilder(context, index);
        DartRuntimePrimitives.Assert(() => (((Widget)child__40762).key is not null), () => (object?)"All list items must have a key");
        OverlayState overlay__40894 = ((OverlayState)(object?)Overlay.of(context, debugRequiredFor: this.widget));
        return ((Widget)(object?)new _ReorderableItem__reorderable_list( _ReorderableItemGlobalKey__reorderable_list.Create(key: ((Widget)child__40762).key!, index: index, state: this), index: index, capturedThemes: InheritedTheme.capture(from: context, to: overlay__40894.context), child: _wrapWithSemantics(child__40762, index)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _wrapWithSemantics(Widget child, long index)
    {
        var semanticsActions__41324 = new DartMap<global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction, global::System.Action>();
        void moveToStart()
        {
            _handleReorderItem(index, 0L);
        }
        void moveToEnd()
        {
            _handleReorderItem(index, ((SliverReorderableList)this.widget).itemCount);
        }
        void moveBefore()
        {
            _handleReorderItem(index, (index - 1L));
        }
        void moveAfter()
        {
            _handleReorderItem(index, (index + 2L));
        }
        WidgetsLocalizations localizations__41845 = ((WidgetsLocalizations)(object?)WidgetsLocalizations.of(this.context));
        var isHorizontal__41905 = (object.Equals(this._scrollDirection, global::Doroti.Generated.Framework.Painting.Axis.horizontal));
        if ((index > 0L))
        {
            semanticsActions__41324[new global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction(label: ((WidgetsLocalizations)localizations__41845).reorderItemToStart)] = (global::System.Action)moveToStart;
            string reorderItemBefore__42175 = ((WidgetsLocalizations)localizations__41845).reorderItemUp;
            if (isHorizontal__41905)
            {
                reorderItemBefore__42175 = ((object.Equals(Directionality.of(this.context), TextDirection.ltr)) ? ((WidgetsLocalizations)localizations__41845).reorderItemLeft : ((WidgetsLocalizations)localizations__41845).reorderItemRight);
            }
            semanticsActions__41324[new global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction(label: reorderItemBefore__42175)] = (global::System.Action)moveBefore;
        }
        if ((index < (((SliverReorderableList)this.widget).itemCount - 1L)))
        {
            string reorderItemAfter__42641 = ((WidgetsLocalizations)localizations__41845).reorderItemDown;
            if (isHorizontal__41905)
            {
                reorderItemAfter__42641 = ((object.Equals(Directionality.of(this.context), TextDirection.ltr)) ? ((WidgetsLocalizations)localizations__41845).reorderItemRight : ((WidgetsLocalizations)localizations__41845).reorderItemLeft);
            }
            semanticsActions__41324[new global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction(label: reorderItemAfter__42641)] = (global::System.Action)moveAfter;
            semanticsActions__41324[new global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction(label: ((WidgetsLocalizations)localizations__41845).reorderItemToEnd)] = (global::System.Action)moveToEnd;
        }
        return ((Widget)(object?)new Semantics(container: true, customSemanticsActions: (DartMap<global::Doroti.Generated.Framework.Semantics.CustomSemanticsAction, global::System.Action>)semanticsActions__41324, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasOverlay(context));
        var childrenDelegate__43618 = new SliverChildBuilderDelegate((global::System.Func<BuildContext, long, Widget>)this._itemBuilder, childCount: ((SliverReorderableList)this.widget).itemCount, findChildIndexCallback: (global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>?)((SliverReorderableList)this.widget).findChildIndexCallback);
        if ((((SliverReorderableList)this.widget).itemExtent is not null))
        {
            return ((Widget)(object?)new SliverFixedExtentList(@delegate: childrenDelegate__43618, itemExtent: DartRuntimePrimitives.RequireValue(((SliverReorderableList)this.widget).itemExtent)));
        }
        else
        {
            if ((((SliverReorderableList)this.widget).itemExtentBuilder is not null))
            {
                return ((Widget)(object?)new SliverVariedExtentList(@delegate: childrenDelegate__43618, itemExtentBuilder: ((SliverReorderableList)this.widget).itemExtentBuilder!));
            }
            else
            {
                if ((((SliverReorderableList)this.widget).prototypeItem is not null))
                {
                    return ((Widget)(object?)new SliverPrototypeExtentList(@delegate: childrenDelegate__43618, prototypeItem: ((SliverReorderableList)this.widget).prototypeItem!));
                }
            }
        }
        return ((Widget)(object?)new SliverList(@delegate: childrenDelegate__43618));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public class _ReorderableItem__reorderable_list : StatefulWidget
{
    public virtual long index { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual CapturedThemes capturedThemes { get; private set; } = default!;

    internal _ReorderableItem__reorderable_list(global::Doroti.Generated.Framework.Foundation.Key key, long index, Widget child, CapturedThemes capturedThemes) : base(key: key)
    {
        this.index = index;
        this.child = child;
        this.capturedThemes = capturedThemes;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _ReorderableItemState__reorderable_list());
}

public class _ReorderableItemState__reorderable_list : State<_ReorderableItem__reorderable_list>
{
    internal virtual SliverReorderableListState _listState { get; set; } = default!;
    internal virtual Offset _startOffset { get; set; } = Offset.zero;
    internal virtual Offset _targetOffset { get; set; } = Offset.zero;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController? _offsetAnimation { get; set; } = default;
    internal virtual bool _dragging { get; set; } = false;

    public virtual global::Doroti.Generated.Framework.Foundation.Key key => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Foundation.Key>(this.widget.key!);
    public virtual long index => ((_ReorderableItem__reorderable_list)this.widget).index;
    public virtual bool dragging
    {
        get => this._dragging;
        set
        {
            var dragging = value;
            if (this.mounted)
            {
                setState(((global::System.Action)(() => {
_dragging = dragging;
})));
            }
        }
    }
    public override void initState()
    {
        _listState = SliverReorderableList.of(this.context);
        this._listState._registerItem(this);
        base.initState();
    }

    public override void dispose()
    {
        this._offsetAnimation?.dispose();
        this._listState._unregisterItem(this.index, this);
        base.dispose();
    }

    public override void didUpdateWidget(_ReorderableItem__reorderable_list oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((_ReorderableItem__reorderable_list)oldWidget).index != ((_ReorderableItem__reorderable_list)this.widget).index))
        {
            this._listState._unregisterItem(((_ReorderableItem__reorderable_list)oldWidget).index, this);
            this._listState._registerItem(this);
        }
    }

    public override Widget build(BuildContext context)
    {
        if (this._dragging)
        {
            global::Doroti.Ui.Size size__45815 = ((global::Doroti.Ui.Size)(object?)Reorderable_listLibrary._extentSize(((SliverReorderableListState)this._listState)._dragInfo!.itemExtent, ((SliverReorderableListState)this._listState)._scrollDirection));
            return ((Widget)(object?)SizedBox.CreateFromSize(size: size__45815));
        }
        this._listState._registerItem(this);
        return ((Widget)(object?)Transform.CreateTranslate(offset: this.offset, child: ((_ReorderableItem__reorderable_list)this.widget).child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void deactivate()
    {
        this._listState._unregisterItem(this.index, this);
        base.deactivate();
    }

    public virtual global::Doroti.Ui.Offset offset
    {
        get
        {
            if ((this._offsetAnimation is not null))
            {
                double animValue__46243 = global::Doroti.Generated.Framework.Animation.Curves.easeInOut.transform(this._offsetAnimation!.value);
                return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(this._startOffset, this._targetOffset, animValue__46243));
            }
            return this._targetOffset;
            return default!;
        }
    }
    public virtual void updateForGap(long dragIndex, long gapIndex, double gapExtent, bool animate, bool reverse)
    {
        global::Doroti.Ui.Offset newTargetOffset__46748 = default!;
        if ((((gapIndex < dragIndex) && (this.index < dragIndex)) && (this.index >= gapIndex)))
        {
            newTargetOffset__46748 = Reorderable_listLibrary._extentOffset((reverse ? -gapExtent : gapExtent), ((SliverReorderableListState)this._listState)._scrollDirection);
        }
        else
        {
            if ((((gapIndex > dragIndex) && (this.index > dragIndex)) && (this.index < gapIndex)))
            {
                newTargetOffset__46748 = Reorderable_listLibrary._extentOffset((reverse ? gapExtent : -gapExtent), ((SliverReorderableListState)this._listState)._scrollDirection);
            }
            else
            {
                newTargetOffset__46748 = Offset.zero;
            }
        }
        if ((!object.Equals(newTargetOffset__46748, this._targetOffset)))
        {
            global::Doroti.Ui.Offset previousTarget__47292 = ((global::Doroti.Ui.Offset)(object?)this._targetOffset);
            _targetOffset = newTargetOffset__46748;
            if (animate)
            {
                if ((this._offsetAnimation is null))
                {
                    _offsetAnimation = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.AnimationController(vsync: this._listState, duration: Duration.Create(milliseconds: 250L));
            __cascade.addListener(() => this.rebuild());
            __cascade.addStatusListener(((AnimationStatusListener)((status) => {
if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isCompleted(status))
{
    _startOffset = this._targetOffset;
    this._offsetAnimation!.dispose();
    _offsetAnimation = null;
}
})));
            __cascade.forward();
            return __cascade;        }))();
                }
                else
                {
                    double currentAnimValue__48037 = global::Doroti.Generated.Framework.Animation.Curves.easeInOut.transform(this._offsetAnimation!.value);
                    global::Doroti.Ui.Offset currentPosition__48132 = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(this._startOffset, previousTarget__47292, currentAnimValue__48037)));
                    _startOffset = currentPosition__48132;
                    this._offsetAnimation!.forward(from: 0.0);
                }
            }
            else
            {
                if ((this._offsetAnimation is not null))
                {
                    this._offsetAnimation!.dispose();
                    _offsetAnimation = null;
                }
                _startOffset = this._targetOffset;
            }
            rebuild();
        }
    }

    public virtual void resetGap()
    {
        if ((this._offsetAnimation is not null))
        {
            this._offsetAnimation!.dispose();
            _offsetAnimation = null;
        }
        _startOffset = Offset.zero;
        _targetOffset = Offset.zero;
        rebuild();
    }

    public virtual global::Doroti.Ui.Rect targetGeometry()
    {
        var itemRenderBox__48823 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)this.context.findRenderObject()!)!;
        global::Doroti.Ui.Offset itemPosition__48898 = ((global::Doroti.Ui.Offset)(object?)(((Offset)((dynamic)itemRenderBox__48823).localToGlobal(Offset.zero)) + this._targetOffset));
        return (itemPosition__48898 & ((global::Doroti.Generated.Framework.Rendering.RenderBox)itemRenderBox__48823).size);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void rebuild()
    {
        if (this.mounted)
        {
            setState(((global::System.Action)(() => {
})));
        }
    }

}

public class ReorderableDragStartListener : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    public ReorderableDragStartListener(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, long index = default!, bool enabled = true) : base(key: key)
    {
        this.child = child;
        this.index = index;
        this.enabled = enabled;
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Listener(onPointerDown: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerDownEvent>)(this.enabled ? ((@event) => { _startDragging(context, @event); }) : null)), child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer createRecognizer()
    {
        return ((global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer)(object?)new global::Doroti.Generated.Framework.Gestures.ImmediateMultiDragGestureRecognizer(debugOwner: this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _startDragging(BuildContext context, global::Doroti.Generated.Framework.Gestures.PointerDownEvent @event)
    {
        global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings? gestureSettings__51361 = ((global::Doroti.Generated.Framework.Gestures.DeviceGestureSettings?)(object?)MediaQuery.maybeGestureSettingsOf(context));
        SliverReorderableListState? list__51461 = ((SliverReorderableListState?)(object?)SliverReorderableList.maybeOf(context));
        list__51461?.startItemDragReorder(index: this.index, @event: @event, recognizer: ((Func<global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer>)(() =>
{            var __cascade = createRecognizer();
            __cascade.gestureSettings = gestureSettings__51361;
            return __cascade;        }))());
    }

}

public class ReorderableDelayedDragStartListener : ReorderableDragStartListener
{
    public ReorderableDelayedDragStartListener(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, long index = default!, bool enabled = true) : base(key: key, child: child, index: index, enabled: enabled)
    {
    }

    public override global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer createRecognizer()
    {
        return ((global::Doroti.Generated.Framework.Gestures.MultiDragGestureRecognizer)(object?)new global::Doroti.Generated.Framework.Gestures.DelayedMultiDragGestureRecognizer(debugOwner: this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _DragItemUpdate__reorderable_list(_DragInfo__reorderable_list item, Offset position, Offset delta);

internal delegate void _DragItemCallback__reorderable_list(_DragInfo__reorderable_list item);

internal class _DragInfo__reorderable_list : global::Doroti.Generated.Framework.Gestures.Drag
{
    public virtual global::Doroti.Generated.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual global::System.Action<_DragInfo__reorderable_list, Offset, Offset>? onUpdate { get; private set; }
    public virtual global::System.Action<_DragInfo__reorderable_list>? onEnd { get; private set; }
    public virtual global::System.Action<_DragInfo__reorderable_list>? onCancel { get; private set; }
    public virtual global::System.Action? onDropCompleted { get; private set; }
    public virtual global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>? proxyDecorator { get; private set; }
    public virtual global::Doroti.Generated.Framework.Scheduler.TickerProvider tickerProvider { get; private set; } = default!;
    public virtual DragBoundaryDelegate<Rect>? boundary { get; set; } = default;
    public virtual SliverReorderableListState listState { get; set; } = default!;
    public virtual long index { get; set; } = default!;
    public virtual Widget child { get; set; } = default!;
    public virtual Offset dragPosition { get; set; } = default!;
    public virtual Offset dragOffset { get; set; } = default!;
    public virtual Size itemSize { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints itemLayoutConstraints { get; set; } = default!;
    public virtual double itemExtent { get; set; } = default!;
    public virtual CapturedThemes capturedThemes { get; set; } = default!;
    public virtual ScrollableState? scrollable { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController? _proxyAnimation { get; set; } = default;
    internal virtual Offset _rawDragPosition { get; set; } = default!;

    internal _DragInfo__reorderable_list(_ReorderableItemState__reorderable_list item, Offset initialPosition = default, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.vertical, global::System.Action<_DragInfo__reorderable_list, Offset, Offset>? onUpdate = null, global::System.Action<_DragInfo__reorderable_list>? onEnd = null, global::System.Action<_DragInfo__reorderable_list>? onCancel = null, global::System.Action? onDropCompleted = null, global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>? proxyDecorator = null, global::Doroti.Generated.Framework.Scheduler.TickerProvider tickerProvider = default!)
    {
        this.scrollDirection = scrollDirection;
        this.onUpdate = onUpdate;
        this.onEnd = onEnd;
        this.onCancel = onCancel;
        this.onDropCompleted = onDropCompleted;
        this.proxyDecorator = proxyDecorator;
        this.tickerProvider = tickerProvider;
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._proxyAnimation?.dispose();
    }

    public virtual void startDrag()
    {
        _proxyAnimation = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Animation.AnimationController(vsync: this.tickerProvider, duration: Duration.Create(milliseconds: 250L));
            __cascade.addStatusListener(((AnimationStatusListener)((status) => {
if (global::Doroti.Generated.Framework.Animation.AnimationStatusMembers.isDismissed(status))
{
    _dropCompleted();
}
})));
            __cascade.forward();
            return __cascade;        }))();
    }

    public override void update(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details)
    {
        global::Doroti.Ui.Offset delta__55389 = ((global::Doroti.Ui.Offset)(object?)Reorderable_listLibrary._restrictAxis(((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta, this.scrollDirection));
        _rawDragPosition += delta__55389;
        dragPosition = _adjustedDragOffset(this._rawDragPosition);
        this.onUpdate?.Invoke(this, this.dragPosition, ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).delta);
    }

    public override void end(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        this._proxyAnimation!.reverse();
        this.onEnd?.Invoke(this);
    }

    public override void cancel()
    {
        this._proxyAnimation?.dispose();
        _proxyAnimation = null;
        this.onCancel?.Invoke(this);
    }

    internal virtual global::Doroti.Ui.Offset _adjustedDragOffset(Offset offset)
    {
        if ((this.boundary is null))
        {
            return offset;
        }
        global::Doroti.Ui.Offset adjOffset__55941 = ((global::Doroti.Ui.Offset)(object?)this.boundary!.nearestPositionWithinBoundary((((offset - this.dragOffset)) & this.itemSize)).shift(this.dragOffset).topLeft);
        return adjOffset__55941;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dropCompleted()
    {
        this._proxyAnimation?.dispose();
        _proxyAnimation = null;
        this.onDropCompleted?.Invoke();
    }

    public virtual Widget createProxy(BuildContext context)
    {
        return ((Widget)(object?)this.capturedThemes.wrap(new _DragItemProxy__reorderable_list(listState: this.listState, index: this.index, size: this.itemSize, constraints: this.itemLayoutConstraints, animation: this._proxyAnimation!, position: ((this.dragPosition - this.dragOffset) - Reorderable_listLibrary._overlayOrigin(context)), proxyDecorator: (global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>?)this.proxyDecorator, child: this.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Reorderable_listLibrary
{
    internal static Offset _overlayOrigin(BuildContext context)
    {
        OverlayState overlay__56707 = ((OverlayState)(object?)Overlay.of(context, debugRequiredFor: ((BuildContext)context).widget));
        var overlayBox__56780 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)overlay__56707.context.findRenderObject()!)!;
        return ((Offset)((dynamic)overlayBox__56780).localToGlobal(Offset.zero));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _DragItemProxy__reorderable_list : StatelessWidget
{
    public virtual SliverReorderableListState listState { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual Offset position { get; private set; } = default!;
    public virtual Size size { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController animation { get; private set; } = default!;
    public virtual global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>? proxyDecorator { get; private set; }

    internal _DragItemProxy__reorderable_list(SliverReorderableListState listState, long index, Widget child, Offset position, Size size, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, global::Doroti.Generated.Framework.Animation.AnimationController animation, global::System.Func<Widget, long, global::Doroti.Generated.Framework.Animation.Animation<double>, Widget>? proxyDecorator)
    {
        this.listState = listState;
        this.index = index;
        this.child = child;
        this.position = position;
        this.size = size;
        this.constraints = constraints;
        this.animation = animation;
        this.proxyDecorator = proxyDecorator;
    }

    public override Widget build(BuildContext context)
    {
        Widget proxyChild__57524 = ((this.proxyDecorator is null ? this.child : this.proxyDecorator.Invoke(this.child, this.index, ((global::Doroti.Generated.Framework.Animation.AnimationController)this.animation).view)));
        global::Doroti.Ui.Offset overlayOrigin__57615 = ((global::Doroti.Ui.Offset)(object?)Reorderable_listLibrary._overlayOrigin(context));
        return ((Widget)(object?)new MediaQuery(data: MediaQuery.of(context).removePadding(removeTop: true), child: new AnimatedBuilder(animation: this.animation, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) => {
global::Doroti.Ui.Offset effectivePosition__58017 = ((global::Doroti.Ui.Offset)(object?)this.position);
global::Doroti.Ui.Offset? dropPosition__58071 = ((global::Doroti.Ui.Offset?)(object?)((SliverReorderableListState)this.listState)._finalDropPosition);
if ((dropPosition__58071 is not null))
{
    Offset dropPosition__58071__value58130 = DartRuntimePrimitives.RequireValue(dropPosition__58071);
    effectivePosition__58017 = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp((DartRuntimePrimitives.RequireValue(dropPosition__58071__value58130) - overlayOrigin__57615), effectivePosition__58017, global::Doroti.Generated.Framework.Animation.Curves.easeOut.transform(((global::Doroti.Generated.Framework.Animation.AnimationController)this.animation).value)));
}
return ((Widget)(object?)new Positioned(left: effectivePosition__58017.dx, top: effectivePosition__58017.dy, child: new SizedBox(width: this.size.width, height: this.size.height, child: new OverflowBox(minWidth: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).minWidth, minHeight: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).minHeight, maxWidth: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxWidth, maxHeight: ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).maxHeight, alignment: ((object.Equals(((SliverReorderableListState)this.listState)._scrollDirection, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? global::Doroti.Generated.Framework.Painting.Alignment.centerLeft : global::Doroti.Generated.Framework.Painting.Alignment.topCenter), child: child))));
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: proxyChild__57524)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Reorderable_listLibrary
{
    internal static double _sizeExtent(Size size, global::Doroti.Generated.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => size.width, global::Doroti.Generated.Framework.Painting.Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Reorderable_listLibrary
{
    internal static Size _extentSize(double extent, global::Doroti.Generated.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Size(extent, 0), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Size(0, extent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Reorderable_listLibrary
{
    internal static double _offsetExtent(Offset offset, global::Doroti.Generated.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => offset.dx, global::Doroti.Generated.Framework.Painting.Axis.vertical => offset.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Reorderable_listLibrary
{
    internal static Offset _extentOffset(double extent, global::Doroti.Generated.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(extent, 0.0), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(0.0, extent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Reorderable_listLibrary
{
    internal static Offset _restrictAxis(Offset offset, global::Doroti.Generated.Framework.Painting.Axis scrollDirection)
    {
        return (scrollDirection switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(offset.dx, 0.0), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(0.0, offset.dy), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ReorderableItemGlobalKey__reorderable_list : GlobalObjectKey<IState>
{
    public virtual global::Doroti.Generated.Framework.Foundation.Key subKey { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;
    public virtual SliverReorderableListState state { get; private set; } = default!;

    internal _ReorderableItemGlobalKey__reorderable_list(global::Doroti.Generated.Framework.Foundation.Key subKey, long index, SliverReorderableListState state) : base(subKey)
    {
        this.subKey = subKey;
        this.index = index;
        this.state = state;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ReorderableItemGlobalKey__reorderable_list;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((__other is _ReorderableItemGlobalKey__reorderable_list) && (object.Equals(((_ReorderableItemGlobalKey__reorderable_list)((_ReorderableItemGlobalKey__reorderable_list)__other)).subKey, this.subKey))) && (((_ReorderableItemGlobalKey__reorderable_list)((_ReorderableItemGlobalKey__reorderable_list)__other)).index == this.index)) && (object.Equals(((_ReorderableItemGlobalKey__reorderable_list)((_ReorderableItemGlobalKey__reorderable_list)__other)).state, this.state)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.subKey, this.index, this.state));
    internal static _ReorderableItemGlobalKey__reorderable_list Create(global::Doroti.Generated.Framework.Foundation.Key key, long index, SliverReorderableListState state) => new(key, index, state);
}

