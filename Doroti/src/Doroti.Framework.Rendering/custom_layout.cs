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

namespace Doroti.Generated.Framework.Rendering;

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
        RenderBox? child__6213 = this._idToChild!.GetValueOrDefault(childId);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child__6213 is null))
                {
                    throw new FlutterError($"The {this} custom multichild layout delegate tried to lay out a non-existent child.\n" + $"There is no child with the id \"{childId}\".");
                }
                if (!this._debugChildrenNeedingLayout!.Remove(child__6213))
                {
                    throw new FlutterError($"The {this} custom multichild layout delegate tried to lay out the child with id \"{childId}\" more than once.\n" + "Each child must be laid out exactly once.");
                }
                try
                {
                    DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid(isAppliedConstraint: true));
                }
                catch (AssertionError exception__6884)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The {this} custom multichild layout delegate provided invalid box constraints for the child with id \"{childId}\"."), new DiagnosticsProperty<AssertionError>("Exception", exception__6884, showName: false), new ErrorDescription("The minimum width and height must be greater than or equal to zero.\n" + "The maximum width must be greater than or equal to the minimum width.\n" + "The maximum height must be greater than or equal to the minimum height.") });
                }
                return true;
            });
        child__6213!.layout(constraints, parentUsesSize: true);
        return ((RenderBox)child__6213).size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void positionChild(object childId, Offset offset)
    {
        RenderBox? child__8051 = this._idToChild!.GetValueOrDefault(childId);
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child__8051 is null))
                {
                    throw new FlutterError($"The {this} custom multichild layout delegate tried to position out a non-existent child:\n" + $"There is no child with the id \"{childId}\".");
                }
                return true;
            });
        var childParentData__8367 = ((MultiChildLayoutParentData?)(object?)child__8051!.parentData!)!;
        childParentData__8367.offset = offset;
    }

    internal virtual DiagnosticsNode _debugDescribeChild(RenderBox child)
    {
        var childParentData__8544 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        return new DiagnosticsProperty<RenderBox>($"{((MultiChildLayoutParentData)childParentData__8544).id}", child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _callPerformLayout(Size size, RenderBox? firstChild)
    {
        DartMap<object, RenderBox>? previousIdToChild__8967 = this._idToChild;
        HashSet<RenderBox>? debugPreviousChildrenNeedingLayout__9020 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugPreviousChildrenNeedingLayout__9020 = this._debugChildrenNeedingLayout;
                _debugChildrenNeedingLayout = new HashSet<RenderBox>();
                return true;
            });
        try
        {
            _idToChild = new DartMap<object, RenderBox>().cast<object, RenderBox>();
            var child__9287 = firstChild;
            while ((child__9287 is not null))
            {
                var childParentData__9351 = ((MultiChildLayoutParentData?)(object?)child__9287.parentData!)!;
                DartRuntimePrimitives.Assert(() =>
                    {
                        if ((((MultiChildLayoutParentData)childParentData__9351).id is null))
                        {
                            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Every child of a RenderCustomMultiChildLayoutBox must have an ID in its parent data."), child__9287!.describeForError("The following child has no ID") });
                        }
                        return true;
                    });
                this._idToChild![((MultiChildLayoutParentData)childParentData__9351).id!] = child__9287;
                DartRuntimePrimitives.Assert(() =>
                    {
                        this._debugChildrenNeedingLayout!.Add(child__9287!);
                        return true;
                    });
                child__9287 = childParentData__9351.nextSibling;
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
            _idToChild = previousIdToChild__8967;
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugChildrenNeedingLayout = debugPreviousChildrenNeedingLayout__9020;
                    return true;
                });
        }
    }

    public virtual global::Doroti.Ui.Size getSize(BoxConstraints constraints) => ((BoxConstraints)constraints).biggest;
    public abstract void performLayout(Size size);
    public abstract bool shouldRelayout(MultiChildLayoutDelegate oldDelegate);
    public override string ToString() => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "MultiChildLayoutDelegate");
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
            MultiChildLayoutDelegate oldDelegate__13588 = this._delegate;
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate__13588))) || newDelegate.shouldRelayout(oldDelegate__13588)))
            {
                markNeedsLayout();
            }
            _delegate = newDelegate;
            if (attached)
            {
                ((MultiChildLayoutDelegate)oldDelegate__13588)._relayout?.removeListener(markNeedsLayout);
                ((MultiChildLayoutDelegate)newDelegate)._relayout?.addListener(markNeedsLayout);
            }
        }
    }
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((MultiChildLayoutParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
        ((MultiChildLayoutDelegate)this._delegate)._relayout?.addListener(markNeedsLayout);
    }

    public override void detach()
    {
        ((MultiChildLayoutDelegate)this._delegate)._relayout?.removeListener(markNeedsLayout);
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((MultiChildLayoutParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
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
        double width__14701 = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(width__14701))
        {
            return width__14701;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double width__14925 = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(width__14925))
        {
            return width__14925;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double height__15149 = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(height__15149))
        {
            return height__15149;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double height__15375 = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(height__15375))
        {
            return height__15375;
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
        var childParentData__173585 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((MultiChildLayoutParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((MultiChildLayoutParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((MultiChildLayoutParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((MultiChildLayoutParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
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
        var childParentData__179226 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((MultiChildLayoutParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((MultiChildLayoutParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
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
            var childParentData__180684 = ((MultiChildLayoutParentData?)(object?)child__180623.parentData!)!;
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
        var childParentData__181479 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData__181479.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((MultiChildLayoutParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((MultiChildLayoutParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((MultiChildLayoutParentData?)(object?)child.parentData!)!;
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
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((MultiChildLayoutParentData?)(object?)child__183606.parentData!)!;
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
            var childParentData__138777 = ((MultiChildLayoutParentData?)(object?)child__138717.parentData!)!;
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
            var childParentData__139488 = ((MultiChildLayoutParentData?)(object?)child__139428.parentData!)!;
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
            var childParentData__140418 = ((MultiChildLayoutParentData?)(object?)child__140279.parentData!)!;
            if (!child__140279.hasSize)
            {
                child__140279 = childParentData__140418.previousSibling;
                continue;
            }
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
            var childParentData__141300 = ((MultiChildLayoutParentData?)(object?)child__141240.parentData!)!;
            // Custom-layout delegates may deliberately leave an inactive slot
            // unlaid (for example Scaffold's transient FAB/snack-bar slots).
            // Such a child has no paint geometry and must not be composited.
            if (child__141240.hasSize)
            {
                context.paintChild(child__141240, (childParentData__141300.offset + offset));
            }
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((MultiChildLayoutParentData?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
