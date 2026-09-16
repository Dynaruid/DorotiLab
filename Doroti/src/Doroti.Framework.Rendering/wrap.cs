// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/wrap.dart
using Doroti.Runtime;
using Doroti.Ui;

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

    public static _AxisSize__wrap empty = Create_(Size.zero);

    internal _AxisSize__wrap(double mainAxisExtent, double crossAxisExtent)
    {
        _size = new global::Doroti.Ui.Size(mainAxisExtent, crossAxisExtent);
    }

    internal static _AxisSize__wrap CreateFromSize(Size size, global::Doroti.Framework.Painting.Axis direction)
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
    public virtual _AxisSize__wrap applyConstraints(BoxConstraints constraints, global::Doroti.Framework.Painting.Axis direction)
    {
        BoxConstraints effectiveConstraints = direction switch { Axis.horizontal => constraints, Axis.vertical => constraints.flipped, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        return Create_(effectiveConstraints.constrain(_size));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _AxisSize__wrap flipped => Create_(_size.flipped);
    public virtual _AxisSize__wrap op_Add(_AxisSize__wrap other) => Create_(new global::Doroti.Ui.Size(_size.width + other._size.width, Math.Max(_size.height, other._size.height)));
    public virtual _AxisSize__wrap op_Subtract(_AxisSize__wrap other) => Create_(new global::Doroti.Ui.Size(_size.width - other._size.width, _size.height - other._size.height));
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
        DartRuntimePrimitives.Assert(() => itemCount > 0L);
        return value switch { WrapAlignment.start => (flipped ? freeSpace : 0.0, itemSpacing), WrapAlignment.end => WrapAlignment.start._distributeSpace(freeSpace, itemSpacing, itemCount, !flipped), WrapAlignment.spaceBetween when itemCount < 2L => WrapAlignment.start._distributeSpace(freeSpace, itemSpacing, itemCount, flipped), WrapAlignment.center => (freeSpace / 2.0, itemSpacing), WrapAlignment.spaceBetween => (0, (freeSpace / (itemCount - 1L)) + itemSpacing), WrapAlignment.spaceAround => (freeSpace / itemCount / 2L, (freeSpace / itemCount) + itemSpacing), WrapAlignment.spaceEvenly => (freeSpace / (itemCount + 1L), (freeSpace / (itemCount + 1L)) + itemSpacing), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
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
    internal static WrapCrossAlignment _flipped(this WrapCrossAlignment value) => value switch { WrapCrossAlignment.start => WrapCrossAlignment.end, WrapCrossAlignment.end => WrapCrossAlignment.start, WrapCrossAlignment.center => WrapCrossAlignment.center, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    internal static double _alignment(this WrapCrossAlignment value) => value switch { WrapCrossAlignment.start => 0, WrapCrossAlignment.end => 1, WrapCrossAlignment.center => 0.5, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
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
        bool needsNewRun = (axisSize.mainAxisExtent + childSize.mainAxisExtent + spacing - maxMainExtent) > Foundation.ConstantsLibrary.precisionErrorTolerance;
        if (needsNewRun)
        {
            return new _RunMetrics__wrap(child, childSize);
        }
        else
        {
            axisSize = axisSize.op_Add(childSize.op_Add(new _AxisSize__wrap(mainAxisExtent: spacing, crossAxisExtent: 0.0)));
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
        _direction = direction;
        _alignment = alignment;
        _spacing = spacing;
        _runAlignment = runAlignment;
        _runSpacing = runSpacing;
        _crossAxisAlignment = crossAxisAlignment;
        _textDirection = textDirection;
        _verticalDirection = verticalDirection;
        _clipBehavior = clipBehavior;
    }

    public virtual global::Doroti.Framework.Painting.Axis direction
    {
        get => _direction;
        set
        {
            var __value = value;
            if (Equals(_direction, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _direction = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual WrapAlignment alignment
    {
        get => _alignment;
        set
        {
            var __value = value;
            if (Equals(_alignment, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _alignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
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
    public virtual WrapAlignment runAlignment
    {
        get => _runAlignment;
        set
        {
            var __value = value;
            if (Equals(_runAlignment, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _runAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual double runSpacing
    {
        get => _runSpacing;
        set
        {
            var __value = value;
            if (_runSpacing == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _runSpacing = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual WrapCrossAlignment crossAxisAlignment
    {
        get => _crossAxisAlignment;
        set
        {
            var __value = value;
            if (Equals(_crossAxisAlignment, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _crossAxisAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
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
    internal virtual bool _debugHasNecessaryDirections
    {
        get
        {
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
            if (Equals(alignment, WrapAlignment.start) || Equals(alignment, WrapAlignment.end))
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
            if (Equals(runAlignment, WrapAlignment.start) || Equals(runAlignment, WrapAlignment.end))
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
            if (Equals(crossAxisAlignment, WrapCrossAlignment.start) || Equals(crossAxisAlignment, WrapCrossAlignment.end))
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
    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        if (__child.parentData is not WrapParentData)
        {
            __child.parentData = new WrapParentData();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        switch (direction)
        {
            case Axis.horizontal:
                {
                    var widthLocal = 0.0;
                    RenderBox? child = firstChild;
                    while (child is not null)
                    {
                        widthLocal = Math.Max(widthLocal, child.getMinIntrinsicWidth(double.PositiveInfinity));
                        child = childAfter(child);
                    }
                    return widthLocal;
                }
            case Axis.vertical:
                {
                    return getDryLayout(new BoxConstraints(maxHeight: height)).width;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        switch (direction)
        {
            case Axis.horizontal:
                {
                    var widthLocal = 0.0;
                    RenderBox? child = firstChild;
                    while (child is not null)
                    {
                        widthLocal += child.getMaxIntrinsicWidth(double.PositiveInfinity);
                        child = childAfter(child);
                    }
                    return widthLocal;
                }
            case Axis.vertical:
                {
                    return getDryLayout(new BoxConstraints(maxHeight: height)).width;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        switch (direction)
        {
            case Axis.horizontal:
                {
                    return getDryLayout(new BoxConstraints(maxWidth: width)).height;
                }
            case Axis.vertical:
                {
                    var heightLocal = 0.0;
                    RenderBox? child = firstChild;
                    while (child is not null)
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
        switch (direction)
        {
            case Axis.horizontal:
                {
                    return getDryLayout(new BoxConstraints(maxWidth: width)).height;
                }
            case Axis.vertical:
                {
                    var heightLocal = 0.0;
                    RenderBox? child = firstChild;
                    while (child is not null)
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
        return direction switch { Axis.horizontal => childSize.width, Axis.vertical => childSize.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getCrossAxisExtent(Size childSize)
    {
        return direction switch { Axis.horizontal => childSize.height, Axis.vertical => childSize.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Offset _getOffset(double mainAxisOffset, double crossAxisOffset)
    {
        return direction switch { Axis.horizontal => new global::Doroti.Ui.Offset(mainAxisOffset, crossAxisOffset), Axis.vertical => new global::Doroti.Ui.Offset(crossAxisOffset, mainAxisOffset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (bool, bool) _areAxesFlipped
    {
        get
        {
            bool flipHorizontal = (textDirection ?? TextDirection.ltr) switch { TextDirection.ltr => false, TextDirection.rtl => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            bool flipVertical = verticalDirection switch { VerticalDirection.down => false, VerticalDirection.up => true, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            return direction switch { Axis.horizontal => (flipHorizontal, flipVertical), Axis.vertical => (flipVertical, flipHorizontal), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        if (firstChild is null)
        {
            return null;
        }
        BoxConstraints childConstraints = direction switch { Axis.horizontal => new BoxConstraints(maxWidth: constraints.maxWidth), Axis.vertical => new BoxConstraints(maxHeight: constraints.maxHeight), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var (childrenAxisSize, runMetrics) = _computeRuns(constraints, ChildLayoutHelper.dryLayoutChild);
        _AxisSize__wrap containerAxisSize = childrenAxisSize.applyConstraints(constraints, direction);
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        void findHighestBaseline(Offset offset, RenderBox child)
        {
            baselineOffset = baselineOffset.minOf(new BaselineOffset(child.getDryBaseline(childConstraints, baseline)).op_Add(offset.dy));
        }
        Size getChildSize(RenderBox child)
        {
            return child.getDryLayout(childConstraints);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        _positionChildren(runMetrics, childrenAxisSize, containerAxisSize, findHighestBaseline, getChildSize);
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeDryLayout(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    // Dart library-private member: distinct from the same name in the base library.
    internal new virtual global::Doroti.Ui.Size _computeDryLayout(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild = default!)
    {
        var (childConstraints, mainAxisLimit) = direction switch { Axis.horizontal => ((BoxConstraints, double))(new BoxConstraints(maxWidth: constraints.maxWidth), constraints.maxWidth), Axis.vertical => ((BoxConstraints, double))(new BoxConstraints(maxHeight: constraints.maxHeight), constraints.maxHeight), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var mainAxisExtent = 0.0;
        var crossAxisExtent = 0.0;
        var runMainAxisExtent = 0.0;
        var runCrossAxisExtent = 0.0;
        var childCount = 0L;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            global::Doroti.Ui.Size childSize = layoutChild(child, childConstraints);
            double childMainAxisExtent = _getMainAxisExtent(childSize);
            double childCrossAxisExtent = _getCrossAxisExtent(childSize);
            if ((childCount > 0L) && ((runMainAxisExtent + childMainAxisExtent + spacing) > mainAxisLimit))
            {
                mainAxisExtent = Math.Max(mainAxisExtent, runMainAxisExtent);
                crossAxisExtent += runCrossAxisExtent + runSpacing;
                runMainAxisExtent = 0.0;
                runCrossAxisExtent = 0.0;
                childCount = 0L;
            }
            runMainAxisExtent += childMainAxisExtent;
            runCrossAxisExtent = Math.Max(runCrossAxisExtent, childCrossAxisExtent);
            if (childCount > 0L)
            {
                runMainAxisExtent += spacing;
            }
            childCount += 1L;
            child = childAfter(child);
        }
        crossAxisExtent += runCrossAxisExtent;
        mainAxisExtent = Math.Max(mainAxisExtent, runMainAxisExtent);
        return constraints.constrain(direction switch { Axis.horizontal => new global::Doroti.Ui.Size(mainAxisExtent, crossAxisExtent), Axis.vertical => new global::Doroti.Ui.Size(crossAxisExtent, mainAxisExtent), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Size _getChildSize(RenderBox child) => child.size;
    internal static void _setChildPosition(Offset offset, RenderBox child)
    {
        ((WrapParentData?)(object?)child.parentData!)!.offset = offset;
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        DartRuntimePrimitives.Assert(() => _debugHasNecessaryDirections);
        if (firstChild is null)
        {
            size = constraintsLocal.smallest;
            _hasVisualOverflow = false;
            return;
        }
        var (childrenAxisSize, runMetrics) = _computeRuns(constraintsLocal, ChildLayoutHelper.layoutChild);
        _AxisSize__wrap containerAxisSize = childrenAxisSize.applyConstraints(constraintsLocal, direction);
        size = containerAxisSize.toSize(direction);
        _AxisSize__wrap freeAxisSize = containerAxisSize.op_Subtract(childrenAxisSize);
        _hasVisualOverflow = (freeAxisSize.mainAxisExtent < 0.0) || (freeAxisSize.crossAxisExtent < 0.0);
        _positionChildren(runMetrics, freeAxisSize, containerAxisSize, _setChildPosition, _getChildSize);
    }

    internal virtual (_AxisSize__wrap, List<_RunMetrics__wrap>) _computeRuns(BoxConstraints constraints, Func<RenderBox, BoxConstraints, Size> layoutChild)
    {
        DartRuntimePrimitives.Assert(() => firstChild is not null);
        var (childConstraints, mainAxisLimit) = direction switch { Axis.horizontal => ((BoxConstraints, double))(new BoxConstraints(maxWidth: constraints.maxWidth), constraints.maxWidth), Axis.vertical => ((BoxConstraints, double))(new BoxConstraints(maxHeight: constraints.maxHeight), constraints.maxHeight), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var (flipMainAxis, _) = _areAxesFlipped;
        double spacingLocal = spacing;
        var runMetrics = new List<_RunMetrics__wrap>();
        _RunMetrics__wrap? currentRun = default!;
        _AxisSize__wrap childrenAxisSize = _AxisSize__wrap.empty;
        for (RenderBox? child = firstChild; child is not null; child = childAfter(child))
        {
            var childSize = _AxisSize__wrap.CreateFromSize(size: layoutChild(child, childConstraints), direction: direction);
            _RunMetrics__wrap? newRun = (currentRun is null) ? new _RunMetrics__wrap(child, childSize) : currentRun.tryAddingNewChild(child, childSize, flipMainAxis, spacingLocal, mainAxisLimit);
            if (newRun is not null)
            {
                runMetrics.Add(newRun);
                childrenAxisSize = childrenAxisSize.op_Add(currentRun?.axisSize.flipped ?? _AxisSize__wrap.empty);
                currentRun = newRun;
            }
        }
        DartRuntimePrimitives.Assert(() => checked((long)runMetrics.Count) != 0);
        double totalRunSpacing = runSpacing * (checked(runMetrics.Count) - 1L);
        childrenAxisSize = childrenAxisSize.op_Add(new _AxisSize__wrap(mainAxisExtent: totalRunSpacing, crossAxisExtent: 0.0).op_Add(currentRun!.axisSize.flipped));
        return (childrenAxisSize.flipped, runMetrics);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _positionChildren(List<_RunMetrics__wrap> runMetrics, _AxisSize__wrap freeAxisSize, _AxisSize__wrap containerAxisSize, Action<Offset, RenderBox> positionChild, Func<RenderBox, Size> getChildSize)
    {
        DartRuntimePrimitives.Assert(() => checked((long)runMetrics.Count) != 0);
        double spacingLocal = spacing;
        double crossAxisFreeSpace = Math.Max(0.0, freeAxisSize.crossAxisExtent);
        var (flipMainAxis, flipCrossAxis) = _areAxesFlipped;
        WrapCrossAlignment effectiveCrossAlignment = flipCrossAxis ? WrapCrossAlignmentMembers._flipped(crossAxisAlignment) : crossAxisAlignment;
        var (runLeadingSpace, runBetweenSpace) = runAlignment._distributeSpace(crossAxisFreeSpace, runSpacing, checked(runMetrics.Count), flipCrossAxis);
        Func<RenderBox, RenderBox?> nextChild = flipMainAxis ? childBefore : childAfter;
        var runCrossAxisOffset = runLeadingSpace;
        IEnumerable<_RunMetrics__wrap> runs = flipCrossAxis ? Enumerable.Reverse(runMetrics) : runMetrics;
        foreach (var run in runs)
        {
            double runCrossAxisExtent = run.axisSize.crossAxisExtent;
            long childCountLocal = run.childCount;
            double mainAxisFreeSpace = Math.Max(0.0, containerAxisSize.mainAxisExtent - run.axisSize.mainAxisExtent);
            var (childLeadingSpace, childBetweenSpace) = alignment._distributeSpace(mainAxisFreeSpace, spacingLocal, childCountLocal, flipMainAxis);
            var childMainAxisOffset = childLeadingSpace;
            long remainingChildCount = run.childCount;
            for (RenderBox? child = run.leadingChild; (child is not null) && (remainingChildCount > 0L); child = nextChild(child), remainingChildCount -= 1L)
            {
                var __pattern28999 = _AxisSize__wrap.CreateFromSize(size: getChildSize(child), direction: direction);
                double childMainAxisExtent = __pattern28999.mainAxisExtent;
                double childCrossAxisExtent = __pattern28999.crossAxisExtent;
                double childCrossAxisOffset = WrapCrossAlignmentMembers._alignment(effectiveCrossAlignment) * (runCrossAxisExtent - childCrossAxisExtent);
                positionChild(_getOffset(childMainAxisOffset, runCrossAxisOffset + childCrossAxisOffset), child);
                childMainAxisOffset += childMainAxisExtent + childBetweenSpace;
            }
            runCrossAxisOffset += runCrossAxisExtent + runBetweenSpace;
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return defaultHitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (_hasVisualOverflow && (!Equals(clipBehavior, Clip.none)))
        {
            _clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, Offset.zero & size, defaultPaint, clipBehavior: clipBehavior, oldLayer: _clipRectLayer.layer);
        }
        else
        {
            _clipRectLayer.layer = null;
            defaultPaint(context, offset);
        }
    }

    public override void dispose()
    {
        _clipRectLayer.layer = null;
        base.dispose();
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.Axis>("direction", direction));
        properties.add(new EnumProperty<WrapAlignment>("alignment", alignment));
        properties.add(new DoubleProperty("spacing", spacing));
        properties.add(new EnumProperty<WrapAlignment>("runAlignment", runAlignment));
        properties.add(new DoubleProperty("runSpacing", runSpacing));
        properties.add(new DoubleProperty("crossAxisAlignment", runSpacing));
        properties.add(new EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection, defaultValue: null));
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.VerticalDirection>("verticalDirection", verticalDirection, defaultValue: VerticalDirection.down));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((WrapParentData?)(object?)_firstChild!.parentData!)!;
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
            var afterParentData = ((WrapParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((WrapParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((WrapParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
        DartRuntimePrimitives.Assert(() => child.parentData is WrapParentData);
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
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((WrapParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((WrapParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
        while (child is not null)
        {
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((WrapParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

