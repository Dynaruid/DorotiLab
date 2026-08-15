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

namespace Doroti.Generated.Framework.Rendering;

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
            var offset__3233 = 0.0;
            double? itemExtent__3261 = default!;
            for (var i__3288 = 0L; (i__3288 < index); i__3288++)
            {
                long? childCount__3332 = ((RenderSliverBoxChildManager)childManager).estimatedChildCount;
                if (((childCount__3332 is not null) && (i__3288 > (DartRuntimePrimitives.RequireValue(childCount__3332) - 1L))))
                {
                    long childCount__3332__value3391 = DartRuntimePrimitives.RequireValue(childCount__3332);
                    break;
                }
                itemExtent__3261 = this.itemExtentBuilder!(i__3288, this.layoutDimensions);
                if ((itemExtent__3261 is null))
                {
                    break;
                }
                offset__3233 += DartRuntimePrimitives.RequireValue(itemExtent__3261);
            }
            return offset__3233;
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
                double actual__4446 = (scrollOffset / itemExtent);
                long round__4500 = actual__4446.round();
                if (((((actual__4446 * itemExtent) - (round__4500 * itemExtent))).abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    return round__4500;
                }
                return actual__4446.floor();
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
                double actual__5596 = ((scrollOffset / itemExtent) - 1L);
                long round__5654 = actual__5596.round();
                if (((((actual__5596 * itemExtent) - (round__5654 * itemExtent))).abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
                {
                    return Math.Max(0L, round__5654);
                }
                return Math.Max(0L, actual__5596.ceil());
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
            var offset__8319 = 0.0;
            double? itemExtent__8347 = default!;
            for (var i__8374 = 0L; (i__8374 < ((RenderSliverBoxChildManager)childManager).childCount); i__8374++)
            {
                itemExtent__8347 = this.itemExtentBuilder!(i__8374, this.layoutDimensions);
                if ((itemExtent__8347 is null))
                {
                    break;
                }
                offset__8319 += DartRuntimePrimitives.RequireValue(itemExtent__8347);
            }
            return offset__8319;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long _getChildIndexForScrollOffset(double scrollOffset, ItemExtentBuilder callback)
    {
        if ((scrollOffset == 0.0))
        {
            return 0L;
        }
        var position__8758 = 0.0;
        var index__8782 = 0L;
        double? itemExtent__8805 = default!;
        while ((position__8758 < scrollOffset))
        {
            long? childCount__8872 = ((RenderSliverBoxChildManager)childManager).estimatedChildCount;
            if (((childCount__8872 is not null) && (index__8782 > (DartRuntimePrimitives.RequireValue(childCount__8872) - 1L))))
            {
                long childCount__8872__value8929 = DartRuntimePrimitives.RequireValue(childCount__8872);
                break;
            }
            itemExtent__8805 = callback(index__8782, this.layoutDimensions);
            if ((itemExtent__8805 is null))
            {
                break;
            }
            position__8758 += DartRuntimePrimitives.RequireValue(itemExtent__8805);
            ++index__8782;
        }
        return (index__8782 - 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _getChildConstraints(long index)
    {
        double extent__9249 = default!;
        if ((this.itemExtentBuilder is null))
        {
            extent__9249 = DartRuntimePrimitives.RequireValue(this.itemExtent);
        }
        else
        {
            extent__9249 = DartRuntimePrimitives.RequireValue(this.itemExtentBuilder!(index, this.layoutDimensions));
        }
        return constraints.asBoxConstraints(minExtent: extent__9249, maxExtent: extent__9249);
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
                    double itemExtent__10500 = DartRuntimePrimitives.RequireValue(this.itemExtent);
                    double scrollExtent__10552 = geometry!.scrollExtent;
                    double count__10612 = (scrollExtent__10552 / itemExtent__10500);
                    double diff__10668 = ((count__10612.roundToDouble() - count__10612)).abs();
                    if ((((diff__10668 * itemExtent__10500) > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) && (diff__10668 > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("RenderSliverFixedExtentBoxAdaptor.computeMaxScrollOffset() returned a value that is not an even multiple of its itemExtent."), new ErrorDescription($"The itemExtent__10500 was {itemExtent__10500}, but the scrollExtent was {scrollExtent__10552}."), new ErrorDescription($"The difference was {diff__10668}, which is greater than precisionErrorTolerance ({(global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)})."), describeForError("The render object in question was") });
                    }
                }
                return true;
            });
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => ((((this.itemExtent is not null) && (this.itemExtentBuilder is null))) || (((this.itemExtent is null) && (this.itemExtentBuilder is not null)))));
        DartRuntimePrimitives.Assert(() => ((this.itemExtentBuilder is not null) || ((double.IsFinite(DartRuntimePrimitives.RequireValue(this.itemExtent)) && (DartRuntimePrimitives.RequireValue(this.itemExtent) >= 0L)))));
        SliverConstraints constraints__11766 = this.constraints;
        childManager.didStartLayout();
        childManager.setDidUnderflow(false);
        double scrollOffset__11892 = (((SliverConstraints)constraints__11766).scrollOffset + ((SliverConstraints)constraints__11766).cacheOrigin);
        DartRuntimePrimitives.Assert(() => (scrollOffset__11892 >= 0.0));
        double remainingExtent__12009 = ((SliverConstraints)constraints__11766).remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => (remainingExtent__12009 >= 0.0));
        double targetEndScrollOffset__12114 = (scrollOffset__11892 + remainingExtent__12009);
        _currentLayoutDimensions = new SliverLayoutDimensions(scrollOffset: ((SliverConstraints)constraints__11766).scrollOffset, precedingScrollExtent: ((SliverConstraints)constraints__11766).precedingScrollExtent, viewportMainAxisExtent: ((SliverConstraints)constraints__11766).viewportMainAxisExtent, crossAxisExtent: ((SliverConstraints)constraints__11766).crossAxisExtent);
        double deprecatedExtraItemExtent__12534 = -1;
        long firstIndex__12581 = getMinChildIndexForScrollOffset(scrollOffset__11892, deprecatedExtraItemExtent__12534);
        long? targetLastIndex__12683 = (double.IsFinite(targetEndScrollOffset__12114) ? getMaxChildIndexForScrollOffset(targetEndScrollOffset__12114, deprecatedExtraItemExtent__12534) : null);
        if ((firstChild is not null))
        {
            long leadingGarbage__12887 = calculateLeadingGarbage(firstIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex__12581)));
            long trailingGarbage__12969 = ((targetLastIndex__12683 is not null) ? calculateTrailingGarbage(lastIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetLastIndex__12683))) : 0L);
            collectGarbage(leadingGarbage__12887, trailingGarbage__12969);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        if ((firstChild is null))
        {
            double layoutOffset__13243 = indexToLayoutOffset(deprecatedExtraItemExtent__12534, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex__12581)));
            if (!addInitialChild(index: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex__12581)), layoutOffset: layoutOffset__13243))
            {
                double max__13501 = default!;
                if ((firstIndex__12581 <= 0L))
                {
                    max__13501 = 0.0;
                }
                else
                {
                    max__13501 = computeMaxScrollOffset(constraints__11766, deprecatedExtraItemExtent__12534);
                }
                geometry = new SliverGeometry(scrollExtent: max__13501, maxPaintExtent: max__13501);
                childManager.didFinishLayout();
                return;
            }
        }
        RenderBox? trailingChildWithLayout__13826 = default!;
        for (long index__13865 = (indexOf(firstChild!) - 1L); (index__13865 >= DartRuntimePrimitives.RequireValue(firstIndex__12581)); --index__13865)
        {
            RenderBox? child__13954 = insertAndLayoutLeadingChild(_getChildConstraints(index__13865));
            if ((child__13954 is null))
            {
                geometry = new SliverGeometry(scrollOffsetCorrection: indexToLayoutOffset(deprecatedExtraItemExtent__12534, index__13865));
                return;
            }
            var childParentData__14425 = ((SliverMultiBoxAdaptorParentData?)(object?)child__13954.parentData!)!;
            childParentData__14425.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent__12534, index__13865);
            DartRuntimePrimitives.Assert(() => (((SliverMultiBoxAdaptorParentData)childParentData__14425).index == index__13865));
            trailingChildWithLayout__13826 ??= child__13954;
        }
        if ((trailingChildWithLayout__13826 is null))
        {
            firstChild!.layout(_getChildConstraints(indexOf(firstChild!)));
            var childParentData__14808 = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
            childParentData__14808.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent__12534, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex__12581)));
            trailingChildWithLayout__13826 = firstChild;
        }
        double estimatedMaxScrollOffset__15045 = double.PositiveInfinity;
        for (long index__15109 = (indexOf(trailingChildWithLayout__13826!) + 1L); ((targetLastIndex__12683 is null) || (index__15109 <= DartRuntimePrimitives.RequireValue(targetLastIndex__12683))); ++index__15109)
        {
            RenderBox? child__15254 = childAfter(trailingChildWithLayout__13826!);
            if (((child__15254 is null) || (indexOf(child__15254) != index__15109)))
            {
                child__15254 = insertAndLayoutChild(_getChildConstraints(index__15109), after: trailingChildWithLayout__13826);
                if ((child__15254 is null))
                {
                    estimatedMaxScrollOffset__15045 = indexToLayoutOffset(deprecatedExtraItemExtent__12534, index__15109);
                    break;
                }
            }
            else
            {
                child__15254.layout(_getChildConstraints(index__15109));
            }
            trailingChildWithLayout__13826 = child__15254;
            var childParentData__15768 = ((SliverMultiBoxAdaptorParentData?)(object?)child__15254.parentData!)!;
            DartRuntimePrimitives.Assert(() => (((SliverMultiBoxAdaptorParentData)childParentData__15768).index == index__15109));
            childParentData__15768.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent__12534, DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__15768).index));
        }
        long lastIndex__16041 = indexOf(lastChild!);
        double leadingScrollOffset__16091 = indexToLayoutOffset(deprecatedExtraItemExtent__12534, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(firstIndex__12581)));
        double trailingScrollOffset__16190 = indexToLayoutOffset(deprecatedExtraItemExtent__12534, (DartRuntimePrimitives.RequireValue(lastIndex__16041) + 1L));
        DartRuntimePrimitives.Assert(() => ((DartRuntimePrimitives.RequireValue(firstIndex__12581) == 0L) || ((DartRuntimePrimitives.RequireValue(childScrollOffset(firstChild!)) - scrollOffset__11892) <= global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance)));
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => (indexOf(firstChild!) == DartRuntimePrimitives.RequireValue(firstIndex__12581)));
        DartRuntimePrimitives.Assert(() => ((targetLastIndex__12683 is null) || (lastIndex__16041 <= DartRuntimePrimitives.RequireValue(targetLastIndex__12683))));
        estimatedMaxScrollOffset__15045 = Math.Min(estimatedMaxScrollOffset__15045, estimateMaxScrollOffset(constraints__11766, firstIndex: DartRuntimePrimitives.RequireValue(firstIndex__12581), lastIndex: DartRuntimePrimitives.RequireValue(lastIndex__16041), leadingScrollOffset: DartRuntimePrimitives.RequireValue(leadingScrollOffset__16091), trailingScrollOffset: DartRuntimePrimitives.RequireValue(trailingScrollOffset__16190)));
        double paintExtent__16915 = calculatePaintOffset(constraints__11766, from: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(leadingScrollOffset__16091)), to: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(trailingScrollOffset__16190)));
        double cacheExtent__17060 = calculateCacheOffset(constraints__11766, from: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(leadingScrollOffset__16091)), to: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(trailingScrollOffset__16190)));
        double targetEndScrollOffsetForPaint__17205 = (((SliverConstraints)constraints__11766).scrollOffset + ((SliverConstraints)constraints__11766).remainingPaintExtent);
        long? targetLastIndexForPaint__17321 = (double.IsFinite(targetEndScrollOffsetForPaint__17205) ? getMaxChildIndexForScrollOffset(targetEndScrollOffsetForPaint__17205, deprecatedExtraItemExtent__12534) : null);
        geometry = new SliverGeometry(scrollExtent: estimatedMaxScrollOffset__15045, paintExtent: paintExtent__16915, cacheExtent: cacheExtent__17060, maxPaintExtent: estimatedMaxScrollOffset__15045, hasVisualOverflow: ((((targetLastIndexForPaint__17321 is not null) && (lastIndex__16041 >= DartRuntimePrimitives.RequireValue(targetLastIndexForPaint__17321)))) || (((SliverConstraints)constraints__11766).scrollOffset > 0.0)));
        if ((estimatedMaxScrollOffset__15045 == DartRuntimePrimitives.RequireValue(trailingScrollOffset__16190)))
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

