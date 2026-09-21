// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_grid.dart
using Doroti.Runtime;

namespace Doroti.Framework.Rendering;

public class SliverGridGeometry
{
    public virtual double scrollOffset { get; private set; } = default!;
    public virtual double crossAxisOffset { get; private set; } = default!;
    public virtual double mainAxisExtent { get; private set; } = default!;
    public virtual double crossAxisExtent { get; private set; } = default!;

    public SliverGridGeometry(
        double scrollOffset,
        double crossAxisOffset,
        double mainAxisExtent,
        double crossAxisExtent
    )
    {
        this.scrollOffset = scrollOffset;
        this.crossAxisOffset = crossAxisOffset;
        this.mainAxisExtent = mainAxisExtent;
        this.crossAxisExtent = crossAxisExtent;
    }

    public virtual double trailingScrollOffset => scrollOffset + mainAxisExtent;

    public virtual BoxConstraints getBoxConstraints(SliverConstraints constraints)
    {
        return constraints.asBoxConstraints(
            minExtent: mainAxisExtent,
            maxExtent: mainAxisExtent,
            crossAxisExtent: crossAxisExtent
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        var properties = new List<string>
        {
            $"scrollOffset: {scrollOffset}",
            $"crossAxisOffset: {crossAxisOffset}",
            $"mainAxisExtent: {mainAxisExtent}",
            $"crossAxisExtent: {crossAxisExtent}",
        };
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

    public SliverGridRegularTileLayout(
        long crossAxisCount,
        double mainAxisStride,
        double crossAxisStride,
        double childMainAxisExtent,
        double childCrossAxisExtent,
        bool reverseCrossAxis
    )
    {
        this.crossAxisCount = crossAxisCount;
        this.mainAxisStride = mainAxisStride;
        this.crossAxisStride = crossAxisStride;
        this.childMainAxisExtent = childMainAxisExtent;
        this.childCrossAxisExtent = childCrossAxisExtent;
        this.reverseCrossAxis = reverseCrossAxis;
        System.Diagnostics.Debug.Assert(crossAxisCount > 0L);
        System.Diagnostics.Debug.Assert(mainAxisStride >= 0L);
        System.Diagnostics.Debug.Assert(crossAxisStride >= 0L);
        System.Diagnostics.Debug.Assert(childMainAxisExtent >= 0L);
        System.Diagnostics.Debug.Assert(childCrossAxisExtent >= 0L);
    }

    public virtual long getMinChildIndexForScrollOffset(double scrollOffset)
    {
        return (mainAxisStride > Foundation.ConstantsLibrary.precisionErrorTolerance)
            ? (crossAxisCount * checked((long)(scrollOffset / mainAxisStride)))
            : 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getMaxChildIndexForScrollOffset(double scrollOffset)
    {
        if (mainAxisStride > 0.0)
        {
            long mainAxisCount = (scrollOffset / mainAxisStride).ceil();
            return Math.Max(0L, (crossAxisCount * mainAxisCount) - 1L);
        }
        return 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getOffsetFromStartInCrossAxis(double crossAxisStart)
    {
        if (reverseCrossAxis)
        {
            return (crossAxisCount * crossAxisStride)
                - crossAxisStart
                - childCrossAxisExtent
                - (crossAxisStride - childCrossAxisExtent);
        }
        return crossAxisStart;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SliverGridGeometry getGeometryForChildIndex(long index)
    {
        double crossAxisStart = index % crossAxisCount * crossAxisStride;
        return new SliverGridGeometry(
            scrollOffset: checked(index / crossAxisCount) * mainAxisStride,
            crossAxisOffset: _getOffsetFromStartInCrossAxis(crossAxisStart),
            mainAxisExtent: childMainAxisExtent,
            crossAxisExtent: childCrossAxisExtent
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxScrollOffset(long childCount)
    {
        if (childCount == 0L)
        {
            return 0.0;
        }
        long mainAxisCount = checked((childCount - 1L) / crossAxisCount) + 1L;
        double mainAxisSpacing = mainAxisStride - childMainAxisExtent;
        return (mainAxisStride * mainAxisCount) - mainAxisSpacing;
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

    public SliverGridDelegateWithFixedCrossAxisCount(
        long crossAxisCount,
        double mainAxisSpacing = 0.0,
        double crossAxisSpacing = 0.0,
        double childAspectRatio = 1.0,
        double? mainAxisExtent = null
    )
    {
        this.crossAxisCount = crossAxisCount;
        this.mainAxisSpacing = mainAxisSpacing;
        this.crossAxisSpacing = crossAxisSpacing;
        this.childAspectRatio = childAspectRatio;
        this.mainAxisExtent = mainAxisExtent;
        System.Diagnostics.Debug.Assert(crossAxisCount > 0L);
        System.Diagnostics.Debug.Assert(mainAxisSpacing >= 0L);
        System.Diagnostics.Debug.Assert(crossAxisSpacing >= 0L);
        System.Diagnostics.Debug.Assert(childAspectRatio > 0L);
        System.Diagnostics.Debug.Assert((mainAxisExtent is null) || (mainAxisExtent >= 0L));
    }

    internal virtual bool _debugAssertIsValid()
    {
        DartRuntimePrimitives.Assert(() => crossAxisCount > 0L);
        DartRuntimePrimitives.Assert(() => mainAxisSpacing >= 0.0);
        DartRuntimePrimitives.Assert(() => crossAxisSpacing >= 0.0);
        DartRuntimePrimitives.Assert(() => childAspectRatio > 0.0);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SliverGridLayout getLayout(SliverConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertIsValid());
        double usableCrossAxisExtent = Math.Max(
            0.0,
            constraints.crossAxisExtent - (crossAxisSpacing * (crossAxisCount - 1L))
        );
        double childCrossAxisExtentLocal = usableCrossAxisExtent / crossAxisCount;
        double childMainAxisExtentLocal =
            mainAxisExtent ?? (childCrossAxisExtentLocal / childAspectRatio);
        return new SliverGridRegularTileLayout(
            crossAxisCount: crossAxisCount,
            mainAxisStride: childMainAxisExtentLocal + mainAxisSpacing,
            crossAxisStride: childCrossAxisExtentLocal + crossAxisSpacing,
            childMainAxisExtent: childMainAxisExtentLocal,
            childCrossAxisExtent: childCrossAxisExtentLocal,
            reverseCrossAxis: Basic_typesLibrary.axisDirectionIsReversed(
                constraints.crossAxisDirection
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(SliverGridDelegate oldDelegate)
    {
        var __oldDelegate = (SliverGridDelegateWithFixedCrossAxisCount)(object)oldDelegate;
        return (__oldDelegate.crossAxisCount != crossAxisCount)
            || (__oldDelegate.mainAxisSpacing != mainAxisSpacing)
            || (__oldDelegate.crossAxisSpacing != crossAxisSpacing)
            || (__oldDelegate.childAspectRatio != childAspectRatio)
            || (__oldDelegate.mainAxisExtent != mainAxisExtent);
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

    public SliverGridDelegateWithMaxCrossAxisExtent(
        double maxCrossAxisExtent,
        double mainAxisSpacing = 0.0,
        double crossAxisSpacing = 0.0,
        double childAspectRatio = 1.0,
        double? mainAxisExtent = null
    )
    {
        this.maxCrossAxisExtent = maxCrossAxisExtent;
        this.mainAxisSpacing = mainAxisSpacing;
        this.crossAxisSpacing = crossAxisSpacing;
        this.childAspectRatio = childAspectRatio;
        this.mainAxisExtent = mainAxisExtent;
        System.Diagnostics.Debug.Assert(maxCrossAxisExtent > 0L);
        System.Diagnostics.Debug.Assert(mainAxisSpacing >= 0L);
        System.Diagnostics.Debug.Assert(crossAxisSpacing >= 0L);
        System.Diagnostics.Debug.Assert(childAspectRatio > 0L);
        System.Diagnostics.Debug.Assert((mainAxisExtent is null) || (mainAxisExtent >= 0L));
    }

    internal virtual bool _debugAssertIsValid(double crossAxisExtent)
    {
        DartRuntimePrimitives.Assert(() => crossAxisExtent > 0.0);
        DartRuntimePrimitives.Assert(() => maxCrossAxisExtent > 0.0);
        DartRuntimePrimitives.Assert(() => mainAxisSpacing >= 0.0);
        DartRuntimePrimitives.Assert(() => crossAxisSpacing >= 0.0);
        DartRuntimePrimitives.Assert(() => childAspectRatio > 0.0);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SliverGridLayout getLayout(SliverConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertIsValid(constraints.crossAxisExtent));
        long crossAxisCountLocal = (
            constraints.crossAxisExtent / (maxCrossAxisExtent + crossAxisSpacing)
        ).ceil();
        crossAxisCountLocal = Math.Max(1L, crossAxisCountLocal);
        double usableCrossAxisExtent = Math.Max(
            0.0,
            constraints.crossAxisExtent - (crossAxisSpacing * (crossAxisCountLocal - 1L))
        );
        double childCrossAxisExtentLocal = usableCrossAxisExtent / crossAxisCountLocal;
        double childMainAxisExtentLocal =
            mainAxisExtent ?? (childCrossAxisExtentLocal / childAspectRatio);
        return new SliverGridRegularTileLayout(
            crossAxisCount: crossAxisCountLocal,
            mainAxisStride: childMainAxisExtentLocal + mainAxisSpacing,
            crossAxisStride: childCrossAxisExtentLocal + crossAxisSpacing,
            childMainAxisExtent: childMainAxisExtentLocal,
            childCrossAxisExtent: childCrossAxisExtentLocal,
            reverseCrossAxis: Basic_typesLibrary.axisDirectionIsReversed(
                constraints.crossAxisDirection
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldRelayout(SliverGridDelegate oldDelegate)
    {
        var __oldDelegate = (SliverGridDelegateWithMaxCrossAxisExtent)(object)oldDelegate;
        return (__oldDelegate.maxCrossAxisExtent != maxCrossAxisExtent)
            || (__oldDelegate.mainAxisSpacing != mainAxisSpacing)
            || (__oldDelegate.crossAxisSpacing != crossAxisSpacing)
            || (__oldDelegate.childAspectRatio != childAspectRatio)
            || (__oldDelegate.mainAxisExtent != mainAxisExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class SliverGridParentData : SliverMultiBoxAdaptorParentData
{
    public virtual double? crossAxisOffset { get; set; } = default;

    public override string ToString() => $"crossAxisOffset={crossAxisOffset}; {base.ToString()}";
}

public class RenderSliverGrid : RenderSliverMultiBoxAdaptor
{
    internal virtual SliverGridDelegate _gridDelegate { get; set; } = default!;

    public RenderSliverGrid(
        RenderSliverBoxChildManager childManager,
        SliverGridDelegate gridDelegate
    )
        : base(childManager: childManager)
    {
        _gridDelegate = gridDelegate;
    }

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverGridParentData)
        {
            child.parentData = new SliverGridParentData();
        }
    }

    public virtual SliverGridDelegate gridDelegate
    {
        get => _gridDelegate;
        set
        {
            var __value = value;
            if (Equals(_gridDelegate, __value))
            {
                return;
            }
            if (
                (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(__value),
                        DartRuntimePrimitives.RuntimeType(_gridDelegate)
                    )
                ) || __value.shouldRelayout(_gridDelegate)
            )
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
        return DartRuntimePrimitives.RequireValue(childParentData.crossAxisOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = constraints;
        childManager.didStartLayout();
        childManager.setDidUnderflow(false);
        double scrollOffsetLocal = constraintsLocal.scrollOffset + constraintsLocal.cacheOrigin;
        DartRuntimePrimitives.Assert(() => scrollOffsetLocal >= 0.0);
        double remainingExtent = constraintsLocal.remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => remainingExtent >= 0.0);
        double targetEndScrollOffset = scrollOffsetLocal + remainingExtent;
        SliverGridLayout layoutLocal = _gridDelegate.getLayout(constraintsLocal);
        long firstIndexLocal = layoutLocal.getMinChildIndexForScrollOffset(scrollOffsetLocal);
        long? targetLastIndex = double.IsFinite(targetEndScrollOffset)
            ? layoutLocal.getMaxChildIndexForScrollOffset(targetEndScrollOffset)
            : null;
        if (firstChild is not null)
        {
            long leadingGarbage = calculateLeadingGarbage(firstIndex: firstIndexLocal);
            long trailingGarbage =
                (targetLastIndex is not null)
                    ? calculateTrailingGarbage(
                        lastIndex: DartRuntimePrimitives.RequireValue(
                            DartRuntimePrimitives.RequireValue(targetLastIndex)
                        )
                    )
                    : 0L;
            collectGarbage(leadingGarbage, trailingGarbage);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        SliverGridGeometry firstChildGridGeometry = layoutLocal.getGeometryForChildIndex(
            firstIndexLocal
        );
        if (firstChild is null)
        {
            if (
                !addInitialChild(
                    index: firstIndexLocal,
                    layoutOffset: firstChildGridGeometry.scrollOffset
                )
            )
            {
                double max = layoutLocal.computeMaxScrollOffset(childManager.childCount);
                geometry = new SliverGeometry(scrollExtent: max, maxPaintExtent: max);
                childManager.didFinishLayout();
                return;
            }
        }
        double leadingScrollOffsetLocal = firstChildGridGeometry.scrollOffset;
        double trailingScrollOffsetLocal = firstChildGridGeometry.trailingScrollOffset;
        RenderBox? trailingChildWithLayout = default!;
        var reachedEnd = false;
        for (
            long indexLocal = indexOf(firstChild!) - 1L;
            indexLocal >= firstIndexLocal;
            --indexLocal
        )
        {
            SliverGridGeometry gridGeometry = layoutLocal.getGeometryForChildIndex(indexLocal);
            RenderBox child = insertAndLayoutLeadingChild(
                gridGeometry.getBoxConstraints(constraintsLocal)
            )!;
            var childParentData = ((SliverGridParentData?)(object?)child.parentData!)!;
            childParentData.layoutOffset = gridGeometry.scrollOffset;
            childParentData.crossAxisOffset = gridGeometry.crossAxisOffset;
            DartRuntimePrimitives.Assert(() => childParentData.index == indexLocal);
            trailingChildWithLayout ??= child;
            trailingScrollOffsetLocal = Math.Max(
                trailingScrollOffsetLocal,
                gridGeometry.trailingScrollOffset
            );
        }
        if (trailingChildWithLayout is null)
        {
            firstChild!.layout(firstChildGridGeometry.getBoxConstraints(constraintsLocal));
            var childParentDataLocal = ((SliverGridParentData?)(object?)firstChild!.parentData!)!;
            childParentDataLocal.layoutOffset = firstChildGridGeometry.scrollOffset;
            childParentDataLocal.crossAxisOffset = firstChildGridGeometry.crossAxisOffset;
            trailingChildWithLayout = firstChild;
        }
        for (
            long indexAlternate = indexOf(trailingChildWithLayout!) + 1L;
            (targetLastIndex is null)
                || (indexAlternate <= DartRuntimePrimitives.RequireValue(targetLastIndex));
            ++indexAlternate
        )
        {
            SliverGridGeometry gridGeometryLocal = layoutLocal.getGeometryForChildIndex(
                indexAlternate
            );
            BoxConstraints childConstraints = gridGeometryLocal.getBoxConstraints(constraintsLocal);
            RenderBox? childLocal = childAfter(trailingChildWithLayout!);
            if ((childLocal is null) || (indexOf(childLocal) != indexAlternate))
            {
                childLocal = insertAndLayoutChild(childConstraints, after: trailingChildWithLayout);
                if (childLocal is null)
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
            var childParentDataAlternate = (
                (SliverGridParentData?)(object?)childLocal.parentData!
            )!;
            childParentDataAlternate.layoutOffset = gridGeometryLocal.scrollOffset;
            childParentDataAlternate.crossAxisOffset = gridGeometryLocal.crossAxisOffset;
            DartRuntimePrimitives.Assert(() => childParentDataAlternate.index == indexAlternate);
            trailingScrollOffsetLocal = Math.Max(
                trailingScrollOffsetLocal,
                gridGeometryLocal.trailingScrollOffset
            );
        }
        long lastIndexLocal = indexOf(lastChild!);
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => indexOf(firstChild!) == firstIndexLocal);
        DartRuntimePrimitives.Assert(() =>
            (targetLastIndex is null)
            || (lastIndexLocal <= DartRuntimePrimitives.RequireValue(targetLastIndex))
        );
        double estimatedTotalExtent = reachedEnd
            ? trailingScrollOffsetLocal
            : childManager.estimateMaxScrollOffset(
                constraintsLocal,
                firstIndex: firstIndexLocal,
                lastIndex: lastIndexLocal,
                leadingScrollOffset: leadingScrollOffsetLocal,
                trailingScrollOffset: trailingScrollOffsetLocal
            );
        double paintExtentLocal = calculatePaintOffset(
            constraintsLocal,
            from: Math.Min(constraintsLocal.scrollOffset, leadingScrollOffsetLocal),
            to: trailingScrollOffsetLocal
        );
        double cacheExtentLocal = calculateCacheOffset(
            constraintsLocal,
            from: leadingScrollOffsetLocal,
            to: trailingScrollOffsetLocal
        );
        geometry = new SliverGeometry(
            scrollExtent: estimatedTotalExtent,
            paintExtent: paintExtentLocal,
            maxPaintExtent: estimatedTotalExtent,
            cacheExtent: cacheExtentLocal,
            hasVisualOverflow: (estimatedTotalExtent > paintExtentLocal)
                || (constraintsLocal.scrollOffset > 0.0)
                || (constraintsLocal.overlap != 0.0)
        );
        if (estimatedTotalExtent == trailingScrollOffsetLocal)
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }
}
