// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/overflow_bar.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum OverflowBarAlignment
{
    start,
    end,
    center
}

public class OverflowBar : MultiChildRenderObjectWidget
{
    public virtual double spacing { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.MainAxisAlignment? alignment { get; private set; }
    public virtual double overflowSpacing { get; private set; } = default!;
    public virtual OverflowBarAlignment overflowAlignment { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.VerticalDirection overflowDirection { get; private set; } = default!;
    public virtual TextDirection? textDirection { get; private set; }

    public OverflowBar(global::Doroti.Framework.Foundation.Key? key = null, double spacing = 0.0, global::Doroti.Framework.Rendering.MainAxisAlignment? alignment = null, double overflowSpacing = 0.0, OverflowBarAlignment overflowAlignment = OverflowBarAlignment.start, global::Doroti.Framework.Painting.VerticalDirection overflowDirection = VerticalDirection.down, TextDirection? textDirection = null, List<Widget> children = default!) : base(key: key, children: children ?? new List<Widget>())
    {
        this.spacing = spacing;
        this.alignment = alignment;
        this.overflowSpacing = overflowSpacing;
        this.overflowAlignment = overflowAlignment;
        this.overflowDirection = overflowDirection;
        this.textDirection = textDirection;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderOverflowBar__overflow_bar(spacing: spacing, alignment: alignment, overflowSpacing: overflowSpacing, overflowAlignment: overflowAlignment, overflowDirection: overflowDirection, textDirection: textDirection ?? Directionality.of(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        DartRuntimePrimitives.Ignore(((Func<_RenderOverflowBar__overflow_bar>)(() =>
{
    var __cascade = ((_RenderOverflowBar__overflow_bar?)renderObject)!;
    __cascade.spacing = spacing;
    __cascade.alignment = alignment;
    __cascade.overflowSpacing = overflowSpacing;
    __cascade.overflowAlignment = overflowAlignment;
    __cascade.overflowDirection = overflowDirection;
    __cascade.textDirection = textDirection ?? Directionality.of(context);
    return __cascade;
}))());
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("spacing", spacing, defaultValue: 0L));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Rendering.MainAxisAlignment>("alignment", alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("overflowSpacing", overflowSpacing, defaultValue: 0L));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<OverflowBarAlignment>("overflowAlignment", overflowAlignment, defaultValue: OverflowBarAlignment.start));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.VerticalDirection>("overflowDirection", overflowDirection, defaultValue: VerticalDirection.down));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection, defaultValue: null));
    }

}

internal class _OverflowBarParentData__overflow_bar : global::Doroti.Framework.Rendering.ContainerBoxParentData<global::Doroti.Framework.Rendering.RenderBox>
{
}

internal class _RenderOverflowBar__overflow_bar : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.ContainerRenderObjectMixin<global::Doroti.Framework.Rendering.RenderBox, _OverflowBarParentData__overflow_bar>, global::Doroti.Framework.Rendering.RenderBoxContainerDefaultsMixin<global::Doroti.Framework.Rendering.RenderBox, _OverflowBarParentData__overflow_bar>
{
    internal virtual double _spacing { get; set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.MainAxisAlignment? _alignment { get; set; } = default;
    internal virtual double _overflowSpacing { get; set; } = default!;
    internal virtual OverflowBarAlignment _overflowAlignment { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.VerticalDirection _overflowDirection { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderOverflowBar__overflow_bar(List<global::Doroti.Framework.Rendering.RenderBox>? children = null, double spacing = 0.0, global::Doroti.Framework.Rendering.MainAxisAlignment? alignment = null, double overflowSpacing = 0.0, OverflowBarAlignment overflowAlignment = OverflowBarAlignment.start, global::Doroti.Framework.Painting.VerticalDirection overflowDirection = VerticalDirection.down, TextDirection textDirection = default!)
    {
        _spacing = spacing;
        _alignment = alignment;
        _overflowSpacing = overflowSpacing;
        _overflowAlignment = overflowAlignment;
        _overflowDirection = overflowDirection;
        _textDirection = textDirection;
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
    public virtual global::Doroti.Framework.Rendering.MainAxisAlignment? alignment
    {
        get => _alignment;
        set
        {
            var __value = value;
            if (Equals(_alignment, __value))
            {
                return;
            }
            _alignment = __value;
            markNeedsLayout();
        }
    }
    public virtual double overflowSpacing
    {
        get => _overflowSpacing;
        set
        {
            var __value = value;
            if (_overflowSpacing == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _overflowSpacing = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual OverflowBarAlignment overflowAlignment
    {
        get => _overflowAlignment;
        set
        {
            var __value = value;
            if (Equals(_overflowAlignment, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _overflowAlignment = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.VerticalDirection overflowDirection
    {
        get => _overflowDirection;
        set
        {
            var __value = value;
            if (Equals(_overflowDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _overflowDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }
    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        if (__child.parentData is not _OverflowBarParentData__overflow_bar)
        {
            __child.parentData = new _OverflowBarParentData__overflow_bar();
        }
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        if (child is null)
        {
            return 0;
        }
        var barWidth = 0.0;
        while (child is not null)
        {
            barWidth += child.getMinIntrinsicWidth(double.PositiveInfinity);
            child = childAfter(child);
        }
        barWidth += spacing * (childCount - 1L);
        var height = 0.0;
        if (barWidth > width)
        {
            child = firstChild;
            while (child is not null)
            {
                height += child.getMinIntrinsicHeight(width);
                child = childAfter(child);
            }
            return height + (overflowSpacing * (childCount - 1L));
        }
        else
        {
            child = firstChild;
            while (child is not null)
            {
                height = Math.Max(height, child.getMinIntrinsicHeight(width));
                child = childAfter(child);
            }
            return height;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        if (child is null)
        {
            return 0;
        }
        var barWidth = 0.0;
        while (child is not null)
        {
            barWidth += child.getMinIntrinsicWidth(double.PositiveInfinity);
            child = childAfter(child);
        }
        barWidth += spacing * (childCount - 1L);
        var height = 0.0;
        if (barWidth > width)
        {
            child = firstChild;
            while (child is not null)
            {
                height += child.getMaxIntrinsicHeight(width);
                child = childAfter(child);
            }
            return height + (overflowSpacing * (childCount - 1L));
        }
        else
        {
            child = firstChild;
            while (child is not null)
            {
                height = Math.Max(height, child.getMaxIntrinsicHeight(width));
                child = childAfter(child);
            }
            return height;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        if (child is null)
        {
            return 0;
        }
        var width = 0.0;
        while (child is not null)
        {
            width += child.getMinIntrinsicWidth(double.PositiveInfinity);
            child = childAfter(child);
        }
        return width + (spacing * (childCount - 1L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        if (child is null)
        {
            return 0;
        }
        var width = 0.0;
        while (child is not null)
        {
            width += child.getMaxIntrinsicWidth(double.PositiveInfinity);
            child = childAfter(child);
        }
        return width + (spacing * (childCount - 1L));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToHighestActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = constraints.loosen();
        var (next, startChild) = overflowDirection switch { VerticalDirection.down => ((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?>, global::Doroti.Framework.Rendering.RenderBox?))(childAfter, firstChild), VerticalDirection.up => ((global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderBox?>, global::Doroti.Framework.Rendering.RenderBox?))(childBefore, lastChild), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var maxChildHeight = 0.0;
        var y = 0.0;
        var childrenWidth = 0.0;
        global::Doroti.Framework.Rendering.BaselineOffset minHorizontalBaseline = BaselineOffset.noBaseline;
        global::Doroti.Framework.Rendering.BaselineOffset verticalBaseline = BaselineOffset.noBaseline;
        for (var child = startChild; child is not null; child = next(child))
        {
            global::Doroti.Ui.Size childSize = child.getDryLayout(childConstraints);
            double heightDiff = childSize.height - maxChildHeight;
            if (heightDiff > 0L)
            {
                minHorizontalBaseline = minHorizontalBaseline.op_Add(heightDiff / 2L);
                maxChildHeight = childSize.height;
            }
            var baselineOffset = new global::Doroti.Framework.Rendering.BaselineOffset(child.getDryBaseline(childConstraints, baseline));
            if (baselineOffset is not null)
            {
                verticalBaseline ??= (baselineOffset.op_Add(y));
                minHorizontalBaseline = minHorizontalBaseline.minOf(baselineOffset.op_Add(maxChildHeight - childSize.height));
            }
            y += childSize.height + overflowSpacing;
            childrenWidth += childSize.width;
        }
        DartRuntimePrimitives.Assert(() => verticalBaseline is null == minHorizontalBaseline is null);
        return ((childrenWidth + (spacing * (childCount - 1L))) > constraints.maxWidth) ? verticalBaseline.offset : minHorizontalBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        if (child is null)
        {
            return constraints.smallest;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = constraints.loosen();
        var childrenWidth = 0.0;
        var maxChildHeight = 0.0;
        var y = 0.0;
        while (child is not null)
        {
            global::Doroti.Ui.Size childSize = child.getDryLayout(childConstraints);
            childrenWidth += childSize.width;
            maxChildHeight = Math.Max(maxChildHeight, childSize.height);
            y += childSize.height + overflowSpacing;
            child = childAfter(child);
        }
        double actualWidth = childrenWidth + (spacing * (childCount - 1L));
        if (actualWidth > constraints.maxWidth)
        {
            return constraints.constrain(new global::Doroti.Ui.Size(constraints.maxWidth, y - overflowSpacing));
        }
        else
        {
            double overallWidth = (alignment is null) ? actualWidth : constraints.maxWidth;
            return constraints.constrain(new global::Doroti.Ui.Size(overallWidth, maxChildHeight));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.RenderBox? child = firstChild;
        if (child is null)
        {
            size = constraints.smallest;
            return;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = constraints.loosen();
        double childrenWidth = 0;
        double maxChildHeight = 0;
        double maxChildWidth = 0;
        while (child is not null)
        {
            child.layout(childConstraints, parentUsesSize: true);
            childrenWidth += child.size.width;
            maxChildHeight = Math.Max(maxChildHeight, child.size.height);
            maxChildWidth = Math.Max(maxChildWidth, child.size.width);
            child = childAfter(child);
        }
        var rtlLocal = Equals(textDirection, TextDirection.rtl);
        double actualWidth = childrenWidth + (spacing * (childCount - 1L));
        if (actualWidth > constraints.maxWidth)
        {
            child = Equals(overflowDirection, VerticalDirection.down) ? firstChild : lastChild;
            global::Doroti.Framework.Rendering.RenderBox? nextChild()
            {
                return Equals(overflowDirection, VerticalDirection.down) ? childAfter(child!) : childBefore(child!);
                throw new InvalidOperationException("Dart control flow completed without a value.");
            }
            double y = 0;
            while (child is not null)
            {
                var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
                double x = overflowAlignment switch { OverflowBarAlignment.center => (constraints.maxWidth - child.size.width) / 2L, OverflowBarAlignment.start => rtlLocal ? (constraints.maxWidth - child.size.width) : 0, OverflowBarAlignment.end => rtlLocal ? 0 : (constraints.maxWidth - child.size.width), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                childParentData.offset = new global::Doroti.Ui.Offset(x, y);
                y += child.size.height + overflowSpacing;
                child = nextChild();
            }
            size = constraints.constrain(new global::Doroti.Ui.Size(constraints.maxWidth, y - overflowSpacing));
        }
        else
        {
            child = firstChild;
            double firstChildWidth = child!.size.width;
            double overallWidth = (alignment is null) ? actualWidth : constraints.maxWidth;
            size = constraints.constrain(new global::Doroti.Ui.Size(overallWidth, maxChildHeight));
            double xLocal = default!;
            double layoutSpacing = spacing;
            switch (alignment)
            {
                case null:
                    {
                        xLocal = rtlLocal ? (size.width - firstChildWidth) : 0;
                        break;
                    }
                case MainAxisAlignment.start:
                    {
                        xLocal = rtlLocal ? (size.width - firstChildWidth) : 0;
                        break;
                    }
                case MainAxisAlignment.center:
                    {
                        double halfRemainingWidth = (size.width - actualWidth) / 2L;
                        xLocal = rtlLocal ? (size.width - halfRemainingWidth - firstChildWidth) : halfRemainingWidth;
                        break;
                    }
                case MainAxisAlignment.end:
                    {
                        xLocal = rtlLocal ? (actualWidth - firstChildWidth) : (size.width - actualWidth);
                        break;
                    }
                case MainAxisAlignment.spaceBetween:
                    {
                        layoutSpacing = (size.width - childrenWidth) / (childCount - 1L);
                        xLocal = rtlLocal ? (size.width - firstChildWidth) : 0;
                        break;
                    }
                case MainAxisAlignment.spaceAround:
                    {
                        layoutSpacing = (childCount > 0L) ? ((size.width - childrenWidth) / childCount) : 0;
                        xLocal = rtlLocal ? (size.width - (layoutSpacing / 2L) - firstChildWidth) : (layoutSpacing / 2L);
                        break;
                    }
                case MainAxisAlignment.spaceEvenly:
                    {
                        layoutSpacing = (size.width - childrenWidth) / (childCount + 1L);
                        xLocal = rtlLocal ? (size.width - layoutSpacing - firstChildWidth) : layoutSpacing;
                        break;
                    }
            }
            while (child is not null)
            {
                var childParentDataLocal = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
                childParentDataLocal.offset = new global::Doroti.Ui.Offset(xLocal, (maxChildHeight - child.size.height) / 2L);
                if (!rtlLocal)
                {
                    xLocal += child.size.width + layoutSpacing;
                }
                child = childAfter(child);
                if (rtlLocal && (child is not null))
                {
                    xLocal -= child.size.width + layoutSpacing;
                }
            }
        }
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        return defaultHitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        defaultPaint(context, offset);
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("spacing", spacing, defaultValue: 0L));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("overflowSpacing", overflowSpacing, defaultValue: 0L));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<OverflowBarAlignment>("overflowAlignment", overflowAlignment, defaultValue: OverflowBarAlignment.start));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.VerticalDirection>("overflowDirection", overflowDirection, defaultValue: VerticalDirection.down));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", textDirection, defaultValue: null));
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((_OverflowBarParentData__overflow_bar?)_firstChild!.parentData!)!;
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
            var afterParentData = ((_OverflowBarParentData__overflow_bar?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((_OverflowBarParentData__overflow_bar?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((_OverflowBarParentData__overflow_bar?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this), () => (object?)"A RenderObject cannot be inserted into itself.");
        DartRuntimePrimitives.Assert(() => !Equals(after, this), () => (object?)"A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject.");
        DartRuntimePrimitives.Assert(() => !Equals(child, after), () => (object?)"A RenderObject cannot be inserted after itself.");
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is _OverflowBarParentData__overflow_bar, () => (object?)$"A child of {GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, " + $"which does not conform to {typeof(_OverflowBarParentData__overflow_bar)}. Class using ContainerRenderObjectMixin " + $"should override setupParentData() to set parentData to type {typeof(_OverflowBarParentData__overflow_bar)}.");
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((global::System.Action<RenderBox>)add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((_OverflowBarParentData__overflow_bar?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((_OverflowBarParentData__overflow_bar?)childParentData.nextSibling!.parentData!)!;
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
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
        var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
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
                var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position - childParentData.offset));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
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
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
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
            var childParentData = ((_OverflowBarParentData__overflow_bar?)child.parentData!)!;
            result.Add(child!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

