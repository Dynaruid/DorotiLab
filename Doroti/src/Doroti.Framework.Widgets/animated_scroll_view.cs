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
        AnimatedListState? result__7599 = ((AnimatedListState?)(object?)AnimatedList.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__7599 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("AnimatedList.of() called with a context that does not contain an AnimatedList."), new global::Doroti.Framework.Foundation.ErrorDescription("No AnimatedList ancestor could be found starting from the context that was passed to AnimatedList.of()."), new global::Doroti.Framework.Foundation.ErrorHint("This can happen when the context provided is from the same StatefulWidget that " + "built the AnimatedList. Please see the AnimatedList documentation for examples " + "of how to refer to an AnimatedListState object:\n" + "  https://api.flutter.dev/flutter/widgets/AnimatedListState-class.html"), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__7599!;
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
        AnimatedGridState? result__15660 = ((AnimatedGridState?)(object?)AnimatedGrid.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__15660 is null))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("AnimatedGrid.of() called with a context that does not contain an AnimatedGrid."), new global::Doroti.Framework.Foundation.ErrorDescription("No AnimatedGrid ancestor could be found starting from the context that was passed to AnimatedGrid.of()."), new global::Doroti.Framework.Foundation.ErrorHint("This can happen when the context provided is from the same StatefulWidget that " + "built the AnimatedGrid. Please see the AnimatedGrid documentation for examples " + "of how to refer to an AnimatedGridState object:\n" + "  https://api.flutter.dev/flutter/widgets/AnimatedGridState-class.html"), context.describeElement("The context used was") }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__15660!;
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
            long itemIndex__25861 = _computeItemIndex(index);
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.insertItem(itemIndex__25861, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            if ((this._itemsCount > 1L))
            {
                ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.insertItem(itemIndex__25861, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
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
            long itemIndex__26919 = _computeItemIndex(index);
            long lengthWithSeparators__26973 = ((this._itemsCount == 0L) ? ((length * 2L) - 1L) : (length * 2L));
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.insertAllItems(itemIndex__26919, lengthWithSeparators__26973, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
    }

    public virtual void removeItem(long index, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, Duration? duration = null)
    {
        global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>? removedSeparatorBuilder__28236 = ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).removedSeparatorBuilder;
        if ((removedSeparatorBuilder__28236 is null))
        {
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(index, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
        else
        {
            long itemIndex__28517 = _computeItemIndex(index);
            long visibleItemsCount__28794 = (this._itemsCount - this._outgoingItemsCount);
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(itemIndex__28517, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            if ((visibleItemsCount__28794 > 1L))
            {
                if ((itemIndex__28517 == (visibleItemsCount__28794 - 1L)))
                {
                    ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem((itemIndex__28517 - 1L), _toRemovedItemBuilder((global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>)removedSeparatorBuilder__28236, (index - 1L)), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
                }
                else
                {
                    ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(itemIndex__28517, _toRemovedItemBuilder((global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>)removedSeparatorBuilder__28236, index), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
                }
            }
        }
    }

    public virtual void removeAllItems(global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, Duration? duration = null)
    {
        global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>? removedSeparatorBuilder__30639 = ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).removedSeparatorBuilder;
        if ((removedSeparatorBuilder__30639 is null))
        {
            ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeAllItems((global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            return;
        }
        for (long index__31066 = (this._itemsCount - 1L); (index__31066 >= 0L); index__31066--)
        {
            if (((checked((long)(index__31066)) & 1L) == 0L))
            {
                ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(index__31066, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
            }
            else
            {
                long itemIndex__31329 = (checked((long)(index__31066 / 2L)));
                ((GlobalKey<_SliverAnimatedMultiBoxAdaptorState__animated_scroll_view<_SliverAnimatedMultiBoxAdaptor__animated_scroll_view>>)this._sliverAnimatedMultiBoxKey).currentState!.removeItem(index__31066, _toRemovedItemBuilder((global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget>)removedSeparatorBuilder__30639, itemIndex__31329), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
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
        long itemsAndSeparatorsCount__31949 = this._itemsCount;
        long separatorsCount__32002 = (checked((long)(itemsAndSeparatorsCount__31949 / 2L)));
        long separatedItemsCount__32064 = (this._itemsCount - separatorsCount__32002);
        var isNewLastIndex__32128 = (index == separatedItemsCount__32064);
        long indexAdjustedForSeparators__32189 = (index * 2L);
        return (isNewLastIndex__32128 ? (indexAdjustedForSeparators__32189 - 1L) : indexAdjustedForSeparators__32189);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> _toRemovedItemBuilder(global::System.Func<BuildContext, long, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, long index)
    {
        return ((global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)((context, animation) => {
return builder(context, index, animation);
throw new InvalidOperationException("Dart closure completed without a value.");
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _wrap(Widget sliver, global::Doroti.Framework.Painting.Axis direction)
    {
        global::Doroti.Framework.Painting.EdgeInsetsGeometry? effectivePadding__32729 = ((_AnimatedScrollView__animated_scroll_view)(object)this.widget).padding;
        if ((((_AnimatedScrollView__animated_scroll_view)(object)this.widget).padding is null))
        {
            MediaQueryData? mediaQuery__32826 = ((MediaQueryData?)(object?)MediaQuery.maybeOf(this.context));
            if ((mediaQuery__32826 is not null))
            {
                global::Doroti.Framework.Painting.EdgeInsets mediaQueryHorizontalPadding__32991 = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)((MediaQueryData)mediaQuery__32826).padding.copyWith(top: 0.0, bottom: 0.0));
                global::Doroti.Framework.Painting.EdgeInsets mediaQueryVerticalPadding__33129 = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)((MediaQueryData)mediaQuery__32826).padding.copyWith(left: 0.0, right: 0.0));
                effectivePadding__32729 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(((object.Equals(direction, global::Doroti.Framework.Painting.Axis.vertical)) ? mediaQueryVerticalPadding__33129 : mediaQueryHorizontalPadding__32991));
                sliver = new MediaQuery(data: mediaQuery__32826.copyWith(padding: ((object.Equals(direction, global::Doroti.Framework.Painting.Axis.vertical)) ? mediaQueryHorizontalPadding__32991 : mediaQueryVerticalPadding__33129)), child: sliver);
            }
        }
        if ((effectivePadding__32729 is not null))
        {
            sliver = new SliverPadding(padding: effectivePadding__32729, sliver: sliver);
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
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
        SliverAnimatedListState? result__39025 = ((SliverAnimatedListState?)(object?)SliverAnimatedList.maybeOf(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__39025 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("SliverAnimatedList.of() called with a context that does not contain a SliverAnimatedList.\n" + "No SliverAnimatedListState ancestor could be found starting from the " + "context that was passed to SliverAnimatedListState.of(). This can " + "happen when the context provided is from the same StatefulWidget that " + "built the AnimatedList. Please see the SliverAnimatedList documentation " + "for examples of how to refer to an AnimatedListState object: " + "https://api.flutter.dev/flutter/widgets/SliverAnimatedListState-class.html\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__39025!;
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
        SliverAnimatedGridState? result__44498 = ((SliverAnimatedGridState?)(object?)context.findAncestorStateOfType<SliverAnimatedGridState>());
        DartRuntimePrimitives.Assert(() =>
            {
                if ((result__44498 is null))
                {
                    throw DartRuntimePrimitives.AsException(global::Doroti.Framework.Foundation.FlutterError.Create("SliverAnimatedGrid.of() called with a context that does not contain a SliverAnimatedGrid.\n" + "No SliverAnimatedGridState ancestor could be found starting from the " + "context that was passed to SliverAnimatedGridState.of(). This can " + "happen when the context provided is from the same StatefulWidget that " + "built the AnimatedGrid. Please see the SliverAnimatedGrid documentation " + "for examples of how to refer to an AnimatedGridState object: " + "https://api.flutter.dev/flutter/widgets/SliverAnimatedGridState-class.html\n" + "The context used was:\n" + $"  {context}"));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result__44498!;
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
        foreach (_ActiveItem__animated_scroll_view item__48695 in this._incomingItems.followedBy(this._outgoingItems.Cast<_ActiveItem__animated_scroll_view>()))
        {
            ((_ActiveItem__animated_scroll_view)item__48695).controller!.dispose();
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
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

    internal virtual _ActiveItem__animated_scroll_view? _removeActiveItemAt(List<_ActiveItem__animated_scroll_view> items, long itemIndex)
    {
        long i__49049 = global::Doroti.Framework.Foundation.CollectionsLibrary.binarySearch(items, _ActiveItem__animated_scroll_view.CreateIndex(itemIndex));
        return ((i__49049 == -1L) ? null : items.removeAt(i__49049));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _ActiveItem__animated_scroll_view? _activeItemAt(List<_ActiveItem__animated_scroll_view> items, long itemIndex)
    {
        long i__49241 = global::Doroti.Framework.Foundation.CollectionsLibrary.binarySearch(items, _ActiveItem__animated_scroll_view.CreateIndex(itemIndex));
        return ((i__49241 == -1L) ? null : items[(int)(i__49241)]);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _indexToItemIndex(long index)
    {
        var itemIndex__49795 = index;
        foreach (_ActiveItem__animated_scroll_view item__49841 in this._outgoingItems)
        {
            if ((((_ActiveItem__animated_scroll_view)item__49841).itemIndex <= itemIndex__49795))
            {
                itemIndex__49795 += 1L;
            }
            else
            {
                break;
            }
        }
        return itemIndex__49795;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _itemIndexToIndex(long itemIndex)
    {
        var index__50052 = itemIndex;
        foreach (_ActiveItem__animated_scroll_view item__50098 in this._outgoingItems)
        {
            DartRuntimePrimitives.Assert(() => (((_ActiveItem__animated_scroll_view)item__50098).itemIndex != itemIndex));
            if ((((_ActiveItem__animated_scroll_view)item__50098).itemIndex < itemIndex))
            {
                index__50052 -= 1L;
            }
            else
            {
                break;
            }
        }
        return index__50052;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual SliverChildDelegate _createDelegate()
    {
        return ((SliverChildDelegate)(object?)new SliverChildBuilderDelegate((global::System.Func<BuildContext, long, Widget>)this._itemBuilder, childCount: this._itemsCount, findChildIndexCallback: ((global::System.Func<global::Doroti.Framework.Foundation.Key, long?>)((((_SliverAnimatedMultiBoxAdaptor__animated_scroll_view)(object)this.widget).findChildIndexCallback is null) ? null : ((key) => {
long? index__50560 = ((_SliverAnimatedMultiBoxAdaptor__animated_scroll_view)(object)this.widget).findChildIndexCallback!(key);
return ((index__50560 is not null) ? _indexToItemIndex(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(index__50560))) : null);
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _itemBuilder(BuildContext context, long itemIndex)
    {
        _ActiveItem__animated_scroll_view? outgoingItem__50786 = ((_ActiveItem__animated_scroll_view?)(object?)_activeItemAt(this._outgoingItems, itemIndex));
        if ((outgoingItem__50786 is not null))
        {
            return ((_ActiveItem__animated_scroll_view)outgoingItem__50786).removedItemBuilder!(context, ((_ActiveItem__animated_scroll_view)outgoingItem__50786).controller!.view);
        }
        _ActiveItem__animated_scroll_view? incomingItem__50992 = ((_ActiveItem__animated_scroll_view?)(object?)_activeItemAt(this._incomingItems, itemIndex));
        global::Doroti.Framework.Animation.Animation<double> animation__51077 = (incomingItem__50992?.controller?.view ?? global::Doroti.Framework.Animation.AnimationsLibrary.kAlwaysCompleteAnimation);
        return this.widget.itemBuilder(context, _itemIndexToIndex(itemIndex), animation__51077);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void insertItem(long index, Duration? duration = null)
    {
        DartRuntimePrimitives.Assert(() => (index >= 0L));
        long itemIndex__51748 = _indexToItemIndex(index);
        DartRuntimePrimitives.Assert(() => ((itemIndex__51748 >= 0L) && (itemIndex__51748 <= this._itemsCount)));
        foreach (_ActiveItem__animated_scroll_view item__51963 in this._incomingItems)
        {
            if ((((_ActiveItem__animated_scroll_view)item__51963).itemIndex >= itemIndex__51748))
            {
                item__51963.itemIndex += 1L;
            }
        }
        foreach (_ActiveItem__animated_scroll_view item__52100 in this._outgoingItems)
        {
            if ((((_ActiveItem__animated_scroll_view)item__52100).itemIndex >= itemIndex__51748))
            {
                item__52100.itemIndex += 1L;
            }
        }
        var controller__52221 = new global::Doroti.Framework.Animation.AnimationController(duration: DartRuntimePrimitives.RequireValue(duration), vsync: this);
        var incomingItem__52298 = new _ActiveItem__animated_scroll_view(controller__52221, itemIndex__51748);
        setState(((global::System.Action)(() => {
DartRuntimePrimitives.Ignore(((Func<List<_ActiveItem__animated_scroll_view>>)(() =>
{            var __cascade = this._incomingItems;
            __cascade.Add(incomingItem__52298);
            __cascade.sort();
            return __cascade;        }))());
_itemsCount += 1L;
})));
        DartRuntimePrimitives.Ignore(controller__52221.forward().then(((global::System.Func<object?, object>)((_) => {
_removeActiveItemAt(this._incomingItems, ((_ActiveItem__animated_scroll_view)incomingItem__52298).itemIndex)!.controller!.dispose();
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
    }

    public virtual void insertAllItems(long index, long length, Duration? duration = null)
    {
        for (var i__52897 = 0L; (i__52897 < length); i__52897++)
        {
            insertItem((index + i__52897), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
        }
    }

    public virtual void removeItem(long index, global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget> builder, Duration? duration = null)
    {
        DartRuntimePrimitives.Assert(() => (index >= 0L));
        long itemIndex__53803 = _indexToItemIndex(index);
        DartRuntimePrimitives.Assert(() => ((itemIndex__53803 >= 0L) && (itemIndex__53803 < this._itemsCount)));
        DartRuntimePrimitives.Assert(() => (_activeItemAt(this._outgoingItems, itemIndex__53803) is null));
        _ActiveItem__animated_scroll_view? incomingItem__53982 = ((_ActiveItem__animated_scroll_view?)(object?)_removeActiveItemAt(this._incomingItems, itemIndex__53803));
        global::Doroti.Framework.Animation.AnimationController controller__54075 = (incomingItem__53982?.controller ?? new global::Doroti.Framework.Animation.AnimationController(duration: DartRuntimePrimitives.RequireValue(duration), value: 1.0, vsync: this));
        var outgoingItem__54208 = _ActiveItem__animated_scroll_view.CreateOutgoing(controller__54075, itemIndex__53803, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder);
        setState(((global::System.Action)(() => {
DartRuntimePrimitives.Ignore(((Func<List<_ActiveItem__animated_scroll_view>>)(() =>
{            var __cascade = this._outgoingItems;
            __cascade.Add(outgoingItem__54208);
            __cascade.sort();
            return __cascade;        }))());
})));
        DartRuntimePrimitives.Ignore(controller__54075.reverse().then(((global::System.Func<object?, object>)((value) => {
_removeActiveItemAt(this._outgoingItems, ((_ActiveItem__animated_scroll_view)outgoingItem__54208).itemIndex)!.controller!.dispose();
foreach (_ActiveItem__animated_scroll_view item__54637 in this._incomingItems)
{
    if ((((_ActiveItem__animated_scroll_view)item__54637).itemIndex > ((_ActiveItem__animated_scroll_view)outgoingItem__54208).itemIndex))
    {
        item__54637.itemIndex -= 1L;
    }
}
foreach (_ActiveItem__animated_scroll_view item__54796 in this._outgoingItems)
{
    if ((((_ActiveItem__animated_scroll_view)item__54796).itemIndex > ((_ActiveItem__animated_scroll_view)outgoingItem__54208).itemIndex))
    {
        item__54796.itemIndex -= 1L;
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
        long visibleItemCount__55676 = (this._itemsCount - checked((long)(this._outgoingItems.Count)));
        for (long i__55745 = (visibleItemCount__55676 - 1L); (i__55745 >= 0L); i__55745--)
        {
            removeItem(i__55745, (global::System.Func<BuildContext, global::Doroti.Framework.Animation.Animation<double>, Widget>)builder, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)));
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

