// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_group.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public class RenderSliverCrossAxisGroup : RenderSliver, ContainerRenderObjectMixin<RenderSliver, SliverPhysicalContainerParentData>
{
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderSliver? _firstChild { get; set; } = default;
    public virtual RenderSliver? _lastChild { get; set; } = default;

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverPhysicalContainerParentData)
        {
            child.parentData = new SliverPhysicalContainerParentData();
            ((SliverPhysicalParentData?)(object?)child.parentData!)!.crossAxisFlex = 1L;
        }
    }

    public override double childMainAxisPosition(RenderObject child) => 0.0;
    public override double childCrossAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        Offset paintOffsetLocal = ((SliverPhysicalParentData?)(object?)__child.parentData!)!.paintOffset;
        return constraints.axis switch { Axis.vertical => paintOffsetLocal.dx, Axis.horizontal => paintOffsetLocal.dy, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        double crossAxisExtentLocal = constraints.crossAxisExtent;
        DartRuntimePrimitives.Assert(() => double.IsFinite(crossAxisExtentLocal));
        var totalFlex = 0L;
        var remainingExtent = crossAxisExtentLocal;
        RenderSliver? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
            long flex = childParentData.crossAxisFlex ?? 0L;
            if (flex == 0L)
            {
                DartRuntimePrimitives.Assert(() => Sliver_groupLibrary._assertOutOfExtent(remainingExtent));
                child.layout(constraints.copyWith(crossAxisExtent: remainingExtent), parentUsesSize: true);
                double? childCrossAxisExtent = child.geometry!.crossAxisExtent;
                DartRuntimePrimitives.Assert(() => childCrossAxisExtent is not null);
                remainingExtent = Math.Max(0.0, remainingExtent - DartRuntimePrimitives.RequireValue(childCrossAxisExtent));
            }
            else
            {
                totalFlex += flex;
            }
            child = childAfter(child);
        }
        double extentPerFlexValue = remainingExtent / totalFlex;
        child = firstChild;
        geometry = SliverGeometry.zero;
        while (child is not null)
        {
            var childParentDataLocal = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
            long flexLocal = childParentDataLocal.crossAxisFlex ?? 0L;
            double childExtent = default!;
            if (flexLocal != 0L)
            {
                childExtent = extentPerFlexValue * flexLocal;
                DartRuntimePrimitives.Assert(() => Sliver_groupLibrary._assertOutOfExtent(childExtent));
                child.layout(constraints.copyWith(crossAxisExtent: extentPerFlexValue * flexLocal), parentUsesSize: true);
            }
            else
            {
                childExtent = DartRuntimePrimitives.RequireValue(child.geometry!.crossAxisExtent);
            }
            SliverGeometry childLayoutGeometry = child.geometry!;
            if (geometry!.scrollExtent < childLayoutGeometry.scrollExtent)
            {
                geometry = childLayoutGeometry;
            }
            child = childAfter(child);
        }
        child = firstChild;
        var offset = 0.0;
        while (child is not null)
        {
            var childParentDataAlternate = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
            SliverGeometry childLayoutGeometryLocal = child.geometry!;
            double remainingExtentLocal = geometry!.scrollExtent - constraints.scrollOffset;
            double paintCorrection = (childLayoutGeometryLocal.paintExtent > remainingExtentLocal) ? (childLayoutGeometryLocal.paintExtent - remainingExtentLocal) : 0.0;
            double childExtentLocal = child.geometry!.crossAxisExtent ?? (extentPerFlexValue * (childParentDataAlternate.crossAxisFlex ?? 0L));
            childParentDataAlternate.paintOffset = constraints.axis switch { Axis.vertical => new Offset(offset, -paintCorrection), Axis.horizontal => new Offset(-paintCorrection, offset), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            offset += childExtentLocal;
            child = childAfter(child);
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderSliver? child = firstChild;
        while (child is not null)
        {
            if (child.geometry!.visible)
            {
                var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
                context.paintChild(child, offset + childParentData.paintOffset);
            }
            child = childAfter(child);
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderSliver)(object)child;
        var childParentData = ((SliverPhysicalParentData?)(object?)__child.parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        RenderSliver? child = lastChild;
        while (child is not null)
        {
            Offset paintOffsetLocal = ((SliverPhysicalParentData?)(object?)child.parentData!)!.paintOffset;
            bool isHit = result.addWithAxisOffset(mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition, paintOffset: paintOffsetLocal, mainAxisOffset: childMainAxisPosition(child), crossAxisOffset: childCrossAxisPosition(child), hitTest: child.hitTest);
            if (isHit)
            {
                return true;
            }
            child = childBefore(child);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderSliver)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderSliver child, RenderSliver? after = null)
    {
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((SliverPhysicalContainerParentData?)(object?)_firstChild!.parentData!)!;
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
            var afterParentData = ((SliverPhysicalContainerParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((SliverPhysicalContainerParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((SliverPhysicalContainerParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is SliverPhysicalContainerParentData);
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderSliver child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderSliver>? children)
    {
        children?.forEach(add);
    }

    public virtual void _removeFromChildList(RenderSliver child)
    {
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((SliverPhysicalContainerParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((SliverPhysicalContainerParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderSliver child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            RenderSliver? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
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
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderSliver? firstChild => _firstChild;
    public virtual RenderSliver? lastChild => _lastChild;
    public virtual RenderSliver? childBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? childAfter(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderSliver child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Sliver_groupLibrary
{
    internal static bool _assertOutOfExtent(double extent)
    {
        if (extent <= 0.0)
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
        if (child.parentData is not SliverPhysicalContainerParentData)
        {
            child.parentData = new SliverPhysicalContainerParentData();
        }
    }

    public override double? childScrollOffset(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        DartRuntimePrimitives.Assert(() => child is RenderSliver);
        double extentOfPinnedSlivers = _maxScrollObstructionExtentBefore(((RenderSliver?)(object?)child)!);
        GrowthDirection growthDirectionLocal = constraints.growthDirection;
        switch (growthDirectionLocal)
        {
            case GrowthDirection.forward:
                {
                    var childScrollOffsetLocal = 0.0;
                    RenderSliver? current = childBefore((RenderSliver)child);
                    while (current is not null)
                    {
                        childScrollOffsetLocal += current.geometry!.scrollExtent;
                        current = childBefore(current);
                    }
                    return childScrollOffsetLocal - extentOfPinnedSlivers;
                }
            case GrowthDirection.reverse:
                {
                    var childScrollOffsetAlternate = 0.0;
                    RenderSliver? currentLocal = childAfter((RenderSliver)child);
                    while (currentLocal is not null)
                    {
                        childScrollOffsetAlternate -= currentLocal.geometry!.scrollExtent;
                        currentLocal = childAfter(currentLocal);
                    }
                    return childScrollOffsetAlternate - extentOfPinnedSlivers;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _maxScrollObstructionExtentBefore(RenderSliver child)
    {
        GrowthDirection growthDirectionLocal = child.constraints.growthDirection;
        switch (growthDirectionLocal)
        {
            case GrowthDirection.forward:
                {
                    var pinnedExtent = 0.0;
                    RenderSliver? current = firstChild;
                    while (!Equals(current, child))
                    {
                        pinnedExtent += current!.geometry!.maxScrollObstructionExtent;
                        current = childAfter(current);
                    }
                    return pinnedExtent;
                }
            case GrowthDirection.reverse:
                {
                    var pinnedExtentLocal = 0.0;
                    RenderSliver? currentLocal = lastChild;
                    while (!Equals(currentLocal, child))
                    {
                        pinnedExtentLocal += currentLocal!.geometry!.maxScrollObstructionExtent;
                        currentLocal = childBefore(currentLocal);
                    }
                    return pinnedExtentLocal;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderSliver)(object)child;
        var childParentData = ((SliverPhysicalParentData?)(object?)__child.parentData!)!;
        return SliverLibrary.applyGrowthDirectionToAxisDirection(__child.constraints.axisDirection, __child.constraints.growthDirection) switch { AxisDirection.down => childParentData.paintOffset.dy, AxisDirection.right => childParentData.paintOffset.dx, AxisDirection.up => geometry!.paintExtent - __child.geometry!.paintExtent - childParentData.paintOffset.dy, AxisDirection.left => geometry!.paintExtent - __child.geometry!.paintExtent - childParentData.paintOffset.dx, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childCrossAxisPosition(RenderObject child) => 0.0;
    public override void performLayout()
    {
        double scrollOffsetLocal = 0;
        double layoutOffset = 0;
        double maxPaintExtentLocal = 0;
        double paintOffsetLocal = constraints.overlap;
        double maxScrollObstructionExtentLocal = 0;
        double cacheOriginLocal = constraints.cacheOrigin;
        double remainingCacheExtentLocal = constraints.remainingCacheExtent;
        var (leadingChild, advance) = constraints.growthDirection switch { GrowthDirection.forward => ((RenderSliver?, Func<RenderSliver, RenderSliver?>))(firstChild, childAfter), GrowthDirection.reverse => ((RenderSliver?, Func<RenderSliver, RenderSliver?>))(lastChild, childBefore), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        var child = leadingChild;
        while (child is not null)
        {
            double beforeOffsetPaintExtent = calculatePaintOffset(constraints, from: 0.0, to: scrollOffsetLocal);
            double childScrollOffset = Math.Max(0.0, constraints.scrollOffset - scrollOffsetLocal);
            double correctedCacheOrigin = Math.Max(cacheOriginLocal, -childScrollOffset);
            double cacheExtentCorrection = cacheOriginLocal - correctedCacheOrigin;
            child.layout(constraints.copyWith(scrollOffset: childScrollOffset, cacheOrigin: correctedCacheOrigin, overlap: Math.Max(0.0, _fixPrecisionError(paintOffsetLocal - beforeOffsetPaintExtent)), remainingPaintExtent: _fixPrecisionError(constraints.remainingPaintExtent - beforeOffsetPaintExtent), remainingCacheExtent: Math.Max(0.0, _fixPrecisionError(remainingCacheExtentLocal + cacheExtentCorrection)), precedingScrollExtent: scrollOffsetLocal + constraints.precedingScrollExtent), parentUsesSize: true);
            SliverGeometry childLayoutGeometry = child.geometry!;
            double? scrollOffsetCorrectionLocal = childLayoutGeometry.scrollOffsetCorrection;
            if (scrollOffsetCorrectionLocal is not null)
            {
                double scrollOffsetCorrection__13115__value13194 = DartRuntimePrimitives.RequireValue(scrollOffsetCorrectionLocal);
                geometry = new SliverGeometry(scrollOffsetCorrection: DartRuntimePrimitives.RequireValue(scrollOffsetCorrection__13115__value13194));
                return;
            }
            DartRuntimePrimitives.Assert(() => childLayoutGeometry.debugAssertIsValid());
            double childPaintOffset = layoutOffset + childLayoutGeometry.paintOrigin;
            var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
            childParentData.paintOffset = constraints.axis switch { Axis.vertical => new Offset(0.0, childPaintOffset), Axis.horizontal => new Offset(childPaintOffset, 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            scrollOffsetLocal += childLayoutGeometry.scrollExtent;
            layoutOffset += childLayoutGeometry.layoutExtent;
            maxPaintExtentLocal += childLayoutGeometry.maxPaintExtent;
            maxScrollObstructionExtentLocal += childLayoutGeometry.maxScrollObstructionExtent;
            paintOffsetLocal = Math.Max(childPaintOffset + childLayoutGeometry.paintExtent, paintOffsetLocal);
            if (childLayoutGeometry.cacheExtent != 0.0)
            {
                remainingCacheExtentLocal = _fixPrecisionError(remainingCacheExtentLocal - childLayoutGeometry.cacheExtent - cacheExtentCorrection);
                cacheOriginLocal = Math.Min(correctedCacheOrigin + childLayoutGeometry.cacheExtent, 0.0);
            }
            child = advance(child);
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((child is not null) && double.IsInfinity(maxPaintExtentLocal))
                    {
                        throw new FlutterError("Unreachable sliver found, you may have a sliver following " + "a sliver with an infinite extent. ");
                    }
                    return true;
                });
        }
        double remainingExtent = Math.Max(0, scrollOffsetLocal - constraints.scrollOffset);
        if (paintOffsetLocal > remainingExtent)
        {
            bool pinnedChildrenOverflow = maxScrollObstructionExtentLocal > (remainingExtent - constraints.overlap);
            double paintCorrection = paintOffsetLocal - remainingExtent;
            paintOffsetLocal = remainingExtent;
            child = firstChild;
            while (child is not null)
            {
                SliverGeometry childLayoutGeometryLocal = child.geometry!;
                var childParentDataLocal = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
                double childMainAxisPaintOffset = constraints.axis switch { Axis.vertical => childParentDataLocal.paintOffset.dy, Axis.horizontal => childParentDataLocal.paintOffset.dx, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                double childPaintEnd = childMainAxisPaintOffset + childLayoutGeometryLocal.paintExtent;
                bool childIsPinned = childLayoutGeometryLocal.maxScrollObstructionExtent > 0L;
                if ((childPaintEnd > remainingExtent) || pinnedChildrenOverflow && childIsPinned)
                {
                    childParentDataLocal.paintOffset = constraints.axis switch { Axis.vertical => new Offset(0.0, childParentDataLocal.paintOffset.dy - paintCorrection), Axis.horizontal => new Offset(childParentDataLocal.paintOffset.dx - paintCorrection, 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
                }
                child = childAfter(child);
            }
        }
        double cacheExtentLocal = calculateCacheOffset(constraints, from: Math.Min(constraints.scrollOffset, 0), to: scrollOffsetLocal);
        double paintExtentLocal = Dart_uiLibrary.clampDouble(paintOffsetLocal, 0, constraints.remainingPaintExtent);
        geometry = new SliverGeometry(scrollExtent: scrollOffsetLocal, paintExtent: paintExtentLocal, cacheExtent: cacheExtentLocal, maxPaintExtent: maxPaintExtentLocal, hasVisualOverflow: (scrollOffsetLocal > constraints.remainingPaintExtent) || (constraints.scrollOffset > 0.0));
        child = leadingChild;
        while (child is not null)
        {
            var childParentDataAlternate = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
            childParentDataAlternate.paintOffset = SliverLibrary.applyGrowthDirectionToAxisDirection(constraints.axisDirection, constraints.growthDirection) switch { AxisDirection.up => new Offset(0.0, paintExtentLocal - childParentDataAlternate.paintOffset.dy - child.geometry!.paintExtent), AxisDirection.left => new Offset(paintExtentLocal - childParentDataAlternate.paintOffset.dx - child.geometry!.paintExtent, 0.0), AxisDirection.right => childParentDataAlternate.paintOffset, AxisDirection.down => childParentDataAlternate.paintOffset, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            child = advance(child);
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderSliver? child = lastChild;
        while (child is not null)
        {
            if (child.geometry!.visible)
            {
                var childParentData = ((SliverPhysicalParentData?)(object?)child.parentData!)!;
                context.paintChild(child, offset + childParentData.paintOffset);
            }
            child = childBefore(child);
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderSliver)(object)child;
        var childParentData = ((SliverPhysicalParentData?)(object?)__child.parentData!)!;
        childParentData.applyPaintTransform(transform);
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        RenderSliver? child = firstChild;
        while (child is not null)
        {
            Offset paintOffsetLocal = ((SliverPhysicalParentData?)(object?)child.parentData!)!.paintOffset;
            bool isHit = result.addWithAxisOffset(mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition, paintOffset: paintOffsetLocal, mainAxisOffset: childMainAxisPosition(child), crossAxisOffset: childCrossAxisPosition(child), hitTest: child.hitTest);
            if (isHit)
            {
                return true;
            }
            child = childAfter(child);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        RenderSliver? child = firstChild;
        while (child is not null)
        {
            if (child.geometry!.visible || (child.geometry!.cacheExtent > 0.0) || child.ensureSemantics)
            {
                visitor(child);
            }
            child = childAfter(child);
        }
    }

    internal static double _fixPrecisionError(double number)
    {
        return (number.abs() < Foundation.ConstantsLibrary.precisionErrorTolerance) ? 0.0 : number;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderSliver child, RenderSliver? equals = null)
    {
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;
    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderSliver)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderSliver)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderSliver)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new ErrorSpacer(), new DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) });
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderSliver child, RenderSliver? after = null)
    {
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((SliverPhysicalContainerParentData?)(object?)_firstChild!.parentData!)!;
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
            var afterParentData = ((SliverPhysicalContainerParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((SliverPhysicalContainerParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((SliverPhysicalContainerParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is SliverPhysicalContainerParentData);
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderSliver child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderSliver>? children)
    {
        children?.forEach(add);
    }

    public virtual void _removeFromChildList(RenderSliver child)
    {
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((SliverPhysicalContainerParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((SliverPhysicalContainerParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderSliver child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            RenderSliver? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderSliver child, RenderSliver? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
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
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void detach()
    {
        base.detach();
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void redepthChildren()
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderSliver? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderSliver? firstChild => _firstChild;
    public virtual RenderSliver? lastChild => _lastChild;
    public virtual RenderSliver? childBefore(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderSliver? childAfter(RenderSliver child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderSliver child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((SliverPhysicalContainerParentData?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

