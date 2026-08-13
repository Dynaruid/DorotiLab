// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_group.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

public class RenderSliverCrossAxisGroup : RenderSliver, ContainerRenderObjectMixin<RenderSliver, SliverPhysicalContainerParentData>
{
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderSliver? _firstChild { get; set; } = default;
    public virtual RenderSliver? _lastChild { get; set; } = default;

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverPhysicalContainerParentData))
        {
            child.parentData = new SliverPhysicalContainerParentData();
            (((SliverPhysicalParentData?)(object?)((RenderObject)child).parentData!)!).crossAxisFlex = 1L;
        }
    }

    public override double childMainAxisPosition(RenderObject child) => 0.0;
    public override double childCrossAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        global::Doroti.Flutter.Ui.Offset paintOffset__1979 = (((SliverPhysicalParentData?)(object?)__child.parentData!)!).paintOffset;
        return (((SliverConstraints)constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => paintOffset__1979.dx, global::Doroti.Generated.Framework.Painting.Axis.horizontal => paintOffset__1979.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        double crossAxisExtent__2311 = ((SliverConstraints)constraints).crossAxisExtent;
        DartRuntimePrimitives.Assert(() => double.IsFinite(crossAxisExtent__2311));
        var totalFlex__2461 = 0L;
        var remainingExtent__2484 = crossAxisExtent__2311;
        RenderSliver? child__2537 = firstChild;
        while ((child__2537 is not null))
        {
            var childParentData__2597 = ((SliverPhysicalParentData?)(object?)child__2537.parentData!)!;
            long flex__2678 = (((SliverPhysicalParentData)childParentData__2597).crossAxisFlex ?? 0L);
            if ((flex__2678 == 0L))
            {
                DartRuntimePrimitives.Assert(() => Sliver_groupLibrary._assertOutOfExtent(remainingExtent__2484));
                child__2537.layout(constraints.copyWith(crossAxisExtent: remainingExtent__2484), parentUsesSize: true);
                double? childCrossAxisExtent__3014 = ((RenderSliver)child__2537).geometry!.crossAxisExtent;
                DartRuntimePrimitives.Assert(() => (childCrossAxisExtent__3014 is not null));
                remainingExtent__2484 = Math.Max(0.0, (remainingExtent__2484 - DartRuntimePrimitives.RequireValue(childCrossAxisExtent__3014)));
            }
            else
            {
                totalFlex__2461 += flex__2678;
            }
            child__2537 = childAfter(child__2537);
        }
        double extentPerFlexValue__3304 = (remainingExtent__2484 / totalFlex__2461);
        child__2537 = firstChild;
        geometry = SliverGeometry.zero;
        while ((child__2537 is not null))
        {
            var childParentData__3634 = ((SliverPhysicalParentData?)(object?)child__2537.parentData!)!;
            long flex__3715 = (((SliverPhysicalParentData)childParentData__3634).crossAxisFlex ?? 0L);
            double childExtent__3771 = default!;
            if ((flex__3715 != 0L))
            {
                childExtent__3771 = (extentPerFlexValue__3304 * flex__3715);
                DartRuntimePrimitives.Assert(() => Sliver_groupLibrary._assertOutOfExtent(childExtent__3771));
                child__2537.layout(constraints.copyWith(crossAxisExtent: (extentPerFlexValue__3304 * flex__3715)), parentUsesSize: true);
            }
            else
            {
                childExtent__3771 = DartRuntimePrimitives.RequireValue(((RenderSliver)child__2537).geometry!.crossAxisExtent);
            }
            SliverGeometry childLayoutGeometry__4152 = ((RenderSliver)child__2537).geometry!;
            if ((geometry!.scrollExtent < ((SliverGeometry)childLayoutGeometry__4152).scrollExtent))
            {
                geometry = childLayoutGeometry__4152;
            }
            child__2537 = childAfter(child__2537);
        }
        child__2537 = firstChild;
        var offset__4519 = 0.0;
        while ((child__2537 is not null))
        {
            var childParentData__4573 = ((SliverPhysicalParentData?)(object?)child__2537.parentData!)!;
            SliverGeometry childLayoutGeometry__4665 = ((RenderSliver)child__2537).geometry!;
            double remainingExtent__4723 = (geometry!.scrollExtent - ((SliverConstraints)constraints).scrollOffset);
            double paintCorrection__4811 = ((((SliverGeometry)childLayoutGeometry__4665).paintExtent > remainingExtent__4723) ? (((SliverGeometry)childLayoutGeometry__4665).paintExtent - remainingExtent__4723) : 0.0);
            double childExtent__4977 = (((RenderSliver)child__2537).geometry!.crossAxisExtent ?? (extentPerFlexValue__3304 * ((((SliverPhysicalParentData)childParentData__4573).crossAxisFlex ?? 0L))));
            childParentData__4573.paintOffset = (((SliverConstraints)constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Flutter.Ui.Offset(offset__4519, -paintCorrection__4811), global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Flutter.Ui.Offset(-paintCorrection__4811, offset__4519), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            offset__4519 += childExtent__4977;
            child__2537 = childAfter(child__2537);
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderSliver? child__5488 = firstChild;
        while ((child__5488 is not null))
        {
            if (((RenderSliver)child__5488).geometry!.visible)
            {
                var childParentData__5588 = ((SliverPhysicalParentData?)(object?)child__5488.parentData!)!;
                context.paintChild(child__5488, (offset + ((SliverPhysicalParentData)childParentData__5588).paintOffset));
            }
            child__5488 = childAfter(child__5488);
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderSliver)(object)child;
        var childParentData__5868 = ((SliverPhysicalParentData?)(object?)__child.parentData!)!;
        childParentData__5868.applyPaintTransform(transform);
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        RenderSliver? child__6162 = lastChild;
        while ((child__6162 is not null))
        {
            global::Doroti.Flutter.Ui.Offset paintOffset__6228 = (((SliverPhysicalParentData?)(object?)child__6162.parentData!)!).paintOffset;
            bool isHit__6320 = result.addWithAxisOffset(mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition, paintOffset: paintOffset__6228, mainAxisOffset: childMainAxisPosition(child__6162), crossAxisOffset: childCrossAxisPosition(child__6162), hitTest: (Func<SliverHitTestResult, double, double, bool>)((RenderSliver)child__6162).hitTest);
            if (isHit__6320)
            {
                return true;
            }
            child__6162 = childBefore(child__6162);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData__173585 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData__173981 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderSliver))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderSliver child, RenderSliver? after = null)
    {
        var childParentData__175971 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((SliverPhysicalContainerParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((SliverPhysicalContainerParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((SliverPhysicalContainerParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((SliverPhysicalContainerParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is SliverPhysicalContainerParentData));
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderSliver child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderSliver>? children)
    {
        children?.forEach(this.add);
    }

    public virtual void _removeFromChildList(RenderSliver child)
    {
        var childParentData__179226 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((SliverPhysicalContainerParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((SliverPhysicalContainerParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderSliver child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderSliver? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((SliverPhysicalContainerParentData?)(object?)child__180623.parentData!)!;
            RenderSliver? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__181479 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
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
        RenderSliver? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((SliverPhysicalContainerParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderSliver? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((SliverPhysicalContainerParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderSliver? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((SliverPhysicalContainerParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderSliver? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((SliverPhysicalContainerParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderSliver? firstChild => this._firstChild;
    public virtual RenderSliver? lastChild => this._lastChild;
    public virtual RenderSliver? childBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? childAfter(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderSliver child__183606 = this.firstChild!;
            var count__183637 = 1L;
            while (true)
            {
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((SliverPhysicalContainerParentData?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Sliver_groupLibrary
{
    internal static bool _assertOutOfExtent(double extent)
    {
        if ((extent <= 0.0))
        {
            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("SliverCrossAxisGroup ran out of extent before child could be laid out."), new ErrorDescription("SliverCrossAxisGroup lays out any slivers with a constrained cross " + "axis before laying out those which expand. In this case, cross axis " + "extent was used up before the next sliver could be laid out."), new ErrorHint("Make sure that the total amount of extent allocated by constrained " + "child slivers does not exceed the cross axis extent that is available " + "for the SliverCrossAxisGroup.") });
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class RenderSliverMainAxisGroup : RenderSliver, ContainerRenderObjectMixin<RenderSliver, SliverPhysicalContainerParentData>
{
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderSliver? _firstChild { get; set; } = default;
    public virtual RenderSliver? _lastChild { get; set; } = default;

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverPhysicalContainerParentData))
        {
            child.parentData = new SliverPhysicalContainerParentData();
        }
    }

    public override double? childScrollOffset(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
        DartRuntimePrimitives.Assert(() => (child is RenderSliver));
        double extentOfPinnedSlivers__8926 = _maxScrollObstructionExtentBefore(((RenderSliver?)(object?)child)!);
        GrowthDirection growthDirection__9034 = ((SliverConstraints)constraints).growthDirection;
        switch (growthDirection__9034)
        {
            case GrowthDirection.forward:
                {
                    var childScrollOffset__9160 = 0.0;
                    RenderSliver? current__9207 = childBefore(((RenderSliver)child));
                    while ((current__9207 is not null))
                    {
                        childScrollOffset__9160 += ((RenderSliver)current__9207).geometry!.scrollExtent;
                        current__9207 = childBefore(current__9207);
                    }
                    return (childScrollOffset__9160 - extentOfPinnedSlivers__8926);
                }
            case GrowthDirection.reverse:
                {
                    var childScrollOffset__9492 = 0.0;
                    RenderSliver? current__9539 = childAfter(((RenderSliver)child));
                    while ((current__9539 is not null))
                    {
                        childScrollOffset__9492 -= ((RenderSliver)current__9539).geometry!.scrollExtent;
                        current__9539 = childAfter(current__9539);
                    }
                    return (childScrollOffset__9492 - extentOfPinnedSlivers__8926);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _maxScrollObstructionExtentBefore(RenderSliver child)
    {
        GrowthDirection growthDirection__9876 = ((RenderSliver)child).constraints.growthDirection;
        switch (growthDirection__9876)
        {
            case GrowthDirection.forward:
                {
                    var pinnedExtent__10008 = 0.0;
                    RenderSliver? current__10050 = firstChild;
                    while ((!object.Equals(current__10050, child)))
                    {
                        pinnedExtent__10008 += current__10050!.geometry!.maxScrollObstructionExtent;
                        current__10050 = childAfter(current__10050);
                    }
                    return pinnedExtent__10008;
                }
            case GrowthDirection.reverse:
                {
                    var pinnedExtent__10308 = 0.0;
                    RenderSliver? current__10350 = lastChild;
                    while ((!object.Equals(current__10350, child)))
                    {
                        pinnedExtent__10308 += current__10350!.geometry!.maxScrollObstructionExtent;
                        current__10350 = childBefore(current__10350);
                    }
                    return pinnedExtent__10308;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        var childParentData__10646 = ((SliverPhysicalParentData?)(object?)__child.parentData!)!;
        return (global::Doroti.Generated.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((RenderSliver)__child).constraints.axisDirection, ((RenderSliver)__child).constraints.growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.down => ((SliverPhysicalParentData)childParentData__10646).paintOffset.dy, global::Doroti.Generated.Framework.Painting.AxisDirection.right => ((SliverPhysicalParentData)childParentData__10646).paintOffset.dx, global::Doroti.Generated.Framework.Painting.AxisDirection.up => ((geometry!.paintExtent - ((RenderSliver)__child).geometry!.paintExtent) - ((SliverPhysicalParentData)childParentData__10646).paintOffset.dy), global::Doroti.Generated.Framework.Painting.AxisDirection.left => ((geometry!.paintExtent - ((RenderSliver)__child).geometry!.paintExtent) - ((SliverPhysicalParentData)childParentData__10646).paintOffset.dx), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childCrossAxisPosition(RenderObject child) => 0.0;
    public override void performLayout()
    {
        double scrollOffset__11352 = 0;
        double layoutOffset__11381 = 0;
        double maxPaintExtent__11410 = 0;
        double paintOffset__11441 = ((SliverConstraints)constraints).overlap;
        double maxScrollObstructionExtent__11487 = 0;
        double cacheOrigin__11531 = ((SliverConstraints)constraints).cacheOrigin;
        double remainingCacheExtent__11581 = ((SliverConstraints)constraints).remainingCacheExtent;
        var (leadingChild__11671, advance__11734) = (((SliverConstraints)constraints).growthDirection switch { GrowthDirection.forward => (((RenderSliver?, Func<RenderSliver, RenderSliver?>))((firstChild, childAfter))), GrowthDirection.reverse => (((RenderSliver?, Func<RenderSliver, RenderSliver?>))((lastChild, childBefore))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        var child__11923 = leadingChild__11671;
        while ((child__11923 is not null))
        {
            double beforeOffsetPaintExtent__11992 = calculatePaintOffset(constraints, from: 0.0, to: scrollOffset__11352);
            double childScrollOffset__12135 = Math.Max(0.0, (((SliverConstraints)constraints).scrollOffset - scrollOffset__11352));
            double correctedCacheOrigin__12230 = Math.Max(cacheOrigin__11531, -childScrollOffset__12135);
            double cacheExtentCorrection__12315 = (cacheOrigin__11531 - correctedCacheOrigin__12230);
            child__11923.layout(constraints.copyWith(scrollOffset: childScrollOffset__12135, cacheOrigin: correctedCacheOrigin__12230, overlap: Math.Max(0.0, _fixPrecisionError((paintOffset__11441 - beforeOffsetPaintExtent__11992))), remainingPaintExtent: _fixPrecisionError((((SliverConstraints)constraints).remainingPaintExtent - beforeOffsetPaintExtent__11992)), remainingCacheExtent: Math.Max(0.0, _fixPrecisionError((remainingCacheExtent__11581 + cacheExtentCorrection__12315))), precedingScrollExtent: (scrollOffset__11352 + ((SliverConstraints)constraints).precedingScrollExtent)), parentUsesSize: true);
            SliverGeometry childLayoutGeometry__13055 = ((RenderSliver)child__11923).geometry!;
            double? scrollOffsetCorrection__13115 = ((SliverGeometry)childLayoutGeometry__13055).scrollOffsetCorrection;
            if ((scrollOffsetCorrection__13115 is not null))
            {
                double scrollOffsetCorrection__13115__value13194 = DartRuntimePrimitives.RequireValue(scrollOffsetCorrection__13115);
                geometry = new SliverGeometry(scrollOffsetCorrection: DartRuntimePrimitives.RequireValue(scrollOffsetCorrection__13115__value13194));
                return;
            }
            DartRuntimePrimitives.Assert(() => childLayoutGeometry__13055.debugAssertIsValid());
            double childPaintOffset__13412 = (layoutOffset__11381 + ((SliverGeometry)childLayoutGeometry__13055).paintOrigin);
            var childParentData__13491 = ((SliverPhysicalParentData?)(object?)child__11923.parentData!)!;
            childParentData__13491.paintOffset = (((SliverConstraints)constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Flutter.Ui.Offset(0.0, childPaintOffset__13412), global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Flutter.Ui.Offset(childPaintOffset__13412, 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            scrollOffset__11352 += ((SliverGeometry)childLayoutGeometry__13055).scrollExtent;
            layoutOffset__11381 += ((SliverGeometry)childLayoutGeometry__13055).layoutExtent;
            maxPaintExtent__11410 += ((SliverGeometry)childLayoutGeometry__13055).maxPaintExtent;
            maxScrollObstructionExtent__11487 += ((SliverGeometry)childLayoutGeometry__13055).maxScrollObstructionExtent;
            paintOffset__11441 = Math.Max((childPaintOffset__13412 + ((SliverGeometry)childLayoutGeometry__13055).paintExtent), paintOffset__11441);
            if ((((SliverGeometry)childLayoutGeometry__13055).cacheExtent != 0.0))
            {
                remainingCacheExtent__11581 = _fixPrecisionError(((remainingCacheExtent__11581 - ((SliverGeometry)childLayoutGeometry__13055).cacheExtent) - cacheExtentCorrection__12315));
                cacheOrigin__11531 = Math.Min((correctedCacheOrigin__12230 + ((SliverGeometry)childLayoutGeometry__13055).cacheExtent), 0.0);
            }
            child__11923 = advance__11734(child__11923);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (((child__11923 is not null) && double.IsInfinity(maxPaintExtent__11410)))
                    {
                        throw new FlutterError("Unreachable sliver found, you may have a sliver following " + "a sliver with an infinite extent. ");
                    }
                    return true;
                });
        }
        double remainingExtent__14738 = Math.Max(0, (scrollOffset__11352 - ((SliverConstraints)constraints).scrollOffset));
        if ((paintOffset__11441 > remainingExtent__14738))
        {
            bool pinnedChildrenOverflow__15094 = (maxScrollObstructionExtent__11487 > (remainingExtent__14738 - ((SliverConstraints)constraints).overlap));
            double paintCorrection__15216 = (paintOffset__11441 - remainingExtent__14738);
            paintOffset__11441 = remainingExtent__14738;
            child__11923 = firstChild;
            while ((child__11923 is not null))
            {
                SliverGeometry childLayoutGeometry__15387 = ((RenderSliver)child__11923).geometry!;
                var childParentData__15440 = ((SliverPhysicalParentData?)(object?)child__11923.parentData!)!;
                double childMainAxisPaintOffset__15526 = (((SliverConstraints)constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => ((SliverPhysicalParentData)childParentData__15440).paintOffset.dy, global::Doroti.Generated.Framework.Painting.Axis.horizontal => ((SliverPhysicalParentData)childParentData__15440).paintOffset.dx, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                double childPaintEnd__15733 = (childMainAxisPaintOffset__15526 + ((SliverGeometry)childLayoutGeometry__15387).paintExtent);
                bool childIsPinned__15828 = (((SliverGeometry)childLayoutGeometry__15387).maxScrollObstructionExtent > 0L);
                if (((childPaintEnd__15733 > remainingExtent__14738) || ((pinnedChildrenOverflow__15094 && childIsPinned__15828))))
                {
                    childParentData__15440.paintOffset = (((SliverConstraints)constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.vertical => new global::Doroti.Flutter.Ui.Offset(0.0, (((SliverPhysicalParentData)childParentData__15440).paintOffset.dy - paintCorrection__15216)), global::Doroti.Generated.Framework.Painting.Axis.horizontal => new global::Doroti.Flutter.Ui.Offset((((SliverPhysicalParentData)childParentData__15440).paintOffset.dx - paintCorrection__15216), 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                }
                child__11923 = childAfter(child__11923);
            }
        }
        double cacheExtent__16332 = calculateCacheOffset(constraints, from: Math.Min(((SliverConstraints)constraints).scrollOffset, 0), to: scrollOffset__11352);
        double paintExtent__16486 = Dart_uiLibrary.clampDouble(paintOffset__11441, 0, ((SliverConstraints)constraints).remainingPaintExtent);
        geometry = new SliverGeometry(scrollExtent: scrollOffset__11352, paintExtent: paintExtent__16486, cacheExtent: cacheExtent__16332, maxPaintExtent: maxPaintExtent__11410, hasVisualOverflow: ((scrollOffset__11352 > ((SliverConstraints)constraints).remainingPaintExtent) || (((SliverConstraints)constraints).scrollOffset > 0.0)));
        child__11923 = leadingChild__11671;
        while ((child__11923 is not null))
        {
            var childParentData__17055 = ((SliverPhysicalParentData?)(object?)child__11923.parentData!)!;
            childParentData__17055.paintOffset = (global::Doroti.Generated.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Generated.Framework.Painting.AxisDirection.up => new global::Doroti.Flutter.Ui.Offset(0.0, ((paintExtent__16486 - ((SliverPhysicalParentData)childParentData__17055).paintOffset.dy) - ((RenderSliver)child__11923).geometry!.paintExtent)), global::Doroti.Generated.Framework.Painting.AxisDirection.left => new global::Doroti.Flutter.Ui.Offset(((paintExtent__16486 - ((SliverPhysicalParentData)childParentData__17055).paintOffset.dx) - ((RenderSliver)child__11923).geometry!.paintExtent), 0.0), global::Doroti.Generated.Framework.Painting.AxisDirection.right => ((SliverPhysicalParentData)childParentData__17055).paintOffset, global::Doroti.Generated.Framework.Painting.AxisDirection.down => ((SliverPhysicalParentData)childParentData__17055).paintOffset, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            child__11923 = advance__11734(child__11923);
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderSliver? child__17799 = lastChild;
        while ((child__17799 is not null))
        {
            if (((RenderSliver)child__17799).geometry!.visible)
            {
                var childParentData__17897 = ((SliverPhysicalParentData?)(object?)child__17799.parentData!)!;
                context.paintChild(child__17799, (offset + ((SliverPhysicalParentData)childParentData__17897).paintOffset));
            }
            child__17799 = childBefore(child__17799);
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderSliver)(object)child;
        var childParentData__18178 = ((SliverPhysicalParentData?)(object?)__child.parentData!)!;
        childParentData__18178.applyPaintTransform(transform);
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        RenderSliver? child__18472 = firstChild;
        while ((child__18472 is not null))
        {
            global::Doroti.Flutter.Ui.Offset paintOffset__18539 = (((SliverPhysicalParentData?)(object?)child__18472.parentData!)!).paintOffset;
            bool isHit__18631 = result.addWithAxisOffset(mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition, paintOffset: paintOffset__18539, mainAxisOffset: childMainAxisPosition(child__18472), crossAxisOffset: childCrossAxisPosition(child__18472), hitTest: (Func<SliverHitTestResult, double, double, bool>)((RenderSliver)child__18472).hitTest);
            if (isHit__18631)
            {
                return true;
            }
            child__18472 = childAfter(child__18472);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        RenderSliver? child__19144 = firstChild;
        while ((child__19144 is not null))
        {
            if (((((RenderSliver)child__19144).geometry!.visible || (((RenderSliver)child__19144).geometry!.cacheExtent > 0.0)) || ((RenderSliver)child__19144).ensureSemantics))
            {
                visitor(child__19144);
            }
            child__19144 = childAfter(child__19144);
        }
    }

    internal static double _fixPrecisionError(double number)
    {
        return ((number.abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance) ? 0.0 : number);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData__173585 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData__173981 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => this._childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderSliver))
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((RenderObject)child).debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderSliver child, RenderSliver? after = null)
    {
        var childParentData__175971 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((SliverPhysicalContainerParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((SliverPhysicalContainerParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((SliverPhysicalContainerParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((SliverPhysicalContainerParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
            }
        }
    }

    public virtual void insert(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._firstChild)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this._lastChild)));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is SliverPhysicalContainerParentData));
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderSliver child)
    {
        insert(child, after: this._lastChild);
    }

    public virtual void addAll(List<RenderSliver>? children)
    {
        children?.forEach(this.add);
    }

    public virtual void _removeFromChildList(RenderSliver child)
    {
        var childParentData__179226 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((SliverPhysicalContainerParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((SliverPhysicalContainerParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderSliver child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderSliver? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((SliverPhysicalContainerParentData?)(object?)child__180623.parentData!)!;
            RenderSliver? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
        }
        this._firstChild = null;
        this._lastChild = null;
        this._childCount = 0L;
    }

    public virtual void move(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(after, this)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(child, after)));
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__181479 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
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
        RenderSliver? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((SliverPhysicalContainerParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderSliver? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((SliverPhysicalContainerParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderSliver? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((SliverPhysicalContainerParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderSliver? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((SliverPhysicalContainerParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderSliver? firstChild => this._firstChild;
    public virtual RenderSliver? lastChild => this._lastChild;
    public virtual RenderSliver? childBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? childAfter(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__183528 = new List<DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderSliver child__183606 = this.firstChild!;
            var count__183637 = 1L;
            while (true)
            {
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((SliverPhysicalContainerParentData?)(object?)child__183606.parentData!)!;
                child__183606 = childParentData__183833.nextSibling!;
            }
        }
        return children__183528;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

