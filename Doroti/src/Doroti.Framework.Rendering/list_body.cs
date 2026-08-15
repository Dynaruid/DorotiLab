// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/list_body.dart
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

public class ListBodyParentData : ContainerBoxParentData<RenderBox>
{
}

internal delegate double _ChildSizingFunction__list_body(RenderBox child);

public class RenderListBody : RenderBox, ContainerRenderObjectMixin<RenderBox, ListBodyParentData>, RenderBoxContainerDefaultsMixin<RenderBox, ListBodyParentData>
{
    internal virtual global::Doroti.Generated.Framework.Painting.AxisDirection _axisDirection { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderListBody(List<RenderBox>? children = null, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection = AxisDirection.down)
    {
        this._axisDirection = axisDirection;
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        if ((__child.parentData is not ListBodyParentData))
        {
            __child.parentData = new ListBodyParentData();
        }
    }

    public virtual global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection
    {
        get => this._axisDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._axisDirection, __value)))
            {
                return;
            }
            _axisDirection = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Generated.Framework.Painting.Axis mainAxis => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this.axisDirection);
    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckConstraints(constraints));
        RenderBox? child__2571 = default!;
        Func<RenderBox, RenderBox?> nextChild__2619 = default!;
        switch (this.axisDirection)
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                {
                    var childConstraints__2736 = BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraints).maxHeight);
                    BaselineOffset baselineOffset__2834 = BaselineOffset.noBaseline;
                    for (child__2571 = firstChild; (child__2571 is not null); child__2571 = childAfter(child__2571))
                    {
                        baselineOffset__2834 = baselineOffset__2834.minOf(new BaselineOffset(child__2571.getDryBaseline(childConstraints__2736, baseline)));
                    }
                    return baselineOffset__2834.offset;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
                {
                    child__2571 = lastChild;
                    nextChild__2619 = childBefore;
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                {
                    child__2571 = firstChild;
                    nextChild__2619 = childAfter;
                    break;
                }
        }
        var childConstraints__3339 = BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraints).maxWidth);
        var mainAxisExtent__3420 = 0.0;
        for (; (child__2571 is not null); child__2571 = nextChild__2619(child__2571))
        {
            double? childBaseline__3516 = child__2571.getDryBaseline(childConstraints__3339, baseline);
            if ((childBaseline__3516 is not null))
            {
                double childBaseline__3516__value3592 = DartRuntimePrimitives.RequireValue(childBaseline__3516);
                return (DartRuntimePrimitives.RequireValue(childBaseline__3516__value3592) + mainAxisExtent__3420);
            }
            mainAxisExtent__3420 += child__2571.getDryLayout(childConstraints__3339).height;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckConstraints(constraints));
        var mainAxisExtent__3915 = 0.0;
        RenderBox? child__3952 = firstChild;
        switch (this.axisDirection)
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                {
                    var innerConstraints__4078 = BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraints).maxHeight);
                    while ((child__3952 is not null))
                    {
                        global::Doroti.Ui.Size childSize__4206 = child__3952.getDryLayout(innerConstraints__4078);
                        mainAxisExtent__3915 += childSize__4206.width;
                        child__3952 = childAfter(child__3952);
                    }
                    return constraints.constrain(new global::Doroti.Ui.Size(mainAxisExtent__3915, ((BoxConstraints)constraints).maxHeight));
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                {
                    var innerConstraints__4505 = BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraints).maxWidth);
                    while ((child__3952 is not null))
                    {
                        global::Doroti.Ui.Size childSize__4631 = child__3952.getDryLayout(innerConstraints__4505);
                        mainAxisExtent__3915 += childSize__4631.height;
                        child__3952 = childAfter(child__3952);
                    }
                    return constraints.constrain(new global::Doroti.Ui.Size(((BoxConstraints)constraints).maxWidth, mainAxisExtent__3915));
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _debugCheckConstraints(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                switch (this.mainAxis)
                {
                    case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                        {
                            if (!((BoxConstraints)constraints).hasBoundedWidth)
                            {
                                return true;
                            }
                            break;
                        }
                    case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                        {
                            if (!((BoxConstraints)constraints).hasBoundedHeight)
                            {
                                return true;
                            }
                            break;
                        }
                }
                throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("RenderListBody must have unlimited space along its main axis."), new ErrorDescription("RenderListBody does not clip or resize its children, so it must be " + "placed in a parent that does not constrain the main " + "axis."), new ErrorHint("You probably want to put the RenderListBody inside a " + "RenderViewport with a matching main axis.") });
            });
        DartRuntimePrimitives.Assert(() =>
            {
                switch (this.mainAxis)
                {
                    case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                        {
                            if (((BoxConstraints)constraints).hasBoundedHeight)
                            {
                                return true;
                            }
                            break;
                        }
                    case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                        {
                            if (((BoxConstraints)constraints).hasBoundedWidth)
                            {
                                return true;
                            }
                            break;
                        }
                }
                throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("RenderListBody must have a bounded constraint for its cross axis."), new ErrorDescription("RenderListBody forces its children to expand to fit the RenderListBody's container, " + "so it must be placed in a parent that constrains the cross " + "axis to a finite dimension."), new ErrorHint("If you are attempting to nest a RenderListBody with " + "one direction inside one of another direction, you will want to " + "wrap the inner one inside a box that fixes the dimension in that direction, " + "for example, a RenderIntrinsicWidth or RenderIntrinsicHeight object. " + "This is relatively expensive, however.") });
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraints__7251 = this.constraints;
        DartRuntimePrimitives.Assert(() => _debugCheckConstraints(constraints__7251));
        var mainAxisExtent__7340 = 0.0;
        RenderBox? child__7377 = firstChild;
        switch (this.axisDirection)
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
                {
                    var innerConstraints__7472 = BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraints__7251).maxHeight);
                    while ((child__7377 is not null))
                    {
                        child__7377.layout(innerConstraints__7472, parentUsesSize: true);
                        var childParentData__7659 = ((ListBodyParentData?)(object?)child__7377.parentData!)!;
                        childParentData__7659.offset = new global::Doroti.Ui.Offset(mainAxisExtent__7340, 0.0);
                        mainAxisExtent__7340 += ((RenderBox)child__7377).size.width;
                        DartRuntimePrimitives.Assert(() => (object.Equals(child__7377.parentData, childParentData__7659)));
                        child__7377 = childParentData__7659.nextSibling;
                    }
                    size = constraints__7251.constrain(new global::Doroti.Ui.Size(mainAxisExtent__7340, ((BoxConstraints)constraints__7251).maxHeight));
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                {
                    var innerConstraints__8068 = BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraints__7251).maxHeight);
                    while ((child__7377 is not null))
                    {
                        child__7377.layout(innerConstraints__8068, parentUsesSize: true);
                        var childParentData__8255 = ((ListBodyParentData?)(object?)child__7377.parentData!)!;
                        mainAxisExtent__7340 += ((RenderBox)child__7377).size.width;
                        DartRuntimePrimitives.Assert(() => (object.Equals(child__7377.parentData, childParentData__8255)));
                        child__7377 = childParentData__8255.nextSibling;
                    }
                    var position__8484 = 0.0;
                    child__7377 = firstChild;
                    while ((child__7377 is not null))
                    {
                        var childParentData__8576 = ((ListBodyParentData?)(object?)child__7377.parentData!)!;
                        position__8484 += ((RenderBox)child__7377).size.width;
                        childParentData__8576.offset = new global::Doroti.Ui.Offset((mainAxisExtent__7340 - position__8484), 0.0);
                        DartRuntimePrimitives.Assert(() => (object.Equals(child__7377.parentData, childParentData__8576)));
                        child__7377 = childParentData__8576.nextSibling;
                    }
                    size = constraints__7251.constrain(new global::Doroti.Ui.Size(mainAxisExtent__7340, ((BoxConstraints)constraints__7251).maxHeight));
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                {
                    var innerConstraints__8990 = BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraints__7251).maxWidth);
                    while ((child__7377 is not null))
                    {
                        child__7377.layout(innerConstraints__8990, parentUsesSize: true);
                        var childParentData__9175 = ((ListBodyParentData?)(object?)child__7377.parentData!)!;
                        childParentData__9175.offset = new global::Doroti.Ui.Offset(0.0, mainAxisExtent__7340);
                        mainAxisExtent__7340 += ((RenderBox)child__7377).size.height;
                        DartRuntimePrimitives.Assert(() => (object.Equals(child__7377.parentData, childParentData__9175)));
                        child__7377 = childParentData__9175.nextSibling;
                    }
                    size = constraints__7251.constrain(new global::Doroti.Ui.Size(((BoxConstraints)constraints__7251).maxWidth, mainAxisExtent__7340));
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
                {
                    var innerConstraints__9582 = BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraints__7251).maxWidth);
                    while ((child__7377 is not null))
                    {
                        child__7377.layout(innerConstraints__9582, parentUsesSize: true);
                        var childParentData__9767 = ((ListBodyParentData?)(object?)child__7377.parentData!)!;
                        mainAxisExtent__7340 += ((RenderBox)child__7377).size.height;
                        DartRuntimePrimitives.Assert(() => (object.Equals(child__7377.parentData, childParentData__9767)));
                        child__7377 = childParentData__9767.nextSibling;
                    }
                    var position__9997 = 0.0;
                    child__7377 = firstChild;
                    while ((child__7377 is not null))
                    {
                        var childParentData__10089 = ((ListBodyParentData?)(object?)child__7377.parentData!)!;
                        position__9997 += ((RenderBox)child__7377).size.height;
                        childParentData__10089.offset = new global::Doroti.Ui.Offset(0.0, (mainAxisExtent__7340 - position__9997));
                        DartRuntimePrimitives.Assert(() => (object.Equals(child__7377.parentData, childParentData__10089)));
                        child__7377 = childParentData__10089.nextSibling;
                    }
                    size = constraints__7251.constrain(new global::Doroti.Ui.Size(((BoxConstraints)constraints__7251).maxWidth, mainAxisExtent__7340));
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => size.isFinite);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Generated.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
    }

    internal virtual double _getIntrinsicCrossAxis(Func<RenderBox, double> childSize)
    {
        var extent__10780 = 0.0;
        RenderBox? child__10809 = firstChild;
        while ((child__10809 is not null))
        {
            extent__10780 = Math.Max(extent__10780, childSize(child__10809));
            var childParentData__10920 = ((ListBodyParentData?)(object?)child__10809.parentData!)!;
            child__10809 = childParentData__10920.nextSibling;
        }
        return extent__10780;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getIntrinsicMainAxis(Func<RenderBox, double> childSize)
    {
        var extent__11125 = 0.0;
        RenderBox? child__11154 = firstChild;
        while ((child__11154 is not null))
        {
            extent__11125 += childSize(child__11154);
            var childParentData__11248 = ((ListBodyParentData?)(object?)child__11154.parentData!)!;
            child__11154 = childParentData__11248.nextSibling;
        }
        return extent__11125;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return (this.mainAxis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => _getIntrinsicMainAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicWidth(height)))), global::Doroti.Generated.Framework.Painting.Axis.vertical => _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicWidth(height)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return (this.mainAxis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => _getIntrinsicMainAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicWidth(height)))), global::Doroti.Generated.Framework.Painting.Axis.vertical => _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicWidth(height)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return (this.mainAxis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => _getIntrinsicMainAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicHeight(width)))), global::Doroti.Generated.Framework.Painting.Axis.vertical => _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicHeight(width)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return (this.mainAxis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => _getIntrinsicMainAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicHeight(width)))), global::Doroti.Generated.Framework.Painting.Axis.vertical => _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicHeight(width)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToFirstActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        defaultPaint(context, offset);
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        return defaultHitTestChildren(result, position: position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((ListBodyParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((ListBodyParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((ListBodyParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((ListBodyParentData?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((ListBodyParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((ListBodyParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((ListBodyParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((ListBodyParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((ListBodyParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
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
        DartRuntimePrimitives.Assert(() => (child.parentData is ListBodyParentData));
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
        var childParentData__179226 = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((ListBodyParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((ListBodyParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
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
            var childParentData__180684 = ((ListBodyParentData?)(object?)child__180623.parentData!)!;
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
        var childParentData__181479 = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childParentData__181891 = ((ListBodyParentData?)(object?)child__181803.parentData!)!;
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
            var childParentData__182148 = ((ListBodyParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((ListBodyParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((ListBodyParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((ListBodyParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((ListBodyParentData?)(object?)child.parentData!)!;
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
                var childParentData__183833 = ((ListBodyParentData?)(object?)child__183606.parentData!)!;
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
            var childParentData__138777 = ((ListBodyParentData?)(object?)child__138717.parentData!)!;
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
            var childParentData__139488 = ((ListBodyParentData?)(object?)child__139428.parentData!)!;
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
            var childParentData__140418 = ((ListBodyParentData?)(object?)child__140279.parentData!)!;
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
            var childParentData__141300 = ((ListBodyParentData?)(object?)child__141240.parentData!)!;
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
            var childParentData__141892 = ((ListBodyParentData?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

