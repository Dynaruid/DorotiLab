// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_resizing_header.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class SliverResizingHeader : StatelessWidget
{
    public virtual Widget? minExtentPrototype { get; private set; }
    public virtual Widget? maxExtentPrototype { get; private set; }
    public virtual Widget? child { get; private set; }

    public SliverResizingHeader(global::Doroti.Framework.Foundation.Key? key = null, Widget? minExtentPrototype = null, Widget? maxExtentPrototype = null, Widget? child = null) : base(key: key)
    {
        this.minExtentPrototype = minExtentPrototype;
        this.maxExtentPrototype = maxExtentPrototype;
        this.child = child;
    }

    internal virtual Widget? _excludeFocus(Widget? extentPrototype)
    {
        return ((Widget?)(object?)((extentPrototype is not null) ? new ExcludeFocus(child: extentPrototype) : null));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new _SliverResizingHeader__sliver_resizing_header(minExtentPrototype: _excludeFocus(this.minExtentPrototype), maxExtentPrototype: _excludeFocus(this.maxExtentPrototype), child: new Semantics(container: true, explicitChildNodes: true, child: (this.child ?? SizedBox.CreateShrink()))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _Slot__sliver_resizing_header
{
    minExtent,
    maxExtent,
    child
}

internal class _SliverResizingHeader__sliver_resizing_header : SlottedMultiChildRenderObjectWidget<_Slot__sliver_resizing_header, global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual Widget? minExtentPrototype { get; private set; }
    public virtual Widget? maxExtentPrototype { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    internal _SliverResizingHeader__sliver_resizing_header(Widget? minExtentPrototype = null, Widget? maxExtentPrototype = null, Widget child = default!)
    {
        this.minExtentPrototype = minExtentPrototype;
        this.maxExtentPrototype = maxExtentPrototype;
        this.child = child;
    }

    public override IEnumerable<_Slot__sliver_resizing_header> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_Slot__sliver_resizing_header>>(System.Enum.GetValues<_Slot__sliver_resizing_header>().ToList());
    public override Widget? childForSlot(_Slot__sliver_resizing_header slot)
    {
        return (slot switch { _Slot__sliver_resizing_header.minExtent => this.minExtentPrototype, _Slot__sliver_resizing_header.maxExtent => this.maxExtentPrototype, _Slot__sliver_resizing_header.child => this.child, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverResizingHeader__sliver_resizing_header());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _RenderSliverResizingHeader__sliver_resizing_header : global::Doroti.Framework.Rendering.RenderSliver, SlottedContainerRenderObjectMixin<_Slot__sliver_resizing_header, global::Doroti.Framework.Rendering.RenderBox>, global::Doroti.Framework.Rendering.RenderSliverHelpers
{
    public virtual DartMap<_Slot__sliver_resizing_header, global::Doroti.Framework.Rendering.RenderBox> _slotToChild { get; set; } = new DartMap<_Slot__sliver_resizing_header, global::Doroti.Framework.Rendering.RenderBox>();

    public virtual global::Doroti.Framework.Rendering.RenderBox? minExtentPrototype => childForSlot(_Slot__sliver_resizing_header.minExtent);
    public virtual global::Doroti.Framework.Rendering.RenderBox? maxExtentPrototype => childForSlot(_Slot__sliver_resizing_header.maxExtent);
    public virtual global::Doroti.Framework.Rendering.RenderBox? child => childForSlot(DartRuntimePrimitives.RequireValue(_Slot__sliver_resizing_header.child));
    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> children => DartRuntimePrimitives.ConvertValue<IEnumerable<global::Doroti.Framework.Rendering.RenderBox>>(new List<global::Doroti.Framework.Rendering.RenderBox>());
    public virtual double boxExtent(global::Doroti.Framework.Rendering.RenderBox box)
    {
        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)box).hasSize);
        return (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).axis switch { global::Doroti.Framework.Painting.Axis.vertical => ((global::Doroti.Framework.Rendering.RenderBox)box).size.height, global::Doroti.Framework.Painting.Axis.horizontal => ((global::Doroti.Framework.Rendering.RenderBox)box).size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double childExtent => ((this.child is null) ? 0 : boxExtent(this.child!));
    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        if ((((global::Doroti.Framework.Rendering.ParentData?)((dynamic)child).parentData) is not global::Doroti.Framework.Rendering.SliverPhysicalParentData))
        {
            ((dynamic)child).parentData = new global::Doroti.Framework.Rendering.SliverPhysicalParentData();
        }
    }

    public virtual void setChildParentData(global::Doroti.Framework.Rendering.RenderObject child, global::Doroti.Framework.Rendering.SliverConstraints constraints, global::Doroti.Framework.Rendering.SliverGeometry geometry)
    {
        var childParentData = ((global::Doroti.Framework.Rendering.SliverPhysicalParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)child).parentData)!)!;
        global::Doroti.Framework.Painting.AxisDirection direction = global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((global::Doroti.Framework.Rendering.SliverConstraints)constraints).axisDirection, ((global::Doroti.Framework.Rendering.SliverConstraints)constraints).growthDirection);
        childParentData.paintOffset = (direction switch { global::Doroti.Framework.Painting.AxisDirection.up => new global::Doroti.Ui.Offset(0.0, -((((global::Doroti.Framework.Rendering.SliverGeometry)geometry).scrollExtent - ((((global::Doroti.Framework.Rendering.SliverGeometry)geometry).paintExtent + ((global::Doroti.Framework.Rendering.SliverConstraints)constraints).scrollOffset))))), global::Doroti.Framework.Painting.AxisDirection.right => new global::Doroti.Ui.Offset(-((global::Doroti.Framework.Rendering.SliverConstraints)constraints).scrollOffset, 0.0), global::Doroti.Framework.Painting.AxisDirection.down => new global::Doroti.Ui.Offset(0.0, -((global::Doroti.Framework.Rendering.SliverConstraints)constraints).scrollOffset), global::Doroti.Framework.Painting.AxisDirection.left => new global::Doroti.Ui.Offset(-((((global::Doroti.Framework.Rendering.SliverGeometry)geometry).scrollExtent - ((((global::Doroti.Framework.Rendering.SliverGeometry)geometry).paintExtent + ((global::Doroti.Framework.Rendering.SliverConstraints)constraints).scrollOffset)))), 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    }

    public override double childMainAxisPosition(global::Doroti.Framework.Rendering.RenderObject child) => 0;
    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.SliverConstraints constraintsLocal = this.constraints;
        global::Doroti.Framework.Rendering.BoxConstraints prototypeBoxConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraintsLocal.asBoxConstraints());
        double minExtentLocal = 0;
        if ((this.minExtentPrototype is not null))
        {
            this.minExtentPrototype!.layout(prototypeBoxConstraints, parentUsesSize: true);
            minExtentLocal = boxExtent(this.minExtentPrototype!);
        }
        double maxExtentLocal = default!;
        if ((this.maxExtentPrototype is not null))
        {
            this.maxExtentPrototype!.layout(prototypeBoxConstraints, parentUsesSize: true);
            maxExtentLocal = boxExtent(this.maxExtentPrototype!);
        }
        else
        {
            global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)this.child!.getDryLayout(prototypeBoxConstraints));
            maxExtentLocal = (((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).axis switch { global::Doroti.Framework.Painting.Axis.vertical => childSize.height, global::Doroti.Framework.Painting.Axis.horizontal => childSize.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        double scrollOffsetLocal = ((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).scrollOffset;
        double shrinkOffset = Math.Min(scrollOffsetLocal, maxExtentLocal);
        global::Doroti.Framework.Rendering.BoxConstraints boxConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraintsLocal.asBoxConstraints(minExtent: minExtentLocal, maxExtent: Math.Max(minExtentLocal, (maxExtentLocal - shrinkOffset))));
        this.child?.layout(boxConstraints, parentUsesSize: true);
        double remainingPaintExtentLocal = ((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).remainingPaintExtent;
        double layoutExtentLocal = Math.Min(this.childExtent, (maxExtentLocal - scrollOffsetLocal));
        geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: maxExtentLocal, paintOrigin: ((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).overlap, paintExtent: Math.Min(this.childExtent, remainingPaintExtentLocal), layoutExtent: Dart_uiLibrary.clampDouble(layoutExtentLocal, 0, remainingPaintExtentLocal), maxPaintExtent: this.childExtent, maxScrollObstructionExtent: minExtentLocal, cacheExtent: calculateCacheOffset(constraintsLocal, from: 0.0, to: this.childExtent), hasVisualOverflow: true);
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var childParentData = ((global::Doroti.Framework.Rendering.SliverPhysicalParentData?)(object?)((global::Doroti.Framework.Rendering.ParentData?)((dynamic)child).parentData)!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (((this.child is not null) && this.geometry!.visible))
        {
            var childParentData = ((global::Doroti.Framework.Rendering.SliverPhysicalParentData?)(object?)this.child!.parentData!)!;
            context.paintChild(this.child!, (offset + ((global::Doroti.Framework.Rendering.SliverPhysicalParentData)childParentData).paintOffset));
        }
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        DartRuntimePrimitives.Assert(() => (this.geometry!.hitTestExtent > 0.0));
        if ((this.child is not null))
        {
            return hitTestBoxChild(global::Doroti.Framework.Rendering.BoxHitTestResult.CreateWrap(result), this.child!, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if (((this.geometry is not null) && (this.geometry!.layoutExtent < this.childExtent)))
        {
            config.addTagForChildren(global::Doroti.Framework.Rendering.RenderViewport.excludeFromScrolling);
        }
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? childForSlot(_Slot__sliver_resizing_header slot) => this._slotToChild.GetValueOrDefault(slot);
    public virtual string debugNameForSlot(_Slot__sliver_resizing_header slot)
    {
        if (true)
        {
            return slot.ToString();
        }
        return slot.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            ((dynamic)child).attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            ((dynamic)child).detach();
        }
    }

    public override void redepthChildren()
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)this.redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var childToSlot = new DartMap<global::Doroti.Framework.Rendering.RenderBox, _Slot__sliver_resizing_header>(this._slotToChild.Values, this._slotToChild.Keys);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            _addDiagnostics(child, value, debugNameForSlot(((_Slot__sliver_resizing_header)DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_Slot__sliver_resizing_header>(childToSlot, child)))));
        }
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _addDiagnostics(global::Doroti.Framework.Rendering.RenderBox child, List<global::Doroti.Framework.Foundation.DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(global::Doroti.Framework.Rendering.RenderBox? child, _Slot__sliver_resizing_header slot)
    {
        global::Doroti.Framework.Rendering.RenderBox? oldChild = this._slotToChild.GetValueOrDefault(slot);
        if ((oldChild is not null))
        {
            dropChild(oldChild);
            this._slotToChild.remove(slot);
        }
        if ((child is not null))
        {
            this._slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, _Slot__sliver_resizing_header slot, _Slot__sliver_resizing_header oldSlot)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(slot, oldSlot)));
        global::Doroti.Framework.Rendering.RenderBox? oldChild = this._slotToChild.GetValueOrDefault(oldSlot);
        if ((object.Equals(oldChild, child)))
        {
            _setChild(((global::Doroti.Framework.Rendering.RenderBox)(object)null), oldSlot);
        }
        _setChild(child, slot);
    }

    public virtual bool _getRightWayUp(SliverConstraints constraints)
    {
        bool reversed = global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).axisDirection);
        return (((SliverConstraints)constraints).growthDirection switch { GrowthDirection.forward => !reversed, GrowthDirection.reverse => reversed, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestBoxChild(BoxHitTestResult result, RenderBox child, double mainAxisPosition, double crossAxisPosition)
    {
        bool rightWayUp = _getRightWayUp(this.constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        double absolutePosition = (mainAxisPosition - delta);
        double absoluteCrossAxisPosition = (crossAxisPosition - crossAxisDelta);
        global::Doroti.Ui.Offset paintOffsetLocal = default!;
        global::Doroti.Ui.Offset transformedPosition = default!;
        switch (((SliverConstraints)this.constraints).axis)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp)
                    {
                        absolutePosition = (((RenderBox)child).size.width - absolutePosition);
                        delta = ((this.geometry!.paintExtent - ((RenderBox)child).size.width) - delta);
                    }
                    paintOffsetLocal = new global::Doroti.Ui.Offset(delta, crossAxisDelta);
                    transformedPosition = new global::Doroti.Ui.Offset(absolutePosition, absoluteCrossAxisPosition);
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    if (!rightWayUp)
                    {
                        absolutePosition = (((RenderBox)child).size.height - absolutePosition);
                        delta = ((this.geometry!.paintExtent - ((RenderBox)child).size.height) - delta);
                    }
                    paintOffsetLocal = new global::Doroti.Ui.Offset(crossAxisDelta, delta);
                    transformedPosition = new global::Doroti.Ui.Offset(absoluteCrossAxisPosition, absolutePosition);
                    break;
                }
        }
        return result.addWithOutOfBandPosition(paintOffset: paintOffsetLocal, hitTest: ((global::System.Func<BoxHitTestResult, bool>)((result) =>
        {
            return child.hitTest(result, position: transformedPosition);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyPaintTransformForBoxChild(RenderBox child, Matrix4 transform)
    {
        bool rightWayUp = _getRightWayUp(this.constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        switch (((SliverConstraints)this.constraints).axis)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp)
                    {
                        delta = ((this.geometry!.paintExtent - ((RenderBox)child).size.width) - delta);
                    }
                    transform.translateByDouble(delta, crossAxisDelta, 0, 1);
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    if (!rightWayUp)
                    {
                        delta = ((this.geometry!.paintExtent - ((RenderBox)child).size.height) - delta);
                    }
                    transform.translateByDouble(crossAxisDelta, delta, 0, 1);
                    break;
                }
        }
    }

}

