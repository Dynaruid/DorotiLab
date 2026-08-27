// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/custom_layout.dart
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

public class MultiChildLayoutParentData : ContainerBoxParentData<RenderBox>
{
    public virtual object? id { get; set; } = default;

    public override string ToString() => $"{base.ToString()}; id={this.id}";
}

public abstract class MultiChildLayoutDelegate
{
    internal virtual Listenable? _relayout { get; private set; }
    internal virtual DartMap<object, RenderBox>? _idToChild { get; set; } = default;
    internal virtual HashSet<RenderBox>? _debugChildrenNeedingLayout { get; set; } = default;

    protected MultiChildLayoutDelegate(Listenable? relayout = null)
    {
        this._relayout = relayout;
    }

    public virtual bool hasChild(object childId) => (this._idToChild!.ContainsKey(childId));
    public virtual global::Doroti.Ui.Size layoutChild(object childId, BoxConstraints constraints)
    {
        RenderBox? child = this._idToChild!.GetValueOrDefault(childId);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is null))
                {
                    throw new FlutterError($"The {this} custom multichild layout delegate tried to lay out a non-existent child.\n" + $"There is no child with the id \"{childId}\".");
                }
                if (!this._debugChildrenNeedingLayout!.Remove(child))
                {
                    throw new FlutterError($"The {this} custom multichild layout delegate tried to lay out the child with id \"{childId}\" more than once.\n" + "Each child must be laid out exactly once.");
                }
                try
                {
                    DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid(isAppliedConstraint: true));
                }
                catch (AssertionError exception)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {this} custom multichild layout delegate provided invalid box constraints for the child with id \"{childId}\"."), new DiagnosticsProperty<AssertionError>("Exception", exception, showName: false), new ErrorDescription("The minimum width and height must be greater than or equal to zero.\n" + "The maximum width must be greater than or equal to the minimum width.\n" + "The maximum height must be greater than or equal to the minimum height.") });
                }
                return true;
            });
        child!.layout(constraints, parentUsesSize: true);
        return ((RenderBox)child).size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void positionChild(object childId, Offset offset)
    {
        RenderBox? child = this._idToChild!.GetValueOrDefault(childId);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is null))
                {
                    throw new FlutterError($"The {this} custom multichild layout delegate tried to position out a non-existent child:\n" + $"There is no child with the id \"{childId}\".");
                }
                return true;
            });
        var childParentData = ((MultiChildLayoutParentData?)(object?)child!.parentData!)!;
        childParentData.offset = offset;
    }

    internal virtual DiagnosticsNode _debugDescribeChild(RenderBox child)
    {
        var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        return new DiagnosticsProperty<RenderBox>($"{((MultiChildLayoutParentData)childParentData).id}", child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _callPerformLayout(Size size, RenderBox? firstChild)
    {
        DartMap<object, RenderBox>? previousIdToChild = this._idToChild;
        HashSet<RenderBox>? debugPreviousChildrenNeedingLayout = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugPreviousChildrenNeedingLayout = this._debugChildrenNeedingLayout;
                _debugChildrenNeedingLayout = new HashSet<RenderBox>();
                return true;
            });
        try
        {
            _idToChild = new DartMap<object, RenderBox>().cast<object, RenderBox>();
            var child = firstChild;
            while ((child is not null))
            {
                var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        if ((((MultiChildLayoutParentData)childParentData).id is null))
                        {
                            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Every child of a RenderCustomMultiChildLayoutBox must have an ID in its parent data."), child!.describeForError("The following child has no ID") });
                        }
                        return true;
                    });
                this._idToChild![((MultiChildLayoutParentData)childParentData).id!] = child;
                DartRuntimePrimitives.Assert(() =>
                    {
                        this._debugChildrenNeedingLayout!.Add(child!);
                        return true;
                    });
                child = childParentData.nextSibling;
            }
            performLayout(size);
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((checked((long)(this._debugChildrenNeedingLayout!.Count)) != 0))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Each child must be laid out exactly once."), new DiagnosticsBlock(name: $"The {this} custom multichild layout delegate forgot " + "to lay out the following " + $"{((checked((long)(this._debugChildrenNeedingLayout!.Count)) > 1L) ? "children" : "child__9287")}", children: this._debugChildrenNeedingLayout!.map<RenderBox, DiagnosticsNode>(this._debugDescribeChild).ToList()) });
                    }
                    return true;
                });
        }
        finally
        {
            _idToChild = previousIdToChild;
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugChildrenNeedingLayout = debugPreviousChildrenNeedingLayout;
                    return true;
                });
        }
    }

    public virtual global::Doroti.Ui.Size getSize(BoxConstraints constraints) => ((BoxConstraints)constraints).biggest;
    public abstract void performLayout(Size size);
    public abstract bool shouldRelayout(MultiChildLayoutDelegate oldDelegate);
    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MultiChildLayoutDelegate");
}

public class RenderCustomMultiChildLayoutBox : RenderBox, ContainerRenderObjectMixin<RenderBox, MultiChildLayoutParentData>, RenderBoxContainerDefaultsMixin<RenderBox, MultiChildLayoutParentData>
{
    internal virtual MultiChildLayoutDelegate _delegate { get; set; } = default!;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderCustomMultiChildLayoutBox(List<RenderBox>? children = null, MultiChildLayoutDelegate @delegate = default!)
    {
        this._delegate = @delegate;
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        if ((__child.parentData is not MultiChildLayoutParentData))
        {
            __child.parentData = new MultiChildLayoutParentData();
        }
    }

    public virtual MultiChildLayoutDelegate @delegate
    {
        get => this._delegate;
        set
        {
            var newDelegate = value;
            if ((object.Equals(this._delegate, newDelegate)))
            {
                return;
            }
            MultiChildLayoutDelegate oldDelegate = this._delegate;
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || newDelegate.shouldRelayout(oldDelegate)))
            {
                markNeedsLayout();
            }
            _delegate = newDelegate;
            if (attached)
            {
                ((MultiChildLayoutDelegate)oldDelegate)._relayout?.removeListener(markNeedsLayout);
                ((MultiChildLayoutDelegate)newDelegate)._relayout?.addListener(markNeedsLayout);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        ((MultiChildLayoutDelegate)this._delegate)._relayout?.addListener(markNeedsLayout);
    }

    public override void detach()
    {
        ((MultiChildLayoutDelegate)this._delegate)._relayout?.removeListener(markNeedsLayout);
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    internal virtual global::Doroti.Ui.Size _getSize(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        return constraints.constrain(this._delegate.getSize(constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _getSize(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _getSize(constraints);
        this.@delegate._callPerformLayout(size, firstChild);
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
        var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((MultiChildLayoutParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData = ((MultiChildLayoutParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((MultiChildLayoutParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((MultiChildLayoutParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
        DartRuntimePrimitives.Assert(() => (child.parentData is MultiChildLayoutParentData));
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
        var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((MultiChildLayoutParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((MultiChildLayoutParentData?)(object?)childParentData.nextSibling!.parentData!)!;
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
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child {count}"));
                if ((object.Equals(child, this.lastChild)))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
            if (!child.hasSize)
            {
                child = childParentData.previousSibling;
                continue;
            }
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
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
            // Custom-layout delegates may deliberately leave an inactive slot
            // unlaid (for example Scaffold's transient FAB/snack-bar slots).
            // Such a child has no paint geometry and must not be composited.
            if (child.hasSize)
            {
                context.paintChild(child, (childParentData.offset + offset));
            }
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
