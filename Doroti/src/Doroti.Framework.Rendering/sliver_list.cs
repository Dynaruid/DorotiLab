// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_list.dart
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

public class RenderSliverList : RenderSliverMultiBoxAdaptor
{
    public RenderSliverList(RenderSliverBoxChildManager childManager) : base(childManager: childManager)
    {
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
        BoxConstraints childConstraints = constraintsLocal.asBoxConstraints();
        var leadingGarbage = 0L;
        var trailingGarbage = 0L;
        var reachedEnd = false;
        if ((firstChild is null))
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
        if ((childScrollOffset(firstChild!) is null))
        {
            var leadingChildrenWithoutLayoutOffset = 0L;
            while (((earliestUsefulChild is not null) && (childScrollOffset(earliestUsefulChild) is null)))
            {
                earliestUsefulChild = childAfter(earliestUsefulChild);
                leadingChildrenWithoutLayoutOffset += 1L;
            }
            collectGarbage(leadingChildrenWithoutLayoutOffset, 0L);
            if ((firstChild is null))
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
        for (double earliestScrollOffset = DartRuntimePrimitives.RequireValue(childScrollOffset(earliestUsefulChild!)); (earliestScrollOffset > scrollOffsetLocal); earliestScrollOffset = DartRuntimePrimitives.RequireValue(childScrollOffset(earliestUsefulChild)))
        {
            earliestUsefulChild = insertAndLayoutLeadingChild(childConstraints, parentUsesSize: true);
            if ((earliestUsefulChild is null))
            {
                var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
                childParentData.layoutOffset = 0.0;
                if ((scrollOffsetLocal == 0.0))
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
            double firstChildScrollOffset = (earliestScrollOffset - paintExtentOf(firstChild!));
            if ((firstChildScrollOffset < -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                geometry = new SliverGeometry(scrollOffsetCorrection: -firstChildScrollOffset);
                var childParentDataLocal = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
                childParentDataLocal.layoutOffset = 0.0;
                return;
            }
            var childParentDataAlternate = ((SliverMultiBoxAdaptorParentData?)(object?)earliestUsefulChild.parentData!)!;
            childParentDataAlternate.layoutOffset = firstChildScrollOffset;
            DartRuntimePrimitives.Assert(() => (object.Equals(earliestUsefulChild, firstChild)));
            leadingChildWithLayout = earliestUsefulChild;
            trailingChildWithLayout ??= earliestUsefulChild;
        }
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)) > -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance));
        if ((scrollOffsetLocal < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            while ((indexOf(firstChild!) > 0L))
            {
                double earliestScrollOffsetLocal = DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!));
                earliestUsefulChild = insertAndLayoutLeadingChild(childConstraints, parentUsesSize: true);
                DartRuntimePrimitives.Assert(() => (earliestUsefulChild is not null));
                double firstChildScrollOffsetLocal = (earliestScrollOffsetLocal - paintExtentOf(firstChild!));
                var childParentDataNested = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
                childParentDataNested.layoutOffset = 0.0;
                if ((firstChildScrollOffsetLocal < -global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    geometry = new SliverGeometry(scrollOffsetCorrection: -firstChildScrollOffsetLocal);
                    return;
                }
            }
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(earliestUsefulChild, firstChild)));
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(childScrollOffset(earliestUsefulChild!)) <= scrollOffsetLocal));
        if ((leadingChildWithLayout is null))
        {
            earliestUsefulChild!.layout(childConstraints, parentUsesSize: true);
            leadingChildWithLayout = earliestUsefulChild;
            trailingChildWithLayout = earliestUsefulChild;
        }
        var inLayoutRange = true;
        var child = earliestUsefulChild;
        long indexLocal = indexOf(child!);
        double endScrollOffset = (DartRuntimePrimitives.RequireValue(childScrollOffset(child)) + paintExtentOf(child));
        bool advance()
        {
            DartRuntimePrimitives.Assert(() => (child is not null));
            if ((object.Equals(child, trailingChildWithLayout)))
            {
                inLayoutRange = false;
            }
            child = childAfter(child!);
            if ((child is null))
            {
                inLayoutRange = false;
            }
            indexLocal += 1L;
            if (!inLayoutRange)
            {
                if (((child is null) || (indexOf(child!) != indexLocal)))
                {
                    child = insertAndLayoutChild(childConstraints, after: trailingChildWithLayout, parentUsesSize: true);
                    if ((child is null))
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
            DartRuntimePrimitives.Assert(() => (child is not null));
            var childParentDataCurrent = ((SliverMultiBoxAdaptorParentData?)(object?)child!.parentData!)!;
            childParentDataCurrent.layoutOffset = endScrollOffset;
            DartRuntimePrimitives.Assert(() => (((SliverMultiBoxAdaptorParentData)childParentDataCurrent).index == indexLocal));
            endScrollOffset = (DartRuntimePrimitives.RequireValue(childScrollOffset(child!)) + paintExtentOf(child!));
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        while ((endScrollOffset < scrollOffsetLocal))
        {
            leadingGarbage += 1L;
            if (!advance())
            {
                DartRuntimePrimitives.Assert(() => (leadingGarbage == childCount));
                DartRuntimePrimitives.Assert(() => (child is null));
                collectGarbage((leadingGarbage - 1L), 0L);
                DartRuntimePrimitives.Assert(() => (object.Equals(firstChild, lastChild)));
                double extent = (DartRuntimePrimitives.RequireValue(childScrollOffset(lastChild!)) + paintExtentOf(lastChild!));
                geometry = new SliverGeometry(scrollExtent: extent, maxPaintExtent: extent);
                return;
            }
        }
        while ((endScrollOffset < targetEndScrollOffset))
        {
            if (!advance())
            {
                reachedEnd = true;
                break;
            }
        }
        if ((child is not null))
        {
            child = childAfter(child!);
            while ((child is not null))
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
            estimatedMaxScrollOffset = childManager.estimateMaxScrollOffset(constraintsLocal, firstIndex: indexOf(firstChild!), lastIndex: indexOf(lastChild!), leadingScrollOffset: childScrollOffset(firstChild!), trailingScrollOffset: endScrollOffset);
            DartRuntimePrimitives.Assert(() => (estimatedMaxScrollOffset >= (endScrollOffset - DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)))));
        }
        double paintExtentLocal = calculatePaintOffset(constraintsLocal, from: DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)), to: endScrollOffset);
        double cacheExtentLocal = calculateCacheOffset(constraintsLocal, from: DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)), to: endScrollOffset);
        double targetEndScrollOffsetForPaint = (((SliverConstraints)constraintsLocal).scrollOffset + ((SliverConstraints)constraintsLocal).remainingPaintExtent);
        geometry = new SliverGeometry(scrollExtent: estimatedMaxScrollOffset, paintExtent: paintExtentLocal, cacheExtent: cacheExtentLocal, maxPaintExtent: estimatedMaxScrollOffset, hasVisualOverflow: ((endScrollOffset > targetEndScrollOffsetForPaint) || (((SliverConstraints)constraintsLocal).scrollOffset > 0.0)));
        if ((estimatedMaxScrollOffset == endScrollOffset))
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }

}

