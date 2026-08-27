// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/flex.dart
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

    public static _AxisSize__flex empty = _AxisSize__flex.Create_(Size.zero);

    internal _AxisSize__flex(double mainAxisExtent, double crossAxisExtent)
    {
        this._size = new global::Doroti.Ui.Size(mainAxisExtent, crossAxisExtent);
    }

    internal static _AxisSize__flex CreateFromSize(Size size, global::Doroti.Framework.Painting.Axis direction)
    {
        return _AxisSize__flex.Create_(_convert(size, direction));
    }

    internal static global::Doroti.Ui.Size _convert(Size size, global::Doroti.Framework.Painting.Axis direction)
    {
        return (direction switch { global::Doroti.Framework.Painting.Axis.horizontal => size, global::Doroti.Framework.Painting.Axis.vertical => size.flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double mainAxisExtent => _size.width;
    public virtual double crossAxisExtent => _size.height;
    public virtual global::Doroti.Ui.Size toSize(global::Doroti.Framework.Painting.Axis direction) => _convert(_size, direction);
    public virtual _AxisSize__flex applyConstraints(BoxConstraints constraints, global::Doroti.Framework.Painting.Axis direction)
    {
        BoxConstraints effectiveConstraints = (direction switch { global::Doroti.Framework.Painting.Axis.horizontal => constraints, global::Doroti.Framework.Painting.Axis.vertical => ((BoxConstraints)constraints).flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return _AxisSize__flex.Create_(effectiveConstraints.constrain(_size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _AxisSize__flex op_Add(_AxisSize__flex other) => _AxisSize__flex.Create_(new global::Doroti.Ui.Size((_size.width + other._size.width), Math.Max(_size.height, other._size.height)));
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

    public static _AscentDescent__flex none = _AscentDescent__flex.Create_(null);

    internal static _AscentDescent__flex Create(double? baselineOffset, double crossSize)
    {
        return ((baselineOffset is null) ? none : _AscentDescent__flex.Create_((DartRuntimePrimitives.RequireValue(baselineOffset), (crossSize - DartRuntimePrimitives.RequireValue(baselineOffset)))));
    }

    public virtual double? baselineOffset => ascentDescent?.Item1;
    public virtual _AscentDescent__flex op_Add(_AscentDescent__flex other) => (((this).ascentDescent, (other).ascentDescent) switch { (null, var v) => v, (var vLocal, null) => vLocal, ((double xAscent, double xDescent), (double yAscent, double yDescent)) => _AscentDescent__flex.Create_((Math.Max(xAscent, yAscent), Math.Max(xDescent, yDescent))) });
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
        System.Diagnostics.Debug.Assert(((spacePerFlex is { } __finite3047 ? double.IsFinite(__finite3047) : (bool?)null) ?? true));
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

    public override string ToString() => $"{base.ToString()}; flex={this.flex}; fit={this.fit}";
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
        DartRuntimePrimitives.Assert(() => (itemCount >= 0L));
        return (value switch { MainAxisAlignment.start => (((double, double))((flipped ? (((double, double))(freeSpace, spacing)) : (((double, double))(0.0, spacing))))), MainAxisAlignment.end => (((double, double))(MainAxisAlignment.start._distributeSpace(freeSpace, itemCount, !flipped, spacing))), MainAxisAlignment.spaceBetween when (itemCount < 2L) => (((double, double))(MainAxisAlignment.start._distributeSpace(freeSpace, itemCount, flipped, spacing))), MainAxisAlignment.spaceAround when (itemCount == 0L) => (((double, double))(MainAxisAlignment.start._distributeSpace(freeSpace, itemCount, flipped, spacing))), MainAxisAlignment.center => (((double, double))(((freeSpace / 2.0), spacing))), MainAxisAlignment.spaceBetween => (((double, double))((0.0, ((freeSpace / ((itemCount - 1L))) + spacing)))), MainAxisAlignment.spaceAround => (((double, double))((((freeSpace / itemCount) / 2L), ((freeSpace / itemCount) + spacing)))), MainAxisAlignment.spaceEvenly => (((double, double))(((freeSpace / ((itemCount + 1L))), ((freeSpace / ((itemCount + 1L))) + spacing)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
        return (value switch { CrossAxisAlignment.stretch => 0.0, CrossAxisAlignment.baseline => 0.0, CrossAxisAlignment.start => (flipped ? freeSpace : 0.0), CrossAxisAlignment.center => (freeSpace / 2L), CrossAxisAlignment.end => CrossAxisAlignment.start._getChildCrossAxisOffset(freeSpace, !flipped), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
    public virtual List<global::Doroti.Framework.Painting.TextPainter> _indicatorLabel { get; set; } = new List<global::Doroti.Framework.Painting.TextPainter>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(System.Enum.GetValues<_OverflowSide__debug_overflow_indicator>().ToList().Count)))), ((i) => new global::Doroti.Framework.Painting.TextPainter(textDirection: TextDirection.ltr))));
    public virtual bool _overflowReportNeeded { get; set; } = true;

    public RenderFlex(List<RenderBox>? children = null, global::Doroti.Framework.Painting.Axis direction = Axis.horizontal, MainAxisSize mainAxisSize = MainAxisSize.max, MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start, CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center, TextDirection? textDirection = null, global::Doroti.Framework.Painting.VerticalDirection verticalDirection = VerticalDirection.down, TextBaseline? textBaseline = null, Clip clipBehavior = Clip.none, double spacing = 0.0)
    {
        this._direction = direction;
        this._mainAxisAlignment = mainAxisAlignment;
        this._mainAxisSize = mainAxisSize;
        this._crossAxisAlignment = crossAxisAlignment;
        this._textDirection = textDirection;
        this._verticalDirection = verticalDirection;
        this._textBaseline = textBaseline;
        this._clipBehavior = clipBehavior;
        this._spacing = spacing;
        System.Diagnostics.Debug.Assert((spacing >= 0.0));
    }

    public virtual global::Doroti.Framework.Painting.Axis direction
    {
        get => this._direction;
        set
        {
            var __value = value;
            if ((!object.Equals(this._direction, DartRuntimePrimitives.RequireValue(__value))))
            {
                _direction = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual MainAxisAlignment mainAxisAlignment
    {
        get => this._mainAxisAlignment;
        set
        {
            var __value = value;
            if ((!object.Equals(this._mainAxisAlignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                _mainAxisAlignment = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual MainAxisSize mainAxisSize
    {
        get => this._mainAxisSize;
        set
        {
            var __value = value;
            if ((!object.Equals(this._mainAxisSize, DartRuntimePrimitives.RequireValue(__value))))
            {
                _mainAxisSize = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual CrossAxisAlignment crossAxisAlignment
    {
        get => this._crossAxisAlignment;
        set
        {
            var __value = value;
            if ((!object.Equals(this._crossAxisAlignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                _crossAxisAlignment = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Ui.TextDirection? textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((!object.Equals(this._textDirection, __value)))
            {
                _textDirection = __value;
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Framework.Painting.VerticalDirection verticalDirection
    {
        get => this._verticalDirection;
        set
        {
            var __value = value;
            if ((!object.Equals(this._verticalDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                _verticalDirection = DartRuntimePrimitives.RequireValue(__value);
                markNeedsLayout();
            }
        }
    }
    public virtual global::Doroti.Ui.TextBaseline? textBaseline
    {
        get => this._textBaseline;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => ((!object.Equals(this._crossAxisAlignment, CrossAxisAlignment.baseline)) || (__value is not null)));
            if ((!object.Equals(this._textBaseline, __value)))
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
            if (RenderObject.debugCheckingIntrinsics)
            {
                return true;
            }
            if (((firstChild is not null) && (!object.Equals(lastChild, firstChild))))
            {
                switch (this.direction)
                {
                    case global::Doroti.Framework.Painting.Axis.horizontal:
                        {
                            DartRuntimePrimitives.Assert(() => (this.textDirection is not null));
                            break;
                        }
                    case global::Doroti.Framework.Painting.Axis.vertical:
                        {
                            break;
                        }
                }
            }
            if (((object.Equals(this.mainAxisAlignment, MainAxisAlignment.start)) || (object.Equals(this.mainAxisAlignment, MainAxisAlignment.end))))
            {
                switch (this.direction)
                {
                    case global::Doroti.Framework.Painting.Axis.horizontal:
                        {
                            DartRuntimePrimitives.Assert(() => (this.textDirection is not null));
                            break;
                        }
                    case global::Doroti.Framework.Painting.Axis.vertical:
                        {
                            break;
                        }
                }
            }
            if (((object.Equals(this.crossAxisAlignment, CrossAxisAlignment.start)) || (object.Equals(this.crossAxisAlignment, CrossAxisAlignment.end))))
            {
                switch (this.direction)
                {
                    case global::Doroti.Framework.Painting.Axis.horizontal:
                        {
                            break;
                        }
                    case global::Doroti.Framework.Painting.Axis.vertical:
                        {
                            DartRuntimePrimitives.Assert(() => (this.textDirection is not null));
                            break;
                        }
                }
            }
            return true;
            return default!;
        }
    }
    internal virtual bool _hasOverflow => (this._overflow > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance);
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(DartRuntimePrimitives.RequireValue(__value), this._clipBehavior)))
            {
                _clipBehavior = DartRuntimePrimitives.RequireValue(__value);
                markNeedsPaint();
                markNeedsSemanticsUpdate();
            }
        }
    }
    public virtual double spacing
    {
        get => this._spacing;
        set
        {
            var __value = value;
            if ((this._spacing == DartRuntimePrimitives.RequireValue(__value)))
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
        if ((__child.parentData is not FlexParentData))
        {
            __child.parentData = new FlexParentData();
        }
    }

    internal virtual double _getIntrinsicSize(global::Doroti.Framework.Painting.Axis sizingDirection, double extent, Func<RenderBox, double, double> childSize)
    {
        if ((object.Equals(this._direction, sizingDirection)))
        {
            var totalFlex = 0.0;
            double inflexibleSpace = (this.spacing * ((childCount - 1L)));
            var maxFlexFractionSoFar = 0.0;
            for (RenderBox? childLocal = firstChild; (childLocal is not null); childLocal = childAfter(childLocal))
            {
                long flex = _getFlex(childLocal);
                totalFlex += flex;
                if ((flex > 0L))
                {
                    double flexFraction = (childSize(childLocal, extent) / flex);
                    maxFlexFractionSoFar = Math.Max(maxFlexFractionSoFar, flexFraction);
                }
                else
                {
                    inflexibleSpace += childSize(childLocal, extent);
                }
            }
            return ((maxFlexFractionSoFar * totalFlex) + inflexibleSpace);
        }
        else
        {
            bool isHorizontal = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => true, global::Doroti.Framework.Painting.Axis.vertical => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            Size layoutChild(RenderBox child, BoxConstraints constraints)
            {
                double mainAxisSizeFromConstraints = (isHorizontal ? ((BoxConstraints)constraints).maxWidth : ((BoxConstraints)constraints).maxHeight);
                DartRuntimePrimitives.Assert(() => ((((_getFlex(child) != 0L) && double.IsFinite(extent))) == double.IsFinite(mainAxisSizeFromConstraints)));
                double maxMainAxisSize = (double.IsFinite(mainAxisSizeFromConstraints) ? mainAxisSizeFromConstraints : ((isHorizontal ? child.getMaxIntrinsicWidth(double.PositiveInfinity) : child.getMaxIntrinsicHeight(double.PositiveInfinity))));
                return (isHorizontal ? new global::Doroti.Ui.Size(maxMainAxisSize, childSize(child, maxMainAxisSize)) : new global::Doroti.Ui.Size(childSize(child, maxMainAxisSize), maxMainAxisSize));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            return _computeSizes(constraints: (isHorizontal ? new BoxConstraints(maxWidth: extent) : new BoxConstraints(maxHeight: extent)), layoutChild: (Func<RenderBox, BoxConstraints, Size>)layoutChild, getBaseline: (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline).axisSize.crossAxisExtent;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return _getIntrinsicSize(sizingDirection: global::Doroti.Framework.Painting.Axis.horizontal, extent: height, childSize: ((Func<RenderBox, double, double>)((child, extent) => child.getMinIntrinsicWidth(extent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return _getIntrinsicSize(sizingDirection: global::Doroti.Framework.Painting.Axis.horizontal, extent: height, childSize: ((Func<RenderBox, double, double>)((child, extent) => child.getMaxIntrinsicWidth(extent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return _getIntrinsicSize(sizingDirection: global::Doroti.Framework.Painting.Axis.vertical, extent: width, childSize: ((Func<RenderBox, double, double>)((child, extent) => child.getMinIntrinsicHeight(extent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return _getIntrinsicSize(sizingDirection: global::Doroti.Framework.Painting.Axis.vertical, extent: width, childSize: ((Func<RenderBox, double, double>)((child, extent) => child.getMaxIntrinsicHeight(extent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return (this._direction switch { global::Doroti.Framework.Painting.Axis.horizontal => defaultComputeDistanceToHighestActualBaseline(baseline), global::Doroti.Framework.Painting.Axis.vertical => defaultComputeDistanceToFirstActualBaseline(baseline), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _getFlex(RenderBox child)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        return (((FlexParentData)childParentData).flex ?? 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlexFit _getFit(RenderBox child)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        return (((FlexParentData)childParentData).fit ?? FlexFit.tight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isBaselineAligned
    {
        get
        {
            return (this.crossAxisAlignment switch { CrossAxisAlignment.baseline => (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => true, global::Doroti.Framework.Painting.Axis.vertical => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.stretch => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    internal virtual double _getCrossSize(Size size)
    {
        return (this._direction switch { global::Doroti.Framework.Painting.Axis.horizontal => size.height, global::Doroti.Framework.Painting.Axis.vertical => size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getMainSize(Size size)
    {
        return (this._direction switch { global::Doroti.Framework.Painting.Axis.horizontal => size.width, global::Doroti.Framework.Painting.Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _flipMainAxis => ((firstChild is not null) && (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (this.textDirection switch { null => false, TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), global::Doroti.Framework.Painting.Axis.vertical => (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => false, global::Doroti.Framework.Painting.VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    internal virtual bool _flipCrossAxis => ((firstChild is not null) && (this.direction switch { global::Doroti.Framework.Painting.Axis.vertical => (this.textDirection switch { null => false, TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), global::Doroti.Framework.Painting.Axis.horizontal => (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => false, global::Doroti.Framework.Painting.VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    internal virtual BoxConstraints _constraintsForNonFlexChild(BoxConstraints constraints)
    {
        bool fillCrossAxis = (this.crossAxisAlignment switch { CrossAxisAlignment.stretch => true, CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.baseline => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (this._direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (fillCrossAxis ? BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraints).maxHeight) : new BoxConstraints(maxHeight: ((BoxConstraints)constraints).maxHeight)), global::Doroti.Framework.Painting.Axis.vertical => (fillCrossAxis ? BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraints).maxWidth) : new BoxConstraints(maxWidth: ((BoxConstraints)constraints).maxWidth)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _constraintsForFlexChild(RenderBox child, BoxConstraints constraints, double maxChildExtent)
    {
        DartRuntimePrimitives.Assert(() => (_getFlex(child) > 0.0));
        DartRuntimePrimitives.Assert(() => (maxChildExtent >= 0.0));
        double minChildExtent = (_getFit(child) switch { FlexFit.tight => maxChildExtent, FlexFit.loose => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        bool fillCrossAxis = (this.crossAxisAlignment switch { CrossAxisAlignment.stretch => true, CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.baseline => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (this._direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new BoxConstraints(minWidth: minChildExtent, maxWidth: maxChildExtent, minHeight: (fillCrossAxis ? ((BoxConstraints)constraints).maxHeight : 0.0), maxHeight: ((BoxConstraints)constraints).maxHeight), global::Doroti.Framework.Painting.Axis.vertical => new BoxConstraints(minWidth: (fillCrossAxis ? ((BoxConstraints)constraints).maxWidth : 0.0), maxWidth: ((BoxConstraints)constraints).maxWidth, minHeight: minChildExtent, maxHeight: maxChildExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        _LayoutSizes__flex sizes = _computeSizes(constraints: constraints, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, getBaseline: (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        if (this._isBaselineAligned)
        {
            return ((_LayoutSizes__flex)sizes).baselineOffset;
        }
        return (this._direction switch { global::Doroti.Framework.Painting.Axis.horizontal => _computeDryDistanceToHighestBaseline(constraints, baseline, sizes), global::Doroti.Framework.Painting.Axis.vertical => _computeDryDistanceToFirstBaseline(constraints, baseline, sizes), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double? _computeDryDistanceToHighestBaseline(BoxConstraints constraints, TextBaseline baseline, _LayoutSizes__flex sizes)
    {
        BoxConstraints nonFlexConstraints = _constraintsForNonFlexChild(constraints);
        BoxConstraints constraintsForChild(RenderBox child)
        {
            double? spacePerFlexLocal = ((_LayoutSizes__flex)sizes).spacePerFlex;
            long flex = default!;
            return (((spacePerFlexLocal is not null) && ((flex = _getFlex(child)) > 0L)) ? _constraintsForFlexChild(child, constraints, (flex * DartRuntimePrimitives.RequireValue(spacePerFlexLocal))) : nonFlexConstraints);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool flipMainAxis = this._flipMainAxis;
        bool flipCrossAxis = this._flipCrossAxis;
        var (nextChild, topLeftChild) = (flipMainAxis ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild)));
        double? baselineOffsetLocal = ((this._isBaselineAligned && (this.textBaseline is not null)) ? ((_LayoutSizes__flex)sizes).baselineOffset : null);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        for (var childLocal = topLeftChild; (childLocal is not null); childLocal = nextChild(childLocal))
        {
            BoxConstraints childConstraints = constraintsForChild(childLocal);
            double? childBaseline = childLocal.getDryBaseline(childConstraints, baseline);
            if ((childBaseline is not null))
            {
                double childBaseline__38778__value38854 = DartRuntimePrimitives.RequireValue(childBaseline);
                double? childBaselineOffset = ((this._isBaselineAligned && (this.textBaseline is not null)) ? childLocal.getDryBaseline(childConstraints, DartRuntimePrimitives.RequireValue(this.textBaseline)) : null);
                bool baselineAlign = ((baselineOffsetLocal is not null) && (childBaselineOffset is not null));
                double childCrossPosition = default!;
                if (baselineAlign)
                {
                    childCrossPosition = (DartRuntimePrimitives.RequireValue(baselineOffsetLocal) - DartRuntimePrimitives.RequireValue(childBaselineOffset));
                }
                else
                {
                    if (((object.Equals(this.crossAxisAlignment, CrossAxisAlignment.baseline)) && (object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal))))
                    {
                        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(childConstraints);
                        childCrossPosition = CrossAxisAlignment.start._getChildCrossAxisOffset((((_LayoutSizes__flex)sizes).axisSize.crossAxisExtent - _getCrossSize(childSize)), false);
                    }
                    else
                    {
                        global::Doroti.Ui.Size childSizeLocal = childLocal.getDryLayout(childConstraints);
                        childCrossPosition = this.crossAxisAlignment._getChildCrossAxisOffset((((_LayoutSizes__flex)sizes).axisSize.crossAxisExtent - _getCrossSize(childSizeLocal)), flipCrossAxis);
                    }
                }
                BaselineOffset candidate = (new BaselineOffset(DartRuntimePrimitives.RequireValue(childBaseline__38778__value38854)).op_Add(childCrossPosition));
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
            double? spacePerFlexLocal = ((_LayoutSizes__flex)sizes).spacePerFlex;
            long flex = default!;
            return (((spacePerFlexLocal is not null) && ((flex = _getFlex(child)) > 0L)) ? _constraintsForFlexChild(child, constraints, (flex * DartRuntimePrimitives.RequireValue(spacePerFlexLocal))) : nonFlexConstraints);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double remainingSpace = Math.Max(0.0, ((_LayoutSizes__flex)sizes).mainAxisFreeSpace);
        bool flipMainAxis = this._flipMainAxis;
        var (leadingSpace, betweenSpace) = this.mainAxisAlignment._distributeSpace(remainingSpace, childCount, flipMainAxis, this.spacing);
        var mainPositions = new DartMap<RenderBox, double>();
        var (nextChildPaintOrder, startChild) = (flipMainAxis ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild)));
        var pos = leadingSpace;
        for (var childLocal = startChild; (childLocal is not null); childLocal = nextChildPaintOrder(childLocal))
        {
            mainPositions[DartRuntimePrimitives.RequireReference(childLocal)] = pos;
            BoxConstraints cc = constraintsForChild(childLocal);
            global::Doroti.Ui.Size cs = childLocal.getDryLayout(cc);
            pos += (_getMainSize(cs) + betweenSpace);
        }
        for (RenderBox? childAlternate = firstChild; (childAlternate is not null); childAlternate = childAfter(childAlternate))
        {
            BoxConstraints ccLocal = constraintsForChild(childAlternate);
            double? childBaseline = childAlternate.getDryBaseline(ccLocal, baseline);
            if ((childBaseline is not null))
            {
                double childBaseline__42618__value42680 = DartRuntimePrimitives.RequireValue(childBaseline);
                double? position = mainPositions.GetValueOrDefault(childAlternate);
                return (DartRuntimePrimitives.RequireValue(childBaseline__42618__value42680) + ((position ?? leadingSpace)));
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
        if ((constraintsError is not null))
        {
            DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(error: constraintsError));
            return Size.zero;
        }
        return _computeSizes(constraints: constraints, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, getBaseline: (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline).axisSize.toSize(this.direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual FlutterError? _debugCheckConstraints(BoxConstraints constraints, bool reportParentConstraints)
    {
        FlutterError? result = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                double maxMainSize = ((object.Equals(this._direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? ((BoxConstraints)constraints).maxWidth : ((BoxConstraints)constraints).maxHeight);
                bool canFlex = (maxMainSize < double.PositiveInfinity);
                RenderBox? child = firstChild;
                while ((child is not null))
                {
                    long flex = _getFlex(child);
                    if ((flex > 0L))
                    {
                        var identity = ((object.Equals(this._direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? "row" : "column");
                        var axis = ((object.Equals(this._direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? "horizontal" : "vertical");
                        var dimension = ((object.Equals(this._direction, global::Doroti.Framework.Painting.Axis.horizontal)) ? "width" : "height");
                        DiagnosticsNode error = default!;
                        DiagnosticsNode message = default!;
                        var addendum = new List<DiagnosticsNode>();
                        if ((!canFlex && (((object.Equals(this.mainAxisSize, MainAxisSize.max)) || (object.Equals(_getFit(child), FlexFit.tight))))))
                        {
                            error = new ErrorSummary($"RenderFlex children have non-zero flex__44015 but incoming {dimension} constraints are unbounded.");
                            message = new ErrorDescription($"When a {identity} is in a parent that does not provide a finite {dimension} constraint, for example " + $"if it is in a {axis} scrollable, it will try to shrink-wrap its children along the {axis} " + "axis. Setting a flex on a child (e.g. using Expanded) indicates that the child is to " + $"expand to fill the remaining space in the {axis} direction.");
                            if (reportParentConstraints)
                            {
                                RenderBox? node = this;
                                switch (this._direction)
                                {
                                    case global::Doroti.Framework.Painting.Axis.horizontal:
                                        {
                                            while ((!node!.constraints.hasBoundedWidth && (node.parent is RenderBox)))
                                            {
                                                node = ((RenderBox?)(object?)node.parent!)!;
                                            }
                                            if (!((RenderBox)node).constraints.hasBoundedWidth)
                                            {
                                                node = null;
                                            }
                                            break;
                                        }
                                    case global::Doroti.Framework.Painting.Axis.vertical:
                                        {
                                            while ((!node!.constraints.hasBoundedHeight && (node.parent is RenderBox)))
                                            {
                                                node = ((RenderBox?)(object?)node.parent!)!;
                                            }
                                            if (!((RenderBox)node).constraints.hasBoundedHeight)
                                            {
                                                node = null;
                                            }
                                            break;
                                        }
                                }
                                if ((node is not null))
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
        DartRuntimePrimitives.Assert(() => this._debugHasNecessaryDirections);
        double maxMainSize = _getMainSize(((BoxConstraints)constraints).biggest);
        bool canFlex = double.IsFinite(maxMainSize);
        BoxConstraints nonFlexChildConstraints = _constraintsForNonFlexChild(constraints);
        global::Doroti.Ui.TextBaseline? textBaselineLocal = (this._isBaselineAligned ? ((this.textBaseline ?? throw new FlutterError("To use CrossAxisAlignment.baseline, you must also specify which baseline to use using the \"textBaseline\" argument."))) : null);
        var totalFlex = 0L;
        RenderBox? firstFlexChild = default!;
        _AscentDescent__flex accumulatedAscentDescent = _AscentDescent__flex.none;
        var accumulatedSize = _AxisSize__flex.Create_(new global::Doroti.Ui.Size((this.spacing * ((childCount - 1L))), 0.0));
        for (RenderBox? child = firstChild; (child is not null); child = childAfter(child))
        {
            long flex = default!;
            if ((canFlex && ((flex = _getFlex(child)) > 0L)))
            {
                totalFlex += flex;
                firstFlexChild ??= child;
            }
            else
            {
                var childSize = _AxisSize__flex.CreateFromSize(size: layoutChild(child, nonFlexChildConstraints), direction: this.direction);
                accumulatedSize = accumulatedSize.op_Add(childSize);
                double? baselineOffsetLocal = ((textBaselineLocal is null) ? null : getBaseline(child, nonFlexChildConstraints, DartRuntimePrimitives.RequireValue(textBaselineLocal)));
                accumulatedAscentDescent = accumulatedAscentDescent.op_Add(_AscentDescent__flex.Create(baselineOffset: baselineOffsetLocal, crossSize: ((_AxisSize__flex)childSize).crossAxisExtent));
            }
        }
        DartRuntimePrimitives.Assert(() => (((totalFlex == 0L)) == ((firstFlexChild is null))));
        DartRuntimePrimitives.Assert(() => ((firstFlexChild is null) || canFlex));
        double flexSpace = Math.Max(0.0, (maxMainSize - ((_AxisSize__flex)accumulatedSize).mainAxisExtent));
        double spacePerFlexLocal = (flexSpace / totalFlex);
        for (var childLocal = firstFlexChild; ((childLocal is not null) && (totalFlex > 0L)); childLocal = childAfter(childLocal))
        {
            long flexLocal = _getFlex(childLocal);
            if ((flexLocal == 0L))
            {
                continue;
            }
            totalFlex -= flexLocal;
            DartRuntimePrimitives.Assert(() => double.IsFinite(spacePerFlexLocal));
            double maxChildExtent = (spacePerFlexLocal * flexLocal);
            DartRuntimePrimitives.Assert(() => ((object.Equals(_getFit(childLocal), FlexFit.loose)) || (maxChildExtent < double.PositiveInfinity)));
            BoxConstraints childConstraints = _constraintsForFlexChild(childLocal, constraints, maxChildExtent);
            var childSizeLocal = _AxisSize__flex.CreateFromSize(size: layoutChild(childLocal, childConstraints), direction: this.direction);
            accumulatedSize = accumulatedSize.op_Add(childSizeLocal);
            double? baselineOffsetAlternate = ((textBaselineLocal is null) ? null : getBaseline(childLocal, childConstraints, DartRuntimePrimitives.RequireValue(textBaselineLocal)));
            accumulatedAscentDescent = accumulatedAscentDescent.op_Add(_AscentDescent__flex.Create(baselineOffset: baselineOffsetAlternate, crossSize: ((_AxisSize__flex)childSizeLocal).crossAxisExtent));
        }
        DartRuntimePrimitives.Assert(() => (totalFlex == 0L));
        accumulatedSize = accumulatedSize.op_Add(((accumulatedAscentDescent).ascentDescent switch { null => _AxisSize__flex.empty, (double ascent, double descent) => new _AxisSize__flex(mainAxisExtent: 0, crossAxisExtent: (ascent + descent)) }));
        double idealMainSize = (this.mainAxisSize switch { MainAxisSize.max when double.IsFinite(maxMainSize) => maxMainSize, MainAxisSize.max => ((_AxisSize__flex)accumulatedSize).mainAxisExtent, MainAxisSize.min => ((_AxisSize__flex)accumulatedSize).mainAxisExtent, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        _AxisSize__flex constrainedSize = new _AxisSize__flex(mainAxisExtent: idealMainSize, crossAxisExtent: ((_AxisSize__flex)accumulatedSize).crossAxisExtent).applyConstraints(constraints, this.direction);
        return new _LayoutSizes__flex(axisSize: constrainedSize, mainAxisFreeSpace: (((_AxisSize__flex)constrainedSize).mainAxisExtent - ((_AxisSize__flex)accumulatedSize).mainAxisExtent), baselineOffset: ((_AscentDescent__flex)accumulatedAscentDescent).baselineOffset, spacePerFlex: ((firstFlexChild is null) ? null : spacePerFlexLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = this.constraints;
        DartRuntimePrimitives.Assert(() =>
            {
                FlutterError? constraintsError = _debugCheckConstraints(constraints: constraintsLocal, reportParentConstraints: true);
                if ((constraintsError is not null))
                {
                    throw constraintsError;
                }
                return true;
            });
        _LayoutSizes__flex sizes = _computeSizes(constraints: constraintsLocal, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, getBaseline: (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getBaseline);
        double crossAxisExtentLocal = ((_LayoutSizes__flex)sizes).axisSize.crossAxisExtent;
        size = ((_LayoutSizes__flex)sizes).axisSize.toSize(this.direction);
        _overflow = Math.Max(0.0, -((_LayoutSizes__flex)sizes).mainAxisFreeSpace);
        double remainingSpace = Math.Max(0.0, ((_LayoutSizes__flex)sizes).mainAxisFreeSpace);
        bool flipMainAxis = this._flipMainAxis;
        bool flipCrossAxis = this._flipCrossAxis;
        var (leadingSpace, betweenSpace) = this.mainAxisAlignment._distributeSpace(remainingSpace, childCount, flipMainAxis, this.spacing);
        var (nextChild, topLeftChild) = (flipMainAxis ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild)));
        double? baselineOffsetLocal = ((_LayoutSizes__flex)sizes).baselineOffset;
        DartRuntimePrimitives.Assert(() => ((baselineOffsetLocal is null) || (((object.Equals(this.crossAxisAlignment, CrossAxisAlignment.baseline)) && (object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal))))));
        var childMainPosition = leadingSpace;
        for (var child = topLeftChild; (child is not null); child = nextChild(child))
        {
            double? childBaselineOffset = default!;
            bool baselineAlign = ((baselineOffsetLocal is not null) && ((childBaselineOffset = child.getDistanceToBaseline(DartRuntimePrimitives.RequireValue(this.textBaseline), onlyReal: true)) is not null));
            double childCrossPosition = default!;
            if (baselineAlign)
            {
                childCrossPosition = (DartRuntimePrimitives.RequireValue(baselineOffsetLocal) - DartRuntimePrimitives.RequireValue(childBaselineOffset));
            }
            else
            {
                if (((object.Equals(this.crossAxisAlignment, CrossAxisAlignment.baseline)) && (object.Equals(this.direction, global::Doroti.Framework.Painting.Axis.horizontal))))
                {
                    childCrossPosition = CrossAxisAlignment.start._getChildCrossAxisOffset((crossAxisExtentLocal - _getCrossSize(((RenderBox)child).size)), false);
                }
                else
                {
                    childCrossPosition = this.crossAxisAlignment._getChildCrossAxisOffset((crossAxisExtentLocal - _getCrossSize(((RenderBox)child).size)), flipCrossAxis);
                }
            }
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            childParentData.offset = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(childMainPosition, childCrossPosition), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(childCrossPosition, childMainPosition), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            childMainPosition += (_getMainSize(((RenderBox)child).size) + betweenSpace);
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return defaultHitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (!this._hasOverflow)
        {
            defaultPaint(context, offset);
            return;
        }
        if (size.isEmpty)
        {
            return;
        }
        this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (Action<PaintingContext, Offset>)defaultPaint, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
        DartRuntimePrimitives.Assert(() =>
            {
                var debugOverflowHints = new List<DiagnosticsNode> { new ErrorDescription($"The overflowing {this.GetType()} has an orientation of {this._direction}."), new ErrorDescription($"The edge of the {this.GetType()} that is overflowing has been marked " + "in the rendering with a yellow and black striped pattern. This is " + $"usually caused by the contents being too big for the {this.GetType()}."), new ErrorHint("Consider applying a flex factor (e.g. using an Expanded widget) to " + $"force the children of the {this.GetType()} to fit within the available " + "space instead of being sized to their natural size."), new ErrorHint("This is considered an error condition because it indicates that there " + "is content that cannot be seen. If the content is legitimately bigger " + "than the available space, consider clipping it with a ClipRect widget " + "before putting it in the flex, or using a scrollable container rather " + "than a Flex, like a ListView.") };
                global::Doroti.Ui.Rect overflowChildRect = (this._direction switch { global::Doroti.Framework.Painting.Axis.horizontal => global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, (size.width + this._overflow), 0.0), global::Doroti.Framework.Painting.Axis.vertical => global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, 0.0, (size.height + this._overflow)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                paintOverflowIndicator(context, offset, (Offset.zero & size), overflowChildRect, overflowHints: debugOverflowHints);
                return true;
            });
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        foreach (global::Doroti.Framework.Painting.TextPainter painter in this._indicatorLabel)
        {
            painter.dispose();
        }
        base.dispose();
    }

    public override Rect? describeApproximatePaintClip(RenderObject child)
    {
        switch (this.clipBehavior)
        {
            case Clip.none:
                {
                    return null;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    return (this._hasOverflow ? (Offset.zero & size) : null);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShort()
    {
        string header = base.toStringShort();
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (this._hasOverflow)
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
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.Axis>("direction", this.direction));
        properties.add(new EnumProperty<MainAxisAlignment>("mainAxisAlignment", this.mainAxisAlignment));
        properties.add(new EnumProperty<MainAxisSize>("mainAxisSize", this.mainAxisSize));
        properties.add(new EnumProperty<CrossAxisAlignment>("crossAxisAlignment", this.crossAxisAlignment));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.VerticalDirection>("verticalDirection", this.verticalDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.TextBaseline>("textBaseline", this.textBaseline, defaultValue: null));
        properties.add(new DoubleProperty("spacing", this.spacing, defaultValue: null));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((FlexParentData?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData.previousSibling = child;
            }
            this._firstChild = child;
            this._lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this._firstChild is not null));
            DartRuntimePrimitives.Assert(() => (this._lastChild is not null));
            DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(after, equals: this._firstChild));
            DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(after, equals: this._lastChild));
            var afterParentData = ((FlexParentData?)(object?)after.parentData!)!;
            if ((afterParentData.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = ((FlexParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((FlexParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is FlexParentData));
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach(this.add);
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = ((FlexParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((FlexParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
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
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child = this.firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if ((object.Equals(child, this.lastChild)))
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
        while ((child is not null))
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if ((result is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy);
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
        while ((child is not null))
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            BaselineOffset candidate = (new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while ((child is not null))
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child!.hitTest(result, position: transformed);
                return default;
            })));
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
        while ((child is not null))
        {
            var childParentData = ((FlexParentData?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while ((child is not null))
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
        DartRuntimePrimitives.Assert(() => (value > 0.0));
        return (value switch { > 10.0 => value.toStringAsFixed(0L), > 1.0 => value.toStringAsFixed(1L), _ => value.toStringAsPrecision(3L) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_OverflowRegionData__debug_overflow_indicator> _calculateOverflowRegions(RelativeRect overflow, Rect containerRect)
    {
        var regions = new List<_OverflowRegionData__debug_overflow_indicator>();
        if ((((RelativeRect)overflow).left > 0.0))
        {
            var markerRect = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, (containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction), containerRect.height);
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect, label: $"LEFT OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).left)} PIXELS", labelOffset: (markerRect.centerLeft + new global::Doroti.Ui.Offset((DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), 0.0)), rotation: (Dart_mathLibrary.pi / 2.0), side: _OverflowSide__debug_overflow_indicator.left));
        }
        if ((((RelativeRect)overflow).right > 0.0))
        {
            var markerRectLocal = global::Doroti.Ui.Rect.fromLTWH((containerRect.width * ((1.0 - DebugOverflowIndicatorMixin._indicatorFraction))), 0.0, (containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction), containerRect.height);
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectLocal, label: $"RIGHT OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).right)} PIXELS", labelOffset: (markerRectLocal.centerRight - new global::Doroti.Ui.Offset((DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), 0.0)), rotation: (-Dart_mathLibrary.pi / 2.0), side: _OverflowSide__debug_overflow_indicator.right));
        }
        if ((((RelativeRect)overflow).top > 0.0))
        {
            var markerRectAlternate = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, containerRect.width, (containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction));
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectAlternate, label: $"TOP OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).top)} PIXELS", labelOffset: (markerRectAlternate.topCenter + new global::Doroti.Ui.Offset(0.0, DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels)), side: _OverflowSide__debug_overflow_indicator.top));
        }
        if ((((RelativeRect)overflow).bottom > 0.0))
        {
            var markerRectNested = global::Doroti.Ui.Rect.fromLTWH(0.0, (containerRect.height * ((1.0 - DebugOverflowIndicatorMixin._indicatorFraction))), containerRect.width, (containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction));
            regions.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRectNested, label: $"BOTTOM OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).bottom)} PIXELS", labelOffset: (markerRectNested.bottomCenter - new global::Doroti.Ui.Offset(0.0, (DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels))), side: _OverflowSide__debug_overflow_indicator.bottom));
        }
        return regions;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reportOverflow(RelativeRect overflow, List<DiagnosticsNode>? overflowHints)
    {
        overflowHints ??= new List<DiagnosticsNode>();
        if ((checked((long)(overflowHints.Count)) == 0))
        {
            overflowHints.Add(new ErrorDescription($"The edge of the {this.GetType()} that is " + "overflowing has been marked in the rendering with a yellow and black " + "striped pattern. This is usually caused by the contents being too big " + $"for the {this.GetType()}."));
            overflowHints.Add(new ErrorHint("This is considered an error condition because it indicates that there " + "is content that cannot be seen. If the content is legitimately bigger " + "than the available space, consider clipping it with a ClipRect widget " + $"before putting it in the {this.GetType()}, or using a scrollable " + "container, like a ListView."));
        }
        var overflows = new List<string>();
        var overflowText = "";
        DartRuntimePrimitives.Assert(() => (checked((long)(overflows.Count)) != 0));
        switch (checked((long)(overflows.Count)))
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
                    overflows[(int)((checked((long)(overflows.Count)) - 1L))] = $"and {overflows[(int)((checked((long)(overflows.Count)) - 1L))]}";
                    overflowText = string.Join(", ", overflows);
                    break;
                }
        }
        FlutterError.reportError(new FlutterErrorDetails(exception: new FlutterError($"A {this.GetType()} overflowed by {overflowText}."), library: "rendering library", context: new ErrorDescription("during layout"), informationCollector: (() => new List<DiagnosticsNode> { describeForError($"The specific {this.GetType()} in question is"), new DiagnosticsNode(DartCoreExtensions.repeat("◢◤", ((checked((long)(FlutterError.wrapWidth / 2L))))), allowWrap: false) })));
    }

    public virtual void paintOverflowIndicator(PaintingContext context, Offset offset, Rect containerRect, Rect childRect, List<DiagnosticsNode>? overflowHints = null)
    {
        var overflow = RelativeRect.CreateFromRect(containerRect, childRect);
        if (((((((RelativeRect)overflow).left <= 0.0) && (((RelativeRect)overflow).right <= 0.0)) && (((RelativeRect)overflow).top <= 0.0)) && (((RelativeRect)overflow).bottom <= 0.0)))
        {
            return;
        }
        List<_OverflowRegionData__debug_overflow_indicator> overflowRegions = _calculateOverflowRegions(overflow, containerRect);
        foreach (var region in overflowRegions)
        {
            ((PaintingContext)context).canvas.drawRect(((_OverflowRegionData__debug_overflow_indicator)region).rect.shift(offset), DebugOverflowIndicatorMixin._indicatorPaint);
            var textSpan = ((global::Doroti.Framework.Painting.TextSpan?)(object?)this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].text)!;
            if ((textSpan?.text != ((_OverflowRegionData__debug_overflow_indicator)region).label))
            {
                this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].text = new global::Doroti.Framework.Painting.TextSpan(text: ((_OverflowRegionData__debug_overflow_indicator)region).label, style: DebugOverflowIndicatorMixin._indicatorTextStyle);
                this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].layout();
            }
            global::Doroti.Ui.Offset labelOffsetLocal = (((_OverflowRegionData__debug_overflow_indicator)region).labelOffset + offset);
            var centerOffset = new global::Doroti.Ui.Offset((-this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].width / 2.0), 0.0);
            global::Doroti.Ui.Rect textBackgroundRect = (centerOffset & this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].size);
            ((PaintingContext)context).canvas.save();
            ((PaintingContext)context).canvas.translate(labelOffsetLocal.dx, labelOffsetLocal.dy);
            ((PaintingContext)context).canvas.rotate(((_OverflowRegionData__debug_overflow_indicator)region).rotation);
            ((PaintingContext)context).canvas.drawRect(textBackgroundRect, DebugOverflowIndicatorMixin._labelBackgroundPaint);
            this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region).side))].paint(((PaintingContext)context).canvas, centerOffset);
            ((PaintingContext)context).canvas.restore();
        }
        if (this._overflowReportNeeded)
        {
            this._overflowReportNeeded = false;
            _reportOverflow(overflow, overflowHints);
        }
    }

    public override void reassemble()
    {
        base.reassemble();
        DartRuntimePrimitives.Assert(() =>
            {
                this._overflowReportNeeded = true;
                return true;
            });
    }

}

