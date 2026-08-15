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

namespace Doroti.Generated.Framework.Rendering;

public class RenderSliverList : RenderSliverMultiBoxAdaptor
{
    public RenderSliverList(RenderSliverBoxChildManager childManager) : base(childManager: childManager)
    {
    }

    public override void performLayout()
    {
        SliverConstraints constraints__1989 = this.constraints;
        childManager.didStartLayout();
        childManager.setDidUnderflow(false);
        double scrollOffset__2115 = (((SliverConstraints)constraints__1989).scrollOffset + ((SliverConstraints)constraints__1989).cacheOrigin);
        DartRuntimePrimitives.Assert(() => (scrollOffset__2115 >= 0.0));
        double remainingExtent__2232 = ((SliverConstraints)constraints__1989).remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => (remainingExtent__2232 >= 0.0));
        double targetEndScrollOffset__2337 = (scrollOffset__2115 + remainingExtent__2232);
        BoxConstraints childConstraints__2418 = constraints__1989.asBoxConstraints();
        var leadingGarbage__2477 = 0L;
        var trailingGarbage__2505 = 0L;
        var reachedEnd__2534 = false;
        if ((firstChild is null))
        {
            if (!addInitialChild())
            {
                geometry = SliverGeometry.zero;
                childManager.didFinishLayout();
                return;
            }
        }
        RenderBox? leadingChildWithLayout__4042 = default!;
        RenderBox? trailingChildWithLayout__4066 = default!;
        RenderBox? earliestUsefulChild__4107 = firstChild;
        if ((childScrollOffset(firstChild!) is null))
        {
            var leadingChildrenWithoutLayoutOffset__4475 = 0L;
            while (((earliestUsefulChild__4107 is not null) && (childScrollOffset(earliestUsefulChild__4107) is null)))
            {
                earliestUsefulChild__4107 = childAfter(earliestUsefulChild__4107);
                leadingChildrenWithoutLayoutOffset__4475 += 1L;
            }
            collectGarbage(leadingChildrenWithoutLayoutOffset__4475, 0L);
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
        earliestUsefulChild__4107 = firstChild;
        for (double earliestScrollOffset__5356 = DartRuntimePrimitives.RequireValue(childScrollOffset(earliestUsefulChild__4107!)); (earliestScrollOffset__5356 > scrollOffset__2115); earliestScrollOffset__5356 = DartRuntimePrimitives.RequireValue(childScrollOffset(earliestUsefulChild__4107)))
        {
            earliestUsefulChild__4107 = insertAndLayoutLeadingChild(childConstraints__2418, parentUsesSize: true);
            if ((earliestUsefulChild__4107 is null))
            {
                var childParentData__5758 = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
                childParentData__5758.layoutOffset = 0.0;
                if ((scrollOffset__2115 == 0.0))
                {
                    firstChild!.layout(childConstraints__2418, parentUsesSize: true);
                    earliestUsefulChild__4107 = firstChild;
                    leadingChildWithLayout__4042 = earliestUsefulChild__4107;
                    trailingChildWithLayout__4066 ??= earliestUsefulChild__4107;
                    break;
                }
                else
                {
                    geometry = new SliverGeometry(scrollOffsetCorrection: -scrollOffset__2115);
                    return;
                }
            }
            double firstChildScrollOffset__6718 = (earliestScrollOffset__5356 - paintExtentOf(firstChild!));
            if ((firstChildScrollOffset__6718 < -global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                geometry = new SliverGeometry(scrollOffsetCorrection: -firstChildScrollOffset__6718);
                var childParentData__7151 = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
                childParentData__7151.layoutOffset = 0.0;
                return;
            }
            var childParentData__7310 = ((SliverMultiBoxAdaptorParentData?)(object?)earliestUsefulChild__4107.parentData!)!;
            childParentData__7310.layoutOffset = firstChildScrollOffset__6718;
            DartRuntimePrimitives.Assert(() => (object.Equals(earliestUsefulChild__4107, firstChild)));
            leadingChildWithLayout__4042 = earliestUsefulChild__4107;
            trailingChildWithLayout__4066 ??= earliestUsefulChild__4107;
        }
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)) > -global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance));
        if ((scrollOffset__2115 < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            while ((indexOf(firstChild!) > 0L))
            {
                double earliestScrollOffset__8015 = DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!));
                earliestUsefulChild__4107 = insertAndLayoutLeadingChild(childConstraints__2418, parentUsesSize: true);
                DartRuntimePrimitives.Assert(() => (earliestUsefulChild__4107 is not null));
                double firstChildScrollOffset__8422 = (earliestScrollOffset__8015 - paintExtentOf(firstChild!));
                var childParentData__8512 = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
                childParentData__8512.layoutOffset = 0.0;
                if ((firstChildScrollOffset__8422 < -global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    geometry = new SliverGeometry(scrollOffsetCorrection: -firstChildScrollOffset__8422);
                    return;
                }
            }
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(earliestUsefulChild__4107, firstChild)));
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(childScrollOffset(earliestUsefulChild__4107!)) <= scrollOffset__2115));
        if ((leadingChildWithLayout__4042 is null))
        {
            earliestUsefulChild__4107!.layout(childConstraints__2418, parentUsesSize: true);
            leadingChildWithLayout__4042 = earliestUsefulChild__4107;
            trailingChildWithLayout__4066 = earliestUsefulChild__4107;
        }
        var inLayoutRange__10027 = true;
        var child__10057 = earliestUsefulChild__4107;
        long index__10094 = indexOf(child__10057!);
        double endScrollOffset__10130 = (DartRuntimePrimitives.RequireValue(childScrollOffset(child__10057)) + paintExtentOf(child__10057));
        bool advance()
        {
            DartRuntimePrimitives.Assert(() => (child__10057 is not null));
            if ((object.Equals(child__10057, trailingChildWithLayout__4066)))
            {
                inLayoutRange__10027 = false;
            }
            child__10057 = childAfter(child__10057!);
            if ((child__10057 is null))
            {
                inLayoutRange__10027 = false;
            }
            index__10094 += 1L;
            if (!inLayoutRange__10027)
            {
                if (((child__10057 is null) || (indexOf(child__10057!) != index__10094)))
                {
                    child__10057 = insertAndLayoutChild(childConstraints__2418, after: trailingChildWithLayout__4066, parentUsesSize: true);
                    if ((child__10057 is null))
                    {
                        return false;
                    }
                }
                else
                {
                    child__10057!.layout(childConstraints__2418, parentUsesSize: true);
                }
                trailingChildWithLayout__4066 = child__10057;
            }
            DartRuntimePrimitives.Assert(() => (child__10057 is not null));
            var childParentData__11262 = ((SliverMultiBoxAdaptorParentData?)(object?)child__10057!.parentData!)!;
            childParentData__11262.layoutOffset = endScrollOffset__10130;
            DartRuntimePrimitives.Assert(() => (((SliverMultiBoxAdaptorParentData)childParentData__11262).index == index__10094));
            endScrollOffset__10130 = (DartRuntimePrimitives.RequireValue(childScrollOffset(child__10057!)) + paintExtentOf(child__10057!));
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        while ((endScrollOffset__10130 < scrollOffset__2115))
        {
            leadingGarbage__2477 += 1L;
            if (!advance())
            {
                DartRuntimePrimitives.Assert(() => (leadingGarbage__2477 == childCount));
                DartRuntimePrimitives.Assert(() => (child__10057 is null));
                collectGarbage((leadingGarbage__2477 - 1L), 0L);
                DartRuntimePrimitives.Assert(() => (object.Equals(firstChild, lastChild)));
                double extent__11977 = (DartRuntimePrimitives.RequireValue(childScrollOffset(lastChild!)) + paintExtentOf(lastChild!));
                geometry = new SliverGeometry(scrollExtent: extent__11977, maxPaintExtent: extent__11977);
                return;
            }
        }
        while ((endScrollOffset__10130 < targetEndScrollOffset__2337))
        {
            if (!advance())
            {
                reachedEnd__2534 = true;
                break;
            }
        }
        if ((child__10057 is not null))
        {
            child__10057 = childAfter(child__10057!);
            while ((child__10057 is not null))
            {
                trailingGarbage__2505 += 1L;
                child__10057 = childAfter(child__10057!);
            }
        }
        collectGarbage(leadingGarbage__2477, trailingGarbage__2505);
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        double estimatedMaxScrollOffset__12852 = default!;
        if (reachedEnd__2534)
        {
            estimatedMaxScrollOffset__12852 = endScrollOffset__10130;
        }
        else
        {
            estimatedMaxScrollOffset__12852 = childManager.estimateMaxScrollOffset(constraints__1989, firstIndex: indexOf(firstChild!), lastIndex: indexOf(lastChild!), leadingScrollOffset: childScrollOffset(firstChild!), trailingScrollOffset: endScrollOffset__10130);
            DartRuntimePrimitives.Assert(() => (estimatedMaxScrollOffset__12852 >= (endScrollOffset__10130 - DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)))));
        }
        double paintExtent__13370 = calculatePaintOffset(constraints__1989, from: DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)), to: endScrollOffset__10130);
        double cacheExtent__13521 = calculateCacheOffset(constraints__1989, from: DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)), to: endScrollOffset__10130);
        double targetEndScrollOffsetForPaint__13672 = (((SliverConstraints)constraints__1989).scrollOffset + ((SliverConstraints)constraints__1989).remainingPaintExtent);
        geometry = new SliverGeometry(scrollExtent: estimatedMaxScrollOffset__12852, paintExtent: paintExtent__13370, cacheExtent: cacheExtent__13521, maxPaintExtent: estimatedMaxScrollOffset__12852, hasVisualOverflow: ((endScrollOffset__10130 > targetEndScrollOffsetForPaint__13672) || (((SliverConstraints)constraints__1989).scrollOffset > 0.0)));
        if ((estimatedMaxScrollOffset__12852 == endScrollOffset__10130))
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }

}

