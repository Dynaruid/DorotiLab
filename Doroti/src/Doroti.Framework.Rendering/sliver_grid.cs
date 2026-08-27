// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_grid.dart
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

public class SliverGridGeometry
{
    public virtual double scrollOffset { get; private set; } = default!;
    public virtual double crossAxisOffset { get; private set; } = default!;
    public virtual double mainAxisExtent { get; private set; } = default!;
    public virtual double crossAxisExtent { get; private set; } = default!;

    public SliverGridGeometry(double scrollOffset, double crossAxisOffset, double mainAxisExtent, double crossAxisExtent)
    {
        this.scrollOffset = scrollOffset;
        this.crossAxisOffset = crossAxisOffset;
        this.mainAxisExtent = mainAxisExtent;
        this.crossAxisExtent = crossAxisExtent;
    }

    public virtual double trailingScrollOffset => (this.scrollOffset + this.mainAxisExtent);
    public virtual BoxConstraints getBoxConstraints(SliverConstraints constraints)
    {
        return constraints.asBoxConstraints(minExtent: this.mainAxisExtent, maxExtent: this.mainAxisExtent, crossAxisExtent: this.crossAxisExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        var properties = new List<string> { $"scrollOffset: {this.scrollOffset}", $"crossAxisOffset: {this.crossAxisOffset}", $"mainAxisExtent: {this.mainAxisExtent}", $"crossAxisExtent: {this.crossAxisExtent}" };
        return $"SliverGridGeometry({string.Join(", ", properties)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface SliverGridLayout
{
    public long getMinChildIndexForScrollOffset(double scrollOffset);
    public long getMaxChildIndexForScrollOffset(double scrollOffset);
    public SliverGridGeometry getGeometryForChildIndex(long index);
    public double computeMaxScrollOffset(long childCount);
}

public class SliverGridRegularTileLayout : SliverGridLayout
{
    public virtual long crossAxisCount { get; private set; } = default!;
    public virtual double mainAxisStride { get; private set; } = default!;
    public virtual double crossAxisStride { get; private set; } = default!;
    public virtual double childMainAxisExtent { get; private set; } = default!;
    public virtual double childCrossAxisExtent { get; private set; } = default!;
    public virtual bool reverseCrossAxis { get; private set; } = default!;

    public SliverGridRegularTileLayout(long crossAxisCount, double mainAxisStride, double crossAxisStride, double childMainAxisExtent, double childCrossAxisExtent, bool reverseCrossAxis)
    {
        this.crossAxisCount = crossAxisCount;
        this.mainAxisStride = mainAxisStride;
        this.crossAxisStride = crossAxisStride;
        this.childMainAxisExtent = childMainAxisExtent;
        this.childCrossAxisExtent = childCrossAxisExtent;
        this.reverseCrossAxis = reverseCrossAxis;
        System.Diagnostics.Debug.Assert((crossAxisCount > 0L));
        System.Diagnostics.Debug.Assert((mainAxisStride >= 0L));
        System.Diagnostics.Debug.Assert((crossAxisStride >= 0L));
        System.Diagnostics.Debug.Assert((childMainAxisExtent >= 0L));
        System.Diagnostics.Debug.Assert((childCrossAxisExtent >= 0L));
    }

    public virtual long getMinChildIndexForScrollOffset(double scrollOffset)
    {
        return ((this.mainAxisStride > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) ? (this.crossAxisCount * ((checked((long)(scrollOffset / this.mainAxisStride))))) : 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getMaxChildIndexForScrollOffset(double scrollOffset)
    {
        if ((this.mainAxisStride > 0.0))
        {
            long mainAxisCount = ((scrollOffset / this.mainAxisStride)).ceil();
            return Math.Max(0L, ((this.crossAxisCount * mainAxisCount) - 1L));
        }
        return 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getOffsetFromStartInCrossAxis(double crossAxisStart)
    {
        if (this.reverseCrossAxis)
        {
            return ((((this.crossAxisCount * this.crossAxisStride) - crossAxisStart) - this.childCrossAxisExtent) - ((this.crossAxisStride - this.childCrossAxisExtent)));
        }
        return crossAxisStart;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SliverGridGeometry getGeometryForChildIndex(long index)
    {
        double crossAxisStart = (((index % this.crossAxisCount)) * this.crossAxisStride);
        return new SliverGridGeometry(scrollOffset: (((checked((long)(index / this.crossAxisCount)))) * this.mainAxisStride), crossAxisOffset: _getOffsetFromStartInCrossAxis(crossAxisStart), mainAxisExtent: this.childMainAxisExtent, crossAxisExtent: this.childCrossAxisExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxScrollOffset(long childCount)
    {
        if ((childCount == 0L))
        {
            return 0.0;
        }
        long mainAxisCount = (((checked((long)(((childCount - 1L)) / this.crossAxisCount)))) + 1L);
        double mainAxisSpacing = (this.mainAxisStride - this.childMainAxisExtent);
        return ((this.mainAxisStride * mainAxisCount) - mainAxisSpacing);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface SliverGridDelegate
{
    public SliverGridLayout getLayout(SliverConstraints constraints);
    public bool shouldRelayout(SliverGridDelegate oldDelegate);
}

public class SliverGridDelegateWithFixedCrossAxisCount : SliverGridDelegate
{
    public virtual long crossAxisCount { get; private set; } = default!;
    public virtual double mainAxisSpacing { get; private set; } = default!;
    public virtual double crossAxisSpacing { get; private set; } = default!;
    public virtual double childAspectRatio { get; private set; } = default!;
    public virtual double? mainAxisExtent { get; private set; }

    public SliverGridDelegateWithFixedCrossAxisCount(long crossAxisCount, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, double? mainAxisExtent = null)
    {
        this.crossAxisCount = crossAxisCount;
        this.mainAxisSpacing = mainAxisSpacing;
        this.crossAxisSpacing = crossAxisSpacing;
        this.childAspectRatio = childAspectRatio;
        this.mainAxisExtent = mainAxisExtent;
        System.Diagnostics.Debug.Assert((crossAxisCount > 0L));
        System.Diagnostics.Debug.Assert((mainAxisSpacing >= 0L));
        System.Diagnostics.Debug.Assert((crossAxisSpacing >= 0L));
        System.Diagnostics.Debug.Assert((childAspectRatio > 0L));
        System.Diagnostics.Debug.Assert(((mainAxisExtent is null) || (mainAxisExtent >= 0L)));
    }

    internal virtual bool _debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() => (this.crossAxisCount > 0L));
        DartRuntimePrimitives.Assert(() => (this.mainAxisSpacing >= 0.0));
        DartRuntimePrimitives.Assert(() => (this.crossAxisSpacing >= 0.0));
        DartRuntimePrimitives.Assert(() => (this.childAspectRatio > 0.0));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SliverGridLayout getLayout(SliverConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertIsValid());
        double usableCrossAxisExtent = Math.Max(0.0, (((SliverConstraints)constraints).crossAxisExtent - (this.crossAxisSpacing * ((this.crossAxisCount - 1L)))));
        double childCrossAxisExtentLocal = (usableCrossAxisExtent / this.crossAxisCount);
        double childMainAxisExtentLocal = (this.mainAxisExtent ?? (childCrossAxisExtentLocal / this.childAspectRatio));
        return new SliverGridRegularTileLayout(crossAxisCount: this.crossAxisCount, mainAxisStride: (childMainAxisExtentLocal + this.mainAxisSpacing), crossAxisStride: (childCrossAxisExtentLocal + this.crossAxisSpacing), childMainAxisExtent: childMainAxisExtentLocal, childCrossAxisExtent: childCrossAxisExtentLocal, reverseCrossAxis: global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).crossAxisDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(SliverGridDelegate oldDelegate)
    {
        var __oldDelegate = (SliverGridDelegateWithFixedCrossAxisCount)(object)oldDelegate;
        return (((((((SliverGridDelegateWithFixedCrossAxisCount)__oldDelegate).crossAxisCount != this.crossAxisCount) || (((SliverGridDelegateWithFixedCrossAxisCount)__oldDelegate).mainAxisSpacing != this.mainAxisSpacing)) || (((SliverGridDelegateWithFixedCrossAxisCount)__oldDelegate).crossAxisSpacing != this.crossAxisSpacing)) || (((SliverGridDelegateWithFixedCrossAxisCount)__oldDelegate).childAspectRatio != this.childAspectRatio)) || (((SliverGridDelegateWithFixedCrossAxisCount)__oldDelegate).mainAxisExtent != this.mainAxisExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverGridDelegateWithMaxCrossAxisExtent : SliverGridDelegate
{
    public virtual double maxCrossAxisExtent { get; private set; } = default!;
    public virtual double mainAxisSpacing { get; private set; } = default!;
    public virtual double crossAxisSpacing { get; private set; } = default!;
    public virtual double childAspectRatio { get; private set; } = default!;
    public virtual double? mainAxisExtent { get; private set; }

    public SliverGridDelegateWithMaxCrossAxisExtent(double maxCrossAxisExtent, double mainAxisSpacing = 0.0, double crossAxisSpacing = 0.0, double childAspectRatio = 1.0, double? mainAxisExtent = null)
    {
        this.maxCrossAxisExtent = maxCrossAxisExtent;
        this.mainAxisSpacing = mainAxisSpacing;
        this.crossAxisSpacing = crossAxisSpacing;
        this.childAspectRatio = childAspectRatio;
        this.mainAxisExtent = mainAxisExtent;
        System.Diagnostics.Debug.Assert((maxCrossAxisExtent > 0L));
        System.Diagnostics.Debug.Assert((mainAxisSpacing >= 0L));
        System.Diagnostics.Debug.Assert((crossAxisSpacing >= 0L));
        System.Diagnostics.Debug.Assert((childAspectRatio > 0L));
        System.Diagnostics.Debug.Assert(((mainAxisExtent is null) || (mainAxisExtent >= 0L)));
    }

    internal virtual bool _debugAssertIsValid(double crossAxisExtent)
    {
        DartRuntimePrimitives.Assert(() => (crossAxisExtent > 0.0));
        DartRuntimePrimitives.Assert(() => (this.maxCrossAxisExtent > 0.0));
        DartRuntimePrimitives.Assert(() => (this.mainAxisSpacing >= 0.0));
        DartRuntimePrimitives.Assert(() => (this.crossAxisSpacing >= 0.0));
        DartRuntimePrimitives.Assert(() => (this.childAspectRatio > 0.0));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SliverGridLayout getLayout(SliverConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertIsValid(((SliverConstraints)constraints).crossAxisExtent));
        long crossAxisCountLocal = ((((SliverConstraints)constraints).crossAxisExtent / ((this.maxCrossAxisExtent + this.crossAxisSpacing)))).ceil();
        crossAxisCountLocal = Math.Max(1L, crossAxisCountLocal);
        double usableCrossAxisExtent = Math.Max(0.0, (((SliverConstraints)constraints).crossAxisExtent - (this.crossAxisSpacing * ((crossAxisCountLocal - 1L)))));
        double childCrossAxisExtentLocal = (usableCrossAxisExtent / crossAxisCountLocal);
        double childMainAxisExtentLocal = (this.mainAxisExtent ?? (childCrossAxisExtentLocal / this.childAspectRatio));
        return new SliverGridRegularTileLayout(crossAxisCount: crossAxisCountLocal, mainAxisStride: (childMainAxisExtentLocal + this.mainAxisSpacing), crossAxisStride: (childCrossAxisExtentLocal + this.crossAxisSpacing), childMainAxisExtent: childMainAxisExtentLocal, childCrossAxisExtent: childCrossAxisExtentLocal, reverseCrossAxis: global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).crossAxisDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(SliverGridDelegate oldDelegate)
    {
        var __oldDelegate = (SliverGridDelegateWithMaxCrossAxisExtent)(object)oldDelegate;
        return (((((((SliverGridDelegateWithMaxCrossAxisExtent)__oldDelegate).maxCrossAxisExtent != this.maxCrossAxisExtent) || (((SliverGridDelegateWithMaxCrossAxisExtent)__oldDelegate).mainAxisSpacing != this.mainAxisSpacing)) || (((SliverGridDelegateWithMaxCrossAxisExtent)__oldDelegate).crossAxisSpacing != this.crossAxisSpacing)) || (((SliverGridDelegateWithMaxCrossAxisExtent)__oldDelegate).childAspectRatio != this.childAspectRatio)) || (((SliverGridDelegateWithMaxCrossAxisExtent)__oldDelegate).mainAxisExtent != this.mainAxisExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class SliverGridParentData : SliverMultiBoxAdaptorParentData
{
    public virtual double? crossAxisOffset { get; set; } = default;

    public override string ToString() => $"crossAxisOffset={this.crossAxisOffset}; {base.ToString()}";
}

public class RenderSliverGrid : RenderSliverMultiBoxAdaptor
{
    internal virtual SliverGridDelegate _gridDelegate { get; set; } = default!;

    public RenderSliverGrid(RenderSliverBoxChildManager childManager, SliverGridDelegate gridDelegate) : base(childManager: childManager)
    {
        this._gridDelegate = gridDelegate;
    }

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverGridParentData))
        {
            child.parentData = new SliverGridParentData();
        }
    }

    public virtual SliverGridDelegate gridDelegate
    {
        get => this._gridDelegate;
        set
        {
            var __value = value;
            if ((object.Equals(this._gridDelegate, __value)))
            {
                return;
            }
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(__value), DartRuntimePrimitives.RuntimeType(this._gridDelegate))) || __value.shouldRelayout(this._gridDelegate)))
            {
                markNeedsLayout();
            }
            _gridDelegate = __value;
        }
    }
    public override double childCrossAxisPosition(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        var childParentData = ((SliverGridParentData?)(object?)__child.parentData!)!;
        return DartRuntimePrimitives.RequireValue(((SliverGridParentData)childParentData).crossAxisOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = this.constraints;
        childManager.didStartLayout();
        childManager.setDidUnderflow(false);
        double scrollOffsetLocal = (((SliverConstraints)constraintsLocal).scrollOffset + ((SliverConstraints)constraintsLocal).cacheOrigin);
        DartRuntimePrimitives.Assert(() => (scrollOffsetLocal >= 0.0));
        double remainingExtent = ((SliverConstraints)constraintsLocal).remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => (remainingExtent >= 0.0));
        double targetEndScrollOffset = (scrollOffsetLocal + remainingExtent);
        SliverGridLayout layoutLocal = this._gridDelegate.getLayout(constraintsLocal);
        long firstIndexLocal = layoutLocal.getMinChildIndexForScrollOffset(scrollOffsetLocal);
        long? targetLastIndex = (double.IsFinite(targetEndScrollOffset) ? layoutLocal.getMaxChildIndexForScrollOffset(targetEndScrollOffset) : null);
        if ((firstChild is not null))
        {
            long leadingGarbage = calculateLeadingGarbage(firstIndex: firstIndexLocal);
            long trailingGarbage = ((targetLastIndex is not null) ? calculateTrailingGarbage(lastIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetLastIndex))) : 0L);
            collectGarbage(leadingGarbage, trailingGarbage);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        SliverGridGeometry firstChildGridGeometry = layoutLocal.getGeometryForChildIndex(firstIndexLocal);
        if ((firstChild is null))
        {
            if (!addInitialChild(index: firstIndexLocal, layoutOffset: ((SliverGridGeometry)firstChildGridGeometry).scrollOffset))
            {
                double max = layoutLocal.computeMaxScrollOffset(((RenderSliverBoxChildManager)childManager).childCount);
                geometry = new SliverGeometry(scrollExtent: max, maxPaintExtent: max);
                childManager.didFinishLayout();
                return;
            }
        }
        double leadingScrollOffsetLocal = ((SliverGridGeometry)firstChildGridGeometry).scrollOffset;
        double trailingScrollOffsetLocal = ((SliverGridGeometry)firstChildGridGeometry).trailingScrollOffset;
        RenderBox? trailingChildWithLayout = default!;
        var reachedEnd = false;
        for (long indexLocal = (indexOf(firstChild!) - 1L); (indexLocal >= firstIndexLocal); --indexLocal)
        {
            SliverGridGeometry gridGeometry = layoutLocal.getGeometryForChildIndex(indexLocal);
            RenderBox child = insertAndLayoutLeadingChild(gridGeometry.getBoxConstraints(constraintsLocal))!;
            var childParentData = ((SliverGridParentData?)(object?)child.parentData!)!;
            childParentData.layoutOffset = ((SliverGridGeometry)gridGeometry).scrollOffset;
            childParentData.crossAxisOffset = ((SliverGridGeometry)gridGeometry).crossAxisOffset;
            DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(childParentData) == indexLocal));
            trailingChildWithLayout ??= child;
            trailingScrollOffsetLocal = Math.Max(trailingScrollOffsetLocal, ((SliverGridGeometry)gridGeometry).trailingScrollOffset);
        }
        if ((trailingChildWithLayout is null))
        {
            firstChild!.layout(firstChildGridGeometry.getBoxConstraints(constraintsLocal));
            var childParentDataLocal = ((SliverGridParentData?)(object?)firstChild!.parentData!)!;
            childParentDataLocal.layoutOffset = ((SliverGridGeometry)firstChildGridGeometry).scrollOffset;
            childParentDataLocal.crossAxisOffset = ((SliverGridGeometry)firstChildGridGeometry).crossAxisOffset;
            trailingChildWithLayout = firstChild;
        }
        for (long indexAlternate = (indexOf(trailingChildWithLayout!) + 1L); ((targetLastIndex is null) || (indexAlternate <= DartRuntimePrimitives.RequireValue(targetLastIndex))); ++indexAlternate)
        {
            SliverGridGeometry gridGeometryLocal = layoutLocal.getGeometryForChildIndex(indexAlternate);
            BoxConstraints childConstraints = gridGeometryLocal.getBoxConstraints(constraintsLocal);
            RenderBox? childLocal = childAfter(trailingChildWithLayout!);
            if (((childLocal is null) || (indexOf(childLocal) != indexAlternate)))
            {
                childLocal = insertAndLayoutChild(childConstraints, after: trailingChildWithLayout);
                if ((childLocal is null))
                {
                    reachedEnd = true;
                    break;
                }
            }
            else
            {
                childLocal.layout(childConstraints);
            }
            trailingChildWithLayout = childLocal;
            var childParentDataAlternate = ((SliverGridParentData?)(object?)childLocal.parentData!)!;
            childParentDataAlternate.layoutOffset = ((SliverGridGeometry)gridGeometryLocal).scrollOffset;
            childParentDataAlternate.crossAxisOffset = ((SliverGridGeometry)gridGeometryLocal).crossAxisOffset;
            DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(childParentDataAlternate) == indexAlternate));
            trailingScrollOffsetLocal = Math.Max(trailingScrollOffsetLocal, ((SliverGridGeometry)gridGeometryLocal).trailingScrollOffset);
        }
        long lastIndexLocal = indexOf(lastChild!);
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => (indexOf(firstChild!) == firstIndexLocal));
        DartRuntimePrimitives.Assert(() => ((targetLastIndex is null) || (lastIndexLocal <= DartRuntimePrimitives.RequireValue(targetLastIndex))));
        double estimatedTotalExtent = (reachedEnd ? trailingScrollOffsetLocal : childManager.estimateMaxScrollOffset(constraintsLocal, firstIndex: firstIndexLocal, lastIndex: lastIndexLocal, leadingScrollOffset: leadingScrollOffsetLocal, trailingScrollOffset: trailingScrollOffsetLocal));
        double paintExtentLocal = calculatePaintOffset(constraintsLocal, from: Math.Min(((SliverConstraints)constraintsLocal).scrollOffset, leadingScrollOffsetLocal), to: trailingScrollOffsetLocal);
        double cacheExtentLocal = calculateCacheOffset(constraintsLocal, from: leadingScrollOffsetLocal, to: trailingScrollOffsetLocal);
        geometry = new SliverGeometry(scrollExtent: estimatedTotalExtent, paintExtent: paintExtentLocal, maxPaintExtent: estimatedTotalExtent, cacheExtent: cacheExtentLocal, hasVisualOverflow: (((estimatedTotalExtent > paintExtentLocal) || (((SliverConstraints)constraintsLocal).scrollOffset > 0.0)) || (((SliverConstraints)constraintsLocal).overlap != 0.0)));
        if ((estimatedTotalExtent == trailingScrollOffsetLocal))
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }

}

