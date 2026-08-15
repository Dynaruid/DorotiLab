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

namespace Doroti.Generated.Framework.Rendering;

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
        var properties__3120 = new List<string> { $"scrollOffset: {this.scrollOffset}", $"crossAxisOffset: {this.crossAxisOffset}", $"mainAxisExtent: {this.mainAxisExtent}", $"crossAxisExtent: {this.crossAxisExtent}" };
        return $"SliverGridGeometry({string.Join(", ", properties__3120)})";
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
        return ((this.mainAxisStride > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) ? (this.crossAxisCount * ((checked((long)(scrollOffset / this.mainAxisStride))))) : 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getMaxChildIndexForScrollOffset(double scrollOffset)
    {
        if ((this.mainAxisStride > 0.0))
        {
            long mainAxisCount__8754 = ((scrollOffset / this.mainAxisStride)).ceil();
            return Math.Max(0L, ((this.crossAxisCount * mainAxisCount__8754) - 1L));
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
        double crossAxisStart__9276 = (((index % this.crossAxisCount)) * this.crossAxisStride);
        return new SliverGridGeometry(scrollOffset: (((checked((long)(index / this.crossAxisCount)))) * this.mainAxisStride), crossAxisOffset: _getOffsetFromStartInCrossAxis(crossAxisStart__9276), mainAxisExtent: this.childMainAxisExtent, crossAxisExtent: this.childCrossAxisExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxScrollOffset(long childCount)
    {
        if ((childCount == 0L))
        {
            return 0.0;
        }
        long mainAxisCount__9821 = (((checked((long)(((childCount - 1L)) / this.crossAxisCount)))) + 1L);
        double mainAxisSpacing__9896 = (this.mainAxisStride - this.childMainAxisExtent);
        return ((this.mainAxisStride * mainAxisCount__9821) - mainAxisSpacing__9896);
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
        double usableCrossAxisExtent__15191 = Math.Max(0.0, (((SliverConstraints)constraints).crossAxisExtent - (this.crossAxisSpacing * ((this.crossAxisCount - 1L)))));
        double childCrossAxisExtent__15337 = (usableCrossAxisExtent__15191 / this.crossAxisCount);
        double childMainAxisExtent__15417 = (this.mainAxisExtent ?? (childCrossAxisExtent__15337 / this.childAspectRatio));
        return new SliverGridRegularTileLayout(crossAxisCount: this.crossAxisCount, mainAxisStride: (childMainAxisExtent__15417 + this.mainAxisSpacing), crossAxisStride: (childCrossAxisExtent__15337 + this.crossAxisSpacing), childMainAxisExtent: childMainAxisExtent__15417, childCrossAxisExtent: childCrossAxisExtent__15337, reverseCrossAxis: global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).crossAxisDirection));
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
        long crossAxisCount__19713 = ((((SliverConstraints)constraints).crossAxisExtent / ((this.maxCrossAxisExtent + this.crossAxisSpacing)))).ceil();
        crossAxisCount__19713 = Math.Max(1L, crossAxisCount__19713);
        double usableCrossAxisExtent__20007 = Math.Max(0.0, (((SliverConstraints)constraints).crossAxisExtent - (this.crossAxisSpacing * ((crossAxisCount__19713 - 1L)))));
        double childCrossAxisExtent__20153 = (usableCrossAxisExtent__20007 / crossAxisCount__19713);
        double childMainAxisExtent__20233 = (this.mainAxisExtent ?? (childCrossAxisExtent__20153 / this.childAspectRatio));
        return new SliverGridRegularTileLayout(crossAxisCount: crossAxisCount__19713, mainAxisStride: (childMainAxisExtent__20233 + this.mainAxisSpacing), crossAxisStride: (childCrossAxisExtent__20153 + this.crossAxisSpacing), childMainAxisExtent: childMainAxisExtent__20233, childCrossAxisExtent: childCrossAxisExtent__20153, reverseCrossAxis: global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).crossAxisDirection));
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
        var childParentData__23149 = ((SliverGridParentData?)(object?)__child.parentData!)!;
        return DartRuntimePrimitives.RequireValue(((SliverGridParentData)childParentData__23149).crossAxisOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        SliverConstraints constraints__23325 = this.constraints;
        childManager.didStartLayout();
        childManager.setDidUnderflow(false);
        double scrollOffset__23451 = (((SliverConstraints)constraints__23325).scrollOffset + ((SliverConstraints)constraints__23325).cacheOrigin);
        DartRuntimePrimitives.Assert(() => (scrollOffset__23451 >= 0.0));
        double remainingExtent__23568 = ((SliverConstraints)constraints__23325).remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => (remainingExtent__23568 >= 0.0));
        double targetEndScrollOffset__23673 = (scrollOffset__23451 + remainingExtent__23568);
        SliverGridLayout layout__23757 = this._gridDelegate.getLayout(constraints__23325);
        long firstIndex__23819 = layout__23757.getMinChildIndexForScrollOffset(scrollOffset__23451);
        long? targetLastIndex__23901 = (double.IsFinite(targetEndScrollOffset__23673) ? layout__23757.getMaxChildIndexForScrollOffset(targetEndScrollOffset__23673) : null);
        if ((firstChild is not null))
        {
            long leadingGarbage__24084 = calculateLeadingGarbage(firstIndex: firstIndex__23819);
            long trailingGarbage__24166 = ((targetLastIndex__23901 is not null) ? calculateTrailingGarbage(lastIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetLastIndex__23901))) : 0L);
            collectGarbage(leadingGarbage__24084, trailingGarbage__24166);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        SliverGridGeometry firstChildGridGeometry__24420 = layout__23757.getGeometryForChildIndex(firstIndex__23819);
        if ((firstChild is null))
        {
            if (!addInitialChild(index: firstIndex__23819, layoutOffset: ((SliverGridGeometry)firstChildGridGeometry__24420).scrollOffset))
            {
                double max__24727 = layout__23757.computeMaxScrollOffset(((RenderSliverBoxChildManager)childManager).childCount);
                geometry = new SliverGeometry(scrollExtent: max__24727, maxPaintExtent: max__24727);
                childManager.didFinishLayout();
                return;
            }
        }
        double leadingScrollOffset__24952 = ((SliverGridGeometry)firstChildGridGeometry__24420).scrollOffset;
        double trailingScrollOffset__25022 = ((SliverGridGeometry)firstChildGridGeometry__24420).trailingScrollOffset;
        RenderBox? trailingChildWithLayout__25105 = default!;
        var reachedEnd__25138 = false;
        for (long index__25172 = (indexOf(firstChild!) - 1L); (index__25172 >= firstIndex__23819); --index__25172)
        {
            SliverGridGeometry gridGeometry__25269 = layout__23757.getGeometryForChildIndex(index__25172);
            RenderBox child__25346 = insertAndLayoutLeadingChild(gridGeometry__25269.getBoxConstraints(constraints__23325))!;
            var childParentData__25458 = ((SliverGridParentData?)(object?)child__25346.parentData!)!;
            childParentData__25458.layoutOffset = ((SliverGridGeometry)gridGeometry__25269).scrollOffset;
            childParentData__25458.crossAxisOffset = ((SliverGridGeometry)gridGeometry__25269).crossAxisOffset;
            DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(childParentData__25458) == index__25172));
            trailingChildWithLayout__25105 ??= child__25346;
            trailingScrollOffset__25022 = Math.Max(trailingScrollOffset__25022, ((SliverGridGeometry)gridGeometry__25269).trailingScrollOffset);
        }
        if ((trailingChildWithLayout__25105 is null))
        {
            firstChild!.layout(firstChildGridGeometry__24420.getBoxConstraints(constraints__23325));
            var childParentData__25979 = ((SliverGridParentData?)(object?)firstChild!.parentData!)!;
            childParentData__25979.layoutOffset = ((SliverGridGeometry)firstChildGridGeometry__24420).scrollOffset;
            childParentData__25979.crossAxisOffset = ((SliverGridGeometry)firstChildGridGeometry__24420).crossAxisOffset;
            trailingChildWithLayout__25105 = firstChild;
        }
        for (long index__26271 = (indexOf(trailingChildWithLayout__25105!) + 1L); ((targetLastIndex__23901 is null) || (index__26271 <= DartRuntimePrimitives.RequireValue(targetLastIndex__23901))); ++index__26271)
        {
            SliverGridGeometry gridGeometry__26430 = layout__23757.getGeometryForChildIndex(index__26271);
            BoxConstraints childConstraints__26512 = gridGeometry__26430.getBoxConstraints(constraints__23325);
            RenderBox? child__26593 = childAfter(trailingChildWithLayout__25105!);
            if (((child__26593 is null) || (indexOf(child__26593) != index__26271)))
            {
                child__26593 = insertAndLayoutChild(childConstraints__26512, after: trailingChildWithLayout__25105);
                if ((child__26593 is null))
                {
                    reachedEnd__25138 = true;
                    break;
                }
            }
            else
            {
                child__26593.layout(childConstraints__26512);
            }
            trailingChildWithLayout__25105 = child__26593;
            var childParentData__27022 = ((SliverGridParentData?)(object?)child__26593.parentData!)!;
            childParentData__27022.layoutOffset = ((SliverGridGeometry)gridGeometry__26430).scrollOffset;
            childParentData__27022.crossAxisOffset = ((SliverGridGeometry)gridGeometry__26430).crossAxisOffset;
            DartRuntimePrimitives.Assert(() => (FoundationRuntimePorts.EnumIndex(childParentData__27022) == index__26271));
            trailingScrollOffset__25022 = Math.Max(trailingScrollOffset__25022, ((SliverGridGeometry)gridGeometry__26430).trailingScrollOffset);
        }
        long lastIndex__27380 = indexOf(lastChild!);
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => (indexOf(firstChild!) == firstIndex__23819));
        DartRuntimePrimitives.Assert(() => ((targetLastIndex__23901 is null) || (lastIndex__27380 <= DartRuntimePrimitives.RequireValue(targetLastIndex__23901))));
        double estimatedTotalExtent__27608 = (reachedEnd__25138 ? trailingScrollOffset__25022 : childManager.estimateMaxScrollOffset(constraints__23325, firstIndex: firstIndex__23819, lastIndex: lastIndex__27380, leadingScrollOffset: leadingScrollOffset__24952, trailingScrollOffset: trailingScrollOffset__25022));
        double paintExtent__27956 = calculatePaintOffset(constraints__23325, from: Math.Min(((SliverConstraints)constraints__23325).scrollOffset, leadingScrollOffset__24952), to: trailingScrollOffset__25022);
        double cacheExtent__28136 = calculateCacheOffset(constraints__23325, from: leadingScrollOffset__24952, to: trailingScrollOffset__25022);
        geometry = new SliverGeometry(scrollExtent: estimatedTotalExtent__27608, paintExtent: paintExtent__27956, maxPaintExtent: estimatedTotalExtent__27608, cacheExtent: cacheExtent__28136, hasVisualOverflow: (((estimatedTotalExtent__27608 > paintExtent__27956) || (((SliverConstraints)constraints__23325).scrollOffset > 0.0)) || (((SliverConstraints)constraints__23325).overlap != 0.0)));
        if ((estimatedTotalExtent__27608 == trailingScrollOffset__25022))
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }

}

