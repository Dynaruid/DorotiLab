// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/reorderable_list.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate void ReorderCallback(long oldIndex, long newIndex);

public delegate Widget ReorderItemProxyDecorator(Widget child, long index, Animation<double> animation);

public delegate DragBoundaryDelegate<Rect>? ReorderDragBoundaryProvider(BuildContext context);

public class ReorderableList : StatefulWidget
{
    public virtual Func<BuildContext, long, Widget> itemBuilder { get; private set; } = default!;
    public virtual long itemCount { get; private set; } = default!;
    public virtual Action<long, long>? onReorder { get; private set; }
    public virtual Action<long, long>? onReorderItem { get; private set; }
    public virtual Action<long>? onReorderStart { get; private set; }
    public virtual Action<long>? onReorderEnd { get; private set; }
    public virtual Func<Widget, long, Animation<double>, Widget>? proxyDecorator { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual bool shrinkWrap { get; private set; } = default!;
    public virtual double anchor { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double? itemExtent { get; private set; }
    public virtual ItemExtentBuilder? itemExtentBuilder { get; private set; }
    public virtual Widget? prototypeItem { get; private set; }
    public virtual double? autoScrollerVelocityScalar { get; private set; }
    public virtual Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider { get; private set; }

    public ReorderableList(Key? key = null, Func<BuildContext, long, Widget> itemBuilder = default!, long itemCount = default!, Action<long, long>? onReorder = null, Action<long, long>? onReorderItem = null, Action<long>? onReorderStart = null, Action<long>? onReorderEnd = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, Widget? prototypeItem = null, Func<Widget, long, Animation<double>, Widget>? proxyDecorator = null, EdgeInsetsGeometry? padding = null, Axis scrollDirection = Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, double anchor = 0.0, double? cacheExtent = null, ScrollCacheExtent? scrollCacheExtent = null, DragStartBehavior dragStartBehavior = DragStartBehavior.start, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, double? autoScrollerVelocityScalar = null, Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider = null) : base(key: key)
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
        System.Diagnostics.Debug.Assert(itemCount >= 0L);
        System.Diagnostics.Debug.Assert((itemExtent is null) && (prototypeItem is null) || (itemExtent is null) && (itemExtentBuilder is null) || (prototypeItem is null) && (itemExtentBuilder is null));
        System.Diagnostics.Debug.Assert((onReorderItem is not null) && (onReorder is null) || (onReorderItem is null) && (onReorder is not null));
    }

    public static ReorderableListState of(BuildContext context)
    {
        ReorderableListState? result = context.findAncestorStateOfType<ReorderableListState>();
        DartRuntimePrimitives.Assert(() =>
            {
                if (result is null)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("ReorderableList.of() called with a context that does not contain a ReorderableList."), new ErrorDescription("No ReorderableList ancestor could be found starting from the context that was passed to ReorderableList.of()."), new ErrorHint("This can happen when the context provided is from the same StatefulWidget that " + "built the ReorderableList. Please see the ReorderableList documentation for examples " + "of how to refer to an ReorderableListState object:\n" + "  https://api.flutter.dev/flutter/widgets/ReorderableListState-class.html"), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ReorderableListState? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<ReorderableListState>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new ReorderableListState());
}

public class ReorderableListState : State<ReorderableList>
{
    internal virtual GlobalKey<SliverReorderableListState> _sliverReorderableListKey { get; private set; } = GlobalKey<SliverReorderableListState>.Create();

    internal virtual ScrollCacheExtent? _effectiveScrollCacheExtent
    {
        get
        {
            if (widget.scrollCacheExtent is not null)
            {
                return widget.scrollCacheExtent;
            }
            if (widget.cacheExtent is not null)
            {
                return ScrollCacheExtent.CreatePixels(DartRuntimePrimitives.RequireValue(widget.cacheExtent));
            }
            return null;
        }
    }
    public virtual void startItemDragReorder(long index, Gestures.PointerDownEvent @event, MultiDragGestureRecognizer recognizer)
    {
        _sliverReorderableListKey.currentState!.startItemDragReorder(index: index, @event: @event, recognizer: recognizer);
    }

    public virtual void cancelReorder()
    {
        _sliverReorderableListKey.currentState!.cancelReorder();
    }

    public override Widget build(BuildContext context)
    {
        return new CustomScrollView(scrollDirection: widget.scrollDirection, reverse: widget.reverse, controller: widget.controller, primary: widget.primary, physics: widget.physics, shrinkWrap: widget.shrinkWrap, anchor: widget.anchor, scrollCacheExtent: _effectiveScrollCacheExtent, dragStartBehavior: widget.dragStartBehavior, keyboardDismissBehavior: widget.keyboardDismissBehavior, restorationId: widget.restorationId, clipBehavior: widget.clipBehavior, slivers: new List<Widget> { new SliverPadding(padding: widget.padding ?? EdgeInsets.zero, sliver: new SliverReorderableList(key: _sliverReorderableListKey, itemExtent: widget.itemExtent, prototypeItem: widget.prototypeItem, itemBuilder: widget.itemBuilder, itemExtentBuilder: widget.itemExtentBuilder, itemCount: widget.itemCount, onReorder: widget.onReorder, onReorderItem: widget.onReorderItem, onReorderStart: widget.onReorderStart, onReorderEnd: widget.onReorderEnd, proxyDecorator: widget.proxyDecorator, autoScrollerVelocityScalar: widget.autoScrollerVelocityScalar, dragBoundaryProvider: widget.dragBoundaryProvider)) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverReorderableList : StatefulWidget
{
    internal const double _kDefaultAutoScrollVelocityScalar = 50;
    public virtual Func<BuildContext, long, Widget> itemBuilder { get; private set; } = default!;
    public virtual Func<Key, long?>? findChildIndexCallback { get; private set; }
    public virtual long itemCount { get; private set; } = default!;
    public virtual Action<long, long>? onReorder { get; private set; }
    public virtual Action<long, long>? onReorderItem { get; private set; }
    public virtual Action<long>? onReorderStart { get; private set; }
    public virtual Action<long>? onReorderEnd { get; private set; }
    public virtual Func<Widget, long, Animation<double>, Widget>? proxyDecorator { get; private set; }
    public virtual double? itemExtent { get; private set; }
    public virtual ItemExtentBuilder? itemExtentBuilder { get; private set; }
    public virtual Widget? prototypeItem { get; private set; }
    public virtual double autoScrollerVelocityScalar { get; private set; } = default!;
    public virtual Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider { get; private set; }

    public SliverReorderableList(Key? key = null, Func<BuildContext, long, Widget> itemBuilder = default!, Func<Key, long?>? findChildIndexCallback = null, long itemCount = default!, Action<long, long>? onReorder = null, Action<long, long>? onReorderItem = null, Action<long>? onReorderStart = null, Action<long>? onReorderEnd = null, double? itemExtent = null, ItemExtentBuilder? itemExtentBuilder = null, Widget? prototypeItem = null, Func<Widget, long, Animation<double>, Widget>? proxyDecorator = null, Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider = null, double? autoScrollerVelocityScalar = null) : base(key: key)
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
        this.autoScrollerVelocityScalar = autoScrollerVelocityScalar ?? _kDefaultAutoScrollVelocityScalar;
        System.Diagnostics.Debug.Assert(itemCount >= 0L);
        System.Diagnostics.Debug.Assert((itemExtent is null) && (prototypeItem is null) || (itemExtent is null) && (itemExtentBuilder is null) || (prototypeItem is null) && (itemExtentBuilder is null));
        System.Diagnostics.Debug.Assert((onReorderItem is not null) && (onReorder is null) || (onReorderItem is null) && (onReorder is not null));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new SliverReorderableListState());
    public static SliverReorderableListState of(BuildContext context)
    {
        SliverReorderableListState? result = context.findAncestorStateOfType<SliverReorderableListState>();
        DartRuntimePrimitives.Assert(() =>
            {
                if (result is null)
                {
                    throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("SliverReorderableList.of() called with a context that does not contain a SliverReorderableList."), new ErrorDescription("No SliverReorderableList ancestor could be found starting from the context that was passed to SliverReorderableList.of()."), new ErrorHint("This can happen when the context provided is from the same StatefulWidget that " + "built the SliverReorderableList. Please see the SliverReorderableList documentation for examples " + "of how to refer to an SliverReorderableList object:\n" + "  https://api.flutter.dev/flutter/widgets/SliverReorderableListState-class.html"), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SliverReorderableListState? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<SliverReorderableListState>();
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
    internal virtual MultiDragGestureRecognizer? _recognizer { get; set; } = default;
    internal virtual long? _recognizerPointer { get; set; } = default;
    internal virtual EdgeDraggingAutoScroller? _autoScroller { get; set; } = default;
    internal virtual ScrollableState _scrollable { get; set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual Axis _scrollDirection => Basic_typesLibrary.axisDirectionToAxis(_scrollable.axisDirection);
    internal virtual bool _reverse => Basic_typesLibrary.axisDirectionIsReversed(_scrollable.axisDirection);
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _scrollable = Scrollable.of(context);
        if (!Equals(_autoScroller?.scrollable, _scrollable))
        {
            _autoScroller?.stopAutoScroll();
            _autoScroller = new EdgeDraggingAutoScroller(_scrollable, onScrollViewScrolled: () => _handleScrollableAutoScrolled(), velocityScalar: widget.autoScrollerVelocityScalar);
        }
    }

    public override void didUpdateWidget(SliverReorderableList oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (widget.itemCount != oldWidget.itemCount)
        {
            cancelReorder();
        }
        if (widget.autoScrollerVelocityScalar != oldWidget.autoScrollerVelocityScalar)
        {
            _autoScroller?.stopAutoScroll();
            _autoScroller = new EdgeDraggingAutoScroller(_scrollable, onScrollViewScrolled: () => _handleScrollableAutoScrolled(), velocityScalar: widget.autoScrollerVelocityScalar);
        }
    }

    public override void dispose()
    {
        _dragReset();
        _recognizer?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual void startItemDragReorder(long index, Gestures.PointerDownEvent @event, MultiDragGestureRecognizer recognizer)
    {
        DartRuntimePrimitives.Assert(() => (0L <= index) && (index < widget.itemCount));
        setState(() =>
        {
            if (_dragInfo is not null)
            {
                cancelReorder();
            }
            else
            {
                if ((_recognizer is not null) && (_recognizerPointer != @event.pointer))
                {
                    _recognizer!.dispose();
                    _recognizer = null;
                    _recognizerPointer = null;
                }
            }
            if (_items.ContainsKey(index))
            {
                _dragIndex = index;
                _recognizer = ((Func<MultiDragGestureRecognizer>)(() =>
            {
                var __cascade = recognizer;
                __cascade.onStart = _dragStart;
                __cascade.addPointer(@event);
                return __cascade;
            }))();
                _recognizerPointer = @event.pointer;
            }
            else
            {
                throw new Exception("Attempting to start a drag on a non-visible item");
            }
        });
    }

    public virtual void cancelReorder()
    {
        setState(() =>
        {
            _dragReset();
        });
    }

    internal virtual void _registerItem(_ReorderableItemState__reorderable_list item)
    {
        if ((_dragInfo is not null) && (!Equals(_items.GetValueOrDefault(item.index), item)))
        {
            item.updateForGap(_dragInfo!.index, _dragInfo!.index, _dragInfo!.itemExtent, false, _reverse);
        }
        _items[item.index] = item;
        if (item.index == _dragInfo?.index)
        {
            item.dragging = true;
            item.rebuild();
        }
    }

    internal virtual void _unregisterItem(long index, _ReorderableItemState__reorderable_list item)
    {
        _ReorderableItemState__reorderable_list? currentItem = _items.GetValueOrDefault(index);
        if (Equals(currentItem, item))
        {
            _items.remove(index);
        }
    }

    internal virtual Drag? _dragStart(Offset position)
    {
        DartRuntimePrimitives.Assert(() => _dragInfo is null);
        _ReorderableItemState__reorderable_list itemLocal = _items.GetValueOrDefault(DartRuntimePrimitives.RequireValue(_dragIndex))!;
        itemLocal.dragging = true;
        widget.onReorderStart?.Invoke(DartRuntimePrimitives.RequireValue(_dragIndex));
        itemLocal.rebuild();
        _insertIndex = itemLocal.index;
        _dragInfo = new _DragInfo__reorderable_list(item: itemLocal, initialPosition: position, scrollDirection: _scrollDirection, onUpdate: _dragUpdate, onCancel: _dragCancel, onEnd: _dragEnd, onDropCompleted: () => _dropCompleted(), proxyDecorator: widget.proxyDecorator, tickerProvider: this);
        _dragInfo!.startDrag();
        OverlayState overlay = Overlay.of(context, debugRequiredFor: widget);
        DartRuntimePrimitives.Assert(() => _overlayEntry is null);
        _overlayEntry = new OverlayEntry(builder: _dragInfo!.createProxy);
        overlay.insert(_overlayEntry!);
        foreach (_ReorderableItemState__reorderable_list childItem in _items.Values)
        {
            if (Equals(childItem, itemLocal) || !childItem.mounted)
            {
                continue;
            }
            childItem.updateForGap(DartRuntimePrimitives.RequireValue(_insertIndex), DartRuntimePrimitives.RequireValue(_insertIndex), _dragInfo!.itemExtent, false, _reverse);
        }
        return (Drag?)_dragInfo;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dragUpdate(_DragInfo__reorderable_list item, Offset position, Offset delta)
    {
        setState(() =>
        {
            _overlayEntry?.markNeedsBuild();
            _dragUpdateItems();
            _autoScroller?.startAutoScrollIfNecessary(_dragTargetRect);
        });
    }

    internal virtual void _dragCancel(_DragInfo__reorderable_list item)
    {
        setState(() =>
        {
            _dragReset();
        });
    }

    internal virtual void _dragEnd(_DragInfo__reorderable_list item)
    {
        setState(() =>
        {
            if ((DartRuntimePrimitives.RequireValue(_insertIndex) - item.index) == 1L)
            {
                _finalDropPosition = _itemOffsetAt(DartRuntimePrimitives.RequireValue(_insertIndex) - 1L);
            }
            else
            {
                if (_insertIndex == item.index)
                {
                    _finalDropPosition = _itemOffsetAt(DartRuntimePrimitives.RequireValue(_insertIndex));
                }
                else
                {
                    if (_reverse)
                    {
                        if (DartRuntimePrimitives.RequireValue(_insertIndex) >= checked(_items.Count))
                        {
                            _finalDropPosition = _itemOffsetAt(checked(_items.Count) - 1L) - Reorderable_listLibrary._extentOffset(item.itemExtent, _scrollDirection);
                        }
                        else
                        {
                            _finalDropPosition = _itemOffsetAt(DartRuntimePrimitives.RequireValue(_insertIndex)) + Reorderable_listLibrary._extentOffset(_itemExtentAt(DartRuntimePrimitives.RequireValue(_insertIndex)), _scrollDirection);
                        }
                    }
                    else
                    {
                        if (DartRuntimePrimitives.RequireValue(_insertIndex) == 0L)
                        {
                            _finalDropPosition = _itemOffsetAt(0L) - Reorderable_listLibrary._extentOffset(item.itemExtent, _scrollDirection);
                        }
                        else
                        {
                            long atIndex = DartRuntimePrimitives.RequireValue(_insertIndex) - 1L;
                            _finalDropPosition = _itemOffsetAt(atIndex) + Reorderable_listLibrary._extentOffset(_itemExtentAt(atIndex), _scrollDirection);
                        }
                    }
                }
            }
        });
        widget.onReorderEnd?.Invoke(DartRuntimePrimitives.RequireValue(_insertIndex));
    }

    internal virtual void _dropCompleted()
    {
        long oldIndex = DartRuntimePrimitives.RequireValue(_dragIndex);
        long newIndex = DartRuntimePrimitives.RequireValue(_insertIndex);
        _handleReorderItem(oldIndex, newIndex);
        setState(() =>
        {
            _dragReset();
        });
    }

    internal virtual void _dragReset()
    {
        if (_dragInfo is not null)
        {
            if ((_dragIndex is not null) && _items.ContainsKey(DartRuntimePrimitives.RequireValue(_dragIndex)))
            {
                _ReorderableItemState__reorderable_list dragItem = _items.GetValueOrDefault(DartRuntimePrimitives.RequireValue(_dragIndex))!;
                dragItem._dragging = false;
                dragItem.rebuild();
                _dragIndex = null;
            }
            _dragInfo?.dispose();
            _dragInfo = null;
            _autoScroller?.stopAutoScroll();
            _resetItemGap();
            _recognizer?.dispose();
            _recognizer = null;
            _overlayEntry?.remove();
            _overlayEntry?.dispose();
            _overlayEntry = null;
            _finalDropPosition = null;
        }
    }

    internal virtual void _resetItemGap()
    {
        foreach (_ReorderableItemState__reorderable_list item in _items.Values)
        {
            item.resetGap();
        }
    }

    internal virtual void _handleReorderItem(long oldIndex, long newIndex)
    {
        if ((widget.onReorder is not null) && (oldIndex != newIndex))
        {
            widget.onReorder?.Invoke(oldIndex, newIndex);
            return;
        }
        if (newIndex > oldIndex)
        {
            newIndex -= 1L;
        }
        if (oldIndex != newIndex)
        {
            widget.onReorderItem?.Invoke(oldIndex, newIndex);
        }
    }

    internal virtual void _handleScrollableAutoScrolled()
    {
        if (_dragInfo is null)
        {
            return;
        }
        _dragUpdateItems();
        _autoScroller?.startAutoScrollIfNecessary(_dragTargetRect);
    }

    internal virtual void _dragUpdateItems()
    {
        DartRuntimePrimitives.Assert(() => _dragInfo is not null);
        double gapExtent = _dragInfo!.itemExtent;
        double proxyItemStart = Reorderable_listLibrary._offsetExtent(_dragInfo!.dragPosition - _dragInfo!.dragOffset, _scrollDirection);
        double proxyItemEnd = proxyItemStart + gapExtent;
        long newIndex = DartRuntimePrimitives.RequireValue(_insertIndex);
        foreach (_ReorderableItemState__reorderable_list item in _items.Values)
        {
            if (_reverse && (item.index == DartRuntimePrimitives.RequireValue(_dragIndex)) || !item.mounted)
            {
                continue;
            }
            Rect geometry = item.targetGeometry();
            double itemStart = Equals(_scrollDirection, Axis.vertical) ? geometry.top : geometry.left;
            double itemExtentLocal = Equals(_scrollDirection, Axis.vertical) ? geometry.height : geometry.width;
            double itemEnd = itemStart + itemExtentLocal;
            double itemMiddle = itemStart + (itemExtentLocal / 2L);
            if (_reverse)
            {
                if ((itemEnd >= proxyItemEnd) && (proxyItemEnd >= itemMiddle))
                {
                    newIndex = item.index;
                    break;
                }
                else
                {
                    if ((itemMiddle >= proxyItemStart) && (proxyItemStart >= itemStart))
                    {
                        newIndex = item.index + 1L;
                        break;
                    }
                    else
                    {
                        if ((itemStart > proxyItemEnd) && (newIndex < item.index + 1L))
                        {
                            newIndex = item.index + 1L;
                        }
                        else
                        {
                            if ((proxyItemStart > itemEnd) && (newIndex > item.index))
                            {
                                newIndex = item.index;
                            }
                        }
                    }
                }
            }
            else
            {
                if (item.index == DartRuntimePrimitives.RequireValue(_dragIndex))
                {
                    if ((itemMiddle <= proxyItemEnd) && (proxyItemEnd <= itemEnd))
                    {
                        newIndex = DartRuntimePrimitives.RequireValue(_dragIndex);
                    }
                }
                else
                {
                    if ((itemStart <= proxyItemStart) && (proxyItemStart <= itemMiddle))
                    {
                        newIndex = item.index;
                        break;
                    }
                    else
                    {
                        if ((itemMiddle <= proxyItemEnd) && (proxyItemEnd <= itemEnd))
                        {
                            newIndex = item.index + 1L;
                            break;
                        }
                        else
                        {
                            if ((itemEnd < proxyItemStart) && (newIndex < item.index + 1L))
                            {
                                newIndex = item.index + 1L;
                            }
                            else
                            {
                                if ((proxyItemEnd < itemStart) && (newIndex > item.index))
                                {
                                    newIndex = item.index;
                                }
                            }
                        }
                    }
                }
            }
        }
        if (newIndex != _insertIndex)
        {
            _insertIndex = newIndex;
            foreach (_ReorderableItemState__reorderable_list itemLocal in _items.Values)
            {
                if ((itemLocal.index == DartRuntimePrimitives.RequireValue(_dragIndex)) || !itemLocal.mounted)
                {
                    continue;
                }
                itemLocal.updateForGap(DartRuntimePrimitives.RequireValue(_dragIndex), newIndex, gapExtent, true, _reverse);
            }
        }
    }

    internal virtual Rect _dragTargetRect
    {
        get
        {
            Offset origin = _dragInfo!.dragPosition - _dragInfo!.dragOffset;
            return Rect.fromLTWH(origin.dx, origin.dy, _dragInfo!.itemSize.width, _dragInfo!.itemSize.height);
        }
    }
    internal virtual Offset _itemOffsetAt(long index)
    {
        return _items.GetValueOrDefault(index)!.targetGeometry().topLeft;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _itemExtentAt(long index)
    {
        return Reorderable_listLibrary._sizeExtent(_items.GetValueOrDefault(index)!.targetGeometry().size, _scrollDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _itemBuilder(BuildContext context, long index)
    {
        if ((_dragInfo is not null) && (index >= widget.itemCount))
        {
            return _scrollDirection switch { Axis.horizontal => new SizedBox(width: _dragInfo!.itemExtent), Axis.vertical => new SizedBox(height: _dragInfo!.itemExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
        Widget childLocal = widget.itemBuilder(context, index);
        DartRuntimePrimitives.Assert(() => childLocal.key is not null, () => (object?)"All list items must have a key");
        OverlayState overlay = Overlay.of(context, debugRequiredFor: widget);
        return new _ReorderableItem__reorderable_list(_ReorderableItemGlobalKey__reorderable_list.Create(key: childLocal.key!, index: index, state: this), index: index, capturedThemes: InheritedTheme.capture(from: context, to: overlay.context), child: _wrapWithSemantics(childLocal, index));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _wrapWithSemantics(Widget child, long index)
    {
        var semanticsActions = new DartMap<CustomSemanticsAction, Action>();
        void moveToStart()
        {
            _handleReorderItem(index, 0L);
        }
        void moveToEnd()
        {
            _handleReorderItem(index, widget.itemCount);
        }
        void moveBefore()
        {
            _handleReorderItem(index, index - 1L);
        }
        void moveAfter()
        {
            _handleReorderItem(index, index + 2L);
        }
        WidgetsLocalizations localizations = WidgetsLocalizations.of(context);
        var isHorizontal = Equals(_scrollDirection, Axis.horizontal);
        if (index > 0L)
        {
            semanticsActions[new CustomSemanticsAction(label: localizations.reorderItemToStart)] = moveToStart;
            string reorderItemBefore = localizations.reorderItemUp;
            if (isHorizontal)
            {
                reorderItemBefore = Equals(Directionality.of(context), TextDirection.ltr) ? localizations.reorderItemLeft : localizations.reorderItemRight;
            }
            semanticsActions[new CustomSemanticsAction(label: reorderItemBefore)] = moveBefore;
        }
        if (index < (widget.itemCount - 1L))
        {
            string reorderItemAfter = localizations.reorderItemDown;
            if (isHorizontal)
            {
                reorderItemAfter = Equals(Directionality.of(context), TextDirection.ltr) ? localizations.reorderItemRight : localizations.reorderItemLeft;
            }
            semanticsActions[new CustomSemanticsAction(label: reorderItemAfter)] = moveAfter;
            semanticsActions[new CustomSemanticsAction(label: localizations.reorderItemToEnd)] = moveToEnd;
        }
        return new Semantics(container: true, customSemanticsActions: semanticsActions, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasOverlay(context));
        var childrenDelegate = new SliverChildBuilderDelegate(_itemBuilder, childCount: widget.itemCount, findChildIndexCallback: widget.findChildIndexCallback);
        if (widget.itemExtent is not null)
        {
            return new SliverFixedExtentList(@delegate: childrenDelegate, itemExtent: DartRuntimePrimitives.RequireValue(widget.itemExtent));
        }
        else
        {
            if (widget.itemExtentBuilder is not null)
            {
                return new SliverVariedExtentList(@delegate: childrenDelegate, itemExtentBuilder: widget.itemExtentBuilder!);
            }
            else
            {
                if (widget.prototypeItem is not null)
                {
                    return new SliverPrototypeExtentList(@delegate: childrenDelegate, prototypeItem: widget.prototypeItem!);
                }
            }
        }
        return new SliverList(@delegate: childrenDelegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

public class _ReorderableItem__reorderable_list : StatefulWidget
{
    public virtual long index { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual CapturedThemes capturedThemes { get; private set; } = default!;

    internal _ReorderableItem__reorderable_list(Key key, long index, Widget child, CapturedThemes capturedThemes) : base(key: key)
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
    internal virtual AnimationController? _offsetAnimation { get; set; } = default;
    internal virtual bool _dragging { get; set; } = false;

    public virtual Key key => DartRuntimePrimitives.ConvertValue<Key>(widget.key!);
    public virtual long index => widget.index;
    public virtual bool dragging
    {
        get => _dragging;
        set
        {
            var dragging = value;
            if (mounted)
            {
                setState(() =>
                {
                    _dragging = dragging;
                });
            }
        }
    }
    public override void initState()
    {
        _listState = SliverReorderableList.of(context);
        _listState._registerItem(this);
        base.initState();
    }

    public override void dispose()
    {
        _offsetAnimation?.dispose();
        _listState._unregisterItem(index, this);
        base.dispose();
    }

    public override void didUpdateWidget(_ReorderableItem__reorderable_list oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.index != widget.index)
        {
            _listState._unregisterItem(oldWidget.index, this);
            _listState._registerItem(this);
        }
    }

    public override Widget build(BuildContext context)
    {
        if (_dragging)
        {
            Size sizeLocal = Reorderable_listLibrary._extentSize(_listState._dragInfo!.itemExtent, _listState._scrollDirection);
            return SizedBox.CreateFromSize(size: sizeLocal);
        }
        _listState._registerItem(this);
        return Transform.CreateTranslate(offset: offset, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void deactivate()
    {
        _listState._unregisterItem(index, this);
        base.deactivate();
    }

    public virtual Offset offset
    {
        get
        {
            if (_offsetAnimation is not null)
            {
                double animValue = Curves.easeInOut.transform(_offsetAnimation!.value);
                return DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(_startOffset, _targetOffset, animValue));
            }
            return _targetOffset;
        }
    }
    public virtual void updateForGap(long dragIndex, long gapIndex, double gapExtent, bool animate, bool reverse)
    {
        Offset newTargetOffset = default!;
        if ((gapIndex < dragIndex) && (index < dragIndex) && (index >= gapIndex))
        {
            newTargetOffset = Reorderable_listLibrary._extentOffset(reverse ? -gapExtent : gapExtent, _listState._scrollDirection);
        }
        else
        {
            if ((gapIndex > dragIndex) && (index > dragIndex) && (index < gapIndex))
            {
                newTargetOffset = Reorderable_listLibrary._extentOffset(reverse ? gapExtent : -gapExtent, _listState._scrollDirection);
            }
            else
            {
                newTargetOffset = Offset.zero;
            }
        }
        if (!Equals(newTargetOffset, _targetOffset))
        {
            Offset previousTarget = _targetOffset;
            _targetOffset = newTargetOffset;
            if (animate)
            {
                if (_offsetAnimation is null)
                {
                    _offsetAnimation = ((Func<AnimationController>)(() =>
{
    var __cascade = new AnimationController(vsync: _listState, duration: Duration.Create(milliseconds: 250L));
    __cascade.addListener(rebuild);
    __cascade.addStatusListener((status) =>
    {
        if (AnimationStatusMembers.isCompleted(status))
        {
            _startOffset = _targetOffset;
            _offsetAnimation!.dispose();
            _offsetAnimation = null;
        }
    });
    __cascade.forward();
    return __cascade;
}))();
                }
                else
                {
                    double currentAnimValue = Curves.easeInOut.transform(_offsetAnimation!.value);
                    Offset currentPosition = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(_startOffset, previousTarget, currentAnimValue));
                    _startOffset = currentPosition;
                    _offsetAnimation!.forward(from: 0.0);
                }
            }
            else
            {
                if (_offsetAnimation is not null)
                {
                    _offsetAnimation!.dispose();
                    _offsetAnimation = null;
                }
                _startOffset = _targetOffset;
            }
            rebuild();
        }
    }

    public virtual void resetGap()
    {
        if (_offsetAnimation is not null)
        {
            _offsetAnimation!.dispose();
            _offsetAnimation = null;
        }
        _startOffset = Offset.zero;
        _targetOffset = Offset.zero;
        rebuild();
    }

    public virtual Rect targetGeometry()
    {
        var itemRenderBox = ((RenderBox?)context.findRenderObject()!)!;
        Offset itemPosition = itemRenderBox.localToGlobal(Offset.zero) + _targetOffset;
        return itemPosition & itemRenderBox.size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void rebuild()
    {
        if (mounted)
        {
            setState(() =>
            {
            });
        }
    }

}

public class ReorderableDragStartListener : StatelessWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    public ReorderableDragStartListener(Key? key = null, Widget child = default!, long index = default!, bool enabled = true) : base(key: key)
    {
        this.child = child;
        this.index = index;
        this.enabled = enabled;
    }

    public override Widget build(BuildContext context)
    {
        return new Listener(onPointerDown: enabled ? ((@event) => { _startDragging(context, @event); }) : null, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual MultiDragGestureRecognizer createRecognizer()
    {
        return new ImmediateMultiDragGestureRecognizer(debugOwner: this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _startDragging(BuildContext context, Gestures.PointerDownEvent @event)
    {
        DeviceGestureSettings? gestureSettingsLocal = MediaQuery.maybeGestureSettingsOf(context);
        SliverReorderableListState? list = SliverReorderableList.maybeOf(context);
        list?.startItemDragReorder(index: index, @event: @event, recognizer: ((Func<MultiDragGestureRecognizer>)(() =>
{
    var __cascade = createRecognizer();
    __cascade.gestureSettings = gestureSettingsLocal;
    return __cascade;
}))());
    }

}

public class ReorderableDelayedDragStartListener : ReorderableDragStartListener
{
    public ReorderableDelayedDragStartListener(Key? key = null, Widget child = default!, long index = default!, bool enabled = true) : base(key: key, child: child, index: index, enabled: enabled)
    {
    }

    public override MultiDragGestureRecognizer createRecognizer()
    {
        return new DelayedMultiDragGestureRecognizer(debugOwner: this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _DragItemUpdate__reorderable_list(_DragInfo__reorderable_list item, Offset position, Offset delta);

internal delegate void _DragItemCallback__reorderable_list(_DragInfo__reorderable_list item);

internal class _DragInfo__reorderable_list : Drag
{
    public virtual Axis scrollDirection { get; private set; } = default!;
    public virtual Action<_DragInfo__reorderable_list, Offset, Offset>? onUpdate { get; private set; }
    public virtual Action<_DragInfo__reorderable_list>? onEnd { get; private set; }
    public virtual Action<_DragInfo__reorderable_list>? onCancel { get; private set; }
    public virtual Action? onDropCompleted { get; private set; }
    public virtual Func<Widget, long, Animation<double>, Widget>? proxyDecorator { get; private set; }
    public virtual Scheduler.TickerProvider tickerProvider { get; private set; } = default!;
    public virtual DragBoundaryDelegate<Rect>? boundary { get; set; } = default;
    public virtual SliverReorderableListState listState { get; set; } = default!;
    public virtual long index { get; set; } = default!;
    public virtual Widget child { get; set; } = default!;
    public virtual Offset dragPosition { get; set; } = default!;
    public virtual Offset dragOffset { get; set; } = default!;
    public virtual Size itemSize { get; set; } = default!;
    public virtual BoxConstraints itemLayoutConstraints { get; set; } = default!;
    public virtual double itemExtent { get; set; } = default!;
    public virtual CapturedThemes capturedThemes { get; set; } = default!;
    public virtual ScrollableState? scrollable { get; set; } = default;
    internal virtual AnimationController? _proxyAnimation { get; set; } = default;
    internal virtual Offset _rawDragPosition { get; set; } = default!;

    internal _DragInfo__reorderable_list(_ReorderableItemState__reorderable_list item, Offset initialPosition = default, Axis scrollDirection = Axis.vertical, Action<_DragInfo__reorderable_list, Offset, Offset>? onUpdate = null, Action<_DragInfo__reorderable_list>? onEnd = null, Action<_DragInfo__reorderable_list>? onCancel = null, Action? onDropCompleted = null, Func<Widget, long, Animation<double>, Widget>? proxyDecorator = null, Scheduler.TickerProvider tickerProvider = default!)
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
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _proxyAnimation?.dispose();
    }

    public virtual void startDrag()
    {
        _proxyAnimation = ((Func<AnimationController>)(() =>
{
    var __cascade = new AnimationController(vsync: tickerProvider, duration: Duration.Create(milliseconds: 250L));
    __cascade.addStatusListener((status) =>
    {
        if (AnimationStatusMembers.isDismissed(status))
        {
            _dropCompleted();
        }
    });
    __cascade.forward();
    return __cascade;
}))();
    }

    public override void update(DragUpdateDetails details)
    {
        Offset deltaLocal = Reorderable_listLibrary._restrictAxis(details.delta, scrollDirection);
        _rawDragPosition += deltaLocal;
        dragPosition = _adjustedDragOffset(_rawDragPosition);
        onUpdate?.Invoke(this, dragPosition, details.delta);
    }

    public override void end(DragEndDetails details)
    {
        _proxyAnimation!.reverse();
        onEnd?.Invoke(this);
    }

    public override void cancel()
    {
        _proxyAnimation?.dispose();
        _proxyAnimation = null;
        onCancel?.Invoke(this);
    }

    internal virtual Offset _adjustedDragOffset(Offset offset)
    {
        if (boundary is null)
        {
            return offset;
        }
        Offset adjOffset = boundary!.nearestPositionWithinBoundary(offset - dragOffset & itemSize).shift(dragOffset).topLeft;
        return adjOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _dropCompleted()
    {
        _proxyAnimation?.dispose();
        _proxyAnimation = null;
        onDropCompleted?.Invoke();
    }

    public virtual Widget createProxy(BuildContext context)
    {
        return capturedThemes.wrap(new _DragItemProxy__reorderable_list(listState: listState, index: index, size: itemSize, constraints: itemLayoutConstraints, animation: _proxyAnimation!, position: dragPosition - dragOffset - Reorderable_listLibrary._overlayOrigin(context), proxyDecorator: proxyDecorator, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Reorderable_listLibrary
{
    internal static Offset _overlayOrigin(BuildContext context)
    {
        OverlayState overlay = Overlay.of(context, debugRequiredFor: context.widget);
        var overlayBox = ((RenderBox?)overlay.context.findRenderObject()!)!;
        return overlayBox.localToGlobal(Offset.zero);
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
    public virtual BoxConstraints constraints { get; private set; } = default!;
    public virtual AnimationController animation { get; private set; } = default!;
    public virtual Func<Widget, long, Animation<double>, Widget>? proxyDecorator { get; private set; }

    internal _DragItemProxy__reorderable_list(SliverReorderableListState listState, long index, Widget child, Offset position, Size size, BoxConstraints constraints, AnimationController animation, Func<Widget, long, Animation<double>, Widget>? proxyDecorator)
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
        Widget proxyChild = proxyDecorator is null ? child : proxyDecorator.Invoke(child, index, animation.view);
        Offset overlayOrigin = Reorderable_listLibrary._overlayOrigin(context);
        return new MediaQuery(data: MediaQuery.of(context).removePadding(removeTop: true), child: new AnimatedBuilder(animation: animation, builder: (context, child) =>
        {
            Offset effectivePosition = position;
            Offset? dropPosition = listState._finalDropPosition;
            if (dropPosition is not null)
            {
                Offset dropPosition__58071__value58130 = DartRuntimePrimitives.RequireValue(dropPosition);
                effectivePosition = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(DartRuntimePrimitives.RequireValue(dropPosition__58071__value58130) - overlayOrigin, effectivePosition, Curves.easeOut.transform(animation.value)));
            }
            return new Positioned(left: effectivePosition.dx, top: effectivePosition.dy, child: new SizedBox(width: size.width, height: size.height, child: new OverflowBox(minWidth: constraints.minWidth, minHeight: constraints.minHeight, maxWidth: constraints.maxWidth, maxHeight: constraints.maxHeight, alignment: Equals(listState._scrollDirection, Axis.horizontal) ? Alignment.centerLeft : Alignment.topCenter, child: child)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: proxyChild));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Reorderable_listLibrary
{
    internal static double _sizeExtent(Size size, Axis scrollDirection)
    {
        return scrollDirection switch { Axis.horizontal => size.width, Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Reorderable_listLibrary
{
    internal static Size _extentSize(double extent, Axis scrollDirection)
    {
        return scrollDirection switch { Axis.horizontal => new Size(extent, 0), Axis.vertical => new Size(0, extent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Reorderable_listLibrary
{
    internal static double _offsetExtent(Offset offset, Axis scrollDirection)
    {
        return scrollDirection switch { Axis.horizontal => offset.dx, Axis.vertical => offset.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Reorderable_listLibrary
{
    internal static Offset _extentOffset(double extent, Axis scrollDirection)
    {
        return scrollDirection switch { Axis.horizontal => new Offset(extent, 0.0), Axis.vertical => new Offset(0.0, extent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Reorderable_listLibrary
{
    internal static Offset _restrictAxis(Offset offset, Axis scrollDirection)
    {
        return scrollDirection switch { Axis.horizontal => new Offset(offset.dx, 0.0), Axis.vertical => new Offset(0.0, offset.dy), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ReorderableItemGlobalKey__reorderable_list : GlobalObjectKey<IState>
{
    public virtual Key subKey { get; private set; } = default!;
    public virtual long index { get; private set; } = default!;
    public virtual SliverReorderableListState state { get; private set; } = default!;

    internal _ReorderableItemGlobalKey__reorderable_list(Key subKey, long index, SliverReorderableListState state) : base(subKey)
    {
        this.subKey = subKey;
        this.index = index;
        this.state = state;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ReorderableItemGlobalKey__reorderable_list;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _ReorderableItemGlobalKey__reorderable_list) && Equals(__other.subKey, subKey) && (__other.index == index) && Equals(__other.state, state);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(subKey, index, state));
    internal static _ReorderableItemGlobalKey__reorderable_list Create(Key key, long index, SliverReorderableListState state) => new(key, index, state);
}

