// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/wrap.dart
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

internal delegate RenderBox? _NextChild__wrap(RenderBox child);

internal delegate void _PositionChild__wrap(Offset offset, RenderBox child);

internal delegate Size _GetChildSize__wrap(RenderBox child);

public class _AxisSize__wrap
{
    public global::Doroti.Ui.Size _size { get; }

    private _AxisSize__wrap(global::Doroti.Ui.Size _size)
    {
        this._size = _size;
    }

    public static _AxisSize__wrap Create_(global::Doroti.Ui.Size _size) => new _AxisSize__wrap(_size);

    public static implicit operator global::Doroti.Ui.Size(_AxisSize__wrap value) => value._size;
    public static implicit operator _AxisSize__wrap(global::Doroti.Ui.Size value) => new _AxisSize__wrap(value);

    public static _AxisSize__wrap empty = _AxisSize__wrap.Create_(Size.zero);

    internal _AxisSize__wrap(double mainAxisExtent, double crossAxisExtent)
    {
        this._size = new global::Doroti.Ui.Size(mainAxisExtent, crossAxisExtent);
    }

    internal static _AxisSize__wrap CreateFromSize(Size size, global::Doroti.Framework.Painting.Axis direction)
    {
        return _AxisSize__wrap.Create_(_convert(size, direction));
    }

    internal static global::Doroti.Ui.Size _convert(Size size, global::Doroti.Framework.Painting.Axis direction)
    {
        return (direction switch { global::Doroti.Framework.Painting.Axis.horizontal => size, global::Doroti.Framework.Painting.Axis.vertical => size.flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double mainAxisExtent => _size.width;
    public virtual double crossAxisExtent => _size.height;
    public virtual global::Doroti.Ui.Size toSize(global::Doroti.Framework.Painting.Axis direction) => _convert(_size, direction);
    public virtual _AxisSize__wrap applyConstraints(BoxConstraints constraints, global::Doroti.Framework.Painting.Axis direction)
    {
        BoxConstraints effectiveConstraints = (direction switch { global::Doroti.Framework.Painting.Axis.horizontal => constraints, global::Doroti.Framework.Painting.Axis.vertical => ((BoxConstraints)constraints).flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return _AxisSize__wrap.Create_(effectiveConstraints.constrain(_size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _AxisSize__wrap flipped => _AxisSize__wrap.Create_(_size.flipped);
    public virtual _AxisSize__wrap op_Add(_AxisSize__wrap other) => _AxisSize__wrap.Create_(new global::Doroti.Ui.Size((_size.width + other._size.width), Math.Max(_size.height, other._size.height)));
    public virtual _AxisSize__wrap op_Subtract(_AxisSize__wrap other) => _AxisSize__wrap.Create_(new global::Doroti.Ui.Size((_size.width - other._size.width), (_size.height - other._size.height)));
}

public enum WrapAlignment
{
    start,
    end,
    center,
    spaceBetween,
    spaceAround,
    spaceEvenly
}

public static class WrapAlignmentMembers
{
    internal static (double, double) _distributeSpace(this WrapAlignment value, double freeSpace, double itemSpacing, long itemCount, bool flipped)
    {
        DartRuntimePrimitives.Assert(() => (itemCount > 0L));
        return (value switch { WrapAlignment.start => (((double, double))(((flipped ? freeSpace : 0.0), itemSpacing))), WrapAlignment.end => (((double, double))(WrapAlignment.start._distributeSpace(freeSpace, itemSpacing, itemCount, !flipped))), WrapAlignment.spaceBetween when (itemCount < 2L) => (((double, double))(WrapAlignment.start._distributeSpace(freeSpace, itemSpacing, itemCount, flipped))), WrapAlignment.center => (((double, double))(((freeSpace / 2.0), itemSpacing))), WrapAlignment.spaceBetween => (((double, double))((0, ((freeSpace / ((itemCount - 1L))) + itemSpacing)))), WrapAlignment.spaceAround => (((double, double))((((freeSpace / itemCount) / 2L), ((freeSpace / itemCount) + itemSpacing)))), WrapAlignment.spaceEvenly => (((double, double))(((freeSpace / ((itemCount + 1L))), ((freeSpace / ((itemCount + 1L))) + itemSpacing)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public enum WrapCrossAlignment
{
    start,
    end,
    center
}

public static class WrapCrossAlignmentMembers
{
    internal static WrapCrossAlignment _flipped(this WrapCrossAlignment value) => (value switch { WrapCrossAlignment.start => WrapCrossAlignment.end, WrapCrossAlignment.end => WrapCrossAlignment.start, WrapCrossAlignment.center => WrapCrossAlignment.center, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    internal static double _alignment(this WrapCrossAlignment value) => (value switch { WrapCrossAlignment.start => 0, WrapCrossAlignment.end => 1, WrapCrossAlignment.center => 0.5, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
}

internal class _RunMetrics__wrap
{
    public virtual _AxisSize__wrap axisSize { get; set; } = default!;
    public virtual long childCount { get; set; } = 1L;
    public virtual RenderBox leadingChild { get; set; } = default!;

    internal _RunMetrics__wrap(RenderBox leadingChild, _AxisSize__wrap axisSize)
    {
        this.leadingChild = leadingChild;
        this.axisSize = axisSize;
    }

    public virtual _RunMetrics__wrap? tryAddingNewChild(RenderBox child, _AxisSize__wrap childSize, bool flipMainAxis, double spacing, double maxMainExtent)
    {
        bool needsNewRun = ((((((_AxisSize__wrap)this.axisSize).mainAxisExtent + ((_AxisSize__wrap)childSize).mainAxisExtent) + spacing) - maxMainExtent) > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance);
        if (needsNewRun)
        {
            return new _RunMetrics__wrap(child, childSize);
        }
        else
        {
            axisSize = axisSize.op_Add((childSize.op_Add(new _AxisSize__wrap(mainAxisExtent: spacing, crossAxisExtent: 0.0))));
            childCount += 1L;
            if (flipMainAxis)
            {
                leadingChild = child;
            }
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class WrapParentData : ContainerBoxParentData<RenderBox>
{
}

public class RenderWrap : RenderBox, ContainerRenderObjectMixin<RenderBox, WrapParentData>, RenderBoxContainerDefaultsMixin<RenderBox, WrapParentData>
{
    internal virtual global::Doroti.Framework.Painting.Axis _direction { get; set; } = default!;
    internal virtual WrapAlignment _alignment { get; set; } = default!;
    internal virtual double _spacing { get; set; } = default!;
    internal virtual WrapAlignment _runAlignment { get; set; } = default!;
    internal virtual double _runSpacing { get; set; } = default!;
    internal virtual WrapCrossAlignment _crossAxisAlignment { get; set; } = default!;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.VerticalDirection _verticalDirection { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.none;
    internal virtual bool _hasVisualOverflow { get; set; } = false;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderWrap(List<RenderBox>? children = null, global::Doroti.Framework.Painting.Axis direction = Axis.horizontal, WrapAlignment alignment = WrapAlignment.start, double spacing = 0.0, WrapAlignment runAlignment = WrapAlignment.start, double runSpacing = 0.0, WrapCrossAlignment crossAxisAlignment = WrapCrossAlignment.start, TextDirection? textDirection = null, global::Doroti.Framework.Painting.VerticalDirection verticalDirection = VerticalDirection.down, Clip clipBehavior = Clip.none)
    {
        this._direction = direction;
        this._alignment = alignment;
        this._spacing = spacing;
        this._runAlignment = runAlignment;
        this._runSpacing = runSpacing;
        this._crossAxisAlignment = crossAxisAlignment;
        this._textDirection = textDirection;
        this._verticalDirection = verticalDirection;
        this._clipBehavior = clipBehavior;
    }

    public virtual global::Doroti.Framework.Painting.Axis direction
    {
        get => this._direction;
        set
        {
            var __value = value;
            if ((object.Equals(this._direction, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _direction = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual WrapAlignment alignment
    {
        get => this._alignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._alignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _alignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
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
    public virtual WrapAlignment runAlignment
    {
        get => this._runAlignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._runAlignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _runAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double runSpacing
    {
        get => this._runSpacing;
        set
        {
            var __value = value;
            if ((this._runSpacing == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _runSpacing = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual WrapCrossAlignment crossAxisAlignment
    {
        get => this._crossAxisAlignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._crossAxisAlignment, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _crossAxisAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
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
    internal virtual bool _debugHasNecessaryDirections
    {
        get
        {
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
            if (((object.Equals(this.alignment, WrapAlignment.start)) || (object.Equals(this.alignment, WrapAlignment.end))))
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
            if (((object.Equals(this.runAlignment, WrapAlignment.start)) || (object.Equals(this.runAlignment, WrapAlignment.end))))
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
            if (((object.Equals(this.crossAxisAlignment, WrapCrossAlignment.start)) || (object.Equals(this.crossAxisAlignment, WrapCrossAlignment.end))))
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
    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        if ((__child.parentData is not WrapParentData))
        {
            __child.parentData = new WrapParentData();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        switch (this.direction)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    var widthLocal = 0.0;
                    RenderBox? child = firstChild;
                    while ((child is not null))
                    {
                        widthLocal = Math.Max(widthLocal, child.getMinIntrinsicWidth(double.PositiveInfinity));
                        child = childAfter(child);
                    }
                    return widthLocal;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    return getDryLayout(new BoxConstraints(maxHeight: height)).width;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        switch (this.direction)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    var widthLocal = 0.0;
                    RenderBox? child = firstChild;
                    while ((child is not null))
                    {
                        widthLocal += child.getMaxIntrinsicWidth(double.PositiveInfinity);
                        child = childAfter(child);
                    }
                    return widthLocal;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    return getDryLayout(new BoxConstraints(maxHeight: height)).width;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        switch (this.direction)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    return getDryLayout(new BoxConstraints(maxWidth: width)).height;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    var heightLocal = 0.0;
                    RenderBox? child = firstChild;
                    while ((child is not null))
                    {
                        heightLocal = Math.Max(heightLocal, child.getMinIntrinsicHeight(double.PositiveInfinity));
                        child = childAfter(child);
                    }
                    return heightLocal;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        switch (this.direction)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    return getDryLayout(new BoxConstraints(maxWidth: width)).height;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    var heightLocal = 0.0;
                    RenderBox? child = firstChild;
                    while ((child is not null))
                    {
                        heightLocal += child.getMaxIntrinsicHeight(double.PositiveInfinity);
                        child = childAfter(child);
                    }
                    return heightLocal;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToHighestActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getMainAxisExtent(Size childSize)
    {
        return (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => childSize.width, global::Doroti.Framework.Painting.Axis.vertical => childSize.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getCrossAxisExtent(Size childSize)
    {
        return (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => childSize.height, global::Doroti.Framework.Painting.Axis.vertical => childSize.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _getOffset(double mainAxisOffset, double crossAxisOffset)
    {
        return (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Offset(mainAxisOffset, crossAxisOffset), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Offset(crossAxisOffset, mainAxisOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (bool, bool) _areAxesFlipped
    {
        get
        {
            bool flipHorizontal = ((this.textDirection ?? TextDirection.ltr) switch { TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            bool flipVertical = (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => false, global::Doroti.Framework.Painting.VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (((bool, bool))((flipHorizontal, flipVertical))), global::Doroti.Framework.Painting.Axis.vertical => (((bool, bool))((flipVertical, flipHorizontal))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        if ((firstChild is null))
        {
            return null;
        }
        BoxConstraints childConstraints = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new BoxConstraints(maxWidth: ((BoxConstraints)constraints).maxWidth), global::Doroti.Framework.Painting.Axis.vertical => new BoxConstraints(maxHeight: ((BoxConstraints)constraints).maxHeight), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var (childrenAxisSize, runMetrics) = _computeRuns(constraints, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild);
        _AxisSize__wrap containerAxisSize = childrenAxisSize.applyConstraints(constraints, this.direction);
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        void findHighestBaseline(Offset offset, RenderBox child)
        {
            baselineOffset = baselineOffset.minOf((new BaselineOffset(child.getDryBaseline(childConstraints, baseline)).op_Add(offset.dy)));
        }
        Size getChildSize(RenderBox child)
        {
            return child.getDryLayout(childConstraints);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        _positionChildren(runMetrics, childrenAxisSize, containerAxisSize, (Action<Offset, RenderBox>)findHighestBaseline, (Func<RenderBox, Size>)getChildSize);
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeDryLayout(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild = default!)
    {
        var (childConstraints, mainAxisLimit) = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (((BoxConstraints, double))((new BoxConstraints(maxWidth: ((BoxConstraints)constraints).maxWidth), ((BoxConstraints)constraints).maxWidth))), global::Doroti.Framework.Painting.Axis.vertical => (((BoxConstraints, double))((new BoxConstraints(maxHeight: ((BoxConstraints)constraints).maxHeight), ((BoxConstraints)constraints).maxHeight))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var mainAxisExtent = 0.0;
        var crossAxisExtent = 0.0;
        var runMainAxisExtent = 0.0;
        var runCrossAxisExtent = 0.0;
        var childCount = 0L;
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            global::Doroti.Ui.Size childSize = layoutChild(child, childConstraints);
            double childMainAxisExtent = _getMainAxisExtent(childSize);
            double childCrossAxisExtent = _getCrossAxisExtent(childSize);
            if (((childCount > 0L) && (((runMainAxisExtent + childMainAxisExtent) + this.spacing) > mainAxisLimit)))
            {
                mainAxisExtent = Math.Max(mainAxisExtent, runMainAxisExtent);
                crossAxisExtent += (runCrossAxisExtent + this.runSpacing);
                runMainAxisExtent = 0.0;
                runCrossAxisExtent = 0.0;
                childCount = 0L;
            }
            runMainAxisExtent += childMainAxisExtent;
            runCrossAxisExtent = Math.Max(runCrossAxisExtent, childCrossAxisExtent);
            if ((childCount > 0L))
            {
                runMainAxisExtent += this.spacing;
            }
            childCount += 1L;
            child = childAfter(child);
        }
        crossAxisExtent += runCrossAxisExtent;
        mainAxisExtent = Math.Max(mainAxisExtent, runMainAxisExtent);
        return constraints.constrain((this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Size(mainAxisExtent, crossAxisExtent), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Size(crossAxisExtent, mainAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Size _getChildSize(RenderBox child) => ((RenderBox)child).size;
    internal static void _setChildPosition(Offset offset, RenderBox child)
    {
        (((WrapParentData?)(object?)child.parentData!)!).offset = offset;
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = this.constraints;
        DartRuntimePrimitives.Assert(() => this._debugHasNecessaryDirections);
        if ((firstChild is null))
        {
            size = ((BoxConstraints)constraintsLocal).smallest;
            _hasVisualOverflow = false;
            return;
        }
        var (childrenAxisSize, runMetrics) = _computeRuns(constraintsLocal, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild);
        _AxisSize__wrap containerAxisSize = childrenAxisSize.applyConstraints(constraintsLocal, this.direction);
        size = containerAxisSize.toSize(this.direction);
        _AxisSize__wrap freeAxisSize = (containerAxisSize.op_Subtract(childrenAxisSize));
        _hasVisualOverflow = ((((_AxisSize__wrap)freeAxisSize).mainAxisExtent < 0.0) || (((_AxisSize__wrap)freeAxisSize).crossAxisExtent < 0.0));
        _positionChildren(runMetrics, freeAxisSize, containerAxisSize, (Action<Offset, RenderBox>)_setChildPosition, (Func<RenderBox, Size>)_getChildSize);
    }

    internal virtual (_AxisSize__wrap, List<_RunMetrics__wrap>) _computeRuns(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild)
    {
        DartRuntimePrimitives.Assert(() => (firstChild is not null));
        var (childConstraints, mainAxisLimit) = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (((BoxConstraints, double))((new BoxConstraints(maxWidth: ((BoxConstraints)constraints).maxWidth), ((BoxConstraints)constraints).maxWidth))), global::Doroti.Framework.Painting.Axis.vertical => (((BoxConstraints, double))((new BoxConstraints(maxHeight: ((BoxConstraints)constraints).maxHeight), ((BoxConstraints)constraints).maxHeight))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var (flipMainAxis, _) = this._areAxesFlipped;
        double spacingLocal = this.spacing;
        var runMetrics = new List<_RunMetrics__wrap>();
        _RunMetrics__wrap? currentRun = default!;
        _AxisSize__wrap childrenAxisSize = _AxisSize__wrap.empty;
        for (RenderBox? child = firstChild; (child is not null); child = childAfter(child))
        {
            var childSize = _AxisSize__wrap.CreateFromSize(size: layoutChild(child, childConstraints), direction: this.direction);
            _RunMetrics__wrap? newRun = ((currentRun is null) ? new _RunMetrics__wrap(child, childSize) : currentRun.tryAddingNewChild(child, childSize, flipMainAxis, spacingLocal, mainAxisLimit));
            if ((newRun is not null))
            {
                runMetrics.Add(newRun);
                childrenAxisSize = childrenAxisSize.op_Add((currentRun?.axisSize.flipped ?? _AxisSize__wrap.empty));
                currentRun = newRun;
            }
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(runMetrics.Count)) != 0));
        double totalRunSpacing = (this.runSpacing * ((checked((long)(runMetrics.Count)) - 1L)));
        childrenAxisSize = childrenAxisSize.op_Add((new _AxisSize__wrap(mainAxisExtent: totalRunSpacing, crossAxisExtent: 0.0).op_Add(currentRun!.axisSize.flipped)));
        return (((_AxisSize__wrap)childrenAxisSize).flipped, runMetrics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _positionChildren(List<_RunMetrics__wrap> runMetrics, _AxisSize__wrap freeAxisSize, _AxisSize__wrap containerAxisSize, Action<Offset, RenderBox> positionChild, Func<RenderBox, Size> getChildSize)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(runMetrics.Count)) != 0));
        double spacingLocal = this.spacing;
        double crossAxisFreeSpace = Math.Max(0.0, ((_AxisSize__wrap)freeAxisSize).crossAxisExtent);
        var (flipMainAxis, flipCrossAxis) = this._areAxesFlipped;
        WrapCrossAlignment effectiveCrossAlignment = (flipCrossAxis ? WrapCrossAlignmentMembers._flipped(this.crossAxisAlignment) : this.crossAxisAlignment);
        var (runLeadingSpace, runBetweenSpace) = this.runAlignment._distributeSpace(crossAxisFreeSpace, this.runSpacing, checked((long)(runMetrics.Count)), flipCrossAxis);
        Func<RenderBox, RenderBox?> nextChild = (flipMainAxis ? childBefore : childAfter);
        var runCrossAxisOffset = runLeadingSpace;
        IEnumerable<_RunMetrics__wrap> runs = (flipCrossAxis ? System.Linq.Enumerable.Reverse(runMetrics) : runMetrics);
        foreach (var run in runs)
        {
            double runCrossAxisExtent = ((_RunMetrics__wrap)run).axisSize.crossAxisExtent;
            long childCountLocal = ((_RunMetrics__wrap)run).childCount;
            double mainAxisFreeSpace = Math.Max(0.0, (((_AxisSize__wrap)containerAxisSize).mainAxisExtent - ((_RunMetrics__wrap)run).axisSize.mainAxisExtent));
            var (childLeadingSpace, childBetweenSpace) = this.alignment._distributeSpace(mainAxisFreeSpace, spacingLocal, childCountLocal, flipMainAxis);
            var childMainAxisOffset = childLeadingSpace;
            long remainingChildCount = ((_RunMetrics__wrap)run).childCount;
            for (RenderBox? child = ((_RunMetrics__wrap)run).leadingChild; ((child is not null) && (remainingChildCount > 0L)); child = nextChild(child), remainingChildCount -= 1L)
            {
                var __pattern28999 = _AxisSize__wrap.CreateFromSize(size: getChildSize(child), direction: this.direction);
                double childMainAxisExtent = __pattern28999.mainAxisExtent;
                double childCrossAxisExtent = __pattern28999.crossAxisExtent;
                double childCrossAxisOffset = (WrapCrossAlignmentMembers._alignment(effectiveCrossAlignment) * ((runCrossAxisExtent - childCrossAxisExtent)));
                positionChild(_getOffset(childMainAxisOffset, (runCrossAxisOffset + childCrossAxisOffset)), child);
                childMainAxisOffset += (childMainAxisExtent + childBetweenSpace);
            }
            runCrossAxisOffset += (runCrossAxisExtent + runBetweenSpace);
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return defaultHitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((this._hasVisualOverflow && (!object.Equals(this.clipBehavior, Clip.none))))
        {
            this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (Action<PaintingContext, Offset>)defaultPaint, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
        }
        else
        {
            this._clipRectLayer.layer = null;
            defaultPaint(context, offset);
        }
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.Axis>("direction", this.direction));
        properties.add(new EnumProperty<WrapAlignment>("alignment", this.alignment));
        properties.add(new DoubleProperty("spacing", this.spacing));
        properties.add(new EnumProperty<WrapAlignment>("runAlignment", this.runAlignment));
        properties.add(new DoubleProperty("runSpacing", this.runSpacing));
        properties.add(new DoubleProperty("crossAxisAlignment", this.runSpacing));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.VerticalDirection>("verticalDirection", this.verticalDirection, defaultValue: global::Doroti.Framework.Painting.VerticalDirection.down));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((WrapParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData = ((WrapParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((WrapParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((WrapParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
        DartRuntimePrimitives.Assert(() => (child.parentData is WrapParentData));
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
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((WrapParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((WrapParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
                var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

