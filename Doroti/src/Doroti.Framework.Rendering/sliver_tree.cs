// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_tree.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public delegate void TreeSliverNodesAnimation();

public class TreeSliverNodeParentData : SliverMultiBoxAdaptorParentData
{
    public virtual long depth { get; set; } = 0L;

}

public class TreeSliverIndentationType
{
    internal virtual double _value { get; private set; } = default!;
    public static TreeSliverIndentationType standard = new TreeSliverIndentationType(10.0);
    public static TreeSliverIndentationType none = new TreeSliverIndentationType(0.0);

    public TreeSliverIndentationType(double value)
    {
        this._value = value;
    }

    public virtual double value => this._value;
    public static TreeSliverIndentationType custom(double value)
    {
        DartRuntimePrimitives.Assert(() => (value >= 0.0));
        return new TreeSliverIndentationType(value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate void _PaintSegment__sliver_tree();

public class RenderTreeSliver : RenderSliverVariedExtentList
{
    internal virtual DartMap<UniqueKey, (long fromIndex, long toIndex, double value)> _activeAnimations { get; set; } = default!;
    internal virtual double _indentation { get; set; } = default!;
    internal virtual DartMap<long, UniqueKey> _animationLeadingIndices { get; private set; } = new DartMap<long, UniqueKey>();
    internal virtual DartMap<UniqueKey, double> _animationOffsets { get; private set; } = new DartMap<UniqueKey, double>();
    internal virtual DartMap<UniqueKey, LayerHandle<ClipRectLayer>> _clipHandles { get; private set; } = new DartMap<UniqueKey, LayerHandle<ClipRectLayer>>();

    public RenderTreeSliver(RenderSliverBoxChildManager childManager, ItemExtentBuilder itemExtentBuilder, DartMap<UniqueKey, (long fromIndex, long toIndex, double value)> activeAnimations, double indentation) : base(childManager: childManager, itemExtentBuilder: itemExtentBuilder)
    {
        this._activeAnimations = activeAnimations;
        this._indentation = indentation;
    }

    public virtual DartMap<UniqueKey, (long fromIndex, long toIndex, double value)> activeAnimations
    {
        get => this._activeAnimations;
        set
        {
            var __value = value;
            if ((object.Equals(this._activeAnimations, __value)))
            {
                return;
            }
            _activeAnimations = __value;
            markNeedsLayout();
        }
    }
    public virtual double indentation
    {
        get => this._indentation;
        set
        {
            var __value = value;
            if ((this._indentation == __value))
            {
                return;
            }
            DartRuntimePrimitives.Assert(() => (this.indentation >= 0.0));
            _indentation = __value;
            markNeedsLayout();
        }
    }
    internal virtual void _updateAnimationCache()
    {
        this._animationLeadingIndices.Clear();
        this._activeAnimations.forEach(((key, animation) =>
        {
            this._animationLeadingIndices[(animation.fromIndex - 1L)] = key;
        }));
        this._animationOffsets.removeWhere(((key, _) => !this._activeAnimations.Keys.contains(key)));
        this._clipHandles.removeWhere(((key, handle) =>
        {
            if (!this._activeAnimations.Keys.contains(key))
            {
                handle.layer = null;
                return true;
            }
            return false;
            return default;
        }));
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        if ((__child.parentData is not TreeSliverNodeParentData))
        {
            __child.parentData = new TreeSliverNodeParentData();
        }
    }

    public override void dispose()
    {
        this._clipHandles.removeWhere(((key, handle) =>
        {
            handle.layer = null;
            return true;
            return default;
        }));
        base.dispose();
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((SliverConstraints)constraints).axisDirection, global::Doroti.Framework.Painting.AxisDirection.down)));
        _updateAnimationCache();
        base.performLayout();
    }

    public override long getMinChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        return _getChildIndexForScrollOffset(scrollOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        return _getChildIndexForScrollOffset(scrollOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _getChildIndexForScrollOffset(double scrollOffset)
    {
        if ((scrollOffset == 0.0))
        {
            return 0L;
        }
        var position__8372 = 0.0;
        var index__8396 = 0L;
        var totalAnimationOffset__8415 = 0.0;
        double? itemExtent__8455 = default!;
        long? childCount__8482 = ((RenderSliverBoxChildManager)childManager).estimatedChildCount;
        while ((position__8372 < scrollOffset))
        {
            if (((childCount__8482 is not null) && (index__8396 > (DartRuntimePrimitives.RequireValue(childCount__8482) - 1L))))
            {
                long childCount__8482__value8577 = DartRuntimePrimitives.RequireValue(childCount__8482);
                break;
            }
            itemExtent__8455 = itemExtentBuilder(index__8396, layoutDimensions);
            if ((itemExtent__8455 is null))
            {
                break;
            }
            if (this._animationLeadingIndices.Keys.contains(index__8396))
            {
                UniqueKey animationKey__8850 = this._animationLeadingIndices.GetValueOrDefault(index__8396)!;
                if ((!this._animationOffsets.ContainsKey(animationKey__8850)))
                {
                    _computeAnimationOffsetFor(animationKey__8850, position__8372);
                }
                totalAnimationOffset__8415 += (DartRuntimePrimitives.RequireValue(this._animationOffsets.GetValueOrDefault(animationKey__8850)) * ((1L - DartRuntimePrimitives.RequireValue(this._activeAnimations.GetValueOrDefault(animationKey__8850)).value)));
            }
            position__8372 += (DartRuntimePrimitives.RequireValue(itemExtent__8455) - totalAnimationOffset__8415);
            ++index__8396;
        }
        return (index__8396 - 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _computeAnimationOffsetFor(UniqueKey key, double position)
    {
        DartRuntimePrimitives.Assert(() => (this._activeAnimations.ContainsKey(key)));
        double targetPosition__9575 = (((SliverConstraints)constraints).scrollOffset + ((SliverConstraints)constraints).remainingCacheExtent);
        var currentPosition__9661 = position;
        long startingIndex__9703 = DartRuntimePrimitives.RequireValue(this._activeAnimations.GetValueOrDefault(key)).fromIndex;
        long lastIndex__9768 = DartRuntimePrimitives.RequireValue(this._activeAnimations.GetValueOrDefault(key)).toIndex;
        var currentIndex__9821 = startingIndex__9703;
        var totalAnimatingOffset__9859 = 0.0;
        while (((currentIndex__9821 <= lastIndex__9768) && (currentPosition__9661 < targetPosition__9575)))
        {
            double itemExtent__10122 = DartRuntimePrimitives.RequireValue(itemExtentBuilder(currentIndex__9821, layoutDimensions));
            totalAnimatingOffset__9859 += itemExtent__10122;
            currentPosition__9661 += itemExtent__10122;
            currentIndex__9821++;
        }
        this._animationOffsets[key] = totalAnimatingOffset__9859;
    }

    public override double childCrossAxisPosition(RenderObject child)
    {
        var parentData__10820 = ((TreeSliverNodeParentData?)(object?)((RenderObject)child).parentData!)!;
        return (((TreeSliverNodeParentData)parentData__10820).depth * this.indentation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double indexToLayoutOffset(double itemExtent, long index)
    {
        var position__11117 = 0.0;
        var currentIndex__11141 = 0L;
        var totalAnimationOffset__11167 = 0.0;
        double? itemExtent__11207 = default!;
        long? childCount__11234 = ((RenderSliverBoxChildManager)childManager).estimatedChildCount;
        while ((currentIndex__11141 < index))
        {
            if (((childCount__11234 is not null) && (currentIndex__11141 > (DartRuntimePrimitives.RequireValue(childCount__11234) - 1L))))
            {
                long childCount__11234__value11326 = DartRuntimePrimitives.RequireValue(childCount__11234);
                break;
            }
            itemExtent__11207 = itemExtentBuilder(currentIndex__11141, layoutDimensions);
            if ((itemExtent__11207 is null))
            {
                break;
            }
            if (this._animationLeadingIndices.Keys.contains(currentIndex__11141))
            {
                UniqueKey animationKey__11620 = this._animationLeadingIndices.GetValueOrDefault(currentIndex__11141)!;
                DartRuntimePrimitives.Assert(() => (this._animationOffsets.ContainsKey(animationKey__11620)));
                totalAnimationOffset__11167 += (DartRuntimePrimitives.RequireValue(this._animationOffsets.GetValueOrDefault(animationKey__11620)) * ((1L - DartRuntimePrimitives.RequireValue(this._activeAnimations.GetValueOrDefault(animationKey__11620)).value)));
            }
            position__11117 += DartRuntimePrimitives.RequireValue(itemExtent__11207);
            currentIndex__11141++;
        }
        return (position__11117 - totalAnimationOffset__11167);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((firstChild is null))
        {
            return;
        }
        RenderBox? nextChild__12287 = firstChild;
        void paintUpTo(long index, RenderBox? startWith, PaintingContext context, Offset offset)
        {
            var child__12415 = startWith;
            while (((child__12415 is not null) && (indexOf(child__12415) <= index)))
            {
                double mainAxisDelta__12512 = childMainAxisPosition(child__12415);
                var parentData__12572 = ((TreeSliverNodeParentData?)(object?)child__12415.parentData!)!;
                global::Doroti.Ui.Offset childOffset__12653 = (new global::Doroti.Ui.Offset((((TreeSliverNodeParentData)parentData__12572).depth * this.indentation), (DartRuntimePrimitives.RequireValue(parentData__12572.layoutOffset) - ((SliverConstraints)constraints).scrollOffset)) + offset);
                if (((mainAxisDelta__12512 < ((SliverConstraints)constraints).remainingPaintExtent) && ((mainAxisDelta__12512 + paintExtentOf(child__12415)) > 0L)))
                {
                    context.paintChild(child__12415, childOffset__12653);
                }
                child__12415 = childAfter(child__12415);
            }
            nextChild__12287 = child__12415;
        }
        if ((checked((long)(this._animationLeadingIndices.Count)) == 0))
        {
            paintUpTo(indexOf(lastChild!), firstChild, context, offset);
            return;
        }
        long leadingIndex__13563 = indexOf(firstChild!);
        List<long> animationIndices__13620 = ((Func<List<long>>)(() =>
{
    var __cascade = this._animationLeadingIndices.Keys.ToList();
    __cascade.sort();
    return __cascade;
}))();
        var paintSegments__13697 = new List<(long leadingIndex, long trailingIndex)>();
        while ((checked((long)(animationIndices__13620.Count)) != 0))
        {
            long trailingIndex__13790 = animationIndices__13620.removeAt(0L);
            paintSegments__13697.Add((leadingIndex: leadingIndex__13563, trailingIndex: trailingIndex__13790));
            leadingIndex__13563 = (trailingIndex__13790 + 1L);
        }
        paintSegments__13697.Add((leadingIndex: leadingIndex__13563, trailingIndex: indexOf(lastChild!)));
        paintUpTo(paintSegments__13697.removeAt(0L).trailingIndex, nextChild__12287, context, offset);
        while ((checked((long)(paintSegments__13697.Count)) != 0))
        {
            (long leadingIndex, long trailingIndex) segment__14300 = paintSegments__13697.removeAt(0L);
            long parentIndex__14878 = Math.Max((segment__14300.leadingIndex - 1L), 0L);
            double leadingOffset__14950 = (indexToLayoutOffset(0.0, parentIndex__14878) + DartRuntimePrimitives.RequireValue(itemExtentBuilder(parentIndex__14878, layoutDimensions)));
            double trailingOffset__15086 = (indexToLayoutOffset(0.0, segment__14300.trailingIndex) + DartRuntimePrimitives.RequireValue(itemExtentBuilder(segment__14300.trailingIndex, layoutDimensions)));
            var rect__15246 = global::Doroti.Ui.Rect.fromPoints(new global::Doroti.Ui.Offset(0.0, leadingOffset__14950), new global::Doroti.Ui.Offset(((SliverConstraints)constraints).crossAxisExtent, trailingOffset__15086));
            UniqueKey key__15519 = this._animationLeadingIndices.GetValueOrDefault(parentIndex__14878)!;
            this._clipHandles[key__15519] ??= new LayerHandle<ClipRectLayer>();
            this._clipHandles[key__15519]!.layer = context.pushClipRect(needsCompositing, offset, rect__15246, ((Action<PaintingContext, Offset>)((context, offset) =>
            {
                paintUpTo(segment__14300.trailingIndex, nextChild__12287, context, offset);
            })), oldLayer: this._clipHandles.GetValueOrDefault(key__15519)!.layer);
        }
    }

}

