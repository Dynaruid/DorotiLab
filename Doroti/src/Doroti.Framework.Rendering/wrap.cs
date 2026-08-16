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
        BoxConstraints effectiveConstraints__1530 = (direction switch { global::Doroti.Framework.Painting.Axis.horizontal => constraints, global::Doroti.Framework.Painting.Axis.vertical => ((BoxConstraints)constraints).flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        return _AxisSize__wrap.Create_(effectiveConstraints__1530.constrain(_size));
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
        bool needsNewRun__6333 = ((((((_AxisSize__wrap)this.axisSize).mainAxisExtent + ((_AxisSize__wrap)childSize).mainAxisExtent) + spacing) - maxMainExtent) > global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance);
        if (needsNewRun__6333)
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
                    var width__18508 = 0.0;
                    RenderBox? child__18540 = firstChild;
                    while ((child__18540 is not null))
                    {
                        width__18508 = Math.Max(width__18508, child__18540.getMinIntrinsicWidth(double.PositiveInfinity));
                        child__18540 = childAfter(child__18540);
                    }
                    return width__18508;
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
                    var width__18976 = 0.0;
                    RenderBox? child__19008 = firstChild;
                    while ((child__19008 is not null))
                    {
                        width__18976 += child__19008.getMaxIntrinsicWidth(double.PositiveInfinity);
                        child__19008 = childAfter(child__19008);
                    }
                    return width__18976;
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
                    var height__19523 = 0.0;
                    RenderBox? child__19556 = firstChild;
                    while ((child__19556 is not null))
                    {
                        height__19523 = Math.Max(height__19523, child__19556.getMinIntrinsicHeight(double.PositiveInfinity));
                        child__19556 = childAfter(child__19556);
                    }
                    return height__19523;
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
                    var height__19995 = 0.0;
                    RenderBox? child__20028 = firstChild;
                    while ((child__20028 is not null))
                    {
                        height__19995 += child__20028.getMaxIntrinsicHeight(double.PositiveInfinity);
                        child__20028 = childAfter(child__20028);
                    }
                    return height__19995;
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
            bool flipHorizontal__21049 = ((this.textDirection ?? TextDirection.ltr) switch { TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            bool flipVertical__21201 = (this.verticalDirection switch { global::Doroti.Framework.Painting.VerticalDirection.down => false, global::Doroti.Framework.Painting.VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (((bool, bool))((flipHorizontal__21049, flipVertical__21201))), global::Doroti.Framework.Painting.Axis.vertical => (((bool, bool))((flipVertical__21201, flipHorizontal__21049))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        if ((firstChild is null))
        {
            return null;
        }
        BoxConstraints childConstraints__21667 = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new BoxConstraints(maxWidth: ((BoxConstraints)constraints).maxWidth), global::Doroti.Framework.Painting.Axis.vertical => new BoxConstraints(maxHeight: ((BoxConstraints)constraints).maxHeight), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var (childrenAxisSize__21882, runMetrics__21918) = _computeRuns(constraints, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.dryLayoutChild);
        _AxisSize__wrap containerAxisSize__22032 = childrenAxisSize__21882.applyConstraints(constraints, this.direction);
        BaselineOffset baselineOffset__22131 = BaselineOffset.noBaseline;
        void findHighestBaseline(Offset offset, RenderBox child)
        {
            baselineOffset__22131 = baselineOffset__22131.minOf((new BaselineOffset(child.getDryBaseline(childConstraints__21667, baseline)).op_Add(offset.dy)));
        }
        Size getChildSize(RenderBox child)
        {
            return child.getDryLayout(childConstraints__21667);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        _positionChildren(runMetrics__21918, childrenAxisSize__21882, containerAxisSize__22032, (Action<Offset, RenderBox>)findHighestBaseline, (Func<RenderBox, Size>)getChildSize);
        return baselineOffset__22131.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _computeDryLayout(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild = default!)
    {
        var (childConstraints__22944, mainAxisLimit__22969) = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (((BoxConstraints, double))((new BoxConstraints(maxWidth: ((BoxConstraints)constraints).maxWidth), ((BoxConstraints)constraints).maxWidth))), global::Doroti.Framework.Painting.Axis.vertical => (((BoxConstraints, double))((new BoxConstraints(maxHeight: ((BoxConstraints)constraints).maxHeight), ((BoxConstraints)constraints).maxHeight))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var mainAxisExtent__23218 = 0.0;
        var crossAxisExtent__23248 = 0.0;
        var runMainAxisExtent__23279 = 0.0;
        var runCrossAxisExtent__23312 = 0.0;
        var childCount__23346 = 0L;
        RenderBox? child__23377 = firstChild;
        while ((child__23377 is not null))
        {
            global::Doroti.Ui.Size childSize__23442 = layoutChild(child__23377, childConstraints__22944);
            double childMainAxisExtent__23511 = _getMainAxisExtent(childSize__23442);
            double childCrossAxisExtent__23583 = _getCrossAxisExtent(childSize__23442);
            if (((childCount__23346 > 0L) && (((runMainAxisExtent__23279 + childMainAxisExtent__23511) + this.spacing) > mainAxisLimit__22969)))
            {
                mainAxisExtent__23218 = Math.Max(mainAxisExtent__23218, runMainAxisExtent__23279);
                crossAxisExtent__23248 += (runCrossAxisExtent__23312 + this.runSpacing);
                runMainAxisExtent__23279 = 0.0;
                runCrossAxisExtent__23312 = 0.0;
                childCount__23346 = 0L;
            }
            runMainAxisExtent__23279 += childMainAxisExtent__23511;
            runCrossAxisExtent__23312 = Math.Max(runCrossAxisExtent__23312, childCrossAxisExtent__23583);
            if ((childCount__23346 > 0L))
            {
                runMainAxisExtent__23279 += this.spacing;
            }
            childCount__23346 += 1L;
            child__23377 = childAfter(child__23377);
        }
        crossAxisExtent__23248 += runCrossAxisExtent__23312;
        mainAxisExtent__23218 = Math.Max(mainAxisExtent__23218, runMainAxisExtent__23279);
        return constraints.constrain((this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => new global::Doroti.Ui.Size(mainAxisExtent__23218, crossAxisExtent__23248), global::Doroti.Framework.Painting.Axis.vertical => new global::Doroti.Ui.Size(crossAxisExtent__23248, mainAxisExtent__23218), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Size _getChildSize(RenderBox child) => ((RenderBox)child).size;
    internal static void _setChildPosition(Offset offset, RenderBox child)
    {
        (((WrapParentData?)(object?)child.parentData!)!).offset = offset;
    }

    public override void performLayout()
    {
        BoxConstraints constraints__24895 = this.constraints;
        DartRuntimePrimitives.Assert(() => this._debugHasNecessaryDirections);
        if ((firstChild is null))
        {
            size = ((BoxConstraints)constraints__24895).smallest;
            _hasVisualOverflow = false;
            return;
        }
        var (childrenAxisSize__25110, runMetrics__25146) = _computeRuns(constraints__24895, (Func<RenderBox, BoxConstraints, Size>)ChildLayoutHelper.layoutChild);
        _AxisSize__wrap containerAxisSize__25257 = childrenAxisSize__25110.applyConstraints(constraints__24895, this.direction);
        size = containerAxisSize__25257.toSize(this.direction);
        _AxisSize__wrap freeAxisSize__25404 = (containerAxisSize__25257.op_Subtract(childrenAxisSize__25110));
        _hasVisualOverflow = ((((_AxisSize__wrap)freeAxisSize__25404).mainAxisExtent < 0.0) || (((_AxisSize__wrap)freeAxisSize__25404).crossAxisExtent < 0.0));
        _positionChildren(runMetrics__25146, freeAxisSize__25404, containerAxisSize__25257, (Action<Offset, RenderBox>)_setChildPosition, (Func<RenderBox, Size>)_getChildSize);
    }

    internal virtual (_AxisSize__wrap, List<_RunMetrics__wrap>) _computeRuns(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild)
    {
        DartRuntimePrimitives.Assert(() => (firstChild is not null));
        var (childConstraints__25897, mainAxisLimit__25922) = (this.direction switch { global::Doroti.Framework.Painting.Axis.horizontal => (((BoxConstraints, double))((new BoxConstraints(maxWidth: ((BoxConstraints)constraints).maxWidth), ((BoxConstraints)constraints).maxWidth))), global::Doroti.Framework.Painting.Axis.vertical => (((BoxConstraints, double))((new BoxConstraints(maxHeight: ((BoxConstraints)constraints).maxHeight), ((BoxConstraints)constraints).maxHeight))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var (flipMainAxis__26179, _) = this._areAxesFlipped;
        double spacing__26232 = this.spacing;
        var runMetrics__26266 = new List<_RunMetrics__wrap>();
        _RunMetrics__wrap? currentRun__26314 = default!;
        _AxisSize__wrap childrenAxisSize__26340 = _AxisSize__wrap.empty;
        for (RenderBox? child__26396 = firstChild; (child__26396 is not null); child__26396 = childAfter(child__26396))
        {
            var childSize__26472 = _AxisSize__wrap.CreateFromSize(size: layoutChild(child__26396, childConstraints__25897), direction: this.direction);
            _RunMetrics__wrap? newRun__26620 = ((currentRun__26314 is null) ? new _RunMetrics__wrap(child__26396, childSize__26472) : currentRun__26314.tryAddingNewChild(child__26396, childSize__26472, flipMainAxis__26179, spacing__26232, mainAxisLimit__25922));
            if ((newRun__26620 is not null))
            {
                runMetrics__26266.Add(newRun__26620);
                childrenAxisSize__26340 = childrenAxisSize__26340.op_Add((currentRun__26314?.axisSize.flipped ?? _AxisSize__wrap.empty));
                currentRun__26314 = newRun__26620;
            }
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(runMetrics__26266.Count)) != 0));
        double totalRunSpacing__27020 = (this.runSpacing * ((checked((long)(runMetrics__26266.Count)) - 1L)));
        childrenAxisSize__26340 = childrenAxisSize__26340.op_Add((new _AxisSize__wrap(mainAxisExtent: totalRunSpacing__27020, crossAxisExtent: 0.0).op_Add(currentRun__26314!.axisSize.flipped)));
        return (((_AxisSize__wrap)childrenAxisSize__26340).flipped, runMetrics__26266);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _positionChildren(List<_RunMetrics__wrap> runMetrics, _AxisSize__wrap freeAxisSize, _AxisSize__wrap containerAxisSize, Action<Offset, RenderBox> positionChild, Func<RenderBox, Size> getChildSize)
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(runMetrics.Count)) != 0));
        double spacing__27515 = this.spacing;
        double crossAxisFreeSpace__27557 = Math.Max(0.0, ((_AxisSize__wrap)freeAxisSize).crossAxisExtent);
        var (flipMainAxis__27640, flipCrossAxis__27659) = this._areAxesFlipped;
        WrapCrossAlignment effectiveCrossAlignment__27722 = (flipCrossAxis__27659 ? WrapCrossAlignmentMembers._flipped(this.crossAxisAlignment) : this.crossAxisAlignment);
        var (runLeadingSpace__27848, runBetweenSpace__27872) = this.runAlignment._distributeSpace(crossAxisFreeSpace__27557, this.runSpacing, checked((long)(runMetrics.Count)), flipCrossAxis__27659);
        Func<RenderBox, RenderBox?> nextChild__28040 = (flipMainAxis__27640 ? childBefore : childAfter);
        var runCrossAxisOffset__28102 = runLeadingSpace__27848;
        IEnumerable<_RunMetrics__wrap> runs__28172 = (flipCrossAxis__27659 ? System.Linq.Enumerable.Reverse(runMetrics) : runMetrics);
        foreach (var run__28244 in runs__28172)
        {
            double runCrossAxisExtent__28278 = ((_RunMetrics__wrap)run__28244).axisSize.crossAxisExtent;
            long childCount__28345 = ((_RunMetrics__wrap)run__28244).childCount;
            double mainAxisFreeSpace__28394 = Math.Max(0.0, (((_AxisSize__wrap)containerAxisSize).mainAxisExtent - ((_RunMetrics__wrap)run__28244).axisSize.mainAxisExtent));
            var (childLeadingSpace__28538, childBetweenSpace__28564) = this.alignment._distributeSpace(mainAxisFreeSpace__28394, spacing__27515, childCount__28345, flipMainAxis__27640);
            var childMainAxisOffset__28719 = childLeadingSpace__28538;
            long remainingChildCount__28771 = ((_RunMetrics__wrap)run__28244).childCount;
            for (RenderBox? child__28840 = ((_RunMetrics__wrap)run__28244).leadingChild; ((child__28840 is not null) && (remainingChildCount__28771 > 0L)); child__28840 = nextChild__28040(child__28840), remainingChildCount__28771 -= 1L)
            {
                var __pattern28999 = _AxisSize__wrap.CreateFromSize(size: getChildSize(child__28840), direction: this.direction);
                double childMainAxisExtent__29043 = __pattern28999.mainAxisExtent;
                double childCrossAxisExtent__29098 = __pattern28999.crossAxisExtent;
                double childCrossAxisOffset__29253 = (WrapCrossAlignmentMembers._alignment(effectiveCrossAlignment__27722) * ((runCrossAxisExtent__28278 - childCrossAxisExtent__29098)));
                positionChild(_getOffset(childMainAxisOffset__28719, (runCrossAxisOffset__28102 + childCrossAxisOffset__29253)), child__28840);
                childMainAxisOffset__28719 += (childMainAxisExtent__29043 + childBetweenSpace__28564);
            }
            runCrossAxisOffset__28102 += (runCrossAxisExtent__28278 + runBetweenSpace__27872);
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
        var childParentData__173585 = ((WrapParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((WrapParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((WrapParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((WrapParentData?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((WrapParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((WrapParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((WrapParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((WrapParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((WrapParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
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
        var childParentData__179226 = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((WrapParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((WrapParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
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
            var childParentData__180684 = ((WrapParentData?)(object?)child__180623.parentData!)!;
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
        var childParentData__181479 = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData__181891 = ((WrapParentData?)(object?)child__181803.parentData!)!;
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
            var childParentData__182148 = ((WrapParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((WrapParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((WrapParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((WrapParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((WrapParentData?)(object?)child.parentData!)!;
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
                var childParentData__183833 = ((WrapParentData?)(object?)child__183606.parentData!)!;
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
            var childParentData__138777 = ((WrapParentData?)(object?)child__138717.parentData!)!;
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
            var childParentData__139488 = ((WrapParentData?)(object?)child__139428.parentData!)!;
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
            var childParentData__140418 = ((WrapParentData?)(object?)child__140279.parentData!)!;
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
            var childParentData__141300 = ((WrapParentData?)(object?)child__141240.parentData!)!;
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
            var childParentData__141892 = ((WrapParentData?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

