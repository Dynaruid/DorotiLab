// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/reorderable_list.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ReorderableListView : StatefulWidget
{
    public virtual Func<BuildContext, long, Widget> itemBuilder { get; private set; } = default!;
    public virtual long itemCount { get; private set; } = default!;
    public virtual Action<long, long>? onReorder { get; private set; }
    public virtual Action<long, long>? onReorderItem { get; private set; }
    public virtual Action<long>? onReorderStart { get; private set; }
    public virtual Action<long>? onReorderEnd { get; private set; }
    public virtual Func<Widget, long, Animation<double>, Widget>? proxyDecorator
    {
        get;
        private set;
    }
    public virtual bool buildDefaultDragHandles { get; private set; } = default!;
    public virtual EdgeInsets? padding { get; private set; }
    public virtual Widget? header { get; private set; }
    public virtual Widget? footer { get; private set; }
    public virtual Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollController? scrollController { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual bool shrinkWrap { get; private set; } = default!;
    public virtual double anchor { get; private set; } = default!;
    public virtual double? cacheExtent { get; private set; }
    public virtual ScrollCacheExtent? scrollCacheExtent { get; private set; }
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }
    public virtual string? restorationId { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual double? itemExtent { get; private set; }
    public virtual ItemExtentBuilder? itemExtentBuilder { get; private set; }
    public virtual Widget? prototypeItem { get; private set; }
    public virtual double? autoScrollerVelocityScalar { get; private set; }
    public virtual Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider
    {
        get;
        private set;
    }
    public virtual MouseCursor? mouseCursor { get; private set; }

    public ReorderableListView(
        Key? key = null,
        List<Widget> children = default!,
        Action<long, long>? onReorder = null,
        Action<long, long>? onReorderItem = null,
        Action<long>? onReorderStart = null,
        Action<long>? onReorderEnd = null,
        double? itemExtent = null,
        ItemExtentBuilder? itemExtentBuilder = null,
        Widget? prototypeItem = null,
        Func<Widget, long, Animation<double>, Widget>? proxyDecorator = null,
        bool buildDefaultDragHandles = true,
        EdgeInsets? padding = null,
        Widget? header = null,
        Widget? footer = null,
        Axis scrollDirection = Axis.vertical,
        bool reverse = false,
        ScrollController? scrollController = null,
        bool? primary = null,
        ScrollPhysics? physics = null,
        bool shrinkWrap = false,
        double anchor = 0.0,
        double? cacheExtent = null,
        ScrollCacheExtent? scrollCacheExtent = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null,
        string? restorationId = null,
        Clip clipBehavior = Clip.hardEdge,
        double? autoScrollerVelocityScalar = null,
        Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider = null,
        MouseCursor? mouseCursor = null
    )
        : base(key: key)
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
        itemBuilder = (context, index) => children[(int)index];
        itemCount = checked(children.Count);
        System.Diagnostics.Debug.Assert(
            ((itemExtent is null) && (prototypeItem is null))
                || ((itemExtent is null) && (itemExtentBuilder is null))
                || ((prototypeItem is null) && (itemExtentBuilder is null))
        );
        System.Diagnostics.Debug.Assert(children.All((w) => w.key is not null));
        System.Diagnostics.Debug.Assert(
            ((onReorderItem is not null) && (onReorder is null))
                || ((onReorderItem is null) && (onReorder is not null))
        );
    }

    public static ReorderableListView CreateBuilder(
        Key? key = null,
        Func<BuildContext, long, Widget> itemBuilder = default!,
        long itemCount = default!,
        Action<long, long>? onReorder = null,
        Action<long, long>? onReorderItem = null,
        Action<long>? onReorderStart = null,
        Action<long>? onReorderEnd = null,
        double? itemExtent = null,
        ItemExtentBuilder? itemExtentBuilder = null,
        Widget? prototypeItem = null,
        Func<Widget, long, Animation<double>, Widget>? proxyDecorator = null,
        bool buildDefaultDragHandles = true,
        EdgeInsets? padding = null,
        Widget? header = null,
        Widget? footer = null,
        Axis scrollDirection = Axis.vertical,
        bool reverse = false,
        ScrollController? scrollController = null,
        bool? primary = null,
        ScrollPhysics? physics = null,
        bool shrinkWrap = false,
        double anchor = 0.0,
        double? cacheExtent = null,
        ScrollCacheExtent? scrollCacheExtent = null,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start,
        ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null,
        string? restorationId = null,
        Clip clipBehavior = Clip.hardEdge,
        double? autoScrollerVelocityScalar = null,
        Func<BuildContext, DragBoundaryDelegate<Rect>?>? dragBoundaryProvider = null,
        MouseCursor? mouseCursor = null
    )
    {
        var __instance = new ReorderableListView(
            key: key,
            children: default!,
            onReorder: onReorder,
            onReorderItem: onReorderItem,
            onReorderStart: onReorderStart,
            onReorderEnd: onReorderEnd,
            itemExtent: itemExtent,
            itemExtentBuilder: itemExtentBuilder,
            prototypeItem: prototypeItem,
            proxyDecorator: proxyDecorator,
            buildDefaultDragHandles: buildDefaultDragHandles,
            padding: padding,
            header: header,
            footer: footer,
            scrollDirection: scrollDirection,
            reverse: reverse,
            scrollController: scrollController,
            primary: primary,
            physics: physics,
            shrinkWrap: shrinkWrap,
            anchor: anchor,
            cacheExtent: cacheExtent,
            scrollCacheExtent: scrollCacheExtent,
            dragStartBehavior: dragStartBehavior,
            keyboardDismissBehavior: keyboardDismissBehavior,
            restorationId: restorationId,
            clipBehavior: clipBehavior,
            autoScrollerVelocityScalar: autoScrollerVelocityScalar,
            dragBoundaryProvider: dragBoundaryProvider,
            mouseCursor: mouseCursor
        );
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _ReorderableListViewState__reorderable_list()
        );
}

internal class _ReorderableListViewState__reorderable_list : State<ReorderableListView>
{
    internal virtual ValueNotifier<bool> _dragging { get; private set; } =
        new ValueNotifier<bool>(false);

    internal virtual Widget _itemBuilder(BuildContext context, long index)
    {
        Widget item = widget.itemBuilder(context, index);
        DartRuntimePrimitives.Assert(() =>
        {
            if (item.key is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create("Every item of ReorderableListView must have a key.")
                );
            }
            return true;
        });
        Key itemGlobalKey = new _ReorderableListViewChildGlobalKey__reorderable_list(
            item.key!,
            this
        );
        if (widget.buildDefaultDragHandles)
        {
            switch (Theme.of(context).platform)
            {
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                case TargetPlatform.macOS:
                {
                    var dragHandle = new ListenableBuilder(
                        listenable: _dragging,
                        builder: (context, child) =>
                        {
                            MouseCursor effectiveMouseCursor = WidgetStateProperty.resolveAs(
                                widget.mouseCursor
                                    ?? WidgetStateMouseCursor.CreateFromMap(
                                        new DartMap<WidgetStatesConstraint, MouseCursor>
                                        {
                                            [WidgetState.dragged.asConstraint()] =
                                                SystemMouseCursors.grabbing,
                                            [WidgetStateMembers.any] = SystemMouseCursors.grab,
                                        }
                                    ),
                                (
                                    (Func<HashSet<WidgetState>>)(
                                        () =>
                                        {
                                            var __collection15120 = new HashSet<WidgetState>();
                                            if (_dragging.value)
                                            {
                                                __collection15120.Add(WidgetState.dragged);
                                            }
                                            return __collection15120;
                                        }
                                    )
                                )()
                            );
                            return new MouseRegion(cursor: effectiveMouseCursor, child: child);
                            throw new InvalidOperationException(
                                "Callback completed without returning a value."
                            );
                        },
                        child: new Icon(Icons.drag_handle)
                    );
                    switch (widget.scrollDirection)
                    {
                        case Axis.horizontal:
                        {
                            return Stack.Create(
                                key: itemGlobalKey,
                                children: new List<Widget>
                                {
                                    DartRuntimePrimitives.ConvertValue<Widget>(item),
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        Positioned.CreateDirectional(
                                            textDirection: Directionality.of(context),
                                            start: 0,
                                            end: 0,
                                            bottom: 8,
                                            child: new Align(
                                                alignment: AlignmentDirectional.bottomCenter,
                                                child: new ReorderableDragStartListener(
                                                    index: index,
                                                    child: dragHandle
                                                )
                                            )
                                        )
                                    ),
                                }
                            );
                        }
                        case Axis.vertical:
                        {
                            return Stack.Create(
                                key: itemGlobalKey,
                                children: new List<Widget>
                                {
                                    DartRuntimePrimitives.ConvertValue<Widget>(item),
                                    DartRuntimePrimitives.ConvertValue<Widget>(
                                        Positioned.CreateDirectional(
                                            textDirection: Directionality.of(context),
                                            top: 0,
                                            bottom: 0,
                                            end: 8,
                                            child: new Align(
                                                alignment: AlignmentDirectional.centerEnd,
                                                child: new ReorderableDragStartListener(
                                                    index: index,
                                                    child: dragHandle
                                                )
                                            )
                                        )
                                    ),
                                }
                            );
                        }
                        default:
                            throw new InvalidOperationException(
                                "Switch expression did not handle the supplied value."
                            );
                    }
                }
                case TargetPlatform.iOS:
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                {
                    return new ReorderableDelayedDragStartListener(
                        key: itemGlobalKey,
                        index: index,
                        child: item
                    );
                }
            }
        }
        return new KeyedSubtree(key: itemGlobalKey, child: item);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _proxyDecorator(Widget child, long index, Animation<double> animation)
    {
        return new AnimatedBuilder(
            animation: animation,
            builder: (context, child) =>
            {
                double animValue = Curves.easeInOut.transform(animation.value);
                double elevationLocal = (
                    Dart_uiLibrary.lerpDouble(0L, 6L, animValue)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                return new Material(elevation: elevationLocal, child: child);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void dispose()
    {
        _dragging.dispose();
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasOverlay(context));
        EdgeInsets paddingLocal = widget.padding ?? EdgeInsets.zero;
        double? start = (widget.header is null) ? null : 0.0;
        double? end = (widget.footer is null) ? null : 0.0;
        if (widget.reverse)
        {
            DartRuntimePrimitives.Ignore((start, end) = (end, start));
        }
        EdgeInsets startPadding = default!;
        EdgeInsets endPadding = default!;
        EdgeInsets listPadding = default!;
        DartRuntimePrimitives.Ignore(
            (startPadding, endPadding, listPadding) = widget.scrollDirection switch
            {
                Axis.horizontal or Axis.vertical when (start ?? end) is null => ((
                    EdgeInsets,
                    EdgeInsets,
                    EdgeInsets
                ))
                    (EdgeInsets.zero, EdgeInsets.zero, paddingLocal),
                Axis.horizontal => ((EdgeInsets, EdgeInsets, EdgeInsets))
                    (
                        paddingLocal.copyWith(left: 0),
                        paddingLocal.copyWith(right: 0),
                        paddingLocal.copyWith(left: start, right: end)
                    ),
                Axis.vertical => ((EdgeInsets, EdgeInsets, EdgeInsets))
                    (
                        paddingLocal.copyWith(top: 0),
                        paddingLocal.copyWith(bottom: 0),
                        paddingLocal.copyWith(top: start, bottom: end)
                    ),
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            }
        );
        var (headerPadding, footerPadding) = widget.reverse
            ? (((EdgeInsets, EdgeInsets))(startPadding, endPadding))
            : (((EdgeInsets, EdgeInsets))(endPadding, startPadding));
        ScrollCacheExtent? scrollCacheExtentLocal =
            widget.scrollCacheExtent
            ?? (
                (widget.cacheExtent is null)
                    ? null
                    : ScrollCacheExtent.CreatePixels(
                        (
                            widget.cacheExtent
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    )
            );
        return new CustomScrollView(
            scrollDirection: widget.scrollDirection,
            reverse: widget.reverse,
            controller: widget.scrollController,
            primary: widget.primary,
            physics: widget.physics,
            shrinkWrap: widget.shrinkWrap,
            anchor: widget.anchor,
            scrollCacheExtent: scrollCacheExtentLocal,
            dragStartBehavior: widget.dragStartBehavior,
            keyboardDismissBehavior: widget.keyboardDismissBehavior,
            restorationId: widget.restorationId,
            clipBehavior: widget.clipBehavior,
            slivers: (
                (Func<List<Widget>>)(
                    () =>
                    {
                        var __collection19473 = new List<Widget>();
                        if (widget.header is not null)
                        {
                            __collection19473.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new SliverPadding(
                                        padding: headerPadding,
                                        sliver: new SliverToBoxAdapter(child: widget.header)
                                    )
                                )
                            );
                        }
                        __collection19473.Add(
                            DartRuntimePrimitives.ConvertValue<Widget>(
                                new SliverPadding(
                                    padding: listPadding,
                                    sliver: new SliverReorderableList(
                                        itemBuilder: _itemBuilder,
                                        itemExtent: widget.itemExtent,
                                        itemExtentBuilder: widget.itemExtentBuilder,
                                        prototypeItem: widget.prototypeItem,
                                        itemCount: widget.itemCount,
                                        onReorder: widget.onReorder,
                                        onReorderItem: widget.onReorderItem,
                                        onReorderStart: (index) =>
                                        {
                                            _dragging.value = true;
                                            widget.onReorderStart?.Invoke(index);
                                        },
                                        onReorderEnd: (index) =>
                                        {
                                            _dragging.value = false;
                                            widget.onReorderEnd?.Invoke(index);
                                        },
                                        proxyDecorator: widget.proxyDecorator ?? _proxyDecorator,
                                        autoScrollerVelocityScalar: widget.autoScrollerVelocityScalar,
                                        dragBoundaryProvider: widget.dragBoundaryProvider
                                    )
                                )
                            )
                        );
                        if (widget.footer is not null)
                        {
                            __collection19473.Add(
                                DartRuntimePrimitives.ConvertValue<Widget>(
                                    new SliverPadding(
                                        padding: footerPadding,
                                        sliver: new SliverToBoxAdapter(child: widget.footer)
                                    )
                                )
                            );
                        }
                        return __collection19473;
                    }
                )
            )()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _ReorderableListViewChildGlobalKey__reorderable_list : GlobalObjectKey<IState>
{
    public virtual Key subKey { get; private set; } = default!;
    public virtual IState state { get; private set; } = default!;

    internal _ReorderableListViewChildGlobalKey__reorderable_list(Key subKey, IState state)
        : base(subKey)
    {
        this.subKey = subKey;
        this.state = state;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ReorderableListViewChildGlobalKey__reorderable_list;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _ReorderableListViewChildGlobalKey__reorderable_list)
            && Equals(__other.subKey, subKey)
            && Equals(__other.state, state);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(subKey, state));
}
