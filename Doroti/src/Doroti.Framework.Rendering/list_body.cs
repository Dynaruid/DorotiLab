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

namespace Doroti.Framework.Rendering;

public class ListBodyParentData : ContainerBoxParentData<RenderBox>
{
}

internal delegate double _ChildSizingFunction__list_body(RenderBox child);

public class RenderListBody : RenderBox, ContainerRenderObjectMixin<RenderBox, ListBodyParentData>, RenderBoxContainerDefaultsMixin<RenderBox, ListBodyParentData>
{
    internal virtual global::Doroti.Framework.Painting.AxisDirection _axisDirection { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderListBody(List<RenderBox>? children = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down)
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

    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection
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
    public virtual global::Doroti.Framework.Painting.Axis mainAxis => global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this.axisDirection);
    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckConstraints(constraints));
        RenderBox? child = default!;
        Func<RenderBox, RenderBox?> nextChild = default!;
        switch (this.axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.right:
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    var childConstraints = BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraints).maxHeight);
                    BaselineOffset baselineOffset = BaselineOffset.noBaseline;
                    for (child = firstChild; (child is not null); child = childAfter(child))
                    {
                        baselineOffset = baselineOffset.minOf(new BaselineOffset(child.getDryBaseline(childConstraints, baseline)));
                    }
                    return baselineOffset.offset;
                }
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    child = lastChild;
                    nextChild = childBefore;
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    child = firstChild;
                    nextChild = childAfter;
                    break;
                }
        }
        var childConstraintsLocal = BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraints).maxWidth);
        var mainAxisExtent = 0.0;
        for (; (child is not null); child = nextChild(child))
        {
            double? childBaseline = child.getDryBaseline(childConstraintsLocal, baseline);
            if ((childBaseline is not null))
            {
                double childBaseline__3516__value3592 = DartRuntimePrimitives.RequireValue(childBaseline);
                return (DartRuntimePrimitives.RequireValue(childBaseline__3516__value3592) + mainAxisExtent);
            }
            mainAxisExtent += child.getDryLayout(childConstraintsLocal).height;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckConstraints(constraints));
        var mainAxisExtent = 0.0;
        RenderBox? child = firstChild;
        switch (this.axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.right:
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    var innerConstraints = BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraints).maxHeight);
                    while ((child is not null))
                    {
                        global::Doroti.Ui.Size childSize = child.getDryLayout(innerConstraints);
                        mainAxisExtent += childSize.width;
                        child = childAfter(child);
                    }
                    return constraints.constrain(new global::Doroti.Ui.Size(mainAxisExtent, ((BoxConstraints)constraints).maxHeight));
                }
            case global::Doroti.Framework.Painting.AxisDirection.up:
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    var innerConstraintsLocal = BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraints).maxWidth);
                    while ((child is not null))
                    {
                        global::Doroti.Ui.Size childSizeLocal = child.getDryLayout(innerConstraintsLocal);
                        mainAxisExtent += childSizeLocal.height;
                        child = childAfter(child);
                    }
                    return constraints.constrain(new global::Doroti.Ui.Size(((BoxConstraints)constraints).maxWidth, mainAxisExtent));
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
                    case global::Doroti.Framework.Painting.Axis.horizontal:
                        {
                            if (!((BoxConstraints)constraints).hasBoundedWidth)
                            {
                                return true;
                            }
                            break;
                        }
                    case global::Doroti.Framework.Painting.Axis.vertical:
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
                    case global::Doroti.Framework.Painting.Axis.horizontal:
                        {
                            if (((BoxConstraints)constraints).hasBoundedHeight)
                            {
                                return true;
                            }
                            break;
                        }
                    case global::Doroti.Framework.Painting.Axis.vertical:
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
        BoxConstraints constraintsLocal = this.constraints;
        DartRuntimePrimitives.Assert(() => _debugCheckConstraints(constraintsLocal));
        var mainAxisExtent = 0.0;
        RenderBox? child = firstChild;
        switch (this.axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    var innerConstraints = BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraintsLocal).maxHeight);
                    while ((child is not null))
                    {
                        child.layout(innerConstraints, parentUsesSize: true);
                        var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
                        childParentData.offset = new global::Doroti.Ui.Offset(mainAxisExtent, 0.0);
                        mainAxisExtent += ((RenderBox)child).size.width;
                        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentData)));
                        child = childParentData.nextSibling;
                    }
                    size = constraintsLocal.constrain(new global::Doroti.Ui.Size(mainAxisExtent, ((BoxConstraints)constraintsLocal).maxHeight));
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    var innerConstraintsLocal = BoxConstraints.CreateTightFor(height: ((BoxConstraints)constraintsLocal).maxHeight);
                    while ((child is not null))
                    {
                        child.layout(innerConstraintsLocal, parentUsesSize: true);
                        var childParentDataLocal = ((ListBodyParentData?)(object?)child.parentData!)!;
                        mainAxisExtent += ((RenderBox)child).size.width;
                        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentDataLocal)));
                        child = childParentDataLocal.nextSibling;
                    }
                    var position = 0.0;
                    child = firstChild;
                    while ((child is not null))
                    {
                        var childParentDataAlternate = ((ListBodyParentData?)(object?)child.parentData!)!;
                        position += ((RenderBox)child).size.width;
                        childParentDataAlternate.offset = new global::Doroti.Ui.Offset((mainAxisExtent - position), 0.0);
                        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentDataAlternate)));
                        child = childParentDataAlternate.nextSibling;
                    }
                    size = constraintsLocal.constrain(new global::Doroti.Ui.Size(mainAxisExtent, ((BoxConstraints)constraintsLocal).maxHeight));
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    var innerConstraintsAlternate = BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraintsLocal).maxWidth);
                    while ((child is not null))
                    {
                        child.layout(innerConstraintsAlternate, parentUsesSize: true);
                        var childParentDataNested = ((ListBodyParentData?)(object?)child.parentData!)!;
                        childParentDataNested.offset = new global::Doroti.Ui.Offset(0.0, mainAxisExtent);
                        mainAxisExtent += ((RenderBox)child).size.height;
                        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentDataNested)));
                        child = childParentDataNested.nextSibling;
                    }
                    size = constraintsLocal.constrain(new global::Doroti.Ui.Size(((BoxConstraints)constraintsLocal).maxWidth, mainAxisExtent));
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    var innerConstraintsNested = BoxConstraints.CreateTightFor(width: ((BoxConstraints)constraintsLocal).maxWidth);
                    while ((child is not null))
                    {
                        child.layout(innerConstraintsNested, parentUsesSize: true);
                        var childParentDataCurrent = ((ListBodyParentData?)(object?)child.parentData!)!;
                        mainAxisExtent += ((RenderBox)child).size.height;
                        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentDataCurrent)));
                        child = childParentDataCurrent.nextSibling;
                    }
                    var positionLocal = 0.0;
                    child = firstChild;
                    while ((child is not null))
                    {
                        var childParentDataNext = ((ListBodyParentData?)(object?)child.parentData!)!;
                        positionLocal += ((RenderBox)child).size.height;
                        childParentDataNext.offset = new global::Doroti.Ui.Offset(0.0, (mainAxisExtent - positionLocal));
                        DartRuntimePrimitives.Assert(() => (object.Equals(child.parentData, childParentDataNext)));
                        child = childParentDataNext.nextSibling;
                    }
                    size = constraintsLocal.constrain(new global::Doroti.Ui.Size(((BoxConstraints)constraintsLocal).maxWidth, mainAxisExtent));
                    break;
                }
        }
        DartRuntimePrimitives.Assert(() => size.isFinite);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
    }

    internal virtual double _getIntrinsicCrossAxis(Func<RenderBox, double> childSize)
    {
        var extent = 0.0;
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            extent = Math.Max(extent, childSize(child));
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        return extent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getIntrinsicMainAxis(Func<RenderBox, double> childSize)
    {
        var extent = 0.0;
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            extent += childSize(child);
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        return extent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return (this.mainAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => _getIntrinsicMainAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicWidth(height)))), global::Doroti.Framework.Painting.Axis.vertical => _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicWidth(height)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return (this.mainAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => _getIntrinsicMainAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicWidth(height)))), global::Doroti.Framework.Painting.Axis.vertical => _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicWidth(height)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return (this.mainAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => _getIntrinsicMainAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicHeight(width)))), global::Doroti.Framework.Painting.Axis.vertical => _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMinIntrinsicHeight(width)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return (this.mainAxis switch { global::Doroti.Framework.Painting.Axis.horizontal => _getIntrinsicMainAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicHeight(width)))), global::Doroti.Framework.Painting.Axis.vertical => _getIntrinsicCrossAxis(((Func<RenderBox, double>)((child) => child.getMaxIntrinsicHeight(width)))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
        var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((ListBodyParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData = ((ListBodyParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((ListBodyParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((ListBodyParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
        var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((ListBodyParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((ListBodyParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
                var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((ListBodyParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

