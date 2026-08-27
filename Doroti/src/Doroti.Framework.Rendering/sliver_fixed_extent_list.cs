// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_fixed_extent_list.dart
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

public abstract class RenderSliverFixedExtentBoxAdaptor : RenderSliverMultiBoxAdaptor
{
    internal virtual SliverLayoutDimensions? _currentLayoutDimensions { get; set; } = default;

    protected RenderSliverFixedExtentBoxAdaptor(RenderSliverBoxChildManager childManager) : base(childManager: childManager)
    {
    }

    public virtual double? itemExtent
    {
        get => throw new NotSupportedException("Dart getter contract has no base implementation.");
        set => throw new NotSupportedException("Dart setter contract has no base implementation.");
    }
    public virtual ItemExtentBuilder? itemExtentBuilder => null;
    public virtual double indexToLayoutOffset(double itemExtent, long index)
    {
        if ((this.itemExtentBuilder is null))
        {
            itemExtent = DartRuntimePrimitives.RequireValue(this.itemExtent);
            return (itemExtent * index);
        }
        else
        {
            var offset = 0.0;
            double? itemExtentLocal = default!;
            for (var i = 0L; (i < index); i++)
            {
                long? childCount = ((RenderSliverBoxChildManager)childManager).estimatedChildCount;
                if (((childCount is not null) && (i > (DartRuntimePrimitives.RequireValue(childCount) - 1L))))
                {
                    long childCount__3332__value3391 = DartRuntimePrimitives.RequireValue(childCount);
                    break;
                }
                itemExtentLocal = this.itemExtentBuilder!(i, this.layoutDimensions);
                if ((itemExtentLocal is null))
                {
                    break;
                }
                offset += DartRuntimePrimitives.RequireValue(itemExtentLocal);
            }
            return offset;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getMinChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if ((this.itemExtentBuilder is null))
        {
            itemExtent = DartRuntimePrimitives.RequireValue(this.itemExtent);
            if ((itemExtent > 0.0))
            {
                double actual = (scrollOffset / itemExtent);
                long roundLocal = actual.round();
                if (((((actual * itemExtent) - (roundLocal * itemExtent))).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    return roundLocal;
                }
                return actual.floor();
            }
            return 0L;
        }
        else
        {
            return _getChildIndexForScrollOffset(scrollOffset, this.itemExtentBuilder!);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if ((this.itemExtentBuilder is null))
        {
            itemExtent = DartRuntimePrimitives.RequireValue(this.itemExtent);
            if ((itemExtent > 0.0))
            {
                double actual = ((scrollOffset / itemExtent) - 1L);
                long roundLocal = actual.round();
                if (((((actual * itemExtent) - (roundLocal * itemExtent))).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    return Math.Max(0L, roundLocal);
                }
                return Math.Max(0L, actual.ceil());
            }
            return 0L;
        }
        else
        {
            return _getChildIndexForScrollOffset(scrollOffset, this.itemExtentBuilder!);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double estimateMaxScrollOffset(SliverConstraints constraints, long? firstIndex = null, long? lastIndex = null, double? leadingScrollOffset = null, double? trailingScrollOffset = null)
    {
        return childManager.estimateMaxScrollOffset(constraints, firstIndex: firstIndex, lastIndex: lastIndex, leadingScrollOffset: leadingScrollOffset, trailingScrollOffset: trailingScrollOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double computeMaxScrollOffset(SliverConstraints constraints, double itemExtent)
    {
        if ((this.itemExtentBuilder is null))
        {
            itemExtent = DartRuntimePrimitives.RequireValue(this.itemExtent);
            return (((RenderSliverBoxChildManager)childManager).childCount * itemExtent);
        }
        else
        {
            var offset = 0.0;
            double? itemExtentLocal = default!;
            for (var i = 0L; (i < ((RenderSliverBoxChildManager)childManager).childCount); i++)
            {
                itemExtentLocal = this.itemExtentBuilder!(i, this.layoutDimensions);
                if ((itemExtentLocal is null))
                {
                    break;
                }
                offset += DartRuntimePrimitives.RequireValue(itemExtentLocal);
            }
            return offset;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _getChildIndexForScrollOffset(double scrollOffset, ItemExtentBuilder callback)
    {
        if ((scrollOffset == 0.0))
        {
            return 0L;
        }
        var position = 0.0;
        var index = 0L;
        double? itemExtent = default!;
        while ((position < scrollOffset))
        {
            long? childCount = ((RenderSliverBoxChildManager)childManager).estimatedChildCount;
            if (((childCount is not null) && (index > (DartRuntimePrimitives.RequireValue(childCount) - 1L))))
            {
                long childCount__8872__value8929 = DartRuntimePrimitives.RequireValue(childCount);
                break;
            }
            itemExtent = callback(index, this.layoutDimensions);
            if ((itemExtent is null))
            {
                break;
            }
            position += DartRuntimePrimitives.RequireValue(itemExtent);
            ++index;
        }
        return (index - 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _getChildConstraints(long index)
    {
        double extent = default!;
        if ((this.itemExtentBuilder is null))
        {
            extent = DartRuntimePrimitives.RequireValue(this.itemExtent);
        }
        else
        {
            extent = DartRuntimePrimitives.RequireValue(this.itemExtentBuilder!(index, this.layoutDimensions));
        }
        return constraints.asBoxConstraints(minExtent: extent, maxExtent: extent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual SliverLayoutDimensions layoutDimensions
    {
        get
        {
            return (this._currentLayoutDimensions ?? new SliverLayoutDimensions(scrollOffset: ((SliverConstraints)constraints).scrollOffset, precedingScrollExtent: ((SliverConstraints)constraints).precedingScrollExtent, viewportMainAxisExtent: ((SliverConstraints)constraints).viewportMainAxisExtent, crossAxisExtent: ((SliverConstraints)constraints).crossAxisExtent));
            return default!;
        }
    }
    public override double paintExtentOf(RenderBox child)
    {
        if ((this.itemExtentBuilder is null))
        {
            return DartRuntimePrimitives.RequireValue(this.itemExtent);
        }
        return DartRuntimePrimitives.RequireValue(this.itemExtentBuilder!(indexOf(child), this.layoutDimensions));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugAssertDoesMeetConstraints()
    {
        base.debugAssertDoesMeetConstraints();
        DartRuntimePrimitives.Assert(() =>
            {
                if (((this.itemExtentBuilder is null) && double.IsFinite(geometry!.scrollExtent)))
                {
                    double itemExtentLocal = DartRuntimePrimitives.RequireValue(this.itemExtent);
                    double scrollExtentLocal = geometry!.scrollExtent;
                    double count = (scrollExtentLocal / itemExtentLocal);
                    double diff = ((count.roundToDouble() - count)).abs();
                    if ((((diff * itemExtentLocal) > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && (diff > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("RenderSliverFixedExtentBoxAdaptor.computeMaxScrollOffset() returned a value that is not an even multiple of its itemExtent."), new ErrorDescription($"The itemExtent__10500 was {itemExtentLocal}, but the scrollExtent was {scrollExtentLocal}."), new ErrorDescription($"The difference was {diff}, which is greater than precisionErrorTolerance ({(global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)})."), describeForError("The render object in question was") });
                    }
                }
                return true;
            });
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => ((((this.itemExtent is not null) && (this.itemExtentBuilder is null))) || (((this.itemExtent is null) && (this.itemExtentBuilder is not null)))));
        DartRuntimePrimitives.Assert(() => ((this.itemExtentBuilder is not null) || ((double.IsFinite(DartRuntimePrimitives.RequireValue(this.itemExtent)) && (DartRuntimePrimitives.RequireValue(this.itemExtent) >= 0L)))));
        SliverConstraints constraintsLocal = this.constraints;
        childManager.didStartLayout();
        childManager.setDidUnderflow(false);
        double scrollOffsetLocal = (((SliverConstraints)constraintsLocal).scrollOffset + ((SliverConstraints)constraintsLocal).cacheOrigin);
        DartRuntimePrimitives.Assert(() => (scrollOffsetLocal >= 0.0));
        double remainingExtent = ((SliverConstraints)constraintsLocal).remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => (remainingExtent >= 0.0));
        double targetEndScrollOffset = (scrollOffsetLocal + remainingExtent);
        _currentLayoutDimensions = new SliverLayoutDimensions(scrollOffset: ((SliverConstraints)constraintsLocal).scrollOffset, precedingScrollExtent: ((SliverConstraints)constraintsLocal).precedingScrollExtent, viewportMainAxisExtent: ((SliverConstraints)constraintsLocal).viewportMainAxisExtent, crossAxisExtent: ((SliverConstraints)constraintsLocal).crossAxisExtent);
        double deprecatedExtraItemExtent = -1;
        long firstIndexLocal = getMinChildIndexForScrollOffset(scrollOffsetLocal, deprecatedExtraItemExtent);
        long? targetLastIndex = (double.IsFinite(targetEndScrollOffset) ? getMaxChildIndexForScrollOffset(targetEndScrollOffset, deprecatedExtraItemExtent) : null);
        if ((firstChild is not null))
        {
            long leadingGarbage = calculateLeadingGarbage(firstIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndexLocal)));
            long trailingGarbage = ((targetLastIndex is not null) ? calculateTrailingGarbage(lastIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetLastIndex))) : 0L);
            collectGarbage(leadingGarbage, trailingGarbage);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        if ((firstChild is null))
        {
            double layoutOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndexLocal)));
            if (!addInitialChild(index: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndexLocal)), layoutOffset: layoutOffsetLocal))
            {
                double max = default!;
                if ((firstIndexLocal <= 0L))
                {
                    max = 0.0;
                }
                else
                {
                    max = computeMaxScrollOffset(constraintsLocal, deprecatedExtraItemExtent);
                }
                geometry = new SliverGeometry(scrollExtent: max, maxPaintExtent: max);
                childManager.didFinishLayout();
                return;
            }
        }
        RenderBox? trailingChildWithLayout = default!;
        for (long indexLocal = (indexOf(firstChild!) - 1L); (indexLocal >= DartRuntimePrimitives.RequireValue(firstIndexLocal)); --indexLocal)
        {
            RenderBox? child = insertAndLayoutLeadingChild(_getChildConstraints(indexLocal));
            if ((child is null))
            {
                geometry = new SliverGeometry(scrollOffsetCorrection: indexToLayoutOffset(deprecatedExtraItemExtent, indexLocal));
                return;
            }
            var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            childParentData.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, indexLocal);
            DartRuntimePrimitives.Assert(() => (((SliverMultiBoxAdaptorParentData)childParentData).index == indexLocal));
            trailingChildWithLayout ??= child;
        }
        if ((trailingChildWithLayout is null))
        {
            firstChild!.layout(_getChildConstraints(indexOf(firstChild!)));
            var childParentDataLocal = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
            childParentDataLocal.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndexLocal)));
            trailingChildWithLayout = firstChild;
        }
        double estimatedMaxScrollOffset = double.PositiveInfinity;
        for (long indexAlternate = (indexOf(trailingChildWithLayout!) + 1L); ((targetLastIndex is null) || (indexAlternate <= DartRuntimePrimitives.RequireValue(targetLastIndex))); ++indexAlternate)
        {
            RenderBox? childLocal = childAfter(trailingChildWithLayout!);
            if (((childLocal is null) || (indexOf(childLocal) != indexAlternate)))
            {
                childLocal = insertAndLayoutChild(_getChildConstraints(indexAlternate), after: trailingChildWithLayout);
                if ((childLocal is null))
                {
                    estimatedMaxScrollOffset = indexToLayoutOffset(deprecatedExtraItemExtent, indexAlternate);
                    break;
                }
            }
            else
            {
                childLocal.layout(_getChildConstraints(indexAlternate));
            }
            trailingChildWithLayout = childLocal;
            var childParentDataAlternate = ((SliverMultiBoxAdaptorParentData?)(object?)childLocal.parentData!)!;
            DartRuntimePrimitives.Assert(() => (((SliverMultiBoxAdaptorParentData)childParentDataAlternate).index == indexAlternate));
            childParentDataAlternate.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentDataAlternate).index));
        }
        long lastIndexLocal = indexOf(lastChild!);
        double leadingScrollOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndexLocal)));
        double trailingScrollOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, (DartRuntimePrimitives.RequireValue(lastIndexLocal) + 1L));
        DartRuntimePrimitives.Assert(() => ((DartRuntimePrimitives.RequireValue(firstIndexLocal) == 0L) || ((DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)) - scrollOffsetLocal) <= global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)));
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => (indexOf(firstChild!) == DartRuntimePrimitives.RequireValue(firstIndexLocal)));
        DartRuntimePrimitives.Assert(() => ((targetLastIndex is null) || (lastIndexLocal <= DartRuntimePrimitives.RequireValue(targetLastIndex))));
        estimatedMaxScrollOffset = Math.Min(estimatedMaxScrollOffset, estimateMaxScrollOffset(constraintsLocal, firstIndex: DartRuntimePrimitives.RequireValue(firstIndexLocal), lastIndex: DartRuntimePrimitives.RequireValue(lastIndexLocal), leadingScrollOffset: DartRuntimePrimitives.RequireValue(leadingScrollOffsetLocal), trailingScrollOffset: DartRuntimePrimitives.RequireValue(trailingScrollOffsetLocal)));
        double paintExtentLocal = calculatePaintOffset(constraintsLocal, from: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(leadingScrollOffsetLocal)), to: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(trailingScrollOffsetLocal)));
        double cacheExtentLocal = calculateCacheOffset(constraintsLocal, from: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(leadingScrollOffsetLocal)), to: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(trailingScrollOffsetLocal)));
        double targetEndScrollOffsetForPaint = (((SliverConstraints)constraintsLocal).scrollOffset + ((SliverConstraints)constraintsLocal).remainingPaintExtent);
        long? targetLastIndexForPaint = (double.IsFinite(targetEndScrollOffsetForPaint) ? getMaxChildIndexForScrollOffset(targetEndScrollOffsetForPaint, deprecatedExtraItemExtent) : null);
        geometry = new SliverGeometry(scrollExtent: estimatedMaxScrollOffset, paintExtent: paintExtentLocal, cacheExtent: cacheExtentLocal, maxPaintExtent: estimatedMaxScrollOffset, hasVisualOverflow: ((((targetLastIndexForPaint is not null) && (lastIndexLocal >= DartRuntimePrimitives.RequireValue(targetLastIndexForPaint)))) || (((SliverConstraints)constraintsLocal).scrollOffset > 0.0)));
        if ((estimatedMaxScrollOffset == DartRuntimePrimitives.RequireValue(trailingScrollOffsetLocal)))
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }

}

public class RenderSliverFixedExtentList : RenderSliverFixedExtentBoxAdaptor
{
    internal virtual double _itemExtent { get; set; } = default!;

    public RenderSliverFixedExtentList(RenderSliverBoxChildManager childManager, double itemExtent) : base(childManager: childManager)
    {
        this._itemExtent = itemExtent;
    }

    public override double? itemExtent
    {
        get => this._itemExtent;
        set
        {
            var __value = DartRuntimePrimitives.RequireValue(value);
            if ((this._itemExtent == __value))
            {
                return;
            }
            _itemExtent = __value;
            markNeedsLayout();
        }
    }
}

public class RenderSliverVariedExtentList : RenderSliverFixedExtentBoxAdaptor
{
    internal virtual ItemExtentBuilder _itemExtentBuilder { get; set; } = default!;

    public RenderSliverVariedExtentList(RenderSliverBoxChildManager childManager, ItemExtentBuilder itemExtentBuilder) : base(childManager: childManager)
    {
        this._itemExtentBuilder = itemExtentBuilder;
    }

    public new ItemExtentBuilder? itemExtentBuilder
    {
        get => this._itemExtentBuilder;
        set
        {
            var __value = value;
            if ((object.Equals((ItemExtentBuilder)this._itemExtentBuilder, (ItemExtentBuilder)__value)))
            {
                return;
            }
            _itemExtentBuilder = __value;
            markNeedsLayout();
        }
    }
    public override double? itemExtent => null;
}

