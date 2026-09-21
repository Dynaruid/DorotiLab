// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_fixed_extent_list.dart
using Doroti.Runtime;

namespace Doroti.Framework.Rendering;

public abstract class RenderSliverFixedExtentBoxAdaptor : RenderSliverMultiBoxAdaptor
{
    internal virtual SliverLayoutDimensions? _currentLayoutDimensions { get; set; } = default;

    protected RenderSliverFixedExtentBoxAdaptor(RenderSliverBoxChildManager childManager)
        : base(childManager: childManager) { }

    public virtual double? itemExtent
    {
        get =>
            throw new NotSupportedException(
                "The generated getter contract has no base implementation."
            );
        set =>
            throw new NotSupportedException(
                "The generated setter contract has no base implementation."
            );
    }

    // Keep getter and setter in one CLR property so the varied-extent
    // implementation participates in the adaptor's virtual layout calls.
    public virtual ItemExtentBuilder? itemExtentBuilder
    {
        get => null;
        set => throw new NotSupportedException("This sliver does not use an item extent builder.");
    }

    public virtual double indexToLayoutOffset(double itemExtent, long index)
    {
        if (itemExtentBuilder is null)
        {
            itemExtent = (
                this.itemExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            return itemExtent * index;
        }
        else
        {
            var offset = 0.0;
            double? itemExtentLocal = default!;
            for (var i = 0L; i < index; i++)
            {
                long? childCount = childManager.estimatedChildCount;
                if (
                    (childCount is not null)
                    && (
                        i
                        > (
                            (
                                childCount
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            ) - 1L
                        )
                    )
                )
                {
                    long childCount__3332__value3391 = (
                        childCount
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    );
                    break;
                }
                itemExtentLocal = itemExtentBuilder!(i, layoutDimensions);
                if (itemExtentLocal is null)
                {
                    break;
                }
                offset += (
                    itemExtentLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            }
            return offset;
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long getMinChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if (itemExtentBuilder is null)
        {
            itemExtent = (
                this.itemExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (itemExtent > 0.0)
            {
                double actual = scrollOffset / itemExtent;
                long roundLocal = actual.round();
                if (
                    ((actual * itemExtent) - (roundLocal * itemExtent)).abs()
                    < Foundation.ConstantsLibrary.precisionErrorTolerance
                )
                {
                    return roundLocal;
                }
                return actual.floor();
            }
            return 0L;
        }
        else
        {
            return _getChildIndexForScrollOffset(scrollOffset, itemExtentBuilder!);
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if (itemExtentBuilder is null)
        {
            itemExtent = (
                this.itemExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (itemExtent > 0.0)
            {
                double actual = (scrollOffset / itemExtent) - 1L;
                long roundLocal = actual.round();
                if (
                    ((actual * itemExtent) - (roundLocal * itemExtent)).abs()
                    < Foundation.ConstantsLibrary.precisionErrorTolerance
                )
                {
                    return Math.Max(0L, roundLocal);
                }
                return Math.Max(0L, actual.ceil());
            }
            return 0L;
        }
        else
        {
            return _getChildIndexForScrollOffset(scrollOffset, itemExtentBuilder!);
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double estimateMaxScrollOffset(
        SliverConstraints constraints,
        long? firstIndex = null,
        long? lastIndex = null,
        double? leadingScrollOffset = null,
        double? trailingScrollOffset = null
    )
    {
        return childManager.estimateMaxScrollOffset(
            constraints,
            firstIndex: firstIndex,
            lastIndex: lastIndex,
            leadingScrollOffset: leadingScrollOffset,
            trailingScrollOffset: trailingScrollOffset
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double computeMaxScrollOffset(SliverConstraints constraints, double itemExtent)
    {
        if (itemExtentBuilder is null)
        {
            itemExtent = (
                this.itemExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            return childManager.childCount * itemExtent;
        }
        else
        {
            var offset = 0.0;
            double? itemExtentLocal = default!;
            for (var i = 0L; i < childManager.childCount; i++)
            {
                itemExtentLocal = itemExtentBuilder!(i, layoutDimensions);
                if (itemExtentLocal is null)
                {
                    break;
                }
                offset += (
                    itemExtentLocal
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
            }
            return offset;
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual long _getChildIndexForScrollOffset(
        double scrollOffset,
        ItemExtentBuilder callback
    )
    {
        if (scrollOffset == 0.0)
        {
            return 0L;
        }
        var position = 0.0;
        var index = 0L;
        double? itemExtent = default!;
        while (position < scrollOffset)
        {
            long? childCount = childManager.estimatedChildCount;
            if (
                (childCount is not null)
                && (
                    index
                    > (
                        (
                            childCount
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        ) - 1L
                    )
                )
            )
            {
                long childCount__8872__value8929 = (
                    childCount
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                break;
            }
            itemExtent = callback(index, layoutDimensions);
            if (itemExtent is null)
            {
                break;
            }
            position += (
                itemExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            ++index;
        }
        return index - 1L;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual BoxConstraints _getChildConstraints(long index)
    {
        double extent = default!;
        if (itemExtentBuilder is null)
        {
            extent = (
                itemExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        else
        {
            extent = (
                itemExtentBuilder!(index, layoutDimensions)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        return constraints.asBoxConstraints(minExtent: extent, maxExtent: extent);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual SliverLayoutDimensions layoutDimensions
    {
        get
        {
            return _currentLayoutDimensions
                ?? new SliverLayoutDimensions(
                    scrollOffset: constraints.scrollOffset,
                    precedingScrollExtent: constraints.precedingScrollExtent,
                    viewportMainAxisExtent: constraints.viewportMainAxisExtent,
                    crossAxisExtent: constraints.crossAxisExtent
                );
        }
    }

    public override double paintExtentOf(RenderBox child)
    {
        if (itemExtentBuilder is null)
        {
            return (
                itemExtent
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        }
        return (
            itemExtentBuilder!(indexOf(child), layoutDimensions)
            ?? throw new global::System.NullReferenceException("A required value was null.")
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void debugAssertDoesMeetConstraints()
    {
        base.debugAssertDoesMeetConstraints();
        DartRuntimePrimitives.Assert(() =>
        {
            if ((itemExtentBuilder is null) && double.IsFinite(geometry!.scrollExtent))
            {
                double itemExtentLocal = (
                    itemExtent
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                double scrollExtentLocal = geometry!.scrollExtent;
                double count = scrollExtentLocal / itemExtentLocal;
                double diff = (count.roundToDouble() - count).abs();
                if (
                    ((diff * itemExtentLocal) > Foundation.ConstantsLibrary.precisionErrorTolerance)
                    && (diff > Foundation.ConstantsLibrary.precisionErrorTolerance)
                )
                {
                    throw new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "RenderSliverFixedExtentBoxAdaptor.computeMaxScrollOffset() returned a value that is not an even multiple of its itemExtent."
                            ),
                            new ErrorDescription(
                                $"The itemExtent__10500 was {itemExtentLocal}, but the scrollExtent was {scrollExtentLocal}."
                            ),
                            new ErrorDescription(
                                $"The difference was {diff}, which is greater than precisionErrorTolerance ({Foundation.ConstantsLibrary.precisionErrorTolerance})."
                            ),
                            describeForError("The render object in question was"),
                        }
                    );
                }
            }
            return true;
        });
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() =>
            ((itemExtent is not null) && (itemExtentBuilder is null))
            || ((itemExtent is null) && (itemExtentBuilder is not null))
        );
        DartRuntimePrimitives.Assert(() =>
            (itemExtentBuilder is not null)
            || (
                double.IsFinite(
                    (
                        itemExtent
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
                && (
                    (
                        itemExtent
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) >= 0L
                )
            )
        );
        SliverConstraints constraintsLocal = constraints;
        childManager.didStartLayout();
        childManager.setDidUnderflow(false);
        double scrollOffsetLocal = constraintsLocal.scrollOffset + constraintsLocal.cacheOrigin;
        DartRuntimePrimitives.Assert(() => scrollOffsetLocal >= 0.0);
        double remainingExtent = constraintsLocal.remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => remainingExtent >= 0.0);
        double targetEndScrollOffset = scrollOffsetLocal + remainingExtent;
        _currentLayoutDimensions = new SliverLayoutDimensions(
            scrollOffset: constraintsLocal.scrollOffset,
            precedingScrollExtent: constraintsLocal.precedingScrollExtent,
            viewportMainAxisExtent: constraintsLocal.viewportMainAxisExtent,
            crossAxisExtent: constraintsLocal.crossAxisExtent
        );
        double deprecatedExtraItemExtent = -1;
        long firstIndexLocal = getMinChildIndexForScrollOffset(
            scrollOffsetLocal,
            deprecatedExtraItemExtent
        );
        long? targetLastIndex = double.IsFinite(targetEndScrollOffset)
            ? getMaxChildIndexForScrollOffset(targetEndScrollOffset, deprecatedExtraItemExtent)
            : null;
        if (firstChild is not null)
        {
            long leadingGarbage = calculateLeadingGarbage(firstIndex: ((firstIndexLocal)));
            long trailingGarbage =
                (targetLastIndex is not null)
                    ? calculateTrailingGarbage(
                        lastIndex: (
                            (
                                targetLastIndex
                                ?? throw new global::System.NullReferenceException(
                                    "A required value was null."
                                )
                            )
                        )
                    )
                    : 0L;
            collectGarbage(leadingGarbage, trailingGarbage);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        if (firstChild is null)
        {
            double layoutOffsetLocal = indexToLayoutOffset(
                deprecatedExtraItemExtent,
                ((firstIndexLocal))
            );
            if (!addInitialChild(index: ((firstIndexLocal)), layoutOffset: layoutOffsetLocal))
            {
                double max = default!;
                if (firstIndexLocal <= 0L)
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
        for (
            long indexLocal = indexOf(firstChild!) - 1L;
            indexLocal >= (firstIndexLocal);
            --indexLocal
        )
        {
            RenderBox? child = insertAndLayoutLeadingChild(_getChildConstraints(indexLocal));
            if (child is null)
            {
                geometry = new SliverGeometry(
                    scrollOffsetCorrection: indexToLayoutOffset(
                        deprecatedExtraItemExtent,
                        indexLocal
                    )
                );
                return;
            }
            var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            childParentData.layoutOffset = indexToLayoutOffset(
                deprecatedExtraItemExtent,
                indexLocal
            );
            DartRuntimePrimitives.Assert(() => childParentData.index == indexLocal);
            trailingChildWithLayout ??= child;
        }
        if (trailingChildWithLayout is null)
        {
            firstChild!.layout(_getChildConstraints(indexOf(firstChild!)));
            var childParentDataLocal = ((SliverMultiBoxAdaptorParentData?)firstChild!.parentData!)!;
            childParentDataLocal.layoutOffset = indexToLayoutOffset(
                deprecatedExtraItemExtent,
                ((firstIndexLocal))
            );
            trailingChildWithLayout = firstChild;
        }
        double estimatedMaxScrollOffset = double.PositiveInfinity;
        for (
            long indexAlternate = indexOf(trailingChildWithLayout!) + 1L;
            (targetLastIndex is null)
                || (
                    indexAlternate
                    <= (
                        targetLastIndex
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                );
            ++indexAlternate
        )
        {
            RenderBox? childLocal = childAfter(trailingChildWithLayout!);
            if ((childLocal is null) || (indexOf(childLocal) != indexAlternate))
            {
                childLocal = insertAndLayoutChild(
                    _getChildConstraints(indexAlternate),
                    after: trailingChildWithLayout
                );
                if (childLocal is null)
                {
                    estimatedMaxScrollOffset = indexToLayoutOffset(
                        deprecatedExtraItemExtent,
                        indexAlternate
                    );
                    break;
                }
            }
            else
            {
                childLocal.layout(_getChildConstraints(indexAlternate));
            }
            trailingChildWithLayout = childLocal;
            var childParentDataAlternate = (
                (SliverMultiBoxAdaptorParentData?)childLocal.parentData!
            )!;
            DartRuntimePrimitives.Assert(() => childParentDataAlternate.index == indexAlternate);
            childParentDataAlternate.layoutOffset = indexToLayoutOffset(
                deprecatedExtraItemExtent,
                (
                    childParentDataAlternate.index
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        }
        long lastIndexLocal = indexOf(lastChild!);
        double leadingScrollOffsetLocal = indexToLayoutOffset(
            deprecatedExtraItemExtent,
            ((firstIndexLocal))
        );
        double trailingScrollOffsetLocal = indexToLayoutOffset(
            deprecatedExtraItemExtent,
            (lastIndexLocal) + 1L
        );
        DartRuntimePrimitives.Assert(() =>
            ((firstIndexLocal) == 0L)
            || (
                (
                    (
                        childScrollOffset(firstChild!)
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ) - scrollOffsetLocal
                ) <= Foundation.ConstantsLibrary.precisionErrorTolerance
            )
        );
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => indexOf(firstChild!) == (firstIndexLocal));
        DartRuntimePrimitives.Assert(() =>
            (targetLastIndex is null)
            || (
                lastIndexLocal
                <= (
                    targetLastIndex
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            )
        );
        estimatedMaxScrollOffset = Math.Min(
            estimatedMaxScrollOffset,
            estimateMaxScrollOffset(
                constraintsLocal,
                firstIndex: (firstIndexLocal),
                lastIndex: (lastIndexLocal),
                leadingScrollOffset: (leadingScrollOffsetLocal),
                trailingScrollOffset: (trailingScrollOffsetLocal)
            )
        );
        double paintExtentLocal = calculatePaintOffset(
            constraintsLocal,
            from: ((leadingScrollOffsetLocal)),
            to: ((trailingScrollOffsetLocal))
        );
        double cacheExtentLocal = calculateCacheOffset(
            constraintsLocal,
            from: ((leadingScrollOffsetLocal)),
            to: ((trailingScrollOffsetLocal))
        );
        double targetEndScrollOffsetForPaint =
            constraintsLocal.scrollOffset + constraintsLocal.remainingPaintExtent;
        long? targetLastIndexForPaint = double.IsFinite(targetEndScrollOffsetForPaint)
            ? getMaxChildIndexForScrollOffset(
                targetEndScrollOffsetForPaint,
                deprecatedExtraItemExtent
            )
            : null;
        geometry = new SliverGeometry(
            scrollExtent: estimatedMaxScrollOffset,
            paintExtent: paintExtentLocal,
            cacheExtent: cacheExtentLocal,
            maxPaintExtent: estimatedMaxScrollOffset,
            hasVisualOverflow: (
                (targetLastIndexForPaint is not null)
                && (
                    lastIndexLocal
                    >= (
                        targetLastIndexForPaint
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            ) || (constraintsLocal.scrollOffset > 0.0)
        );
        if (estimatedMaxScrollOffset == (trailingScrollOffsetLocal))
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }
}

public class RenderSliverFixedExtentList : RenderSliverFixedExtentBoxAdaptor
{
    internal virtual double _itemExtent { get; set; } = default!;

    public RenderSliverFixedExtentList(RenderSliverBoxChildManager childManager, double itemExtent)
        : base(childManager: childManager)
    {
        _itemExtent = itemExtent;
    }

    public override double? itemExtent
    {
        get => _itemExtent;
        set
        {
            var __value = (
                value
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            if (_itemExtent == __value)
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

    public RenderSliverVariedExtentList(
        RenderSliverBoxChildManager childManager,
        ItemExtentBuilder itemExtentBuilder
    )
        : base(childManager: childManager)
    {
        ArgumentNullException.ThrowIfNull(itemExtentBuilder);
        _itemExtentBuilder = itemExtentBuilder;
    }

    [System.Diagnostics.CodeAnalysis.AllowNull]
    public override ItemExtentBuilder itemExtentBuilder
    {
        get => _itemExtentBuilder;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            var __value = value;
            if (Equals(_itemExtentBuilder, __value))
            {
                return;
            }
            _itemExtentBuilder = __value;
            markNeedsLayout();
        }
    }
    public override double? itemExtent => null;
}
