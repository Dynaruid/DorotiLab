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
        var position = 0.0;
        var index = 0L;
        var totalAnimationOffset = 0.0;
        double? itemExtent = default!;
        long? childCount = ((RenderSliverBoxChildManager)childManager).estimatedChildCount;
        while ((position < scrollOffset))
        {
            if (((childCount is not null) && (index > (DartRuntimePrimitives.RequireValue(childCount) - 1L))))
            {
                long childCount__8482__value8577 = DartRuntimePrimitives.RequireValue(childCount);
                break;
            }
            itemExtent = itemExtentBuilder(index, layoutDimensions);
            if ((itemExtent is null))
            {
                break;
            }
            if (this._animationLeadingIndices.Keys.contains(index))
            {
                UniqueKey animationKey = this._animationLeadingIndices.GetValueOrDefault(index)!;
                if ((!this._animationOffsets.ContainsKey(animationKey)))
                {
                    _computeAnimationOffsetFor(animationKey, position);
                }
                totalAnimationOffset += (DartRuntimePrimitives.RequireValue(this._animationOffsets.GetValueOrDefault(animationKey)) * ((1L - DartRuntimePrimitives.RequireValue(this._activeAnimations.GetValueOrDefault(animationKey)).value)));
            }
            position += (DartRuntimePrimitives.RequireValue(itemExtent) - totalAnimationOffset);
            ++index;
        }
        return (index - 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _computeAnimationOffsetFor(UniqueKey key, double position)
    {
        DartRuntimePrimitives.Assert(() => (this._activeAnimations.ContainsKey(key)));
        double targetPosition = (((SliverConstraints)constraints).scrollOffset + ((SliverConstraints)constraints).remainingCacheExtent);
        var currentPosition = position;
        long startingIndex = DartRuntimePrimitives.RequireValue(this._activeAnimations.GetValueOrDefault(key)).fromIndex;
        long lastIndex = DartRuntimePrimitives.RequireValue(this._activeAnimations.GetValueOrDefault(key)).toIndex;
        var currentIndex = startingIndex;
        var totalAnimatingOffset = 0.0;
        while (((currentIndex <= lastIndex) && (currentPosition < targetPosition)))
        {
            double itemExtent = DartRuntimePrimitives.RequireValue(itemExtentBuilder(currentIndex, layoutDimensions));
            totalAnimatingOffset += itemExtent;
            currentPosition += itemExtent;
            currentIndex++;
        }
        this._animationOffsets[key] = totalAnimatingOffset;
    }

    public override double childCrossAxisPosition(RenderObject child)
    {
        var parentDataLocal = ((TreeSliverNodeParentData?)(object?)((RenderObject)child).parentData!)!;
        return (((TreeSliverNodeParentData)parentDataLocal).depth * this.indentation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double indexToLayoutOffset(double itemExtent, long index)
    {
        var position = 0.0;
        var currentIndex = 0L;
        var totalAnimationOffset = 0.0;
        double? itemExtentLocal = default!;
        long? childCount = ((RenderSliverBoxChildManager)childManager).estimatedChildCount;
        while ((currentIndex < index))
        {
            if (((childCount is not null) && (currentIndex > (DartRuntimePrimitives.RequireValue(childCount) - 1L))))
            {
                long childCount__11234__value11326 = DartRuntimePrimitives.RequireValue(childCount);
                break;
            }
            itemExtentLocal = itemExtentBuilder(currentIndex, layoutDimensions);
            if ((itemExtentLocal is null))
            {
                break;
            }
            if (this._animationLeadingIndices.Keys.contains(currentIndex))
            {
                UniqueKey animationKey = this._animationLeadingIndices.GetValueOrDefault(currentIndex)!;
                DartRuntimePrimitives.Assert(() => (this._animationOffsets.ContainsKey(animationKey)));
                totalAnimationOffset += (DartRuntimePrimitives.RequireValue(this._animationOffsets.GetValueOrDefault(animationKey)) * ((1L - DartRuntimePrimitives.RequireValue(this._activeAnimations.GetValueOrDefault(animationKey)).value)));
            }
            position += DartRuntimePrimitives.RequireValue(itemExtentLocal);
            currentIndex++;
        }
        return (position - totalAnimationOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((firstChild is null))
        {
            return;
        }
        RenderBox? nextChild = firstChild;
        void paintUpTo(long index, RenderBox? startWith, PaintingContext context, Offset offset)
        {
            var child = startWith;
            while (((child is not null) && (indexOf(child) <= index)))
            {
                double mainAxisDelta = childMainAxisPosition(child);
                var parentDataLocal = ((TreeSliverNodeParentData?)(object?)child.parentData!)!;
                global::Doroti.Ui.Offset childOffset = (new global::Doroti.Ui.Offset((((TreeSliverNodeParentData)parentDataLocal).depth * this.indentation), (DartRuntimePrimitives.RequireValue(parentDataLocal.layoutOffset) - ((SliverConstraints)constraints).scrollOffset)) + offset);
                if (((mainAxisDelta < ((SliverConstraints)constraints).remainingPaintExtent) && ((mainAxisDelta + paintExtentOf(child)) > 0L)))
                {
                    context.paintChild(child, childOffset);
                }
                child = childAfter(child);
            }
            nextChild = child;
        }
        if ((checked((long)(this._animationLeadingIndices.Count)) == 0))
        {
            paintUpTo(indexOf(lastChild!), firstChild, context, offset);
            return;
        }
        long leadingIndexLocal = indexOf(firstChild!);
        List<long> animationIndices = ((Func<List<long>>)(() =>
{
    var __cascade = this._animationLeadingIndices.Keys.ToList();
    __cascade.sort();
    return __cascade;
}))();
        var paintSegments = new List<(long leadingIndex, long trailingIndex)>();
        while ((checked((long)(animationIndices.Count)) != 0))
        {
            long trailingIndexLocal = animationIndices.removeAt(0L);
            paintSegments.Add((leadingIndex: leadingIndexLocal, trailingIndex: trailingIndexLocal));
            leadingIndexLocal = (trailingIndexLocal + 1L);
        }
        paintSegments.Add((leadingIndex: leadingIndexLocal, trailingIndex: indexOf(lastChild!)));
        paintUpTo(paintSegments.removeAt(0L).trailingIndex, nextChild, context, offset);
        while ((checked((long)(paintSegments.Count)) != 0))
        {
            (long leadingIndex, long trailingIndex) segment = paintSegments.removeAt(0L);
            long parentIndex = Math.Max((segment.leadingIndex - 1L), 0L);
            double leadingOffset = (indexToLayoutOffset(0.0, parentIndex) + DartRuntimePrimitives.RequireValue(itemExtentBuilder(parentIndex, layoutDimensions)));
            double trailingOffset = (indexToLayoutOffset(0.0, segment.trailingIndex) + DartRuntimePrimitives.RequireValue(itemExtentBuilder(segment.trailingIndex, layoutDimensions)));
            var rect = global::Doroti.Ui.Rect.fromPoints(new global::Doroti.Ui.Offset(0.0, leadingOffset), new global::Doroti.Ui.Offset(((SliverConstraints)constraints).crossAxisExtent, trailingOffset));
            UniqueKey key = this._animationLeadingIndices.GetValueOrDefault(parentIndex)!;
            this._clipHandles[key] ??= new LayerHandle<ClipRectLayer>();
            this._clipHandles[key]!.layer = context.pushClipRect(needsCompositing, offset, rect, ((Action<PaintingContext, Offset>)((context, offset) =>
            {
                paintUpTo(segment.trailingIndex, nextChild, context, offset);
            })), oldLayer: this._clipHandles.GetValueOrDefault(key)!.layer);
        }
    }

}

