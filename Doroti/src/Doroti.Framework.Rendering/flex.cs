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

namespace Doroti.Generated.Framework.Rendering;

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

    internal static _AxisSize__flex CreateFromSize(Size size, global::Doroti.Generated.Framework.Painting.Axis direction)
    {
        return _AxisSize__flex.Create_(_convert(size, direction));
    }

    internal static global::Doroti.Ui.Size _convert(Size size, global::Doroti.Generated.Framework.Painting.Axis direction)
    {
        return (direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => size, global::Doroti.Generated.Framework.Painting.Axis.vertical => size.flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double mainAxisExtent => _size.width;
    public virtual double crossAxisExtent => _size.height;
    public virtual global::Doroti.Ui.Size toSize(global::Doroti.Generated.Framework.Painting.Axis direction) => _convert(_size, direction);
    public virtual _AxisSize__flex applyConstraints(BoxConstraints constraints, global::Doroti.Generated.Framework.Painting.Axis direction)
    {
        BoxConstraints effectiveConstraints__1383 = (direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => constraints, global::Doroti.Generated.Framework.Painting.Axis.vertical => ((BoxConstraints)constraints).flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return _AxisSize__flex.Create_(effectiveConstraints__1383.constrain(_size));
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
    public virtual _AscentDescent__flex op_Add(_AscentDescent__flex other) => (((this).ascentDescent, (other).ascentDescent) switch { (null, var v__2468) => v__2468, (var v__2496, null) => v__2496, ((double xAscent__2537, double xDescent__2559), (double yAscent__2590, double yDescent__2612)) => _AscentDescent__flex.Create_((Math.Max(xAscent__2537, yAscent__2590), Math.Max(xDescent__2559, yDescent__2612))) });
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
    internal virtual global::Doroti.Generated.Framework.Painting.Axis _direction { get; set; } = default!;
    internal virtual MainAxisAlignment _mainAxisAlignment { get; set; } = default!;
    internal virtual MainAxisSize _mainAxisSize { get; set; } = default!;
    internal virtual CrossAxisAlignment _crossAxisAlignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.VerticalDirection _verticalDirection { get; set; } = default!;
    internal virtual TextBaseline? _textBaseline { get; set; } = default;
    internal virtual double _overflow { get; set; } = 0;
    internal virtual Clip _clipBehavior { get; set; } = Clip.none;
    internal virtual double _spacing { get; set; } = default!;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;
    public virtual List<global::Doroti.Generated.Framework.Painting.TextPainter> _indicatorLabel { get; set; } = new List<global::Doroti.Generated.Framework.Painting.TextPainter>(System.Linq.Enumerable.Select(System.Linq.Enumerable.Range(0, checked((int)checked((long)(System.Enum.GetValues<_OverflowSide__debug_overflow_indicator>().ToList().Count)))), ((i) => new global::Doroti.Generated.Framework.Painting.TextPainter(textDirection: TextDirection.ltr))));
    public virtual bool _overflowReportNeeded { get; set; } = true;

    public RenderFlex(List<RenderBox>? children = null, global::Doroti.Generated.Framework.Painting.Axis direction = Axis.horizontal, MainAxisSize mainAxisSize = MainAxisSize.max, MainAxisAlignment mainAxisAlignment = MainAxisAlignment.start, CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.center, TextDirection? textDirection = null, global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection = VerticalDirection.down, TextBaseline? textBaseline = null, Clip clipBehavior = Clip.none, double spacing = 0.0)
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

    public virtual global::Doroti.Generated.Framework.Painting.Axis direction
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
    public virtual global::Doroti.Generated.Framework.Painting.VerticalDirection verticalDirection
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
                    case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                        {
                            DartRuntimePrimitives.Assert(() => (this.textDirection is not null));
                            break;
                        }
                    case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                        {
                            break;
                        }
                }
            }
            if (((object.Equals(this.mainAxisAlignment, MainAxisAlignment.start)) || (object.Equals(this.mainAxisAlignment, MainAxisAlignment.end))))
            {
                switch (this.direction)
                {
                    case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                        {
                            DartRuntimePrimitives.Assert(() => (this.textDirection is not null));
                            break;
                        }
                    case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                        {
                            break;
                        }
                }
            }
            if (((object.Equals(this.crossAxisAlignment, CrossAxisAlignment.start)) || (object.Equals(this.crossAxisAlignment, CrossAxisAlignment.end))))
            {
                switch (this.direction)
                {
                    case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                        {
                            break;
                        }
                    case global::Doroti.Generated.Framework.Painting.Axis.vertical:
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
    internal virtual bool _hasOverflow => (this._overflow > global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance);
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

    internal virtual double _getIntrinsicSize(global::Doroti.Generated.Framework.Painting.Axis sizingDirection, double extent, Func<RenderBox, double, double> childSize)
    {
        if ((object.Equals(this._direction, sizingDirection)))
        {
            var totalFlex__29343 = 0.0;
            double inflexibleSpace__29373 = (this.spacing * ((childCount - 1L)));
            var maxFlexFractionSoFar__29429 = 0.0;
            for (RenderBox? child__29479 = firstChild; (child__29479 is not null); child__29479 = childAfter(child__29479))
            {
                long flex__29561 = _getFlex(child__29479);
                totalFlex__29343 += flex__29561;
                if ((flex__29561 > 0L))
                {
                    double flexFraction__29659 = (childSize(child__29479, extent) / flex__29561);
                    maxFlexFractionSoFar__29429 = Math.Max(maxFlexFractionSoFar__29429, flexFraction__29659);
                }
                else
                {
                    inflexibleSpace__29373 += childSize(child__29479, extent);
                }
            }
            return ((maxFlexFractionSoFar__29429 * totalFlex__29343) + inflexibleSpace__29373);
        }
        else
        {
            bool isHorizontal__30230 = (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => true, global::Doroti.Generated.Framework.Painting.Axis.vertical => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            Size layoutChild(RenderBox child, BoxConstraints constraints)
            {
                double mainAxisSizeFromConstraints__30432 = (isHorizontal__30230 ? ((BoxConstraints)constraints).maxWidth : ((BoxConstraints)constraints).maxHeight);
                DartRuntimePrimitives.Assert(() => ((((_getFlex(child) != 0L) && double.IsFinite(extent))) == double.IsFinite(mainAxisSizeFromConstraints__30432)));
                double maxMainAxisSize__30778 = (double.IsFinite(mainAxisSizeFromConstraints__30432) ? mainAxisSizeFromConstraints__30432 : ((isHorizontal__30230 ? child.getMaxIntrinsicWidth(double.PositiveInfinity) : child.getMaxIntrinsicHeight(double.PositiveInfinity))));
                return (isHorizontal__30230 ? new global::Doroti.Ui.Size(maxMainAxisSize__30778, childSize(child, maxMainAxisSize__30778)) : new global::Doroti.Ui.Size(childSize(child, maxMainAxisSize__30778), maxMainAxisSize__30778));
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            return _computeSizes(constraints: (isHorizontal__30230 ? new BoxConstraints(maxWidth: extent) : new BoxConstraints(maxHeight: extent)), layoutChild: (Func<RenderBox, BoxConstraints, Size>)layoutChild, getBaseline: (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline).axisSize.crossAxisExtent;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return _getIntrinsicSize(sizingDirection: global::Doroti.Generated.Framework.Painting.Axis.horizontal, extent: height, childSize: ((Func<RenderBox, double, double>)((child, extent) => child.getMinIntrinsicWidth(extent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return _getIntrinsicSize(sizingDirection: global::Doroti.Generated.Framework.Painting.Axis.horizontal, extent: height, childSize: ((Func<RenderBox, double, double>)((child, extent) => child.getMaxIntrinsicWidth(extent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return _getIntrinsicSize(sizingDirection: global::Doroti.Generated.Framework.Painting.Axis.vertical, extent: width, childSize: ((Func<RenderBox, double, double>)((child, extent) => child.getMinIntrinsicHeight(extent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return _getIntrinsicSize(sizingDirection: global::Doroti.Generated.Framework.Painting.Axis.vertical, extent: width, childSize: ((Func<RenderBox, double, double>)((child, extent) => child.getMaxIntrinsicHeight(extent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return (this._direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => defaultComputeDistanceToHighestActualBaseline(baseline), global::Doroti.Generated.Framework.Painting.Axis.vertical => defaultComputeDistanceToFirstActualBaseline(baseline), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _getFlex(RenderBox child)
    {
        var childParentData__32861 = ((FlexParentData?)(object?)child.parentData!)!;
        return (((FlexParentData)childParentData__32861).flex ?? 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static FlexFit _getFit(RenderBox child)
    {
        var childParentData__33013 = ((FlexParentData?)(object?)child.parentData!)!;
        return (((FlexParentData)childParentData__33013).fit ?? FlexFit.tight);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isBaselineAligned
    {
        get
        {
            return (this.crossAxisAlignment switch { CrossAxisAlignment.baseline => (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => true, global::Doroti.Generated.Framework.Painting.Axis.vertical => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.stretch => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    internal virtual double _getCrossSize(Size size)
    {
        return (this._direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => size.height, global::Doroti.Generated.Framework.Painting.Axis.vertical => size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getMainSize(Size size)
    {
        return (this._direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => size.width, global::Doroti.Generated.Framework.Painting.Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _flipMainAxis => ((firstChild is not null) && (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => (this.textDirection switch { null => false, TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), global::Doroti.Generated.Framework.Painting.Axis.vertical => (this.verticalDirection switch { global::Doroti.Generated.Framework.Painting.VerticalDirection.down => false, global::Doroti.Generated.Framework.Painting.VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    internal virtual bool _flipCrossAxis => ((firstChild is not null) && (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => (this.textDirection switch { null => false, TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), global::Doroti.Generated.Framework.Painting.Axis.horizontal => (this.verticalDirection switch { global::Doroti.Generated.Framework.Painting.VerticalDirection.down => false, global::Doroti.Generated.Framework.Painting.VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
    internal virtual BoxConstraints _constraintsForNonFlexChild(BoxConstraints constraints)
    {
        bool fillCrossAxis__34901 = (this.crossAxisAlignment switch { CrossAxisAlignment.stretch => true, CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.baseline => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (this._direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => (fillCrossAxis__34901 ? BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraints).maxHeight) : new BoxConstraints(maxHeight: ((BoxConstraints)constraints).maxHeight)), global::Doroti.Generated.Framework.Painting.Axis.vertical => (fillCrossAxis__34901 ? BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraints).maxWidth) : new BoxConstraints(maxWidth: ((BoxConstraints)constraints).maxWidth)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BoxConstraints _constraintsForFlexChild(RenderBox child, BoxConstraints constraints, double maxChildExtent)
    {
        DartRuntimePrimitives.Assert(() => (_getFlex(child) > 0.0));
        DartRuntimePrimitives.Assert(() => (maxChildExtent >= 0.0));
        double minChildExtent__35756 = (_getFit(child) switch { FlexFit.tight => maxChildExtent, FlexFit.loose => 0.0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        bool fillCrossAxis__35888 = (this.crossAxisAlignment switch { CrossAxisAlignment.stretch => true, CrossAxisAlignment.start or CrossAxisAlignment.center or CrossAxisAlignment.end => false, CrossAxisAlignment.baseline => false, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return (this._direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new BoxConstraints(minWidth: minChildExtent__35756, maxWidth: maxChildExtent, minHeight: (fillCrossAxis__35888 ? ((BoxConstraints)constraints).maxHeight : 0.0), maxHeight: ((BoxConstraints)constraints).maxHeight), global::Doroti.Generated.Framework.Painting.Axis.vertical => new BoxConstraints(minWidth: (fillCrossAxis__35888 ? ((BoxConstraints)constraints).maxWidth : 0.0), maxWidth: ((BoxConstraints)constraints).maxWidth, minHeight: minChildExtent__35756, maxHeight: maxChildExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        _LayoutSizes__flex sizes__36734 = _computeSizes(constraints: constraints, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, getBaseline: (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline);
        if (this._isBaselineAligned)
        {
            return ((_LayoutSizes__flex)sizes__36734).baselineOffset;
        }
        return (this._direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => _computeDryDistanceToHighestBaseline(constraints, baseline, sizes__36734), global::Doroti.Generated.Framework.Painting.Axis.vertical => _computeDryDistanceToFirstBaseline(constraints, baseline, sizes__36734), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double? _computeDryDistanceToHighestBaseline(BoxConstraints constraints, TextBaseline baseline, _LayoutSizes__flex sizes)
    {
        BoxConstraints nonFlexConstraints__37718 = _constraintsForNonFlexChild(constraints);
        BoxConstraints constraintsForChild(RenderBox child)
        {
            double? spacePerFlex__37859 = ((_LayoutSizes__flex)sizes).spacePerFlex;
            long flex__37910 = default!;
            return (((spacePerFlex__37859 is not null) && ((flex__37910 = _getFlex(child)) > 0L)) ? _constraintsForFlexChild(child, constraints, (flex__37910 * DartRuntimePrimitives.RequireValue(spacePerFlex__37859))) : nonFlexConstraints__37718);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        bool flipMainAxis__38178 = this._flipMainAxis;
        bool flipCrossAxis__38223 = this._flipCrossAxis;
        var (nextChild__38297, topLeftChild__38319) = (flipMainAxis__38178 ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild)));
        double? baselineOffset__38437 = ((this._isBaselineAligned && (this.textBaseline is not null)) ? ((_LayoutSizes__flex)sizes).baselineOffset : null);
        BaselineOffset minBaseline__38564 = BaselineOffset.noBaseline;
        for (var child__38619 = topLeftChild__38319; (child__38619 is not null); child__38619 = nextChild__38297(child__38619))
        {
            BoxConstraints childConstraints__38711 = constraintsForChild(child__38619);
            double? childBaseline__38778 = child__38619.getDryBaseline(childConstraints__38711, baseline);
            if ((childBaseline__38778 is not null))
            {
                double childBaseline__38778__value38854 = DartRuntimePrimitives.RequireValue(childBaseline__38778);
                double? childBaselineOffset__38973 = ((this._isBaselineAligned && (this.textBaseline is not null)) ? child__38619.getDryBaseline(childConstraints__38711, DartRuntimePrimitives.RequireValue(this.textBaseline)) : null);
                bool baselineAlign__39145 = ((baselineOffset__38437 is not null) && (childBaselineOffset__38973 is not null));
                double childCrossPosition__39238 = default!;
                if (baselineAlign__39145)
                {
                    childCrossPosition__39238 = (DartRuntimePrimitives.RequireValue(baselineOffset__38437) - DartRuntimePrimitives.RequireValue(childBaselineOffset__38973));
                }
                else
                {
                    if (((object.Equals(this.crossAxisAlignment, CrossAxisAlignment.baseline)) && (object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal))))
                    {
                        global::Doroti.Ui.Size childSize__39679 = child__38619.getDryLayout(childConstraints__38711);
                        childCrossPosition__39238 = CrossAxisAlignment.start._getChildCrossAxisOffset((((_LayoutSizes__flex)sizes).axisSize.crossAxisExtent - _getCrossSize(childSize__39679)), false);
                    }
                    else
                    {
                        global::Doroti.Ui.Size childSize__40018 = child__38619.getDryLayout(childConstraints__38711);
                        childCrossPosition__39238 = this.crossAxisAlignment._getChildCrossAxisOffset((((_LayoutSizes__flex)sizes).axisSize.crossAxisExtent - _getCrossSize(childSize__40018)), flipCrossAxis__38223);
                    }
                }
                BaselineOffset candidate__40433 = (new BaselineOffset(DartRuntimePrimitives.RequireValue(childBaseline__38778__value38854)).op_Add(childCrossPosition__39238));
                minBaseline__38564 = minBaseline__38564.minOf(candidate__40433);
            }
        }
        return minBaseline__38564.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double? _computeDryDistanceToFirstBaseline(BoxConstraints constraints, TextBaseline baseline, _LayoutSizes__flex sizes)
    {
        BoxConstraints nonFlexConstraints__41073 = _constraintsForNonFlexChild(constraints);
        BoxConstraints constraintsForChild(RenderBox child)
        {
            double? spacePerFlex__41214 = ((_LayoutSizes__flex)sizes).spacePerFlex;
            long flex__41265 = default!;
            return (((spacePerFlex__41214 is not null) && ((flex__41265 = _getFlex(child)) > 0L)) ? _constraintsForFlexChild(child, constraints, (flex__41265 * DartRuntimePrimitives.RequireValue(spacePerFlex__41214))) : nonFlexConstraints__41073);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        double remainingSpace__41544 = Math.Max(0.0, ((_LayoutSizes__flex)sizes).mainAxisFreeSpace);
        bool flipMainAxis__41616 = this._flipMainAxis;
        var (leadingSpace__41664, betweenSpace__41685) = this.mainAxisAlignment._distributeSpace(remainingSpace__41544, childCount, flipMainAxis__41616, this.spacing);
        var mainPositions__41830 = new DartMap<RenderBox, double>();
        var (nextChildPaintOrder__41911, startChild__41943) = (flipMainAxis__41616 ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild)));
        var pos__42049 = leadingSpace__41664;
        for (var child__42082 = startChild__41943; (child__42082 is not null); child__42082 = nextChildPaintOrder__41911(child__42082))
        {
            mainPositions__41830[DartRuntimePrimitives.RequireReference(child__42082)] = pos__42049;
            BoxConstraints cc__42216 = constraintsForChild(child__42082);
            global::Doroti.Ui.Size cs__42266 = child__42082.getDryLayout(cc__42216);
            pos__42049 += (_getMainSize(cs__42266) + betweenSpace__41685);
        }
        for (RenderBox? child__42474 = firstChild; (child__42474 is not null); child__42474 = childAfter(child__42474))
        {
            BoxConstraints cc__42565 = constraintsForChild(child__42474);
            double? childBaseline__42618 = child__42474.getDryBaseline(cc__42565, baseline);
            if ((childBaseline__42618 is not null))
            {
                double childBaseline__42618__value42680 = DartRuntimePrimitives.RequireValue(childBaseline__42618);
                double? position__42727 = mainPositions__41830.GetValueOrDefault(child__42474);
                return (DartRuntimePrimitives.RequireValue(childBaseline__42618__value42680) + ((position__42727 ?? leadingSpace__41664)));
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        FlutterError? constraintsError__43034 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                constraintsError__43034 = _debugCheckConstraints(constraints: constraints, reportParentConstraints: false);
                return true;
            });
        if ((constraintsError__43034 is not null))
        {
            DartRuntimePrimitives.Assert(() => debugCannotComputeDryLayout(error: constraintsError__43034));
            return Size.zero;
        }
        return _computeSizes(constraints: constraints, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild, getBaseline: (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getDryBaseline).axisSize.toSize(this.direction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual FlutterError? _debugCheckConstraints(BoxConstraints constraints, bool reportParentConstraints)
    {
        FlutterError? result__43717 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                double maxMainSize__43760 = ((object.Equals(this._direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? ((BoxConstraints)constraints).maxWidth : ((BoxConstraints)constraints).maxHeight);
                bool canFlex__43889 = (maxMainSize__43760 < double.PositiveInfinity);
                RenderBox? child__43947 = firstChild;
                while ((child__43947 is not null))
                {
                    long flex__44015 = _getFlex(child__43947);
                    if ((flex__44015 > 0L))
                    {
                        var identity__44079 = ((object.Equals(this._direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? "row" : "column");
                        var axis__44156 = ((object.Equals(this._direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? "horizontal" : "vertical");
                        var dimension__44238 = ((object.Equals(this._direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal)) ? "width" : "height");
                        DiagnosticsNode error__44328 = default!;
                        DiagnosticsNode message__44335 = default!;
                        var addendum__44360 = new List<DiagnosticsNode>();
                        if ((!canFlex__43889 && (((object.Equals(this.mainAxisSize, MainAxisSize.max)) || (object.Equals(_getFit(child__43947), FlexFit.tight))))))
                        {
                            error__44328 = new ErrorSummary($"RenderFlex children have non-zero flex__44015 but incoming {dimension__44238} constraints are unbounded.");
                            message__44335 = new ErrorDescription($"When a {identity__44079} is in a parent that does not provide a finite {dimension__44238} constraint, for example " + $"if it is in a {axis__44156} scrollable, it will try to shrink-wrap its children along the {axis__44156} " + "axis. Setting a flex on a child (e.g. using Expanded) indicates that the child is to " + $"expand to fill the remaining space in the {axis__44156} direction.");
                            if (reportParentConstraints)
                            {
                                RenderBox? node__45239 = this;
                                switch (this._direction)
                                {
                                    case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                                        {
                                            while ((!node__45239!.constraints.hasBoundedWidth && (node__45239.parent is RenderBox)))
                                            {
                                                node__45239 = ((RenderBox?)(object?)node__45239.parent!)!;
                                            }
                                            if (!((RenderBox)node__45239).constraints.hasBoundedWidth)
                                            {
                                                node__45239 = null;
                                            }
                                            break;
                                        }
                                    case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                                        {
                                            while ((!node__45239!.constraints.hasBoundedHeight && (node__45239.parent is RenderBox)))
                                            {
                                                node__45239 = ((RenderBox?)(object?)node__45239.parent!)!;
                                            }
                                            if (!((RenderBox)node__45239).constraints.hasBoundedHeight)
                                            {
                                                node__45239 = null;
                                            }
                                            break;
                                        }
                                }
                                if ((node__45239 is not null))
                                {
                                    addendum__44360.Add(node__45239.describeForError("The nearest ancestor providing an unbounded width constraint is"));
                                }
                            }
                            addendum__44360.Add(new ErrorHint("See also: https://flutter.dev/unbounded-constraints"));
                        }
                        else
                        {
                            return true;
                        }
                        result__43717 = new FlutterError(new List<DiagnosticsNode> { error__44328, message__44335, new ErrorDescription("These two directives are mutually exclusive. If a parent is to shrink-wrap its child, the child " + "cannot simultaneously expand to fit its parent."), new ErrorHint("Consider setting mainAxisSize to MainAxisSize.min and using FlexFit.loose fits for the flexible " + "children (using Flexible rather than Expanded). This will allow the flexible children " + "to size themselves to less than the infinite remaining space they would otherwise be " + "forced to take, and then will cause the RenderFlex to shrink-wrap the children " + "rather than expanding to fit the maximum constraints provided by the parent."), new ErrorDescription("If this message did not help you determine the problem, consider using debugDumpRenderTree():\n" + "  https://flutter.dev/to/debug-render-layer\n" + "  https://api.flutter.dev/flutter/rendering/debugDumpRenderTree.html"), describeForError("The affected RenderFlex is", style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<object>("The creator information is set to", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorDescription("If none of the above helps enough to fix this problem, please don't hesitate to file a bug:\n" + "  https://github.com/flutter/flutter/issues/new?template=02_bug.yml") });
                        return true;
                    }
                    child__43947 = childAfter(child__43947);
                }
                return true;
            });
        return result__43717;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _LayoutSizes__flex _computeSizes(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild, Func<RenderBox, BoxConstraints, TextBaseline, double?> getBaseline)
    {
        DartRuntimePrimitives.Assert(() => this._debugHasNecessaryDirections);
        double maxMainSize__48571 = _getMainSize(((BoxConstraints)constraints).biggest);
        bool canFlex__48635 = double.IsFinite(maxMainSize__48571);
        BoxConstraints nonFlexChildConstraints__48692 = _constraintsForNonFlexChild(constraints);
        global::Doroti.Ui.TextBaseline? textBaseline__48845 = (this._isBaselineAligned ? ((this.textBaseline ?? throw new FlutterError("To use CrossAxisAlignment.baseline, you must also specify which baseline to use using the \"textBaseline\" argument."))) : null);
        var totalFlex__49197 = 0L;
        RenderBox? firstFlexChild__49227 = default!;
        _AscentDescent__flex accumulatedAscentDescent__49262 = _AscentDescent__flex.none;
        var accumulatedSize__49412 = _AxisSize__flex.Create_(new global::Doroti.Ui.Size((this.spacing * ((childCount - 1L))), 0.0));
        for (RenderBox? child__49502 = firstChild; (child__49502 is not null); child__49502 = childAfter(child__49502))
        {
            long flex__49582 = default!;
            if ((canFlex__48635 && ((flex__49582 = _getFlex(child__49502)) > 0L)))
            {
                totalFlex__49197 += flex__49582;
                firstFlexChild__49227 ??= child__49502;
            }
            else
            {
                var childSize__49731 = _AxisSize__flex.CreateFromSize(size: layoutChild(child__49502, nonFlexChildConstraints__48692), direction: this.direction);
                accumulatedSize__49412 = accumulatedSize__49412.op_Add(childSize__49731);
                double? baselineOffset__50013 = ((textBaseline__48845 is null) ? null : getBaseline(child__49502, nonFlexChildConstraints__48692, DartRuntimePrimitives.RequireValue(textBaseline__48845)));
                accumulatedAscentDescent__49262 = accumulatedAscentDescent__49262.op_Add(_AscentDescent__flex.Create(baselineOffset: baselineOffset__50013, crossSize: ((_AxisSize__flex)childSize__49731).crossAxisExtent));
            }
        }
        DartRuntimePrimitives.Assert(() => (((totalFlex__49197 == 0L)) == ((firstFlexChild__49227 is null))));
        DartRuntimePrimitives.Assert(() => ((firstFlexChild__49227 is null) || canFlex__48635));
        double flexSpace__50586 = Math.Max(0.0, (maxMainSize__48571 - ((_AxisSize__flex)accumulatedSize__49412).mainAxisExtent));
        double spacePerFlex__50676 = (flexSpace__50586 / totalFlex__49197);
        for (var child__50727 = firstFlexChild__49227; ((child__50727 is not null) && (totalFlex__49197 > 0L)); child__50727 = childAfter(child__50727))
        {
            long flex__50828 = _getFlex(child__50727);
            if ((flex__50828 == 0L))
            {
                continue;
            }
            totalFlex__49197 -= flex__50828;
            DartRuntimePrimitives.Assert(() => double.IsFinite(spacePerFlex__50676));
            double maxChildExtent__50982 = (spacePerFlex__50676 * flex__50828);
            DartRuntimePrimitives.Assert(() => ((object.Equals(_getFit(child__50727), FlexFit.loose)) || (maxChildExtent__50982 < double.PositiveInfinity)));
            BoxConstraints childConstraints__51130 = _constraintsForFlexChild(child__50727, constraints, maxChildExtent__50982);
            var childSize__51256 = _AxisSize__flex.CreateFromSize(size: layoutChild(child__50727, childConstraints__51130), direction: this.direction);
            accumulatedSize__49412 = accumulatedSize__49412.op_Add(childSize__51256);
            double? baselineOffset__51435 = ((textBaseline__48845 is null) ? null : getBaseline(child__50727, childConstraints__51130, DartRuntimePrimitives.RequireValue(textBaseline__48845)));
            accumulatedAscentDescent__49262 = accumulatedAscentDescent__49262.op_Add(_AscentDescent__flex.Create(baselineOffset: baselineOffset__51435, crossSize: ((_AxisSize__flex)childSize__51256).crossAxisExtent));
        }
        DartRuntimePrimitives.Assert(() => (totalFlex__49197 == 0L));
        accumulatedSize__49412 = accumulatedSize__49412.op_Add(((accumulatedAscentDescent__49262).ascentDescent switch { null => _AxisSize__flex.empty, (double ascent__51937, double descent__51958) => new _AxisSize__flex(mainAxisExtent: 0, crossAxisExtent: (ascent__51937 + descent__51958)) }));
        double idealMainSize__52085 = (this.mainAxisSize switch { MainAxisSize.max when double.IsFinite(maxMainSize__48571) => maxMainSize__48571, MainAxisSize.max => ((_AxisSize__flex)accumulatedSize__49412).mainAxisExtent, MainAxisSize.min => ((_AxisSize__flex)accumulatedSize__49412).mainAxisExtent, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        _AxisSize__flex constrainedSize__52296 = new _AxisSize__flex(mainAxisExtent: idealMainSize__52085, crossAxisExtent: ((_AxisSize__flex)accumulatedSize__49412).crossAxisExtent).applyConstraints(constraints, this.direction);
        return new _LayoutSizes__flex(axisSize: constrainedSize__52296, mainAxisFreeSpace: (((_AxisSize__flex)constrainedSize__52296).mainAxisExtent - ((_AxisSize__flex)accumulatedSize__49412).mainAxisExtent), baselineOffset: ((_AscentDescent__flex)accumulatedAscentDescent__49262).baselineOffset, spacePerFlex: ((firstFlexChild__49227 is null) ? null : spacePerFlex__50676));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__52817 = this.constraints;
        DartRuntimePrimitives.Assert(() =>
            {
                FlutterError? constraintsError__52891 = _debugCheckConstraints(constraints: constraints__52817, reportParentConstraints: true);
                if ((constraintsError__52891 is not null))
                {
                    throw constraintsError__52891;
                }
                return true;
            });
        _LayoutSizes__flex sizes__53147 = _computeSizes(constraints: constraints__52817, layoutChild: (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild, getBaseline: (Func<RenderBox, BoxConstraints, TextBaseline, double?>)ChildLayoutHelper.getBaseline);
        double crossAxisExtent__53327 = ((_LayoutSizes__flex)sizes__53147).axisSize.crossAxisExtent;
        size = ((_LayoutSizes__flex)sizes__53147).axisSize.toSize(this.direction);
        _overflow = Math.Max(0.0, -((_LayoutSizes__flex)sizes__53147).mainAxisFreeSpace);
        double remainingSpace__53497 = Math.Max(0.0, ((_LayoutSizes__flex)sizes__53147).mainAxisFreeSpace);
        bool flipMainAxis__53569 = this._flipMainAxis;
        bool flipCrossAxis__53614 = this._flipCrossAxis;
        var (leadingSpace__53664, betweenSpace__53685) = this.mainAxisAlignment._distributeSpace(remainingSpace__53497, childCount, flipMainAxis__53569, this.spacing);
        var (nextChild__53841, topLeftChild__53863) = (flipMainAxis__53569 ? (((Func<RenderBox, RenderBox?>, RenderBox?))(childBefore, lastChild)) : (((Func<RenderBox, RenderBox?>, RenderBox?))(childAfter, firstChild)));
        double? baselineOffset__53981 = ((_LayoutSizes__flex)sizes__53147).baselineOffset;
        DartRuntimePrimitives.Assert(() => ((baselineOffset__53981 is null) || (((object.Equals(this.crossAxisAlignment, CrossAxisAlignment.baseline)) && (object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal))))));
        var childMainPosition__54326 = leadingSpace__53664;
        for (var child__54373 = topLeftChild__53863; (child__54373 is not null); child__54373 = nextChild__53841(child__54373))
        {
            double? childBaselineOffset__54458 = default!;
            bool baselineAlign__54496 = ((baselineOffset__53981 is not null) && ((childBaselineOffset__54458 = child__54373.getDistanceToBaseline(DartRuntimePrimitives.RequireValue(this.textBaseline), onlyReal: true)) is not null));
            double childCrossPosition__54683 = default!;
            if (baselineAlign__54496)
            {
                childCrossPosition__54683 = (DartRuntimePrimitives.RequireValue(baselineOffset__53981) - DartRuntimePrimitives.RequireValue(childBaselineOffset__54458));
            }
            else
            {
                if (((object.Equals(this.crossAxisAlignment, CrossAxisAlignment.baseline)) && (object.Equals(this.direction, global::Doroti.Generated.Framework.Painting.Axis.horizontal))))
                {
                    childCrossPosition__54683 = CrossAxisAlignment.start._getChildCrossAxisOffset((crossAxisExtent__53327 - _getCrossSize(((RenderBox)child__54373).size)), false);
                }
                else
                {
                    childCrossPosition__54683 = this.crossAxisAlignment._getChildCrossAxisOffset((crossAxisExtent__53327 - _getCrossSize(((RenderBox)child__54373).size)), flipCrossAxis__53614);
                }
            }
            var childParentData__55608 = ((FlexParentData?)(object?)child__54373.parentData!)!;
            childParentData__55608.offset = (this.direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(childMainPosition__54326, childCrossPosition__54683), global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(childCrossPosition__54683, childMainPosition__54326), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            childMainPosition__54326 += (_getMainSize(((RenderBox)child__54373).size) + betweenSpace__53685);
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
                var debugOverflowHints__56607 = new List<DiagnosticsNode> { new ErrorDescription($"The overflowing {this.GetType()} has an orientation of {this._direction}."), new ErrorDescription($"The edge of the {this.GetType()} that is overflowing has been marked " + "in the rendering with a yellow and black striped pattern. This is " + $"usually caused by the contents being too big for the {this.GetType()}."), new ErrorHint("Consider applying a flex factor (e.g. using an Expanded widget) to " + $"force the children of the {this.GetType()} to fit within the available " + "space instead of being sized to their natural size."), new ErrorHint("This is considered an error condition because it indicates that there " + "is content that cannot be seen. If the content is legitimately bigger " + "than the available space, consider clipping it with a ClipRect widget " + "before putting it in the flex, or using a scrollable container rather " + "than a Flex, like a ListView.") };
                global::Doroti.Ui.Rect overflowChildRect__57884 = (this._direction switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, (size.width + this._overflow), 0.0), global::Doroti.Generated.Framework.Painting.Axis.vertical => global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, 0.0, (size.height + this._overflow)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                paintOverflowIndicator(context, offset, (Offset.zero & size), overflowChildRect__57884, overflowHints: debugOverflowHints__56607);
                return true;
            });
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        foreach (global::Doroti.Generated.Framework.Painting.TextPainter painter__3670 in this._indicatorLabel)
        {
            painter__3670.dispose();
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
        string header__58826 = base.toStringShort();
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (this._hasOverflow)
            {
                header__58826 += " OVERFLOWING";
            }
        }
        return header__58826;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Generated.Framework.Painting.Axis>("direction", this.direction));
        properties.add(new EnumProperty<MainAxisAlignment>("mainAxisAlignment", this.mainAxisAlignment));
        properties.add(new EnumProperty<MainAxisSize>("mainAxisSize", this.mainAxisSize));
        properties.add(new EnumProperty<CrossAxisAlignment>("crossAxisAlignment", this.crossAxisAlignment));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Generated.Framework.Painting.VerticalDirection>("verticalDirection", this.verticalDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Ui.TextBaseline>("textBaseline", this.textBaseline, defaultValue: null));
        properties.add(new DoubleProperty("spacing", this.spacing, defaultValue: null));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((FlexParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((FlexParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((FlexParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((FlexParentData?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((FlexParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((FlexParentData?)(object?)this._firstChild!.parentData!)!;
                firstChildParentData__176343.previousSibling = child;
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
            var afterParentData__176766 = ((FlexParentData?)(object?)after.parentData!)!;
            if ((afterParentData__176766.nextSibling is null))
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(after, this._lastChild)));
                childParentData__175971.previousSibling = after;
                afterParentData__176766.nextSibling = child;
                this._lastChild = child;
            }
            else
            {
                childParentData__175971.nextSibling = afterParentData__176766.nextSibling;
                childParentData__175971.previousSibling = after;
                var childPreviousSiblingParentData__177424 = ((FlexParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((FlexParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
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
        var childParentData__179226 = ((FlexParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => _debugUltimatePreviousSiblingOf(child, equals: this._firstChild));
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: this._lastChild));
        DartRuntimePrimitives.Assert(() => (this._childCount >= 0L));
        if ((childParentData__179226.previousSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._firstChild, child)));
            this._firstChild = childParentData__179226.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData__179613 = ((FlexParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((FlexParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((FlexParentData?)(object?)child__180623.parentData!)!;
            RenderBox? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
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
        var childParentData__181479 = ((FlexParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
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
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((FlexParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((FlexParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((FlexParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((FlexParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((FlexParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((FlexParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child__183606 = this.firstChild!;
            var count__183637 = 1L;
            while (true)
            {
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((FlexParentData?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child__138717 = firstChild;
        while ((child__138717 is not null))
        {
            var childParentData__138777 = ((FlexParentData?)(object?)child__138717.parentData!)!;
            double? result__138852 = child__138717.getDistanceToActualBaseline(baseline);
            if ((result__138852 is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result__138852);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData__138777.offset.dy);
            }
            child__138717 = childParentData__138777.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline__139372 = BaselineOffset.noBaseline;
        RenderBox? child__139428 = firstChild;
        while ((child__139428 is not null))
        {
            var childParentData__139488 = ((FlexParentData?)(object?)child__139428.parentData!)!;
            BaselineOffset candidate__139570 = (new BaselineOffset(child__139428.getDistanceToActualBaseline(baseline)).op_Add(childParentData__139488.offset.dy));
            minBaseline__139372 = minBaseline__139372.minOf(candidate__139570);
            child__139428 = childParentData__139488.nextSibling;
        }
        return minBaseline__139372.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child__140279 = lastChild;
        while ((child__140279 is not null))
        {
            var childParentData__140418 = ((FlexParentData?)(object?)child__140279.parentData!)!;
            bool isHit__140490 = result.addWithPaintOffset(offset: childParentData__140418.offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData__140418.offset))));
                return child__140279!.hitTest(result, position: transformed);
                return default;
            })));
            if (isHit__140490)
            {
                return true;
            }
            child__140279 = childParentData__140418.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child__141240 = firstChild;
        while ((child__141240 is not null))
        {
            var childParentData__141300 = ((FlexParentData?)(object?)child__141240.parentData!)!;
            context.paintChild(child__141240, (childParentData__141300.offset + offset));
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((FlexParentData?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
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
        var regions__4254 = new List<_OverflowRegionData__debug_overflow_indicator>();
        if ((((RelativeRect)overflow).left > 0.0))
        {
            var markerRect__4332 = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, (containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction), containerRect.height);
            regions__4254.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect__4332, label: $"LEFT OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).left)} PIXELS", labelOffset: (markerRect__4332.centerLeft + new global::Doroti.Ui.Offset((DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), 0.0)), rotation: (Dart_mathLibrary.pi / 2.0), side: _OverflowSide__debug_overflow_indicator.left));
        }
        if ((((RelativeRect)overflow).right > 0.0))
        {
            var markerRect__4921 = global::Doroti.Ui.Rect.fromLTWH((containerRect.width * ((1.0 - DebugOverflowIndicatorMixin._indicatorFraction))), 0.0, (containerRect.width * DebugOverflowIndicatorMixin._indicatorFraction), containerRect.height);
            regions__4254.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect__4921, label: $"RIGHT OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).right)} PIXELS", labelOffset: (markerRect__4921.centerRight - new global::Doroti.Ui.Offset((DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels), 0.0)), rotation: (-Dart_mathLibrary.pi / 2.0), side: _OverflowSide__debug_overflow_indicator.right));
        }
        if ((((RelativeRect)overflow).top > 0.0))
        {
            var markerRect__5558 = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, containerRect.width, (containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction));
            regions__4254.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect__5558, label: $"TOP OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).top)} PIXELS", labelOffset: (markerRect__5558.topCenter + new global::Doroti.Ui.Offset(0.0, DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels)), side: _OverflowSide__debug_overflow_indicator.top));
        }
        if ((((RelativeRect)overflow).bottom > 0.0))
        {
            var markerRect__6054 = global::Doroti.Ui.Rect.fromLTWH(0.0, (containerRect.height * ((1.0 - DebugOverflowIndicatorMixin._indicatorFraction))), containerRect.width, (containerRect.height * DebugOverflowIndicatorMixin._indicatorFraction));
            regions__4254.Add(new _OverflowRegionData__debug_overflow_indicator(rect: markerRect__6054, label: $"BOTTOM OVERFLOWED BY {_formatPixels(((RelativeRect)overflow).bottom)} PIXELS", labelOffset: (markerRect__6054.bottomCenter - new global::Doroti.Ui.Offset(0.0, (DebugOverflowIndicatorMixin._indicatorFontSizePixels + DebugOverflowIndicatorMixin._indicatorLabelPaddingPixels))), side: _OverflowSide__debug_overflow_indicator.bottom));
        }
        return regions__4254;
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
        var overflows__7571 = new List<string>();
        var overflowText__7954 = "";
        DartRuntimePrimitives.Assert(() => (checked((long)(overflows__7571.Count)) != 0));
        switch (checked((long)(overflows__7571.Count)))
        {
            case 1L:
                {
                    overflowText__7954 = overflows__7571.First();
                    break;
                }
            case 2L:
                {
                    overflowText__7954 = $"{overflows__7571.First()} and {overflows__7571.Last()}";
                    break;
                }
            default:
                {
                    overflows__7571[(int)((checked((long)(overflows__7571.Count)) - 1L))] = $"and {overflows__7571[(int)((checked((long)(overflows__7571.Count)) - 1L))]}";
                    overflowText__7954 = string.Join(", ", overflows__7571);
                    break;
                }
        }
        FlutterError.reportError(new FlutterErrorDetails(exception: new FlutterError($"A {this.GetType()} overflowed by {overflowText__7954}."), library: "rendering library", context: new ErrorDescription("during layout"), informationCollector: (() => new List<DiagnosticsNode> { describeForError($"The specific {this.GetType()} in question is"), new DiagnosticsNode(DartCoreExtensions.repeat("◢◤", ((checked((long)(FlutterError.wrapWidth / 2L))))), allowWrap: false) })));
    }

    public virtual void paintOverflowIndicator(PaintingContext context, Offset offset, Rect containerRect, Rect childRect, List<DiagnosticsNode>? overflowHints = null)
    {
        var overflow__9849 = RelativeRect.CreateFromRect(containerRect, childRect);
        if (((((((RelativeRect)overflow__9849).left <= 0.0) && (((RelativeRect)overflow__9849).right <= 0.0)) && (((RelativeRect)overflow__9849).top <= 0.0)) && (((RelativeRect)overflow__9849).bottom <= 0.0)))
        {
            return;
        }
        List<_OverflowRegionData__debug_overflow_indicator> overflowRegions__10097 = _calculateOverflowRegions(overflow__9849, containerRect);
        foreach (var region__10201 in overflowRegions__10097)
        {
            ((PaintingContext)context).canvas.drawRect(((_OverflowRegionData__debug_overflow_indicator)region__10201).rect.shift(offset), DebugOverflowIndicatorMixin._indicatorPaint);
            var textSpan__10317 = ((global::Doroti.Generated.Framework.Painting.TextSpan?)(object?)this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].text)!;
            if ((textSpan__10317?.text != ((_OverflowRegionData__debug_overflow_indicator)region__10201).label))
            {
                this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].text = new global::Doroti.Generated.Framework.Painting.TextSpan(text: ((_OverflowRegionData__debug_overflow_indicator)region__10201).label, style: DebugOverflowIndicatorMixin._indicatorTextStyle);
                this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].layout();
            }
            global::Doroti.Ui.Offset labelOffset__10646 = (((_OverflowRegionData__debug_overflow_indicator)region__10201).labelOffset + offset);
            var centerOffset__10701 = new global::Doroti.Ui.Offset((-this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].width / 2.0), 0.0);
            global::Doroti.Ui.Rect textBackgroundRect__10795 = (centerOffset__10701 & this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].size);
            ((PaintingContext)context).canvas.save();
            ((PaintingContext)context).canvas.translate(labelOffset__10646.dx, labelOffset__10646.dy);
            ((PaintingContext)context).canvas.rotate(((_OverflowRegionData__debug_overflow_indicator)region__10201).rotation);
            ((PaintingContext)context).canvas.drawRect(textBackgroundRect__10795, DebugOverflowIndicatorMixin._labelBackgroundPaint);
            this._indicatorLabel[(int)(FoundationRuntimePorts.EnumIndex(((_OverflowRegionData__debug_overflow_indicator)region__10201).side))].paint(((PaintingContext)context).canvas, centerOffset__10701);
            ((PaintingContext)context).canvas.restore();
        }
        if (this._overflowReportNeeded)
        {
            this._overflowReportNeeded = false;
            _reportOverflow(overflow__9849, overflowHints);
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

