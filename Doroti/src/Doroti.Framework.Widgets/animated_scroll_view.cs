// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/animated_scroll_view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class AnimatedList : _AnimatedScrollView__animated_scroll_view
{
    public AnimatedList(
        Key? key = null,
        Func<BuildContext, long, Animation<double>, Widget> itemBuilder = default!,
        long initialItemCount = 0,
        Axis scrollDirection = Axis.vertical,
        bool reverse = false,
        ScrollController? controller = null,
        bool? primary = null,
        ScrollPhysics? physics = null,
        bool shrinkWrap = false,
        EdgeInsetsGeometry? padding = null,
        Clip clipBehavior = Clip.hardEdge,
        ScrollCacheExtent? scrollCacheExtent = null
    )
        : base(
            key: key,
            itemBuilder: itemBuilder,
            initialItemCount: initialItemCount,
            scrollDirection: scrollDirection,
            reverse: reverse,
            controller: controller,
            primary: DartRuntimePrimitives.RequireValue(primary),
            physics: physics,
            shrinkWrap: shrinkWrap,
            padding: padding,
            clipBehavior: clipBehavior,
            scrollCacheExtent: scrollCacheExtent
        )
    {
        System.Diagnostics.Debug.Assert(initialItemCount >= 0L);
    }

    public static AnimatedList CreateSeparated(
        Key? key = null,
        Func<BuildContext, long, Animation<double>, Widget> itemBuilder = default!,
        Func<BuildContext, long, Animation<double>, Widget> separatorBuilder = default!,
        Func<BuildContext, long, Animation<double>, Widget> removedSeparatorBuilder = default!,
        long initialItemCount = 0,
        Axis scrollDirection = Axis.vertical,
        bool reverse = false,
        ScrollController? controller = null,
        bool? primary = null,
        ScrollPhysics? physics = null,
        bool shrinkWrap = false,
        EdgeInsetsGeometry? padding = null,
        Clip clipBehavior = Clip.hardEdge,
        ScrollCacheExtent? scrollCacheExtent = null
    )
    {
        var __instance = new AnimatedList(
            key: key,
            itemBuilder: (context, index, animation) =>
                (index & 1L) == 0L
                    ? itemBuilder(context, index / 2L, animation)
                    : separatorBuilder(context, index / 2L, animation),
            initialItemCount: _computeChildCountWithSeparators(initialItemCount),
            scrollDirection: scrollDirection,
            reverse: reverse,
            controller: controller,
            primary: primary,
            physics: physics,
            shrinkWrap: shrinkWrap,
            padding: padding,
            clipBehavior: clipBehavior,
            scrollCacheExtent: scrollCacheExtent
        );
        __instance.removedSeparatorBuilder = removedSeparatorBuilder;
        return __instance;
    }

    public static AnimatedListState of(BuildContext context)
    {
        AnimatedListState? result = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (result is null)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "AnimatedList.of() called with a context that does not contain an AnimatedList."
                            ),
                            new ErrorDescription(
                                "No AnimatedList ancestor could be found starting from the context that was passed to AnimatedList.of()."
                            ),
                            new ErrorHint(
                                "This can happen when the context provided is from the same StatefulWidget that "
                                    + "built the AnimatedList. Please see the AnimatedList documentation for examples "
                                    + "of how to refer to an AnimatedListState object:\n"
                                    + "  https://api.flutter.dev/flutter/widgets/AnimatedListState-class.html"
                            ),
                            context.describeElement("The context used was"),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AnimatedListState? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<AnimatedListState>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _computeChildCountWithSeparators(long itemCount)
    {
        if (itemCount == 0L)
        {
            return 0L;
        }
        return (itemCount * 2L) - 1L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new AnimatedListState());
}

public class AnimatedListState : _AnimatedScrollViewState__animated_scroll_view<AnimatedList>
{
    public override Widget build(BuildContext context)
    {
        return _wrap(
            new SliverAnimatedList(
                key: _sliverAnimatedMultiBoxKey,
                itemBuilder: widget.itemBuilder,
                initialItemCount: widget.initialItemCount
            ),
            widget.scrollDirection
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class AnimatedGrid : _AnimatedScrollView__animated_scroll_view
{
    public virtual SliverGridDelegate gridDelegate { get; private set; } = default!;

    public AnimatedGrid(
        Key? key = null,
        Func<BuildContext, long, Animation<double>, Widget> itemBuilder = default!,
        SliverGridDelegate gridDelegate = default!,
        long initialItemCount = 0,
        Axis scrollDirection = Axis.vertical,
        bool reverse = false,
        ScrollController? controller = null,
        bool? primary = null,
        ScrollPhysics? physics = null,
        EdgeInsetsGeometry? padding = null,
        Clip clipBehavior = Clip.hardEdge,
        ScrollCacheExtent? scrollCacheExtent = null
    )
        : base(
            key: key,
            itemBuilder: itemBuilder,
            initialItemCount: initialItemCount,
            scrollDirection: scrollDirection,
            reverse: reverse,
            controller: controller,
            primary: DartRuntimePrimitives.RequireValue(primary),
            physics: physics,
            padding: padding,
            clipBehavior: clipBehavior,
            scrollCacheExtent: scrollCacheExtent
        )
    {
        this.gridDelegate = gridDelegate;
        System.Diagnostics.Debug.Assert(initialItemCount >= 0L);
    }

    public static AnimatedGridState of(BuildContext context)
    {
        AnimatedGridState? result = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (result is null)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "AnimatedGrid.of() called with a context that does not contain an AnimatedGrid."
                            ),
                            new ErrorDescription(
                                "No AnimatedGrid ancestor could be found starting from the context that was passed to AnimatedGrid.of()."
                            ),
                            new ErrorHint(
                                "This can happen when the context provided is from the same StatefulWidget that "
                                    + "built the AnimatedGrid. Please see the AnimatedGrid documentation for examples "
                                    + "of how to refer to an AnimatedGridState object:\n"
                                    + "  https://api.flutter.dev/flutter/widgets/AnimatedGridState-class.html"
                            ),
                            context.describeElement("The context used was"),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AnimatedGridState? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<AnimatedGridState>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new AnimatedGridState());
}

public class AnimatedGridState : _AnimatedScrollViewState__animated_scroll_view<AnimatedGrid>
{
    public override Widget build(BuildContext context)
    {
        return _wrap(
            new SliverAnimatedGrid(
                key: _sliverAnimatedMultiBoxKey,
                gridDelegate: widget.gridDelegate,
                itemBuilder: widget.itemBuilder,
                initialItemCount: widget.initialItemCount
            ),
            widget.scrollDirection
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class _AnimatedScrollView__animated_scroll_view : StatefulWidget
{
    public virtual Func<BuildContext, long, Animation<double>, Widget> itemBuilder
    {
        get;
        private set;
    } = default!;
    public virtual Func<BuildContext, long, Animation<double>, Widget>? removedSeparatorBuilder
    {
        get;
        protected set;
    }
    public virtual long initialItemCount { get; private set; } = default!;
    public virtual Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual bool shrinkWrap { get; private set; } = default!;
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual ScrollCacheExtent? scrollCacheExtent { get; private set; }

    internal _AnimatedScrollView__animated_scroll_view(
        Key? key = null,
        Func<BuildContext, long, Animation<double>, Widget> itemBuilder = default!,
        Func<BuildContext, long, Animation<double>, Widget>? removedSeparatorBuilder = null,
        long initialItemCount = 0,
        Axis scrollDirection = Axis.vertical,
        bool reverse = false,
        ScrollController? controller = null,
        bool? primary = null,
        ScrollPhysics? physics = null,
        bool shrinkWrap = false,
        EdgeInsetsGeometry? padding = null,
        Clip clipBehavior = Clip.hardEdge,
        ScrollCacheExtent? scrollCacheExtent = null
    )
        : base(key: key)
    {
        this.itemBuilder = itemBuilder;
        this.removedSeparatorBuilder = removedSeparatorBuilder;
        this.initialItemCount = initialItemCount;
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.controller = controller;
        this.primary = primary;
        this.physics = physics;
        this.shrinkWrap = shrinkWrap;
        this.padding = padding;
        this.clipBehavior = clipBehavior;
        this.scrollCacheExtent = scrollCacheExtent;
        System.Diagnostics.Debug.Assert(initialItemCount >= 0L);
    }
}

public abstract class _AnimatedScrollViewState__animated_scroll_view<T>
    : State<T>,
        TickerProviderStateMixin<T>
    where T : _AnimatedScrollView__animated_scroll_view
{
    internal virtual GlobalKey<
        _SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>
    > _sliverAnimatedMultiBoxKey { get; private set; } =
        GlobalKey<
            _SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>
        >.Create();
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual void insertItem(long index, Duration? duration = null)
    {
        if (widget.removedSeparatorBuilder is null)
        {
            _sliverAnimatedMultiBoxKey.currentState!.insertItem(
                index,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
        }
        else
        {
            long itemIndex = _computeItemIndex(index);
            _sliverAnimatedMultiBoxKey.currentState!.insertItem(
                itemIndex,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
            if (_itemsCount > 1L)
            {
                _sliverAnimatedMultiBoxKey.currentState!.insertItem(
                    itemIndex,
                    duration: DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(duration)
                    )
                );
            }
        }
    }

    public virtual void insertAllItems(
        long index,
        long length,
        Duration? duration = null,
        bool isAsync = false
    )
    {
        if (widget.removedSeparatorBuilder is null)
        {
            _sliverAnimatedMultiBoxKey.currentState!.insertAllItems(
                index,
                length,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
        }
        else
        {
            long itemIndex = _computeItemIndex(index);
            long lengthWithSeparators = (_itemsCount == 0L) ? ((length * 2L) - 1L) : (length * 2L);
            _sliverAnimatedMultiBoxKey.currentState!.insertAllItems(
                itemIndex,
                lengthWithSeparators,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
        }
    }

    public virtual void removeItem(
        long index,
        Func<BuildContext, Animation<double>, Widget> builder,
        Duration? duration = null
    )
    {
        Func<BuildContext, long, Animation<double>, Widget>? removedSeparatorBuilderLocal =
            widget.removedSeparatorBuilder;
        if (removedSeparatorBuilderLocal is null)
        {
            _sliverAnimatedMultiBoxKey.currentState!.removeItem(
                index,
                builder,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
        }
        else
        {
            long itemIndex = _computeItemIndex(index);
            long visibleItemsCount = _itemsCount - _outgoingItemsCount;
            _sliverAnimatedMultiBoxKey.currentState!.removeItem(
                itemIndex,
                builder,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
            if (visibleItemsCount > 1L)
            {
                if (itemIndex == (visibleItemsCount - 1L))
                {
                    _sliverAnimatedMultiBoxKey.currentState!.removeItem(
                        itemIndex - 1L,
                        _toRemovedItemBuilder(removedSeparatorBuilderLocal, index - 1L),
                        duration: DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(duration)
                        )
                    );
                }
                else
                {
                    _sliverAnimatedMultiBoxKey.currentState!.removeItem(
                        itemIndex,
                        _toRemovedItemBuilder(removedSeparatorBuilderLocal, index),
                        duration: DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(duration)
                        )
                    );
                }
            }
        }
    }

    public virtual void removeAllItems(
        Func<BuildContext, Animation<double>, Widget> builder,
        Duration? duration = null
    )
    {
        Func<BuildContext, long, Animation<double>, Widget>? removedSeparatorBuilderLocal =
            widget.removedSeparatorBuilder;
        if (removedSeparatorBuilderLocal is null)
        {
            _sliverAnimatedMultiBoxKey.currentState!.removeAllItems(
                builder,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
            return;
        }
        for (long index = _itemsCount - 1L; index >= 0L; index--)
        {
            if ((checked(index) & 1L) == 0L)
            {
                _sliverAnimatedMultiBoxKey.currentState!.removeItem(
                    index,
                    builder,
                    duration: DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(duration)
                    )
                );
            }
            else
            {
                long itemIndex = checked(index / 2L);
                _sliverAnimatedMultiBoxKey.currentState!.removeItem(
                    index,
                    _toRemovedItemBuilder(removedSeparatorBuilderLocal, itemIndex),
                    duration: DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(duration)
                    )
                );
            }
        }
    }

    internal virtual long _itemsCount => _sliverAnimatedMultiBoxKey.currentState!._itemsCount;
    internal virtual long _outgoingItemsCount =>
        checked(_sliverAnimatedMultiBoxKey.currentState!._outgoingItems.Count);

    internal virtual long _computeItemIndex(long index)
    {
        if (index == 0L)
        {
            return index;
        }
        long itemsAndSeparatorsCount = _itemsCount;
        long separatorsCount = checked(itemsAndSeparatorsCount / 2L);
        long separatedItemsCount = _itemsCount - separatorsCount;
        var isNewLastIndex = index == separatedItemsCount;
        long indexAdjustedForSeparators = index * 2L;
        return isNewLastIndex ? (indexAdjustedForSeparators - 1L) : indexAdjustedForSeparators;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Func<BuildContext, Animation<double>, Widget> _toRemovedItemBuilder(
        Func<BuildContext, long, Animation<double>, Widget> builder,
        long index
    )
    {
        return (context, animation) =>
        {
            return builder(context, index, animation);
            throw new InvalidOperationException("Dart closure completed without a value.");
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _wrap(Widget sliver, Axis direction)
    {
        EdgeInsetsGeometry? effectivePadding = widget.padding;
        if (widget.padding is null)
        {
            MediaQueryData? mediaQuery = MediaQuery.maybeOf(context);
            if (mediaQuery is not null)
            {
                EdgeInsets mediaQueryHorizontalPadding = mediaQuery.padding.copyWith(
                    top: 0.0,
                    bottom: 0.0
                );
                EdgeInsets mediaQueryVerticalPadding = mediaQuery.padding.copyWith(
                    left: 0.0,
                    right: 0.0
                );
                effectivePadding = DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(
                    Equals(direction, Axis.vertical)
                        ? mediaQueryVerticalPadding
                        : mediaQueryHorizontalPadding
                );
                sliver = new MediaQuery(
                    data: mediaQuery.copyWith(
                        padding: Equals(direction, Axis.vertical)
                            ? mediaQueryHorizontalPadding
                            : mediaQueryVerticalPadding
                    ),
                    child: sliver
                );
            }
        }
        if (effectivePadding is not null)
        {
            sliver = new SliverPadding(padding: effectivePadding, sliver: sliver);
        }
        return new CustomScrollView(
            scrollDirection: widget.scrollDirection,
            reverse: widget.reverse,
            controller: widget.controller,
            primary: widget.primary,
            physics: widget.physics,
            clipBehavior: widget.clipBehavior,
            shrinkWrap: widget.shrinkWrap,
            scrollCacheExtent: widget.scrollCacheExtent,
            slivers: new List<Widget> { sliver }
        );
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
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
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

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
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

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}

public delegate Widget AnimatedItemBuilder(
    BuildContext context,
    long index,
    Animation<double> animation
);

public delegate Widget AnimatedRemovedItemBuilder(
    BuildContext context,
    Animation<double> animation
);

public static partial class Animated_scroll_viewLibrary
{
    internal static Duration _kDuration = Duration.Create(milliseconds: 300L);
}

internal class _ActiveItem__animated_scroll_view : IComparable<_ActiveItem__animated_scroll_view>
{
    public virtual AnimationController? controller { get; private set; }
    public virtual Func<BuildContext, Animation<double>, Widget>? removedItemBuilder
    {
        get;
        private set;
    }
    public virtual long itemIndex { get; set; } = default!;

    internal _ActiveItem__animated_scroll_view(AnimationController? controller, long itemIndex)
    {
        this.controller = controller;
        this.itemIndex = itemIndex;
        removedItemBuilder = null;
    }

    internal static _ActiveItem__animated_scroll_view CreateOutgoing(
        AnimationController? controller,
        long itemIndex,
        Func<BuildContext, Animation<double>, Widget>? removedItemBuilder
    )
    {
        var __instance = new _ActiveItem__animated_scroll_view(controller, itemIndex);
        __instance.controller = controller;
        __instance.itemIndex = itemIndex;
        __instance.removedItemBuilder = removedItemBuilder;
        return __instance;
    }

    internal static _ActiveItem__animated_scroll_view CreateIndex(long itemIndex)
    {
        var __instance = new _ActiveItem__animated_scroll_view(default!, itemIndex);
        __instance.itemIndex = itemIndex;
        __instance.controller = null;
        __instance.removedItemBuilder = null;
        return __instance;
    }

    public virtual long compareTo(_ActiveItem__animated_scroll_view other) =>
        DartRuntimePrimitives.ConvertValue<long>(itemIndex - other.itemIndex);

    public int CompareTo(_ActiveItem__animated_scroll_view? other) =>
        checked((int)compareTo(other!));
}

public class SliverAnimatedList : _SliverAnimatedMultiBoxAdaptor__animated_scroll_view
{
    public SliverAnimatedList(
        Key? key = null,
        Func<BuildContext, long, Animation<double>, Widget> itemBuilder = default!,
        Func<Key, long?>? findChildIndexCallback = null,
        long initialItemCount = 0
    )
        : base(
            key: key,
            itemBuilder: itemBuilder,
            findChildIndexCallback: findChildIndexCallback,
            initialItemCount: initialItemCount
        )
    {
        System.Diagnostics.Debug.Assert(initialItemCount >= 0L);
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new SliverAnimatedListState());

    public static SliverAnimatedListState of(BuildContext context)
    {
        SliverAnimatedListState? result = maybeOf(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (result is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "SliverAnimatedList.of() called with a context that does not contain a SliverAnimatedList.\n"
                            + "No SliverAnimatedListState ancestor could be found starting from the "
                            + "context that was passed to SliverAnimatedListState.of(). This can "
                            + "happen when the context provided is from the same StatefulWidget that "
                            + "built the AnimatedList. Please see the SliverAnimatedList documentation "
                            + "for examples of how to refer to an AnimatedListState object: "
                            + "https://api.flutter.dev/flutter/widgets/SliverAnimatedListState-class.html\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SliverAnimatedListState? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<SliverAnimatedListState>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SliverAnimatedListState
    : _SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<SliverAnimatedList>
{
    public override Widget build(BuildContext context)
    {
        return new SliverList(@delegate: _createDelegate());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SliverAnimatedGrid : _SliverAnimatedMultiBoxAdaptor__animated_scroll_view
{
    public virtual SliverGridDelegate gridDelegate { get; private set; } = default!;

    public SliverAnimatedGrid(
        Key? key = null,
        Func<BuildContext, long, Animation<double>, Widget> itemBuilder = default!,
        SliverGridDelegate gridDelegate = default!,
        Func<Key, long?>? findChildIndexCallback = null,
        long initialItemCount = 0
    )
        : base(
            key: key,
            itemBuilder: itemBuilder,
            findChildIndexCallback: findChildIndexCallback,
            initialItemCount: initialItemCount
        )
    {
        this.gridDelegate = gridDelegate;
        System.Diagnostics.Debug.Assert(initialItemCount >= 0L);
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new SliverAnimatedGridState());

    public static SliverAnimatedGridState of(BuildContext context)
    {
        SliverAnimatedGridState? result =
            context.findAncestorStateOfType<SliverAnimatedGridState>();
        DartRuntimePrimitives.Assert(() =>
        {
            if (result is null)
            {
                throw DartRuntimePrimitives.AsException(
                    FlutterError.Create(
                        "SliverAnimatedGrid.of() called with a context that does not contain a SliverAnimatedGrid.\n"
                            + "No SliverAnimatedGridState ancestor could be found starting from the "
                            + "context that was passed to SliverAnimatedGridState.of(). This can "
                            + "happen when the context provided is from the same StatefulWidget that "
                            + "built the AnimatedGrid. Please see the SliverAnimatedGrid documentation "
                            + "for examples of how to refer to an AnimatedGridState object: "
                            + "https://api.flutter.dev/flutter/widgets/SliverAnimatedGridState-class.html\n"
                            + "The context used was:\n"
                            + $"  {context}"
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SliverAnimatedGridState? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<SliverAnimatedGridState>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SliverAnimatedGridState
    : _SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<SliverAnimatedGrid>
{
    public override Widget build(BuildContext context)
    {
        return new SliverGrid(gridDelegate: widget.gridDelegate, @delegate: _createDelegate());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class _SliverAnimatedMultiBoxAdaptor__animated_scroll_view : StatefulWidget
{
    public virtual Func<BuildContext, long, Animation<double>, Widget> itemBuilder
    {
        get;
        private set;
    } = default!;
    public virtual Func<Key, long?>? findChildIndexCallback { get; private set; }
    public virtual long initialItemCount { get; private set; } = default!;

    internal _SliverAnimatedMultiBoxAdaptor__animated_scroll_view(
        Key? key = null,
        Func<BuildContext, long, Animation<double>, Widget> itemBuilder = default!,
        Func<Key, long?>? findChildIndexCallback = null,
        long initialItemCount = 0
    )
        : base(key: key)
    {
        this.itemBuilder = itemBuilder;
        this.findChildIndexCallback = findChildIndexCallback;
        this.initialItemCount = initialItemCount;
        System.Diagnostics.Debug.Assert(initialItemCount >= 0L);
    }
}

public abstract class _SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<T>
    : State<T>,
        TickerProviderStateMixin<T>
    where T : _SliverAnimatedMultiBoxAdaptor__animated_scroll_view
{
    internal virtual List<_ActiveItem__animated_scroll_view> _incomingItems { get; private set; } =
        new List<_ActiveItem__animated_scroll_view>();
    internal virtual List<_ActiveItem__animated_scroll_view> _outgoingItems { get; private set; } =
        new List<_ActiveItem__animated_scroll_view>();
    internal virtual long _itemsCount { get; set; } = 0L;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _itemsCount = widget.initialItemCount;
    }

    public override void dispose()
    {
        foreach (
            _ActiveItem__animated_scroll_view item in _incomingItems.followedBy(
                _outgoingItems.Cast<_ActiveItem__animated_scroll_view>()
            )
        )
        {
            item.controller!.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
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

    internal virtual _ActiveItem__animated_scroll_view? _removeActiveItemAt(
        List<_ActiveItem__animated_scroll_view> items,
        long itemIndex
    )
    {
        long i = CollectionsLibrary.binarySearch(
            items,
            _ActiveItem__animated_scroll_view.CreateIndex(itemIndex)
        );
        return (i == -1L) ? null : items.removeAt(i);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _ActiveItem__animated_scroll_view? _activeItemAt(
        List<_ActiveItem__animated_scroll_view> items,
        long itemIndex
    )
    {
        long i = CollectionsLibrary.binarySearch(
            items,
            _ActiveItem__animated_scroll_view.CreateIndex(itemIndex)
        );
        return (i == -1L) ? null : items[(int)i];
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _indexToItemIndex(long index)
    {
        var itemIndexLocal = index;
        foreach (_ActiveItem__animated_scroll_view item in _outgoingItems)
        {
            if (item.itemIndex <= itemIndexLocal)
            {
                itemIndexLocal += 1L;
            }
            else
            {
                break;
            }
        }
        return itemIndexLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _itemIndexToIndex(long itemIndex)
    {
        var index = itemIndex;
        foreach (_ActiveItem__animated_scroll_view item in _outgoingItems)
        {
            DartRuntimePrimitives.Assert(() => item.itemIndex != itemIndex);
            if (item.itemIndex < itemIndex)
            {
                index -= 1L;
            }
            else
            {
                break;
            }
        }
        return index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SliverChildDelegate _createDelegate()
    {
        return new SliverChildBuilderDelegate(
            _itemBuilder,
            childCount: _itemsCount,
            findChildIndexCallback: (widget.findChildIndexCallback is null)
                ? null
                : (
                    (key) =>
                    {
                        long? index = widget.findChildIndexCallback!(key);
                        return (index is not null)
                            ? _indexToItemIndex(
                                DartRuntimePrimitives.RequireValue(
                                    DartRuntimePrimitives.RequireValue(index)
                                )
                            )
                            : null;
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _itemBuilder(BuildContext context, long itemIndex)
    {
        _ActiveItem__animated_scroll_view? outgoingItem = _activeItemAt(_outgoingItems, itemIndex);
        if (outgoingItem is not null)
        {
            return outgoingItem.removedItemBuilder!(context, outgoingItem.controller!.view);
        }
        _ActiveItem__animated_scroll_view? incomingItem = _activeItemAt(_incomingItems, itemIndex);
        Animation<double> animation =
            incomingItem?.controller?.view ?? AnimationsLibrary.kAlwaysCompleteAnimation;
        return widget.itemBuilder(context, _itemIndexToIndex(itemIndex), animation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void insertItem(long index, Duration? duration = null)
    {
        DartRuntimePrimitives.Assert(() => index >= 0L);
        long itemIndexLocal = _indexToItemIndex(index);
        DartRuntimePrimitives.Assert(() =>
            (itemIndexLocal >= 0L) && (itemIndexLocal <= _itemsCount)
        );
        foreach (_ActiveItem__animated_scroll_view item in _incomingItems)
        {
            if (item.itemIndex >= itemIndexLocal)
            {
                item.itemIndex += 1L;
            }
        }
        foreach (_ActiveItem__animated_scroll_view itemLocal in _outgoingItems)
        {
            if (itemLocal.itemIndex >= itemIndexLocal)
            {
                itemLocal.itemIndex += 1L;
            }
        }
        var controllerLocal = new AnimationController(
            duration: DartRuntimePrimitives.RequireValue(duration),
            vsync: this
        );
        var incomingItem = new _ActiveItem__animated_scroll_view(controllerLocal, itemIndexLocal);
        setState(() =>
        {
            DartRuntimePrimitives.Ignore(
                (
                    (Func<List<_ActiveItem__animated_scroll_view>>)(
                        () =>
                        {
                            var __cascade = _incomingItems;
                            __cascade.Add(incomingItem);
                            __cascade.sort();
                            return __cascade;
                        }
                    )
                )()
            );
            _itemsCount += 1L;
        });
        DartRuntimePrimitives.Ignore(
            controllerLocal
                .forward()
                .then(
                    (_) =>
                    {
                        _removeActiveItemAt(_incomingItems, incomingItem.itemIndex)!
                            .controller!.dispose();
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
        );
    }

    public virtual void insertAllItems(long index, long length, Duration? duration = null)
    {
        for (var i = 0L; i < length; i++)
        {
            insertItem(
                index + i,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
        }
    }

    public virtual void removeItem(
        long index,
        Func<BuildContext, Animation<double>, Widget> builder,
        Duration? duration = null
    )
    {
        DartRuntimePrimitives.Assert(() => index >= 0L);
        long itemIndexLocal = _indexToItemIndex(index);
        DartRuntimePrimitives.Assert(() =>
            (itemIndexLocal >= 0L) && (itemIndexLocal < _itemsCount)
        );
        DartRuntimePrimitives.Assert(() => _activeItemAt(_outgoingItems, itemIndexLocal) is null);
        _ActiveItem__animated_scroll_view? incomingItem = _removeActiveItemAt(
            _incomingItems,
            itemIndexLocal
        );
        AnimationController controllerLocal =
            incomingItem?.controller
            ?? new AnimationController(
                duration: DartRuntimePrimitives.RequireValue(duration),
                value: 1.0,
                vsync: this
            );
        var outgoingItem = _ActiveItem__animated_scroll_view.CreateOutgoing(
            controllerLocal,
            itemIndexLocal,
            builder
        );
        setState(() =>
        {
            DartRuntimePrimitives.Ignore(
                (
                    (Func<List<_ActiveItem__animated_scroll_view>>)(
                        () =>
                        {
                            var __cascade = _outgoingItems;
                            __cascade.Add(outgoingItem);
                            __cascade.sort();
                            return __cascade;
                        }
                    )
                )()
            );
        });
        DartRuntimePrimitives.Ignore(
            controllerLocal
                .reverse()
                .then(
                    (value) =>
                    {
                        _removeActiveItemAt(_outgoingItems, outgoingItem.itemIndex)!
                            .controller!.dispose();
                        foreach (_ActiveItem__animated_scroll_view item in _incomingItems)
                        {
                            if (item.itemIndex > outgoingItem.itemIndex)
                            {
                                item.itemIndex -= 1L;
                            }
                        }
                        foreach (_ActiveItem__animated_scroll_view itemLocal in _outgoingItems)
                        {
                            if (itemLocal.itemIndex > outgoingItem.itemIndex)
                            {
                                itemLocal.itemIndex -= 1L;
                            }
                        }
                        setState(() =>
                        {
                            _ = _itemsCount -= 1L;
                        });
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
        );
    }

    public virtual void removeAllItems(
        Func<BuildContext, Animation<double>, Widget> builder,
        Duration? duration = null
    )
    {
        DartRuntimePrimitives.Assert(() => _itemsCount >= 0L);
        DartRuntimePrimitives.Assert(() => (_itemsCount - checked(_outgoingItems.Count)) >= 0L);
        long visibleItemCount = _itemsCount - checked(_outgoingItems.Count);
        for (long i = visibleItemCount - 1L; i >= 0L; i--)
        {
            removeItem(
                i,
                builder,
                duration: DartRuntimePrimitives.RequireValue(
                    DartRuntimePrimitives.RequireValue(duration)
                )
            );
        }
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
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
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
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }
}
