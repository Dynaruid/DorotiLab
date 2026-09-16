// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/sliver_resizing_header.dart
using Doroti.Runtime;
using Doroti.Ui;

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
        return (extentPrototype is not null) ? new ExcludeFocus(child: extentPrototype) : null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new _SliverResizingHeader__sliver_resizing_header(minExtentPrototype: _excludeFocus(minExtentPrototype), maxExtentPrototype: _excludeFocus(maxExtentPrototype), child: new Semantics(container: true, explicitChildNodes: true, child: child ?? SizedBox.CreateShrink()));
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

    public override IEnumerable<_Slot__sliver_resizing_header> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_Slot__sliver_resizing_header>>(Enum.GetValues<_Slot__sliver_resizing_header>().ToList());
    public override Widget? childForSlot(_Slot__sliver_resizing_header slot)
    {
        return slot switch { _Slot__sliver_resizing_header.minExtent => minExtentPrototype, _Slot__sliver_resizing_header.maxExtent => maxExtentPrototype, _Slot__sliver_resizing_header.child => child, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSliverResizingHeader__sliver_resizing_header();
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
        DartRuntimePrimitives.Assert(() => box.hasSize);
        return constraints.axis switch { Axis.vertical => box.size.height, Axis.horizontal => box.size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double childExtent => (child is null) ? 0 : boxExtent(child!);
    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        if (child.parentData is not SliverPhysicalParentData)
        {
            child.parentData = new global::Doroti.Framework.Rendering.SliverPhysicalParentData();
        }
    }

    public virtual void setChildParentData(global::Doroti.Framework.Rendering.RenderObject child, global::Doroti.Framework.Rendering.SliverConstraints constraints, global::Doroti.Framework.Rendering.SliverGeometry geometry)
    {
        var childParentData = ((global::Doroti.Framework.Rendering.SliverPhysicalParentData?)child.parentData!)!;
        global::Doroti.Framework.Painting.AxisDirection direction = SliverLibrary.applyGrowthDirectionToAxisDirection(constraints.axisDirection, constraints.growthDirection);
        childParentData.paintOffset = direction switch { AxisDirection.up => new global::Doroti.Ui.Offset(0.0, -(geometry.scrollExtent - (geometry.paintExtent + constraints.scrollOffset))), AxisDirection.right => new global::Doroti.Ui.Offset(-constraints.scrollOffset, 0.0), AxisDirection.down => new global::Doroti.Ui.Offset(0.0, -constraints.scrollOffset), AxisDirection.left => new global::Doroti.Ui.Offset(-(geometry.scrollExtent - (geometry.paintExtent + constraints.scrollOffset)), 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
    }

    public override double childMainAxisPosition(global::Doroti.Framework.Rendering.RenderObject child) => 0;
    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.SliverConstraints constraintsLocal = constraints;
        global::Doroti.Framework.Rendering.BoxConstraints prototypeBoxConstraints = constraintsLocal.asBoxConstraints();
        double minExtentLocal = 0;
        if (minExtentPrototype is not null)
        {
            minExtentPrototype!.layout(prototypeBoxConstraints, parentUsesSize: true);
            minExtentLocal = boxExtent(minExtentPrototype!);
        }
        double maxExtentLocal = default!;
        if (maxExtentPrototype is not null)
        {
            maxExtentPrototype!.layout(prototypeBoxConstraints, parentUsesSize: true);
            maxExtentLocal = boxExtent(maxExtentPrototype!);
        }
        else
        {
            global::Doroti.Ui.Size childSize = child!.getDryLayout(prototypeBoxConstraints);
            maxExtentLocal = constraintsLocal.axis switch { Axis.vertical => childSize.height, Axis.horizontal => childSize.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
        double scrollOffsetLocal = constraintsLocal.scrollOffset;
        double shrinkOffset = Math.Min(scrollOffsetLocal, maxExtentLocal);
        global::Doroti.Framework.Rendering.BoxConstraints boxConstraints = constraintsLocal.asBoxConstraints(minExtent: minExtentLocal, maxExtent: Math.Max(minExtentLocal, maxExtentLocal - shrinkOffset));
        child?.layout(boxConstraints, parentUsesSize: true);
        double remainingPaintExtentLocal = constraintsLocal.remainingPaintExtent;
        double layoutExtentLocal = Math.Min(childExtent, maxExtentLocal - scrollOffsetLocal);
        geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: maxExtentLocal, paintOrigin: constraintsLocal.overlap, paintExtent: Math.Min(childExtent, remainingPaintExtentLocal), layoutExtent: Dart_uiLibrary.clampDouble(layoutExtentLocal, 0, remainingPaintExtentLocal), maxPaintExtent: childExtent, maxScrollObstructionExtent: minExtentLocal, cacheExtent: calculateCacheOffset(constraintsLocal, from: 0.0, to: childExtent), hasVisualOverflow: true);
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var childParentData = ((global::Doroti.Framework.Rendering.SliverPhysicalParentData?)child.parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((child is not null) && geometry!.visible)
        {
            var childParentData = ((global::Doroti.Framework.Rendering.SliverPhysicalParentData?)child!.parentData!)!;
            context.paintChild(child!, offset + childParentData.paintOffset);
        }
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        DartRuntimePrimitives.Assert(() => geometry!.hitTestExtent > 0.0);
        if (child is not null)
        {
            return hitTestBoxChild(BoxHitTestResult.CreateWrap(result), child!, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if ((geometry is not null) && (geometry!.layoutExtent < childExtent))
        {
            config.addTagForChildren(RenderViewport.excludeFromScrolling);
        }
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox? childForSlot(_Slot__sliver_resizing_header slot) => _slotToChild.GetValueOrDefault(slot);
    public virtual string debugNameForSlot(_Slot__sliver_resizing_header slot)
    {
        return slot.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            child.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            child.detach();
        }
    }

    public override void redepthChildren()
    {
        children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        children.forEach((__arg0) => visitor(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var childToSlot = new DartMap<global::Doroti.Framework.Rendering.RenderBox, _Slot__sliver_resizing_header>(_slotToChild.Values, _slotToChild.Keys);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            _addDiagnostics(child, value, debugNameForSlot(DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_Slot__sliver_resizing_header>(childToSlot, child))));
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
        global::Doroti.Framework.Rendering.RenderBox? oldChild = _slotToChild.GetValueOrDefault(slot);
        if (oldChild is not null)
        {
            dropChild(oldChild);
            _slotToChild.remove(slot);
        }
        if (child is not null)
        {
            _slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, _Slot__sliver_resizing_header slot, _Slot__sliver_resizing_header oldSlot)
    {
        DartRuntimePrimitives.Assert(() => !Equals(slot, oldSlot));
        global::Doroti.Framework.Rendering.RenderBox? oldChild = _slotToChild.GetValueOrDefault(oldSlot);
        if (Equals(oldChild, child))
        {
            _setChild(null, oldSlot);
        }
        _setChild(child, slot);
    }

    public virtual bool _getRightWayUp(SliverConstraints constraints)
    {
        bool reversed = Basic_typesLibrary.axisDirectionIsReversed(constraints.axisDirection);
        return constraints.growthDirection switch { GrowthDirection.forward => !reversed, GrowthDirection.reverse => reversed, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestBoxChild(BoxHitTestResult result, RenderBox child, double mainAxisPosition, double crossAxisPosition)
    {
        bool rightWayUp = _getRightWayUp(constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        double absolutePosition = mainAxisPosition - delta;
        double absoluteCrossAxisPosition = crossAxisPosition - crossAxisDelta;
        global::Doroti.Ui.Offset paintOffsetLocal = default!;
        global::Doroti.Ui.Offset transformedPosition = default!;
        switch (constraints.axis)
        {
            case Axis.horizontal:
                {
                    if (!rightWayUp)
                    {
                        absolutePosition = child.size.width - absolutePosition;
                        delta = geometry!.paintExtent - child.size.width - delta;
                    }
                    paintOffsetLocal = new global::Doroti.Ui.Offset(delta, crossAxisDelta);
                    transformedPosition = new global::Doroti.Ui.Offset(absolutePosition, absoluteCrossAxisPosition);
                    break;
                }
            case Axis.vertical:
                {
                    if (!rightWayUp)
                    {
                        absolutePosition = child.size.height - absolutePosition;
                        delta = geometry!.paintExtent - child.size.height - delta;
                    }
                    paintOffsetLocal = new global::Doroti.Ui.Offset(crossAxisDelta, delta);
                    transformedPosition = new global::Doroti.Ui.Offset(absoluteCrossAxisPosition, absolutePosition);
                    break;
                }
        }
        return result.addWithOutOfBandPosition(paintOffset: paintOffsetLocal, hitTest: (result) =>
        {
            return child.hitTest(result, position: transformedPosition);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyPaintTransformForBoxChild(RenderBox child, Matrix4 transform)
    {
        bool rightWayUp = _getRightWayUp(constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        switch (constraints.axis)
        {
            case Axis.horizontal:
                {
                    if (!rightWayUp)
                    {
                        delta = geometry!.paintExtent - child.size.width - delta;
                    }
                    transform.translateByDouble(delta, crossAxisDelta, 0, 1);
                    break;
                }
            case Axis.vertical:
                {
                    if (!rightWayUp)
                    {
                        delta = geometry!.paintExtent - child.size.height - delta;
                    }
                    transform.translateByDouble(crossAxisDelta, delta, 0, 1);
                    break;
                }
        }
    }

}
