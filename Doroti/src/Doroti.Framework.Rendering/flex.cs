// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/flex.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class _AxisSize__flex
{
    public global::Doroti.Ui.Size _size { get; }

    private _AxisSize__flex(global::Doroti.Ui.Size _size)
    {
        this._size = _size;
    }

    public static _AxisSize__flex Create_(global::Doroti.Ui.Size _size) => new _AxisSize__flex(_size);

    public static implicit operator global::Doroti.Ui.Size(_AxisSize__flex value) => value._size;
    public static implicit operator _AxisSize__flex(global::Doroti.Ui.Size value) => new _AxisSize__flex(value);

    public static _AxisSize__flex empty = Create_(Size.zero);

    internal _AxisSize__flex(double mainAxisExtent, double crossAxisExtent)
    {
        _size = new global::Doroti.Ui.Size(mainAxisExtent, crossAxisExtent);
    }

    internal static _AxisSize__flex CreateFromSize(Size size, global::Doroti.Framework.Painting.Axis direction)
    {
        return Create_(_convert(size, direction));
    }

    internal static global::Doroti.Ui.Size _convert(Size size, global::Doroti.Framework.Painting.Axis direction)
    {
        return direction switch { Axis.horizontal => size, Axis.vertical => size.flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double mainAxisExtent => _size.width;
    public virtual double crossAxisExtent => _size.height;
    public virtual global::Doroti.Ui.Size toSize(global::Doroti.Framework.Painting.Axis direction) => _convert(_size, direction);
    public virtual _AxisSize__flex applyConstraints(BoxConstraints constraints, global::Doroti.Framework.Painting.Axis direction)
    {
        BoxConstraints effectiveConstraints = direction switch { Axis.horizontal => constraints, Axis.vertical => constraints.flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return Create_(effectiveConstraints.constrain(_size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _AxisSize__flex op_Add(_AxisSize__flex other) => Create_(new global::Doroti.Ui.Size(_size.width + other._size.width, Math.Max(_size.height, other._size.height)));
}

internal class _AscentDescent__flex
{
    public (double, double)? ascentDescent { get; }

    private _AscentDescent__flex((double, double)? ascentDescent)
    {
        this.ascentDescent = ascentDescent;
    }

    public static _AscentDescent__flex Create_((double, double)? ascentDescent) => new _AscentDescent__flex(ascentDescent);

    public static implicit operator (double, double)?(_AscentDescent__flex value) => value.ascentDescent;
    public static implicit operator _AscentDescent__flex((double, double)? value) => new _AscentDescent__flex(value);

    public static _AscentDescent__flex none = Create_(null);

    internal static _AscentDescent__flex Create(double? baselineOffset, double crossSize)
    {
        return (baselineOffset is null) ? none : Create_((DartRuntimePrimitives.RequireValue(baselineOffset), crossSize - DartRuntimePrimitives.RequireValue(baselineOffset)));
    }

    public virtual double? baselineOffset => ascentDescent?.Item1;
    public virtual _AscentDescent__flex op_Add(_AscentDescent__flex other) => (ascentDescent, other.ascentDescent) switch { (null, var v) => v, (var vLocal, null) => vLocal, ((double xAscent, double xDescent), (double yAscent, double yDescent)) => Create_((Math.Max(xAscent, yAscent), Math.Max(xDescent, yDescent))) };
}

internal delegate double _ChildSizingFunction__flex(RenderBox child, double extent);

internal delegate RenderBox? _NextChild__flex(RenderBox child);

internal class _LayoutSizes__flex
{
    public virtual _AxisSize__flex axisSize { get; private set; } = default!;
    public virtual double mainAxisFreeSpace { get; private set; } = default!;
    public virtual double? baselineOffset { get; private set; }
    public virtual double? spacePerFlex { get; private set; }

    internal _LayoutSizes__flex(_AxisSize__flex axisSize, double? baselineOffset, double mainAxisFreeSpace, double? spacePerFlex)
    {
        this.axisSize = axisSize;
        this.baselineOffset = baselineOffset;
        this.mainAxisFreeSpace = mainAxisFreeSpace;
        this.spacePerFlex = spacePerFlex;
        System.Diagnostics.Debug.Assert((spacePerFlex is { } __finite3047 ? double.IsFinite(__finite3047) : (bool?)null) ?? true);
    }

}

public enum FlexFit
{
    tight,
    loose
}

public class FlexParentData : ContainerBoxParentData<RenderBox>
{
    public virtual long? flex { get; set; } = default;
    public virtual FlexFit? fit { get; set; } = default;

    public override string ToString() => $"{base.ToString()}; flex={flex}; fit={fit}";
}

public enum MainAxisSize
{
    min,
    max
}

public enum MainAxisAlignment
{
    start,
    end,
    center,
    spaceBetween,
    spaceAround,
    spaceEvenly
}

public static class MainAxisAlignmentMembers
{
    internal static (double, double) _distributeSpace(this MainAxisAlignment value, double freeSpace, long itemCount, bool flipped, double spacing)
    {
        DartRuntimePrimitives.Assert(() => itemCount >= 0L);
        return value switch { MainAxisAlignment.start => flipped ? (freeSpace, spacing) : (((double, double))(0.0, spacing)), MainAxisAlignment.end => MainAxisAlignment.start._distributeSpace(freeSpace, itemCount, !flipped, spacing), MainAxisAlignment.spaceBetween when itemCount < 2L => MainAxisAlignment.start._distributeSpace(freeSpace, itemCount, flipped, spacing), MainAxisAlignment.spaceAround when itemCount == 0L => MainAxisAlignment.start._distributeSpace(freeSpace, itemCount, flipped, spacing), MainAxisAlignment.center => (freeSpace / 2.0, spacing), MainAxisAlignment.spaceBetween => (0.0, (freeSpace / (itemCount - 1L)) + spacing), MainAxisAlignment.spaceAround => (freeSpace / itemCount / 2L, (freeSpace / itemCount) + spacing), MainAxisAlignment.spaceEvenly => (freeSpace / (itemCount + 1L), (freeSpace / (itemCount + 1L)) + spacing), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum CrossAxisAlignment
{
    start,
    end,
    center,
    stretch,
    baseline
}

public static class CrossAxisAlignmentMembers
{
    internal static double _getChildCrossAxisOffset(this CrossAxisAlignment value, double freeSpace, bool flipped)
    {
        return value switch { CrossAxisAlignment.stretch => 0.0, CrossAxisAlignment.baseline => 0.0, CrossAxisAlignment.start => flipped ? freeSpace : 0.0, CrossAxisAlignment.center => freeSpace / 2L, CrossAxisAlignment.end => CrossAxisAlignment.start._getChildCrossAxisOffset(freeSpace, !flipped), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RenderFlex : RenderBox, ContainerRenderObjectMixin<RenderBox, FlexParentData>, RenderBoxContainerDefaultsMixin<RenderBox, FlexParentData>, DebugOverflowIndicatorMixin
{
    internal virtual global::Doroti.Framework.Painting.Axis _direction { get; set; } = default!;
    internal virtual MainAxisAlignment _mainAxisAlignment { get; set; } = default!;
    internal virtual MainAxisSize _mainAxisSize { get; set; } = default!;
    internal virtual CrossAxisAlignment _crossAxisAlignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.VerticalDirection _verticalDirection { get; set; } = default!;
    internal virtual TextBaseline? _textBaseline { get; set; } = default;
    internal virtual double _overflow { get; set; } = 0;
    internal virtual Clip _clipBehavior { get; set; } = Clip.none;
    internal virtual double _spacing { get; set; } = default!;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;
    public virtual List<global::Doroti.Framework.Painting.TextPainter> _indicatorLabel { get; set; } = new List<global::Doroti.Framework.Painting.TextPainter>(Enumerable.Select(Enumerable.Range(0, checked((int)checked((long)Enum.GetValues<_OverflowSide__debug_overflow_indicator>().ToList().Count))), (i) => new global::Doroti.Framework.Painting.TextPainter(textDirection: TextDirection.ltr)));
    public virtual bool _overflowReportNeeded { get; set; } = true;

    public RenderFlex(List<RenderBox>? children = null, global::Doroti.Framework.Painting.Axis direction = Axis.horizontal, MainAxisSize mainAxisSize = MainAxisSize.max, MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start, CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center, TextDirection? textDirection = null, global::Doroti.Framework.Painting.VerticalDirection verticalDirection = VerticalDirection.down, TextBaseline? textBaseline = null, Clip clipBehavior = Clip.none, double spacing = 0.0)
    {
        _direction = direction;
        _mainAxisAlignment = mainAxisAlignment;
        _mainAxisSize = mainAxisSize;
        _crossAxisAlignment = crossAxisAlignment;
        _textDirection = textDirection;
        _verticalDirection = verticalDirection;
        _textBaseline = textBaseline;
        _clipBehavior = clipBehavior;
        _spacing = spacing;
        System.Diagnostics.Debug.Assert(spacing >= 0.0);
    }

    public virtual global::Doroti.Framework.Painting.Axis direction
    {
        get => _direction;
        set
        {
            var __value = value;
            if (!Equals(_direction, DartRuntimePrimitives.RequireValue(__value)))
            {
                _direction = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual MainAxisAlignment mainAxisAlignment
    {
        get => _mainAxisAlignment;
        set
        {
            var __value = value;
            if (!Equals(_mainAxisAlignment, DartRuntimePrimitives.RequireValue(__value)))
            {
                _mainAxisAlignment = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual MainAxisSize mainAxisSize
    {
        get => _mainAxisSize;
        set
        {
            var __value = value;
            if (!Equals(_mainAxisSize, DartRuntimePrimitives.RequireValue(__value)))
            {
                _mainAxisSize = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual CrossAxisAlignment crossAxisAlignment
    {
        get => _crossAxisAlignment;
        set
        {
            var __value = value;
            if (!Equals(_crossAxisAlignment, DartRuntimePrimitives.RequireValue(__value)))
            {
                _crossAxisAlignment = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Ui.TextDirection? textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (!Equals(_textDirection, __value))
            {
                _textDirection = __value;
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Framework.Painting.VerticalDirection verticalDirection
    {
        get => _verticalDirection;
        set
        {
            var __value = value;
            if (!Equals(_verticalDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                _verticalDirection = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Ui.TextBaseline? textBaseline
    {
        get => _textBaseline;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (!Equals(_crossAxisAlignment, CrossAxisAlignment.baseline)) || (__value is not null));
            if (!Equals(_textBaseline, __value))
            {
                _textBaseline = __value;
                markNeedsLayout();
            }
        }
    }
    internal virtual bool _debugHasNecessaryDirections
    {
        get
        {
            if (debugCheckingIntrinsics)
            {
                return true;
            }
            if ((firstChild is not null) && (!Equals(lastChild, firstChild)))
            {
                switch (direction)
                {
                    case Axis.horizontal:
                        {
                            DartRuntimePrimitives.Assert(() => textDirection is not null);
                            break;
                        }
                    case Axis.vertical:
                        {
                            break;
                        }
                }
            }
            if (Equals(mainAxisAlignment, MainAxisAlignment.start) || Equals(mainAxisAlignment, MainAxisAlignment.end))
            {
                switch (direction)
                {
                    case Axis.horizontal:
                        {
                            DartRuntimePrimitives.Assert(() => textDirection is not null);
                            break;
                        }
                    case Axis.vertical:
                        {
                            break;
                        }
                }
            }
            if (Equals(crossAxisAlignment, CrossAxisAlignment.start) || Equals(crossAxisAlignment, CrossAxisAlignment.end))
            {
                switch (direction)
                {
                    case Axis.horizontal:
                        {
                            break;
                        }
                    case Axis.vertical:
                        {
                            DartRuntimePrimitives.Assert(() => textDirection is not null);
                            break;
                        }
                }
            }
            return true;
        }
    }
    internal virtual bool _hasOverflow => _overflow > Foundation.ConstantsLibrary.precisionErrorTolerance;
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals(DartRuntimePrimitives.RequireValue(__value), _clipBehavior))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual double spacing
    {
        get => _spacing;
        set
        {
            var __value = value;
            if (_spacing == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _spacing = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        if (__child.parentData is not FlexParentData)
        {
            __child.parentData = new FlexParentData();
        }
    }

    internal virtual double _getIntrinsicSize(global::Doroti.Framework.Painting.Axis sizingDirection, double extent, Func<RenderBox, double, double> childSize)
    {
        if (Equals(_direction, sizingDirection))
        {
            var totalFlex = 0.0;
            double inflexibleSpace = spacing * (childCount - 1L);
            var maxFlexFractionSoFar = 0.0;
            for (RenderBox? childLocal = firstChild; childLocal is not null; childLocal = childAfter(childLocal))
            {
                long flex = _getFlex(childLocal);
                totalFlex += flex;
                if (flex > 0L)
                {
                    double flexFraction = childSize(childLocal, extent) / flex;
                    maxFlexFractionSoFar = Math.Max(maxFlexFractionSoFar, flexFraction);
                }
                else
                {
                    inflexibleSpace += childSize(childLocal, extent);
                }
            }
            return (maxFlexFractionSoFar * totalFlex) + inflexibleSpace;
        }
        else
        {
            bool isHorizontal = direction switch { Axis.horizontal => true, Axis.vertical => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            Size layoutChild(RenderBox child, BoxConstraints constraints)
            {
                double mainAxisSizeFromConstraints = isHorizontal ? constraints.maxWidth : constraints.maxHeight;
                DartRuntimePrimitives.Assert(() => ((_getFlex(child) != 0L) && double.IsFinite(extent)) == double.IsFinite(mainAxisSizeFromConstraints));
                double maxMainAxisSize = double.IsFinite(mainAxisSizeFromConstraints) ? mainAxisSizeFromConstraints : (isHorizontal ? child.getMaxIntrinsicWidth(double.PositiveInfinity) : child.getMaxIntrinsicHeight(double.PositiveInfinity));
                return isHorizontal ? new global::Doroti.Ui.Size(maxMainAxisSize, childSize(child, maxMainAxisSize)) : new global::Doroti.Ui.Size(childSize(child, maxMainAxisSize), maxMainAxisSize);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            return _computeSizes(constraints: isHorizontal ? new BoxConstraints(maxWidth: extent) : new BoxConstraints(maxHeight: extent), layoutChild: layoutChild, getBaseline: ChildLayoutHelper.getDryBaseline).axisSize.crossAxisExtent;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return _getIntrinsicSize(sizingDirection: Axis.horizontal, extent: height, childSize: (child, extent) => child.getMinIntrinsicWidth(extent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return _getIntrinsicSize(sizingDirection: Axis.horizontal, extent: height, childSize: (child, extent) => child.getMaxIntrinsicWidth(extent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return _getIntrinsicSize(sizingDirection: Axis.vertical, extent: width, childSize: (child, extent) => child.getMinIntrinsicHeight(extent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return _getIntrinsicSize(sizingDirection: Axis.vertical, extent: width, childSize: (child, extent) => child.getMaxIntrinsicHeight(extent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return _direction switch { Axis.horizontal => defaultComputeDistanceToHighestActualBaseline(baseline), Axis.vertical => defaultComputeDistanceToFirstActualBaseline(baseline), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _getFlex(RenderBox child)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        return childParentData.flex ?? 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlexFit _getFit(RenderBox child)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        return childParentData.fit ?? FlexFit.tight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isBaselineAligned
    {
        get
        {
            return crossAxisAlignment switch { CrossAxisAlignment.baseline => direction switch { Axis.horizontal => true, Axis.vertical => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }, CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.stretch => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    internal virtual double _getCrossSize(Size size)
    {
        return _direction switch { Axis.horizontal => size.height, Axis.vertical => size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getMainSize(Size size)
    {
        return _direction switch { Axis.horizontal => size.width, Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _flipMainAxis => (firstChild is not null) && (direction switch { Axis.horizontal => textDirection switch { null => false, TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }, Axis.vertical => verticalDirection switch { VerticalDirection.down => false, VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual bool _flipCrossAxis => (firstChild is not null) && (direction switch { Axis.vertical => textDirection switch { null => false, TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }, Axis.horizontal => verticalDirection switch { VerticalDirection.down => false, VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal virtual BoxConstraints _constraintsForNonFlexChild(BoxConstraints constraints)
    {
        bool fillCrossAxis = crossAxisAlignment switch { CrossAxisAlignment.stretch => true, CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.baseline => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return _direction switch { Axis.horizontal => fillCrossAxis ? BoxConstraints.CreateTightFor(height: constraints.maxHeight) : new BoxConstraints(maxHeight: constraints.maxHeight), Axis.vertical => fillCrossAxis ? BoxConstraints.CreateTightFor(width: constraints.maxWidth) : new BoxConstraints(maxWidth: constraints.maxWidth), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _constraintsForFlexChild(RenderBox child, BoxConstraints constraints, double maxChildExtent)
    {
        DartRuntimePrimitives.Assert(() => _getFlex(child) > 0.0);
        DartRuntimePrimitives.Assert(() => maxChildExtent >= 0.0);
        double minChildExtent = _getFit(child) switch { FlexFit.tight => maxChildExtent, FlexFit.loose => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        bool fillCrossAxis = crossAxisAlignment switch { CrossAxisAlignment.stretch => true, CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.baseline => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return _direction switch { Axis.horizontal => new BoxConstraints(minWidth: minChildExtent, maxWidth: maxChildExtent, minHeight: fillCrossAxis ? constraints.maxHeight : 0.0, maxHeight: constraints.maxHeight), Axis.vertical => new BoxConstraints(minWidth: fillCrossAxis ? constraints.maxWidth : 0.0, maxWidth: constraints.maxWidth, minHeight: minChildExtent, maxHeight: maxChildExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        _LayoutSizes__flex sizes = _computeSizes(constraints: constraints, layoutChild: ChildLayoutHelper.dryLayoutChild, getBaseline: ChildLayoutHelper.getDryBaseline);
        if (_isBaselineAligned)
        {
            return sizes.baselineOffset;
        }
        return _direction switch { Axis.horizontal => _computeDryDistanceToHighestBaseline(constraints, baseline, sizes), Axis.vertical => _computeDryDistanceToFirstBaseline(constraints, baseline, sizes), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double? _computeDryDistanceToHighestBaseline(BoxConstraints constraints, TextBaseline baseline, _LayoutSizes__flex sizes)
    {
        BoxConstraints nonFlexConstraints = _constraintsForNonFlexChild(constraints);
        BoxConstraints constraintsForChild(RenderBox child)
        {
            double? spacePerFlexLocal = sizes.spacePerFlex;
            long flex = default!;
            return ((spacePerFlexLocal is not null) && ((flex = _getFlex(child)) > 0L)) ? _constraintsForFlexChild(child, constraints, flex * DartRuntimePrimitives.RequireValue(spacePerFlexLocal)) : nonFlexConstraints;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool flipMainAxis = _flipMainAxis;
        bool flipCrossAxis = _flipCrossAxis;
        var (nextChild, topLeftChild) = flipMainAxis ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild));
        double? baselineOffsetLocal = (_isBaselineAligned && (textBaseline is not null)) ? sizes.baselineOffset : null;
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        for (var childLocal = topLeftChild; childLocal is not null; childLocal = nextChild(childLocal))
        {
            BoxConstraints childConstraints = constraintsForChild(childLocal);
            double? childBaseline = childLocal.getDryBaseline(childConstraints, baseline);
            if (childBaseline is not null)
            {
                double childBaseline__38778__value38854 = DartRuntimePrimitives.RequireValue(childBaseline);
                double? childBaselineOffset = (_isBaselineAligned && (textBaseline is not null)) ? childLocal.getDryBaseline(childConstraints, DartRuntimePrimitives.RequireValue(textBaseline)) : null;
                bool baselineAlign = (baselineOffsetLocal is not null) && (childBaselineOffset is not null);
                double childCrossPosition = default!;
                if (baselineAlign)
                {
                    childCrossPosition = DartRuntimePrimitives.RequireValue(baselineOffsetLocal) - DartRuntimePrimitives.RequireValue(childBaselineOffset);
                }
                else
                {
                    if (Equals(crossAxisAlignment, CrossAxisAlignment.baseline) && Equals(direction, Axis.horizontal))
                    {
                        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(childConstraints);
                        childCrossPosition = CrossAxisAlignment.start._getChildCrossAxisOffset(sizes.axisSize.crossAxisExtent - _getCrossSize(childSize), false);
                    }
                    else
                    {
                        global::Doroti.Ui.Size childSizeLocal = childLocal.getDryLayout(childConstraints);
                        childCrossPosition = crossAxisAlignment._getChildCrossAxisOffset(sizes.axisSize.crossAxisExtent - _getCrossSize(childSizeLocal), flipCrossAxis);
                    }
                }
                BaselineOffset candidate = new BaselineOffset(DartRuntimePrimitives.RequireValue(childBaseline__38778__value38854)).op_Add(childCrossPosition);
                minBaseline = minBaseline.minOf(candidate);
            }
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double? _computeDryDistanceToFirstBaseline(BoxConstraints constraints, TextBaseline baseline, _LayoutSizes__flex sizes)
    {
        BoxConstraints nonFlexConstraints = _constraintsForNonFlexChild(constraints);
        BoxConstraints constraintsForChild(RenderBox child)
        {
            double? spacePerFlexLocal = sizes.spacePerFlex;
            long flex = default!;
            return ((spacePerFlexLocal is not null) && ((flex = _getFlex(child)) > 0L)) ? _constraintsForFlexChild(child, constraints, flex * DartRuntimePrimitives.RequireValue(spacePerFlexLocal)) : nonFlexConstraints;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double remainingSpace = Math.Max(0.0, sizes.mainAxisFreeSpace);
        bool flipMainAxis = _flipMainAxis;
        var (leadingSpace, betweenSpace) = mainAxisAlignment._distributeSpace(remainingSpace, childCount, flipMainAxis, spacing);
        var mainPositions = new DartMap<RenderBox, double>();
        var (nextChildPaintOrder, startChild) = flipMainAxis ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild));
        var pos = leadingSpace;
        for (var childLocal = startChild; childLocal is not null; childLocal = nextChildPaintOrder(childLocal))
        {
            mainPositions[DartRuntimePrimitives.RequireReference(childLocal)] = pos;
            BoxConstraints cc = constraintsForChild(childLocal);
            global::Doroti.Ui.Size cs = childLocal.getDryLayout(cc);
            pos += _getMainSize(cs) + betweenSpace;
        }
        for (RenderBox? childAlternate = firstChild; childAlternate is not null; childAlternate = childAfter(childAlternate))
        {
            BoxConstraints ccLocal = constraintsForChild(childAlternate);
            double? childBaseline = childAlternate.getDryBaseline(ccLocal, baseline);
            if (childBaseline is not null)
            {
                double childBaseline__42618__value42680 = DartRuntimePrimitives.RequireValue(childBaseline);
                double? position = mainPositions.GetValueOrDefault(childAlternate);
                return DartRuntimePrimitives.RequireValue(childBaseline__42618__value42680) + (position ?? leadingSpace);
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        FlutterError? constraintsError = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                constraintsError = _debugCheckConstraints(constraints: constraints, reportParentConstraints: false);
                return true;
            });
        if (constraintsError is not null)
        {
            DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(error: constraintsError));
            return Size.zero;
        }
        return _computeSizes(constraints: constraints, layoutChild: ChildLayoutHelper.dryLayoutChild, getBaseline: ChildLayoutHelper.getDryBaseline).axisSize.toSize(direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual FlutterError? _debugCheckConstraints(BoxConstraints constraints, bool reportParentConstraints)
    {
        FlutterError? result = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                double maxMainSize = Equals(_direction, Axis.horizontal) ? constraints.maxWidth : constraints.maxHeight;
                bool canFlex = maxMainSize < double.PositiveInfinity;
                RenderBox? child = firstChild;
                while (child is not null)
                {
                    long flex = _getFlex(child);
                    if (flex > 0L)
                    {
                        var identity = Equals(_direction, Axis.horizontal) ? "row" : "column";
                        var axis = Equals(_direction, Axis.horizontal) ? "horizontal" : "vertical";
                        var dimension = Equals(_direction, Axis.horizontal) ? "width" : "height";
                        DiagnosticsNode error = default!;
                        DiagnosticsNode message = default!;
                        var addendum = new List<DiagnosticsNode>();
                        if (!canFlex && (Equals(mainAxisSize, MainAxisSize.max) || Equals(_getFit(child), FlexFit.tight)))
                        {
                            error = new ErrorSummary($"RenderFlex children have non-zero flex__44015 but incoming {dimension} constraints are unbounded.");
                            message = new ErrorDescription($"When a {identity} is in a parent that does not provide a finite {dimension} constraint, for example " + $"if it is in a {axis} scrollable, it will try to shrink-wrap its children along the {axis} " + "axis. Setting a flex on a child (e.g. using Expanded) indicates that the child is to " + $"expand to fill the remaining space in the {axis} direction.");
                            if (reportParentConstraints)
                            {
                                RenderBox? node = this;
                                switch (_direction)
                                {
                                    case Axis.horizontal:
                                        {
                                            while (!node!.constraints.hasBoundedWidth && (node.parent is RenderBox))
                                            {
                                                node = ((RenderBox?)(object?)node.parent!)!;
                                            }
                                            if (!node.constraints.hasBoundedWidth)
                                            {
                                                node = null;
                                            }
                                            break;
                                        }
                                    case Axis.vertical:
                                        {
                                            while (!node!.constraints.hasBoundedHeight && (node.parent is RenderBox))
                                            {
                                                node = ((RenderBox?)(object?)node.parent!)!;
                                            }
                                            if (!node.constraints.hasBoundedHeight)
                                            {
                                                node = null;
                                            }
                                            break;
                                        }
                                }
                                if (node is not null)
                                {
                                    addendum.Add(node.describeForError("The nearest ancestor providing an unbounded width constraint is"));
                                }
                            }
                            addendum.Add(new ErrorHint("See also: https://flutter.dev/unbounded-constraints"));
                        }
                        else
                        {
                            return true;
                        }
                        result = new FlutterError(new List<DiagnosticsNode> { error, message, new ErrorDescription("These two directives are mutually exclusive. If a parent is to shrink-wrap its child, the child " + "cannot simultaneously expand to fit its parent."), new ErrorHint("Consider setting mainAxisSize to MainAxisSize.min and using FlexFit.loose fits for the flexible " + "children (using Flexible rather than Expanded). This will allow the flexible children " + "to size themselves to less than the infinite remaining space they would otherwise be " + "forced to take, and then will cause the RenderFlex to shrink-wrap the children " + "rather than expanding to fit the maximum constraints provided by the parent."), new ErrorDescription("If this message did not help you determine the problem, consider using debugDumpRenderTree():\n" + "  https://flutter.dev/to/debug-render-layer\n" + "  https://api.flutter.dev/flutter/rendering/debugDumpRenderTree.html"), describeForError("The affected RenderFlex is", style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<object>("The creator information is set to", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorDescription("If none of the above helps enough to fix this problem, please don't hesitate to file a bug:\n" + "  https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
                        return true;
                    }
                    child = childAfter(child);
                }
                return true;
            });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _LayoutSizes__flex _computeSizes(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline)
    {
        // Kind 8 self time excludes nested layout/build scopes. It remains an
        // upper bound for pure numerics: traversal and baseline callbacks are
        // still owner work and are not declared thread-safe by this timer.
        using var flexProfile = FrameworkWorkProfile.LayoutEnabled
            ? FrameworkWorkProfile.Begin(GetType(), 8) : default;
        DartRuntimePrimitives.Assert(() => _debugHasNecessaryDirections);
        double maxMainSize = _getMainSize(constraints.biggest);
        bool canFlex = double.IsFinite(maxMainSize);
        BoxConstraints nonFlexChildConstraints = _constraintsForNonFlexChild(constraints);
        global::Doroti.Ui.TextBaseline? textBaselineLocal = _isBaselineAligned ? (textBaseline ?? throw new FlutterError("To use CrossAxisAlignment.baseline, you must also specify which baseline to use using the \"textBaseline\" argument.")) : null;
        var totalFlex = 0L;
        RenderBox? firstFlexChild = default!;
        _AscentDescent__flex accumulatedAscentDescent = _AscentDescent__flex.none;
        var accumulatedSize = _AxisSize__flex.Create_(new global::Doroti.Ui.Size(spacing * (childCount - 1L), 0.0));
        for (RenderBox? child = firstChild; child is not null; child = childAfter(child))
        {
            long flex = default!;
            if (canFlex && ((flex = _getFlex(child)) > 0L))
            {
                totalFlex += flex;
                firstFlexChild ??= child;
            }
            else
            {
                var childSize = _AxisSize__flex.CreateFromSize(size: layoutChild(child, nonFlexChildConstraints), direction: direction);
                accumulatedSize = accumulatedSize.op_Add(childSize);
                double? baselineOffsetLocal = (textBaselineLocal is null) ? null : getBaseline(child, nonFlexChildConstraints, DartRuntimePrimitives.RequireValue(textBaselineLocal));
                accumulatedAscentDescent = accumulatedAscentDescent.op_Add(_AscentDescent__flex.Create(baselineOffset: baselineOffsetLocal, crossSize: childSize.crossAxisExtent));
            }
        }
        DartRuntimePrimitives.Assert(() => totalFlex == 0L == firstFlexChild is null);
        DartRuntimePrimitives.Assert(() => (firstFlexChild is null) || canFlex);
        double flexSpace = Math.Max(0.0, maxMainSize - accumulatedSize.mainAxisExtent);
        double spacePerFlexLocal = flexSpace / totalFlex;
        for (var childLocal = firstFlexChild; (childLocal is not null) && (totalFlex > 0L); childLocal = childAfter(childLocal))
        {
            long flexLocal = _getFlex(childLocal);
            if (flexLocal == 0L)
            {
                continue;
            }
            totalFlex -= flexLocal;
            DartRuntimePrimitives.Assert(() => double.IsFinite(spacePerFlexLocal));
            double maxChildExtent = spacePerFlexLocal * flexLocal;
            DartRuntimePrimitives.Assert(() => Equals(_getFit(childLocal), FlexFit.loose) || (maxChildExtent < double.PositiveInfinity));
            BoxConstraints childConstraints = _constraintsForFlexChild(childLocal, constraints, maxChildExtent);
            var childSizeLocal = _AxisSize__flex.CreateFromSize(size: layoutChild(childLocal, childConstraints), direction: direction);
            accumulatedSize = accumulatedSize.op_Add(childSizeLocal);
            double? baselineOffsetAlternate = (textBaselineLocal is null) ? null : getBaseline(childLocal, childConstraints, DartRuntimePrimitives.RequireValue(textBaselineLocal));
            accumulatedAscentDescent = accumulatedAscentDescent.op_Add(_AscentDescent__flex.Create(baselineOffset: baselineOffsetAlternate, crossSize: childSizeLocal.crossAxisExtent));
        }
        DartRuntimePrimitives.Assert(() => totalFlex == 0L);
        accumulatedSize = accumulatedSize.op_Add(accumulatedAscentDescent.ascentDescent switch { null => _AxisSize__flex.empty, (double ascent, double descent) => new _AxisSize__flex(mainAxisExtent: 0, crossAxisExtent: ascent + descent) });
        double idealMainSize = mainAxisSize switch { MainAxisSize.max when double.IsFinite(maxMainSize) => maxMainSize, MainAxisSize.max => accumulatedSize.mainAxisExtent, MainAxisSize.min => accumulatedSize.mainAxisExtent, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        _AxisSize__flex constrainedSize = new _AxisSize__flex(mainAxisExtent: idealMainSize, crossAxisExtent: accumulatedSize.crossAxisExtent).applyConstraints(constraints, direction);
        return new _LayoutSizes__flex(axisSize: constrainedSize, mainAxisFreeSpace: constrainedSize.mainAxisExtent - accumulatedSize.mainAxisExtent, baselineOffset: accumulatedAscentDescent.baselineOffset, spacePerFlex: (firstFlexChild is null) ? null : spacePerFlexLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        DartRuntimePrimitives.Assert(() =>
            {
                FlutterError? constraintsError = _debugCheckConstraints(constraints: constraintsLocal, reportParentConstraints: true);
                if (constraintsError is not null)
                {
                    throw constraintsError;
                }
                return true;
            });
        _LayoutSizes__flex sizes = _computeSizes(constraints: constraintsLocal, layoutChild: ChildLayoutHelper.layoutChild, getBaseline: ChildLayoutHelper.getBaseline);
        double crossAxisExtentLocal = sizes.axisSize.crossAxisExtent;
        size = sizes.axisSize.toSize(direction);
        _overflow = Math.Max(0.0, -sizes.mainAxisFreeSpace);
        double remainingSpace = Math.Max(0.0, sizes.mainAxisFreeSpace);
        bool flipMainAxis = _flipMainAxis;
        bool flipCrossAxis = _flipCrossAxis;
        var (leadingSpace, betweenSpace) = mainAxisAlignment._distributeSpace(remainingSpace, childCount, flipMainAxis, spacing);
        var (nextChild, topLeftChild) = flipMainAxis ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild));
        double? baselineOffsetLocal = sizes.baselineOffset;
        DartRuntimePrimitives.Assert(() => (baselineOffsetLocal is null) || Equals(crossAxisAlignment, CrossAxisAlignment.baseline) && Equals(direction, Axis.horizontal));
        var childMainPosition = leadingSpace;
        for (var child = topLeftChild; child is not null; child = nextChild(child))
        {
            double? childBaselineOffset = default!;
            bool baselineAlign = (baselineOffsetLocal is not null) && ((childBaselineOffset = child.getDistanceToBaseline(DartRuntimePrimitives.RequireValue(textBaseline), onlyReal: true)) is not null);
            double childCrossPosition = default!;
            if (baselineAlign)
            {
                childCrossPosition = DartRuntimePrimitives.RequireValue(baselineOffsetLocal) - DartRuntimePrimitives.RequireValue(childBaselineOffset);
            }
            else
            {
                if (Equals(crossAxisAlignment, CrossAxisAlignment.baseline) && Equals(direction, Axis.horizontal))
                {
                    childCrossPosition = CrossAxisAlignment.start._getChildCrossAxisOffset(crossAxisExtentLocal - _getCrossSize(child.size), false);
                }
                else
                {
                    childCrossPosition = crossAxisAlignment._getChildCrossAxisOffset(crossAxisExtentLocal - _getCrossSize(child.size), flipCrossAxis);
                }
            }
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            childParentData.offset = direction switch { Axis.horizontal => new global::Doroti.Ui.Offset(childMainPosition, childCrossPosition), Axis.vertical => new global::Doroti.Ui.Offset(childCrossPosition, childMainPosition), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            childMainPosition += _getMainSize(child.size) + betweenSpace;
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return defaultHitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (!_hasOverflow)
        {
            defaultPaint(context, offset);
            return;
        }
        if (size.isEmpty)
        {
            return;
        }
        _clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, Offset.zero & size, defaultPaint, clipBehavior: clipBehavior, oldLayer: _clipRectLayer.layer);
        DartRuntimePrimitives.Assert(() =>
            {
                var debugOverflowHints = new List<DiagnosticsNode> { new ErrorDescription($"The overflowing {GetType()} has an orientation of {_direction}."), new ErrorDescription($"The edge of the {GetType()} that is overflowing has been marked " + "in the rendering with a yellow and black striped pattern. This is " + $"usually caused by the contents being too big for the {GetType()}."), new ErrorHint("Consider applying a flex factor (e.g. using an Expanded widget) to " + $"force the children of the {GetType()} to fit within the available " + "space instead of being sized to their natural size."), new ErrorHint("This is considered an error condition because it indicates that there " + "is content that cannot be seen. If the content is legitimately bigger " + "than the available space, consider clipping it with a ClipRect widget " + "before putting it in the flex, or using a scrollable container rather " + "than a Flex, like a ListView.") };
                global::Doroti.Ui.Rect overflowChildRect = _direction switch { Axis.horizontal => Rect.fromLTWH(0.0, 0.0, size.width + _overflow, 0.0), Axis.vertical => Rect.fromLTWH(0.0, 0.0, 0.0, size.height + _overflow), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                paintOverflowIndicator(context, offset, Offset.zero & size, overflowChildRect, overflowHints: debugOverflowHints);
                return true;
            });
    }

    public override void dispose()
    {
        _clipRectLayer.layer = null;
        foreach (global::Doroti.Framework.Painting.TextPainter painter in _indicatorLabel)
        {
            painter.dispose();
        }
        base.dispose();
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        switch (clipBehavior)
        {
            case Clip.none:
                {
                    return null;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    return _hasOverflow ? (Offset.zero & size) : null;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort()
    {
        string header = base.toStringShort();
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (_hasOverflow)
            {
                header += " OVERFLOWING";
            }
        }
        return header;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.Axis>("direction", direction));
        properties.add(new EnumProperty<MainAxisAlignment>("mainAxisAlignment", mainAxisAlignment));
        properties.add(new EnumProperty<MainAxisSize>("mainAxisSize", mainAxisSize));
        properties.add(new EnumProperty<CrossAxisAlignment>("crossAxisAlignment", crossAxisAlignment));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.VerticalDirection>("verticalDirection", verticalDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.TextBaseline>("textBaseline", textBaseline, defaultValue: null));
        properties.add(new DoubleProperty("spacing", spacing, defaultValue: null));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((FlexParentData?)(object?)_firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: _firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: _lastChild));
            var afterParentData = ((FlexParentData?)(object?)after.parentData!)!;
            if (afterParentData.nextSibling is null)
            {
                DartRuntimePrimitives.Assert(() => Equals(after, _lastChild));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                _lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((FlexParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((FlexParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is FlexParentData);
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach(add);
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: _firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((FlexParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((FlexParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderBox child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if (result is not null)
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy;
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            BaselineOffset candidate = new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy);
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
                return child!.hitTest(result, position: transformed);
            });
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string _formatPixels(double value)
    {
        DartRuntimePrimitives.Assert(() => value > 0.0);
        return value switch { > 10.0 => value.toStringAsFixed(0L), > 1.0 => value.toStringAsFixed(1L), _ => value.toStringAsPrecision(3L) };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_OverflowRegionData__debug_overflow_indicator> _calculateOverflowRegions(RelativeRect overflow, Rect containerRect)
    {
        var regions = new List<_OverflowRegionData__debug_overflow_indicator>();
        if (overflow.left > 0.0)
        {
            var markerRect = Rect.fromLTWH(0.0, 0.0, containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction, containerRect.height);
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect, label: $"LEFT OVERFLOWED BY {_formatPixels(overflow.left)} PIXELS", labelOffset: markerRect.centerLeft + new global::Doroti.Ui.Offset(DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels, 0.0), rotation: Dart_mathLibrary.pi / 2.0, side: _OverflowSide__debug_overflow_indicator.left));
        }
        if (overflow.right > 0.0)
        {
            var markerRectLocal = Rect.fromLTWH(containerRect.width * (1.0 - DebugOverflowIndicatorMixin._indicatorFraction), 0.0, containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction, containerRect.height);
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectLocal, label: $"RIGHT OVERFLOWED BY {_formatPixels(overflow.right)} PIXELS", labelOffset: markerRectLocal.centerRight - new global::Doroti.Ui.Offset(DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels, 0.0), rotation: -Dart_mathLibrary.pi / 2.0, side: _OverflowSide__debug_overflow_indicator.right));
        }
        if (overflow.top > 0.0)
        {
            var markerRectAlternate = Rect.fromLTWH(0.0, 0.0, containerRect.width, containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction);
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectAlternate, label: $"TOP OVERFLOWED BY {_formatPixels(overflow.top)} PIXELS", labelOffset: markerRectAlternate.topCenter + new global::Doroti.Ui.Offset(0.0, DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), side: _OverflowSide__debug_overflow_indicator.top));
        }
        if (overflow.bottom > 0.0)
        {
            var markerRectNested = Rect.fromLTWH(0.0, containerRect.height * (1.0 - DebugOverflowIndicatorMixin._indicatorFraction), containerRect.width, containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction);
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectNested, label: $"BOTTOM OVERFLOWED BY {_formatPixels(overflow.bottom)} PIXELS", labelOffset: markerRectNested.bottomCenter - new global::Doroti.Ui.Offset(0.0, DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), side: _OverflowSide__debug_overflow_indicator.bottom));
        }
        return regions;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reportOverflow(RelativeRect overflow, List<DiagnosticsNode>? overflowHints)
    {
        overflowHints ??= new List<DiagnosticsNode>();
        if (checked((long)overflowHints.Count) == 0)
        {
            overflowHints.Add(new ErrorDescription($"The edge of the {GetType()} that is " + "overflowing has been marked in the rendering with a yellow and black " + "striped pattern. This is usually caused by the contents being too big " + $"for the {GetType()}."));
            overflowHints.Add(new ErrorHint("This is considered an error condition because it indicates that there " + "is content that cannot be seen. If the content is legitimately bigger " + "than the available space, consider clipping it with a ClipRect widget " + $"before putting it in the {GetType()}, or using a scrollable " + "container, like a ListView."));
        }
        var overflows = new List<string>();
        var overflowText = "";
        DartRuntimePrimitives.Assert(() => checked((long)overflows.Count) != 0);
        switch (checked((long)overflows.Count))
        {
            case 1L:
                {
                    overflowText = overflows.First();
                    break;
                }
            case 2L:
                {
                    overflowText = $"{overflows.First()} and {overflows.Last()}";
                    break;
                }
            default:
                {
                    overflows[(int)(checked(overflows.Count) - 1L)] = $"and {overflows[(int)(checked(overflows.Count) - 1L)]}";
                    overflowText = string.Join(", ", overflows);
                    break;
                }
        }
        FlutterError.reportError(new FlutterErrorDetails(exception: new FlutterError($"A {GetType()} overflowed by {overflowText}."), library: "rendering library", context: new ErrorDescription("during layout"), informationCollector: () => new List<DiagnosticsNode> { describeForError($"The specific {GetType()} in question is"), new DiagnosticsNode(DartCoreExtensions.repeat("◢◤", checked(FlutterError.wrapWidth / 2L)), allowWrap: false) }));
    }

    public virtual void paintOverflowIndicator(PaintingContext context, Offset offset, Rect containerRect, Rect childRect, List<DiagnosticsNode>? overflowHints = null)
    {
        var overflow = RelativeRect.CreateFromRect(containerRect, childRect);
        if ((overflow.left <= 0.0) && (overflow.right <= 0.0) && (overflow.top <= 0.0) && (overflow.bottom <= 0.0))
        {
            return;
        }
        List<_OverflowRegionData__debug_overflow_indicator> overflowRegions = _calculateOverflowRegions(overflow, containerRect);
        foreach (var region in overflowRegions)
        {
            context.canvas.drawRect(region.rect.shift(offset), DebugOverflowIndicatorMixin._indicatorPaint);
            var textSpan = ((global::Doroti.Framework.Painting.TextSpan?)(object?)_indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].text)!;
            if (textSpan?.text != region.label)
            {
                _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].text = new global::Doroti.Framework.Painting.TextSpan(text: region.label, style: DebugOverflowIndicatorMixin._indicatorTextStyle);
                _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].layout();
            }
            global::Doroti.Ui.Offset labelOffsetLocal = region.labelOffset + offset;
            var centerOffset = new global::Doroti.Ui.Offset(-_indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].width / 2.0, 0.0);
            global::Doroti.Ui.Rect textBackgroundRect = centerOffset & _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].size;
            context.canvas.save();
            context.canvas.translate(labelOffsetLocal.dx, labelOffsetLocal.dy);
            context.canvas.rotate(region.rotation);
            context.canvas.drawRect(textBackgroundRect, DebugOverflowIndicatorMixin._labelBackgroundPaint);
            _indicatorLabel[(int)FoundationRuntimePorts.EnumIndex(region.side)].paint(context.canvas, centerOffset);
            context.canvas.restore();
        }
        if (_overflowReportNeeded)
        {
            _overflowReportNeeded = false;
            _reportOverflow(overflow, overflowHints);
        }
    }

    public override void reassemble()
    {
        base.reassemble();
        DartRuntimePrimitives.Assert(() =>
            {
                _overflowReportNeeded = true;
                return true;
            });
    }

}
