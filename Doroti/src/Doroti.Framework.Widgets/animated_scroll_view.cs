// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/animated_scroll_view.dart
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

public class AnimatedList : _AnimatedScrollView__animated_scroll_view
{
    public AnimatedList(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder = default!, long initialItemCount = 0, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null) : base(key: key, itemBuilder: itemBuilder, initialItemCount: initialItemCount, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: DartRuntimePrimitives.RequireValue(primary), physics: physics, shrinkWrap: shrinkWrap, padding: padding, clipBehavior: clipBehavior, scrollCacheExtent: scrollCacheExtent)
    {
        System.Diagnostics.Debug.Assert((initialItemCount >= 0L));
    }

    public static AnimatedList CreateSeparated(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder = default!, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> separatorBuilder = default!, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> removedSeparatorBuilder = default!, long initialItemCount = 0, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null)
    {
        var __instance = new AnimatedList(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
        return __instance;
    }

    public static AnimatedListState of(BuildContext context)
    {
        AnimatedListState? result = ((AnimatedListState?)(object?)AnimatedList.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("AnimatedList.of() called with a context that does not contain an AnimatedList."), new global::Doroti.Framework.Foundation.ErrorDescription("No AnimatedList ancestor could be found starting from the context that was passed to AnimatedList.of()."), new global::Doroti.Framework.Foundation.ErrorHint("This can happen when the context provided is from the same StatefulWidget that " + "built the AnimatedList. Please see the AnimatedList documentation for examples " + "of how to refer to an AnimatedListState object:\n" + "  https://api.flutter.dev/flutter/widgets/AnimatedListState-class.html"), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AnimatedListState? maybeOf(BuildContext context)
    {
        return ((AnimatedListState?)(object?)context.findAncestorStateOfType<AnimatedListState>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _computeChildCountWithSeparators(long itemCount)
    {
        if ((itemCount == 0L))
        {
            return 0L;
        }
        return ((itemCount * 2L) - 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new AnimatedListState());
}

public class AnimatedListState : _AnimatedScrollViewState__animated_scroll_view<AnimatedList>
{
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)_wrap(new SliverAnimatedList(key: this._sliverAnimatedMultiBoxKey, itemBuilder: (global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>)this.widget.itemBuilder, initialItemCount: this.widget.initialItemCount), this.widget.scrollDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AnimatedGrid : _AnimatedScrollView__animated_scroll_view
{
    public virtual global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate { get; private set; } = default!;

    public AnimatedGrid(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder = default!, global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate = default!, long initialItemCount = 0, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null) : base(key: key, itemBuilder: itemBuilder, initialItemCount: initialItemCount, scrollDirection: scrollDirection, reverse: reverse, controller: controller, primary: DartRuntimePrimitives.RequireValue(primary), physics: physics, padding: padding, clipBehavior: clipBehavior, scrollCacheExtent: scrollCacheExtent)
    {
        this.gridDelegate = gridDelegate;
        System.Diagnostics.Debug.Assert((initialItemCount >= 0L));
    }

    public static AnimatedGridState of(BuildContext context)
    {
        AnimatedGridState? result = ((AnimatedGridState?)(object?)AnimatedGrid.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("AnimatedGrid.of() called with a context that does not contain an AnimatedGrid."), new global::Doroti.Framework.Foundation.ErrorDescription("No AnimatedGrid ancestor could be found starting from the context that was passed to AnimatedGrid.of()."), new global::Doroti.Framework.Foundation.ErrorHint("This can happen when the context provided is from the same StatefulWidget that " + "built the AnimatedGrid. Please see the AnimatedGrid documentation for examples " + "of how to refer to an AnimatedGridState object:\n" + "  https://api.flutter.dev/flutter/widgets/AnimatedGridState-class.html"), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static AnimatedGridState? maybeOf(BuildContext context)
    {
        return ((AnimatedGridState?)(object?)context.findAncestorStateOfType<AnimatedGridState>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new AnimatedGridState());
}

public class AnimatedGridState : _AnimatedScrollViewState__animated_scroll_view<AnimatedGrid>
{
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)_wrap(new SliverAnimatedGrid(key: this._sliverAnimatedMultiBoxKey, gridDelegate: ((AnimatedGrid)(object)this.widget).gridDelegate, itemBuilder: (global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>)this.widget.itemBuilder, initialItemCount: this.widget.initialItemCount), this.widget.scrollDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class _AnimatedScrollView__animated_scroll_view : StatefulWidget
{
    public virtual global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder { get; private set; } = default!;
    public virtual global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>? removedSeparatorBuilder { get; private set; }
    public virtual long initialItemCount { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual bool shrinkWrap { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent { get; private set; }

    internal _AnimatedScrollView__animated_scroll_view(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder = default!, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>? removedSeparatorBuilder = null, long initialItemCount = 0, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.vertical, bool reverse = false, ScrollController? controller = null, bool? primary = null, ScrollPhysics? physics = null, bool shrinkWrap = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null) : base(key: key)
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
        System.Diagnostics.Debug.Assert((initialItemCount >= 0L));
    }

}

public abstract class _AnimatedScrollViewState__animated_scroll_view<T> : State<T>, TickerProviderStateMixin<T> where T : _AnimatedScrollView__animated_scroll_view
{
    internal virtual GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>> _sliverAnimatedMultiBoxKey { get; private set; } = GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>.Create();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual void insertItem(long index, Duration? duration = null)
    {
        if ((((_AnimatedScrollView__animated_scroll_view)(object)this.widget).removedSeparatorBuilder is null))
        {
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.insertItem(index, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
        else
        {
            long itemIndex = _computeItemIndex(index);
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.insertItem(itemIndex, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            if ((this._itemsCount > 1L))
            {
                ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.insertItem(itemIndex, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            }
        }
    }

    public virtual void insertAllItems(long index, long length, Duration? duration = null, bool isAsync = false)
    {
        if ((((_AnimatedScrollView__animated_scroll_view)(object)this.widget).removedSeparatorBuilder is null))
        {
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.insertAllItems(index, length, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
        else
        {
            long itemIndex = _computeItemIndex(index);
            long lengthWithSeparators = ((this._itemsCount == 0L) ? ((length * 2L) - 1L) : (length * 2L));
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.insertAllItems(itemIndex, lengthWithSeparators, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
    }

    public virtual void removeItem(long index, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, Duration? duration = null)
    {
        global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>? removedSeparatorBuilderLocal = ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).removedSeparatorBuilder;
        if ((removedSeparatorBuilderLocal is null))
        {
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(index, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
        else
        {
            long itemIndex = _computeItemIndex(index);
            long visibleItemsCount = (this._itemsCount - this._outgoingItemsCount);
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(itemIndex, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            if ((visibleItemsCount > 1L))
            {
                if ((itemIndex == (visibleItemsCount - 1L)))
                {
                    ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem((itemIndex - 1L), _toRemovedItemBuilder((global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>)removedSeparatorBuilderLocal, (index - 1L)), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
                }
                else
                {
                    ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(itemIndex, _toRemovedItemBuilder((global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>)removedSeparatorBuilderLocal, index), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
                }
            }
        }
    }

    public virtual void removeAllItems(global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, Duration? duration = null)
    {
        global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>? removedSeparatorBuilderLocal = ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).removedSeparatorBuilder;
        if ((removedSeparatorBuilderLocal is null))
        {
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeAllItems((global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            return;
        }
        for (long index = (this._itemsCount - 1L); (index >= 0L); index--)
        {
            if (((checked((long)(index)) & 1L) == 0L))
            {
                ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(index, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            }
            else
            {
                long itemIndex = (checked((long)(index / 2L)));
                ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(index, _toRemovedItemBuilder((global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>)removedSeparatorBuilderLocal, itemIndex), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            }
        }
    }

    internal virtual long _itemsCount => ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!._itemsCount;
    internal virtual long _outgoingItemsCount => checked((long)(((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!._outgoingItems.Count));
    internal virtual long _computeItemIndex(long index)
    {
        if ((index == 0L))
        {
            return index;
        }
        long itemsAndSeparatorsCount = this._itemsCount;
        long separatorsCount = (checked((long)(itemsAndSeparatorsCount / 2L)));
        long separatedItemsCount = (this._itemsCount - separatorsCount);
        var isNewLastIndex = (index == separatedItemsCount);
        long indexAdjustedForSeparators = (index * 2L);
        return (isNewLastIndex ? (indexAdjustedForSeparators - 1L) : indexAdjustedForSeparators);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> _toRemovedItemBuilder(global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, long index)
    {
        return ((global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)((context, animation) =>
        {
            return builder(context, index, animation);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _wrap(Widget sliver, global::Doroti.Framework.Painting.Axis direction)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding = ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).padding;
        if ((((_AnimatedScrollView__animated_scroll_view)(object)this.widget).padding is null))
        {
            MediaQueryData? mediaQuery = ((MediaQueryData?)(object?)MediaQuery.maybeOf(this.context));
            if ((mediaQuery is not null))
            {
                global::Doroti.Framework.Painting.EdgeInsets mediaQueryHorizontalPadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)((MediaQueryData)mediaQuery).padding.copyWith(top: 0.0, bottom: 0.0));
                global::Doroti.Framework.Painting.EdgeInsets mediaQueryVerticalPadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)((MediaQueryData)mediaQuery).padding.copyWith(left: 0.0, right: 0.0));
                effectivePadding = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(((object.Equals(direction, global::Doroti.Framework.Painting.Axis.vertical)) ? mediaQueryVerticalPadding : mediaQueryHorizontalPadding));
                sliver = new MediaQuery(data: mediaQuery.copyWith(padding: ((object.Equals(direction, global::Doroti.Framework.Painting.Axis.vertical)) ? mediaQueryHorizontalPadding : mediaQueryVerticalPadding)), child: sliver);
            }
        }
        if ((effectivePadding is not null))
        {
            sliver = new SliverPadding(padding: effectivePadding, sliver: sliver);
        }
        return ((Widget)(object?)new CustomScrollView(scrollDirection: ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).scrollDirection, reverse: ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).reverse, controller: ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).controller, primary: ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).primary, physics: ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).physics, clipBehavior: ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).clipBehavior, shrinkWrap: ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).shrinkWrap, scrollCacheExtent: ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).scrollCacheExtent, slivers: new List<Widget> { sliver }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        this._tickerModeNotifier = null;
        base.dispose();
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

public delegate Widget AnimatedItemBuilder(BuildContext context, long index, global::Doroti.Framework.Animation.Animation<double> animation);

public delegate Widget AnimatedRemovedItemBuilder(BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation);

public static partial class Animated_scroll_viewLibrary
{
    internal static Duration _kDuration = Duration.Create(milliseconds: 300L);
}

internal class _ActiveItem__animated_scroll_view : IComparable<_ActiveItem__animated_scroll_view>
{
    public virtual global::Doroti.Framework.Animation.AnimationController? controller { get; private set; }
    public virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>? removedItemBuilder { get; private set; }
    public virtual long itemIndex { get; set; } = default!;

    internal _ActiveItem__animated_scroll_view(global::Doroti.Framework.Animation.AnimationController? controller, long itemIndex)
    {
        this.controller = controller;
        this.itemIndex = itemIndex;
        this.removedItemBuilder = null;
    }

    internal static _ActiveItem__animated_scroll_view CreateOutgoing(global::Doroti.Framework.Animation.AnimationController? controller, long itemIndex, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>? removedItemBuilder)
    {
        var __instance = new _ActiveItem__animated_scroll_view(default!, default!);
        __instance.controller = controller;
        __instance.itemIndex = itemIndex;
        __instance.removedItemBuilder = removedItemBuilder;
        return __instance;
    }

    internal static _ActiveItem__animated_scroll_view CreateIndex(long itemIndex)
    {
        var __instance = new _ActiveItem__animated_scroll_view(default!, default!);
        __instance.itemIndex = itemIndex;
        __instance.controller = null;
        __instance.removedItemBuilder = null;
        return __instance;
    }

    public virtual long compareTo(_ActiveItem__animated_scroll_view other) => DartRuntimePrimitives.ConvertValue<long>((this.itemIndex - ((_ActiveItem__animated_scroll_view)other).itemIndex));
    public int CompareTo(_ActiveItem__animated_scroll_view? other) => checked((int)compareTo(other!));
}

public class SliverAnimatedList : _SliverAnimatedMultiBoxAdaptor__animated_scroll_view
{
    public SliverAnimatedList(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long initialItemCount = 0) : base(key: key, itemBuilder: itemBuilder, findChildIndexCallback: findChildIndexCallback, initialItemCount: initialItemCount)
    {
        System.Diagnostics.Debug.Assert((initialItemCount >= 0L));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new SliverAnimatedListState());
    public static SliverAnimatedListState of(BuildContext context)
    {
        SliverAnimatedListState? result = ((SliverAnimatedListState?)(object?)SliverAnimatedList.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("SliverAnimatedList.of() called with a context that does not contain a SliverAnimatedList.\n" + "No SliverAnimatedListState ancestor could be found starting from the " + "context that was passed to SliverAnimatedListState.of(). This can " + "happen when the context provided is from the same StatefulWidget that " + "built the AnimatedList. Please see the SliverAnimatedList documentation " + "for examples of how to refer to an AnimatedListState object: " + "https://api.flutter.dev/flutter/widgets/SliverAnimatedListState-class.html\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SliverAnimatedListState? maybeOf(BuildContext context)
    {
        return ((SliverAnimatedListState?)(object?)context.findAncestorStateOfType<SliverAnimatedListState>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverAnimatedListState : _SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<SliverAnimatedList>
{
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new SliverList(@delegate: _createDelegate()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverAnimatedGrid : _SliverAnimatedMultiBoxAdaptor__animated_scroll_view
{
    public virtual global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate { get; private set; } = default!;

    public SliverAnimatedGrid(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder = default!, global::Doroti.Framework.Rendering.SliverGridDelegate gridDelegate = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long initialItemCount = 0) : base(key: key, itemBuilder: itemBuilder, findChildIndexCallback: findChildIndexCallback, initialItemCount: initialItemCount)
    {
        this.gridDelegate = gridDelegate;
        System.Diagnostics.Debug.Assert((initialItemCount >= 0L));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new SliverAnimatedGridState());
    public static SliverAnimatedGridState of(BuildContext context)
    {
        SliverAnimatedGridState? result = ((SliverAnimatedGridState?)(object?)context.findAncestorStateOfType<SliverAnimatedGridState>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("SliverAnimatedGrid.of() called with a context that does not contain a SliverAnimatedGrid.\n" + "No SliverAnimatedGridState ancestor could be found starting from the " + "context that was passed to SliverAnimatedGridState.of(). This can " + "happen when the context provided is from the same StatefulWidget that " + "built the AnimatedGrid. Please see the SliverAnimatedGrid documentation " + "for examples of how to refer to an AnimatedGridState object: " + "https://api.flutter.dev/flutter/widgets/SliverAnimatedGridState-class.html\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SliverAnimatedGridState? maybeOf(BuildContext context)
    {
        return ((SliverAnimatedGridState?)(object?)context.findAncestorStateOfType<SliverAnimatedGridState>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverAnimatedGridState : _SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<SliverAnimatedGrid>
{
    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new SliverGrid(gridDelegate: ((SliverAnimatedGrid)(object)this.widget).gridDelegate, @delegate: _createDelegate()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class _SliverAnimatedMultiBoxAdaptor__animated_scroll_view : StatefulWidget
{
    public virtual global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback { get; private set; }
    public virtual long initialItemCount { get; private set; } = default!;

    internal _SliverAnimatedMultiBoxAdaptor__animated_scroll_view(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> itemBuilder = default!, global::System.Func<global::Doroti.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long initialItemCount = 0) : base(key: key)
    {
        this.itemBuilder = itemBuilder;
        this.findChildIndexCallback = findChildIndexCallback;
        this.initialItemCount = initialItemCount;
        System.Diagnostics.Debug.Assert((initialItemCount >= 0L));
    }

}

public abstract class _SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<T> : State<T>, TickerProviderStateMixin<T> where T : _SliverAnimatedMultiBoxAdaptor__animated_scroll_view
{
    internal virtual List<_ActiveItem__animated_scroll_view> _incomingItems { get; private set; } = new List<_ActiveItem__animated_scroll_view>();
    internal virtual List<_ActiveItem__animated_scroll_view> _outgoingItems { get; private set; } = new List<_ActiveItem__animated_scroll_view>();
    internal virtual long _itemsCount { get; set; } = 0L;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _itemsCount = ((_SliverAnimatedMultiBoxAdaptor__animated_scroll_view)(object)this.widget).initialItemCount;
    }

    public override void dispose()
    {
        foreach (_ActiveItem__animated_scroll_view item in this._incomingItems.followedBy(this._outgoingItems.Cast<_ActiveItem__animated_scroll_view>()))
        {
            ((_ActiveItem__animated_scroll_view)item).controller!.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual _ActiveItem__animated_scroll_view? _removeActiveItemAt(List<_ActiveItem__animated_scroll_view> items, long itemIndex)
    {
        long i = global::Doroti.Framework.Foundation.CollectionsLibrary.binarySearch(items, _ActiveItem__animated_scroll_view.CreateIndex(itemIndex));
        return ((i == -1L) ? null : items.removeAt(i));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _ActiveItem__animated_scroll_view? _activeItemAt(List<_ActiveItem__animated_scroll_view> items, long itemIndex)
    {
        long i = global::Doroti.Framework.Foundation.CollectionsLibrary.binarySearch(items, _ActiveItem__animated_scroll_view.CreateIndex(itemIndex));
        return ((i == -1L) ? null : items[(int)(i)]);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _indexToItemIndex(long index)
    {
        var itemIndexLocal = index;
        foreach (_ActiveItem__animated_scroll_view item in this._outgoingItems)
        {
            if ((((_ActiveItem__animated_scroll_view)item).itemIndex <= itemIndexLocal))
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
        foreach (_ActiveItem__animated_scroll_view item in this._outgoingItems)
        {
            DartRuntimePrimitives.Assert(() => (((_ActiveItem__animated_scroll_view)item).itemIndex != itemIndex));
            if ((((_ActiveItem__animated_scroll_view)item).itemIndex < itemIndex))
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
        return ((SliverChildDelegate)(object?)new SliverChildBuilderDelegate((global::System.Func<BuildContext, long, Widget>)this._itemBuilder, childCount: this._itemsCount, findChildIndexCallback: ((global::System.Func<global::Doroti.Framework.Foundation.Key, long?>)((((_SliverAnimatedMultiBoxAdaptor__animated_scroll_view)(object)this.widget).findChildIndexCallback is null) ? null : ((key) =>
        {
            long? index = ((_SliverAnimatedMultiBoxAdaptor__animated_scroll_view)(object)this.widget).findChildIndexCallback!(key);
            return ((index is not null) ? _indexToItemIndex(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(index))) : null);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _itemBuilder(BuildContext context, long itemIndex)
    {
        _ActiveItem__animated_scroll_view? outgoingItem = ((_ActiveItem__animated_scroll_view?)(object?)_activeItemAt(this._outgoingItems, itemIndex));
        if ((outgoingItem is not null))
        {
            return ((_ActiveItem__animated_scroll_view)outgoingItem).removedItemBuilder!(context, ((_ActiveItem__animated_scroll_view)outgoingItem).controller!.view);
        }
        _ActiveItem__animated_scroll_view? incomingItem = ((_ActiveItem__animated_scroll_view?)(object?)_activeItemAt(this._incomingItems, itemIndex));
        global::Doroti.Framework.Animation.Animation<double> animation = (incomingItem?.controller?.view ?? global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation);
        return this.widget.itemBuilder(context, _itemIndexToIndex(itemIndex), animation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void insertItem(long index, Duration? duration = null)
    {
        DartRuntimePrimitives.Assert(() => (index >= 0L));
        long itemIndexLocal = _indexToItemIndex(index);
        DartRuntimePrimitives.Assert(() => ((itemIndexLocal >= 0L) && (itemIndexLocal <= this._itemsCount)));
        foreach (_ActiveItem__animated_scroll_view item in this._incomingItems)
        {
            if ((((_ActiveItem__animated_scroll_view)item).itemIndex >= itemIndexLocal))
            {
                item.itemIndex += 1L;
            }
        }
        foreach (_ActiveItem__animated_scroll_view itemLocal in this._outgoingItems)
        {
            if ((((_ActiveItem__animated_scroll_view)itemLocal).itemIndex >= itemIndexLocal))
            {
                itemLocal.itemIndex += 1L;
            }
        }
        var controllerLocal = new global::Doroti.Framework.Animation.AnimationController(duration: DartRuntimePrimitives.RequireValue(duration), vsync: this);
        var incomingItem = new _ActiveItem__animated_scroll_view(controllerLocal, itemIndexLocal);
        setState(((global::System.Action)(() =>
        {
            DartRuntimePrimitives.Ignore(((Func<List<_ActiveItem__animated_scroll_view>>)(() =>
            {
                var __cascade = this._incomingItems;
                __cascade.Add(incomingItem);
                __cascade.sort();
                return __cascade;
            }))());
            _itemsCount += 1L;
        })));
        DartRuntimePrimitives.Ignore(controllerLocal.forward().then(((global::System.Func<object?, object>)((_) =>
        {
            _removeActiveItemAt(this._incomingItems, ((_ActiveItem__animated_scroll_view)incomingItem).itemIndex)!.controller!.dispose();
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
    }

    public virtual void insertAllItems(long index, long length, Duration? duration = null)
    {
        for (var i = 0L; (i < length); i++)
        {
            insertItem((index + i), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
    }

    public virtual void removeItem(long index, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, Duration? duration = null)
    {
        DartRuntimePrimitives.Assert(() => (index >= 0L));
        long itemIndexLocal = _indexToItemIndex(index);
        DartRuntimePrimitives.Assert(() => ((itemIndexLocal >= 0L) && (itemIndexLocal < this._itemsCount)));
        DartRuntimePrimitives.Assert(() => (_activeItemAt(this._outgoingItems, itemIndexLocal) is null));
        _ActiveItem__animated_scroll_view? incomingItem = ((_ActiveItem__animated_scroll_view?)(object?)_removeActiveItemAt(this._incomingItems, itemIndexLocal));
        global::Doroti.Framework.Animation.AnimationController controllerLocal = (incomingItem?.controller ?? new global::Doroti.Framework.Animation.AnimationController(duration: DartRuntimePrimitives.RequireValue(duration), value: 1.0, vsync: this));
        var outgoingItem = _ActiveItem__animated_scroll_view.CreateOutgoing(controllerLocal, itemIndexLocal, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder);
        setState(((global::System.Action)(() =>
        {
            DartRuntimePrimitives.Ignore(((Func<List<_ActiveItem__animated_scroll_view>>)(() =>
            {
                var __cascade = this._outgoingItems;
                __cascade.Add(outgoingItem);
                __cascade.sort();
                return __cascade;
            }))());
        })));
        DartRuntimePrimitives.Ignore(controllerLocal.reverse().then(((global::System.Func<object?, object>)((value) =>
        {
            _removeActiveItemAt(this._outgoingItems, ((_ActiveItem__animated_scroll_view)outgoingItem).itemIndex)!.controller!.dispose();
            foreach (_ActiveItem__animated_scroll_view item in this._incomingItems)
            {
                if ((((_ActiveItem__animated_scroll_view)item).itemIndex > ((_ActiveItem__animated_scroll_view)outgoingItem).itemIndex))
                {
                    item.itemIndex -= 1L;
                }
            }
            foreach (_ActiveItem__animated_scroll_view itemLocal in this._outgoingItems)
            {
                if ((((_ActiveItem__animated_scroll_view)itemLocal).itemIndex > ((_ActiveItem__animated_scroll_view)outgoingItem).itemIndex))
                {
                    itemLocal.itemIndex -= 1L;
                }
            }
            setState(((global::System.Action)(() => { _ = _itemsCount -= 1L; })));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
    }

    public virtual void removeAllItems(global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, Duration? duration = null)
    {
        DartRuntimePrimitives.Assert(() => (this._itemsCount >= 0L));
        DartRuntimePrimitives.Assert(() => ((this._itemsCount - checked((long)(this._outgoingItems.Count))) >= 0L));
        long visibleItemCount = (this._itemsCount - checked((long)(this._outgoingItems.Count)));
        for (long i = (visibleItemCount - 1L); (i >= 0L); i--)
        {
            removeItem(i, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
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
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}
