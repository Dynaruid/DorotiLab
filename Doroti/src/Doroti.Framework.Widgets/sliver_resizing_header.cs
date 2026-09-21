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

    public SliverResizingHeader(
        Key? key = null,
        Widget? minExtentPrototype = null,
        Widget? maxExtentPrototype = null,
        Widget? child = null
    )
        : base(key: key)
    {
        this.minExtentPrototype = minExtentPrototype;
        this.maxExtentPrototype = maxExtentPrototype;
        this.child = child;
    }

    internal virtual Widget? _excludeFocus(Widget? extentPrototype)
    {
        return (extentPrototype is not null) ? new ExcludeFocus(child: extentPrototype) : null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new _SliverResizingHeader__sliver_resizing_header(
            minExtentPrototype: _excludeFocus(minExtentPrototype),
            maxExtentPrototype: _excludeFocus(maxExtentPrototype),
            child: new Semantics(
                container: true,
                explicitChildNodes: true,
                child: child ?? SizedBox.CreateShrink()
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal enum _Slot__sliver_resizing_header
{
    minExtent,
    maxExtent,
    child,
}

internal class _SliverResizingHeader__sliver_resizing_header
    : SlottedMultiChildRenderObjectWidget<_Slot__sliver_resizing_header, RenderBox>
{
    public virtual Widget? minExtentPrototype { get; private set; }
    public virtual Widget? maxExtentPrototype { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    internal _SliverResizingHeader__sliver_resizing_header(
        Widget? minExtentPrototype = null,
        Widget? maxExtentPrototype = null,
        Widget child = default!
    )
    {
        this.minExtentPrototype = minExtentPrototype;
        this.maxExtentPrototype = maxExtentPrototype;
        this.child = child;
    }

    public override IEnumerable<_Slot__sliver_resizing_header> slots =>
        DartRuntimePrimitives.ConvertValue<IEnumerable<_Slot__sliver_resizing_header>>(
            Enum.GetValues<_Slot__sliver_resizing_header>().ToList()
        );

    public override Widget? childForSlot(_Slot__sliver_resizing_header slot)
    {
        return slot switch
        {
            _Slot__sliver_resizing_header.minExtent => minExtentPrototype,
            _Slot__sliver_resizing_header.maxExtent => maxExtentPrototype,
            _Slot__sliver_resizing_header.child => child,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSliverResizingHeader__sliver_resizing_header();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _RenderSliverResizingHeader__sliver_resizing_header
    : RenderSliver,
        SlottedContainerRenderObjectMixin<_Slot__sliver_resizing_header, RenderBox>,
        RenderSliverHelpers
{
    public virtual DartMap<_Slot__sliver_resizing_header, RenderBox> _slotToChild { get; set; } =
        new DartMap<_Slot__sliver_resizing_header, RenderBox>();

    public virtual RenderBox? minExtentPrototype =>
        childForSlot(_Slot__sliver_resizing_header.minExtent);
    public virtual RenderBox? maxExtentPrototype =>
        childForSlot(_Slot__sliver_resizing_header.maxExtent);
    public virtual RenderBox? child => childForSlot((_Slot__sliver_resizing_header.child));
    public virtual IEnumerable<RenderBox> children =>
        DartRuntimePrimitives.ConvertValue<IEnumerable<RenderBox>>(new List<RenderBox>());

    public virtual double boxExtent(RenderBox box)
    {
        DartRuntimePrimitives.Assert(() => box.hasSize);
        return constraints.axis switch
        {
            Axis.vertical => box.size.height,
            Axis.horizontal => box.size.width,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double childExtent => (child is null) ? 0 : boxExtent(child!);

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverPhysicalParentData)
        {
            child.parentData = new SliverPhysicalParentData();
        }
    }

    public virtual void setChildParentData(
        RenderObject child,
        SliverConstraints constraints,
        SliverGeometry geometry
    )
    {
        var childParentData = ((SliverPhysicalParentData?)child.parentData!)!;
        AxisDirection direction = SliverLibrary.applyGrowthDirectionToAxisDirection(
            constraints.axisDirection,
            constraints.growthDirection
        );
        childParentData.paintOffset = direction switch
        {
            AxisDirection.up => new Offset(
                0.0,
                -(geometry.scrollExtent - (geometry.paintExtent + constraints.scrollOffset))
            ),
            AxisDirection.right => new Offset(-constraints.scrollOffset, 0.0),
            AxisDirection.down => new Offset(0.0, -constraints.scrollOffset),
            AxisDirection.left => new Offset(
                -(geometry.scrollExtent - (geometry.paintExtent + constraints.scrollOffset)),
                0.0
            ),
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
    }

    public override double childMainAxisPosition(RenderObject child) => 0;

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = constraints;
        BoxConstraints prototypeBoxConstraints = constraintsLocal.asBoxConstraints();
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
            Size childSize = child!.getDryLayout(prototypeBoxConstraints);
            maxExtentLocal = constraintsLocal.axis switch
            {
                Axis.vertical => childSize.height,
                Axis.horizontal => childSize.width,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
        }
        double scrollOffsetLocal = constraintsLocal.scrollOffset;
        double shrinkOffset = Math.Min(scrollOffsetLocal, maxExtentLocal);
        BoxConstraints boxConstraints = constraintsLocal.asBoxConstraints(
            minExtent: minExtentLocal,
            maxExtent: Math.Max(minExtentLocal, maxExtentLocal - shrinkOffset)
        );
        child?.layout(boxConstraints, parentUsesSize: true);
        double remainingPaintExtentLocal = constraintsLocal.remainingPaintExtent;
        double layoutExtentLocal = Math.Min(childExtent, maxExtentLocal - scrollOffsetLocal);
        geometry = new SliverGeometry(
            scrollExtent: maxExtentLocal,
            paintOrigin: constraintsLocal.overlap,
            paintExtent: Math.Min(childExtent, remainingPaintExtentLocal),
            layoutExtent: Dart_uiLibrary.clampDouble(
                layoutExtentLocal,
                0,
                remainingPaintExtentLocal
            ),
            maxPaintExtent: childExtent,
            maxScrollObstructionExtent: minExtentLocal,
            cacheExtent: calculateCacheOffset(constraintsLocal, from: 0.0, to: childExtent),
            hasVisualOverflow: true
        );
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var childParentData = ((SliverPhysicalParentData?)child.parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null) && geometry!.visible)
        {
            var childParentData = ((SliverPhysicalParentData?)child!.parentData!)!;
            context.paintChild(child!, offset + childParentData.paintOffset);
        }
    }

    public override bool hitTestChildren(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        DartRuntimePrimitives.Assert(() => geometry!.hitTestExtent > 0.0);
        if (child is not null)
        {
            return hitTestBoxChild(
                BoxHitTestResult.CreateWrap(result),
                child!,
                mainAxisPosition: mainAxisPosition,
                crossAxisPosition: crossAxisPosition
            );
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        if ((geometry is not null) && (geometry!.layoutExtent < childExtent))
        {
            config.addTagForChildren(RenderViewport.excludeFromScrolling);
        }
    }

    public virtual RenderBox? childForSlot(_Slot__sliver_resizing_header slot) =>
        _slotToChild.GetValueOrDefault(slot);

    public virtual string debugNameForSlot(_Slot__sliver_resizing_header slot)
    {
        return slot.ToString();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        foreach (RenderBox child in children)
        {
            child.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        foreach (RenderBox child in children)
        {
            child.detach();
        }
    }

    public override void redepthChildren()
    {
        children.forEach(
            (__arg0) =>
                ((Action<RenderObject>)redepthChild)(
                    DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0)
                )
        );
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        children.forEach(
            (__arg0) => visitor(DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0))
        );
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<DiagnosticsNode>();
        var childToSlot = new DartMap<RenderBox, _Slot__sliver_resizing_header>(
            _slotToChild.Values,
            _slotToChild.Keys
        );
        foreach (RenderBox child in children)
        {
            _addDiagnostics(
                child,
                value,
                debugNameForSlot(
                    (
                        DartCollectionRuntime.NullableMapValue<_Slot__sliver_resizing_header>(
                            childToSlot,
                            child
                        )
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            );
        }
        return value;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _addDiagnostics(RenderBox child, List<DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(RenderBox? child, _Slot__sliver_resizing_header slot)
    {
        RenderBox? oldChild = _slotToChild.GetValueOrDefault(slot);
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

    public virtual void _moveChild(
        RenderBox child,
        _Slot__sliver_resizing_header slot,
        _Slot__sliver_resizing_header oldSlot
    )
    {
        DartRuntimePrimitives.Assert(() => !Equals(slot, oldSlot));
        RenderBox? oldChild = _slotToChild.GetValueOrDefault(oldSlot);
        if (Equals(oldChild, child))
        {
            _setChild(null, oldSlot);
        }
        _setChild(child, slot);
    }

    public virtual bool _getRightWayUp(SliverConstraints constraints)
    {
        bool reversed = Basic_typesLibrary.axisDirectionIsReversed(constraints.axisDirection);
        return constraints.growthDirection switch
        {
            GrowthDirection.forward => !reversed,
            GrowthDirection.reverse => reversed,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool hitTestBoxChild(
        BoxHitTestResult result,
        RenderBox child,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        bool rightWayUp = _getRightWayUp(constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        double absolutePosition = mainAxisPosition - delta;
        double absoluteCrossAxisPosition = crossAxisPosition - crossAxisDelta;
        Offset paintOffsetLocal = default!;
        Offset transformedPosition = default!;
        switch (constraints.axis)
        {
            case Axis.horizontal:
            {
                if (!rightWayUp)
                {
                    absolutePosition = child.size.width - absolutePosition;
                    delta = geometry!.paintExtent - child.size.width - delta;
                }
                paintOffsetLocal = new Offset(delta, crossAxisDelta);
                transformedPosition = new Offset(absolutePosition, absoluteCrossAxisPosition);
                break;
            }
            case Axis.vertical:
            {
                if (!rightWayUp)
                {
                    absolutePosition = child.size.height - absolutePosition;
                    delta = geometry!.paintExtent - child.size.height - delta;
                }
                paintOffsetLocal = new Offset(crossAxisDelta, delta);
                transformedPosition = new Offset(absoluteCrossAxisPosition, absolutePosition);
                break;
            }
        }
        return result.addWithOutOfBandPosition(
            paintOffset: paintOffsetLocal,
            hitTest: (result) =>
            {
                return child.hitTest(result, position: transformedPosition);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
