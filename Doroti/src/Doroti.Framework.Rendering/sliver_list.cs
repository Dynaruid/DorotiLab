// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_list.dart
using Doroti.Runtime;

namespace Doroti.Framework.Rendering;

public class RenderSliverList : RenderSliverMultiBoxAdaptor
{
    public RenderSliverList(RenderSliverBoxChildManager childManager)
        : base(childManager: childManager) { }

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
        BoxConstraints childConstraints = constraintsLocal.asBoxConstraints();
        var leadingGarbage = 0L;
        var trailingGarbage = 0L;
        var reachedEnd = false;
        if (firstChild is null)
        {
            if (!addInitialChild())
            {
                geometry = SliverGeometry.zero;
                childManager.didFinishLayout();
                return;
            }
        }
        RenderBox? leadingChildWithLayout = default!;
        RenderBox? trailingChildWithLayout = default!;
        RenderBox? earliestUsefulChild = firstChild;
        if (childScrollOffset(firstChild!) is null)
        {
            var leadingChildrenWithoutLayoutOffset = 0L;
            while (
                (earliestUsefulChild is not null)
                && (childScrollOffset(earliestUsefulChild) is null)
            )
            {
                earliestUsefulChild = childAfter(earliestUsefulChild);
                leadingChildrenWithoutLayoutOffset += 1L;
            }
            collectGarbage(leadingChildrenWithoutLayoutOffset, 0L);
            if (firstChild is null)
            {
                if (!addInitialChild())
                {
                    geometry = SliverGeometry.zero;
                    childManager.didFinishLayout();
                    return;
                }
            }
        }
        earliestUsefulChild = firstChild;
        for (
            double earliestScrollOffset = (
                childScrollOffset(earliestUsefulChild!)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
            earliestScrollOffset > scrollOffsetLocal;
            earliestScrollOffset = (
                    childScrollOffset(earliestUsefulChild)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
        )
        {
            earliestUsefulChild = insertAndLayoutLeadingChild(
                childConstraints,
                parentUsesSize: true
            );
            if (earliestUsefulChild is null)
            {
                var childParentData = ((SliverMultiBoxAdaptorParentData?)firstChild!.parentData!)!;
                childParentData.layoutOffset = 0.0;
                if (scrollOffsetLocal == 0.0)
                {
                    firstChild!.layout(childConstraints, parentUsesSize: true);
                    earliestUsefulChild = firstChild;
                    leadingChildWithLayout = earliestUsefulChild;
                    trailingChildWithLayout ??= earliestUsefulChild;
                    break;
                }
                else
                {
                    geometry = new SliverGeometry(scrollOffsetCorrection: -scrollOffsetLocal);
                    return;
                }
            }
            double firstChildScrollOffset = earliestScrollOffset - paintExtentOf(firstChild!);
            if (firstChildScrollOffset < -Foundation.ConstantsLibrary.precisionErrorTolerance)
            {
                geometry = new SliverGeometry(scrollOffsetCorrection: -firstChildScrollOffset);
                var childParentDataLocal = (
                    (SliverMultiBoxAdaptorParentData?)firstChild!.parentData!
                )!;
                childParentDataLocal.layoutOffset = 0.0;
                return;
            }
            var childParentDataAlternate = (
                (SliverMultiBoxAdaptorParentData?)earliestUsefulChild.parentData!
            )!;
            childParentDataAlternate.layoutOffset = firstChildScrollOffset;
            DartRuntimePrimitives.Assert(() => Equals(earliestUsefulChild, firstChild));
            leadingChildWithLayout = earliestUsefulChild;
            trailingChildWithLayout ??= earliestUsefulChild;
        }
        DartRuntimePrimitives.Assert(() =>
            (
                childScrollOffset(firstChild!)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) > -Foundation.ConstantsLibrary.precisionErrorTolerance
        );
        if (scrollOffsetLocal < Foundation.ConstantsLibrary.precisionErrorTolerance)
        {
            while (indexOf(firstChild!) > 0L)
            {
                double earliestScrollOffsetLocal = (
                    childScrollOffset(firstChild!)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                );
                earliestUsefulChild = insertAndLayoutLeadingChild(
                    childConstraints,
                    parentUsesSize: true
                );
                DartRuntimePrimitives.Assert(() => earliestUsefulChild is not null);
                double firstChildScrollOffsetLocal =
                    earliestScrollOffsetLocal - paintExtentOf(firstChild!);
                var childParentDataNested = (
                    (SliverMultiBoxAdaptorParentData?)firstChild!.parentData!
                )!;
                childParentDataNested.layoutOffset = 0.0;
                if (
                    firstChildScrollOffsetLocal
                    < -Foundation.ConstantsLibrary.precisionErrorTolerance
                )
                {
                    geometry = new SliverGeometry(
                        scrollOffsetCorrection: -firstChildScrollOffsetLocal
                    );
                    return;
                }
            }
        }
        DartRuntimePrimitives.Assert(() => Equals(earliestUsefulChild, firstChild));
        DartRuntimePrimitives.Assert(() =>
            (
                childScrollOffset(earliestUsefulChild!)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) <= scrollOffsetLocal
        );
        if (leadingChildWithLayout is null)
        {
            earliestUsefulChild!.layout(childConstraints, parentUsesSize: true);
            leadingChildWithLayout = earliestUsefulChild;
            trailingChildWithLayout = earliestUsefulChild;
        }
        var inLayoutRange = true;
        RenderBox? child = DartRuntimePrimitives.RequireReference(earliestUsefulChild);
        long indexLocal = indexOf(child!);
        double endScrollOffset =
            (
                childScrollOffset(child)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) + paintExtentOf(child);
        bool advance()
        {
            DartRuntimePrimitives.Assert(() => child is not null);
            if (Equals(child, trailingChildWithLayout))
            {
                inLayoutRange = false;
            }
            child = childAfter(child!);
            if (child is null)
            {
                inLayoutRange = false;
            }
            indexLocal += 1L;
            if (!inLayoutRange)
            {
                if ((child is null) || (indexOf(child!) != indexLocal))
                {
                    child = insertAndLayoutChild(
                        childConstraints,
                        after: trailingChildWithLayout,
                        parentUsesSize: true
                    );
                    if (child is null)
                    {
                        return false;
                    }
                }
                else
                {
                    child!.layout(childConstraints, parentUsesSize: true);
                }
                trailingChildWithLayout = child;
            }
            DartRuntimePrimitives.Assert(() => child is not null);
            var childParentDataCurrent = ((SliverMultiBoxAdaptorParentData?)child!.parentData!)!;
            childParentDataCurrent.layoutOffset = endScrollOffset;
            DartRuntimePrimitives.Assert(() => childParentDataCurrent.index == indexLocal);
            endScrollOffset =
                (
                    childScrollOffset(child!)
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) + paintExtentOf(child!);
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        while (endScrollOffset < scrollOffsetLocal)
        {
            leadingGarbage += 1L;
            if (!advance())
            {
                DartRuntimePrimitives.Assert(() => leadingGarbage == childCount);
                DartRuntimePrimitives.Assert(() => child is null);
                collectGarbage(leadingGarbage - 1L, 0L);
                DartRuntimePrimitives.Assert(() => Equals(firstChild, lastChild));
                double extent =
                    (
                        childScrollOffset(lastChild!)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) + paintExtentOf(lastChild!);
                geometry = new SliverGeometry(scrollExtent: extent, maxPaintExtent: extent);
                return;
            }
        }
        while (endScrollOffset < targetEndScrollOffset)
        {
            if (!advance())
            {
                reachedEnd = true;
                break;
            }
        }
        if (child is not null)
        {
            child = childAfter(child!);
            while (child is not null)
            {
                trailingGarbage += 1L;
                child = childAfter(child!);
            }
        }
        collectGarbage(leadingGarbage, trailingGarbage);
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        double estimatedMaxScrollOffset = default!;
        if (reachedEnd)
        {
            estimatedMaxScrollOffset = endScrollOffset;
        }
        else
        {
            estimatedMaxScrollOffset = childManager.estimateMaxScrollOffset(
                constraintsLocal,
                firstIndex: indexOf(firstChild!),
                lastIndex: indexOf(lastChild!),
                leadingScrollOffset: childScrollOffset(firstChild!),
                trailingScrollOffset: endScrollOffset
            );
            DartRuntimePrimitives.Assert(() =>
                estimatedMaxScrollOffset
                >= (
                    endScrollOffset
                    - (
                        childScrollOffset(firstChild!)
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
            );
        }
        double paintExtentLocal = calculatePaintOffset(
            constraintsLocal,
            from: (
                childScrollOffset(firstChild!)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            to: endScrollOffset
        );
        double cacheExtentLocal = calculateCacheOffset(
            constraintsLocal,
            from: (
                childScrollOffset(firstChild!)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            to: endScrollOffset
        );
        double targetEndScrollOffsetForPaint =
            constraintsLocal.scrollOffset + constraintsLocal.remainingPaintExtent;
        geometry = new SliverGeometry(
            scrollExtent: estimatedMaxScrollOffset,
            paintExtent: paintExtentLocal,
            cacheExtent: cacheExtentLocal,
            maxPaintExtent: estimatedMaxScrollOffset,
            hasVisualOverflow: (endScrollOffset > targetEndScrollOffsetForPaint)
                || (constraintsLocal.scrollOffset > 0.0)
        );
        if (estimatedMaxScrollOffset == endScrollOffset)
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }
}
