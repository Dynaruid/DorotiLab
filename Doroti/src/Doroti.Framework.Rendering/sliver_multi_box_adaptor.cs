// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_multi_box_adaptor.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public interface RenderSliverBoxChildManager
{
    void createChild(long index, RenderBox? after);
    void removeChild(RenderBox child);
    double estimateMaxScrollOffset(
        SliverConstraints constraints,
        long? firstIndex = null,
        long? lastIndex = null,
        double? leadingScrollOffset = null,
        double? trailingScrollOffset = null
    );
    long childCount { get; }
    long? estimatedChildCount => null;
    void didAdoptChild(RenderBox child);
    void setDidUnderflow(bool value);
    void didStartLayout() { }

    void didFinishLayout() { }

    bool debugAssertChildListLocked() => true;
}

public interface KeepAliveParentDataMixin
{
    bool keepAlive { get; set; }

    public bool keptAlive { get; }
}

public interface RenderSliverWithKeepAliveMixin
{
    public void setupParentData(RenderObject child);
}

public class SliverMultiBoxAdaptorParentData
    : SliverLogicalParentData,
        ContainerParentDataMixin<RenderBox>,
        KeepAliveParentDataMixin
{
    public virtual long? index { get; set; } = default;
    internal virtual bool _keptAlive { get; set; } = false;
    public virtual RenderBox? previousSibling { get; set; } = default;
    public virtual RenderBox? nextSibling { get; set; } = default;
    public virtual bool keepAlive { get; set; } = false;

    public virtual bool keptAlive => _keptAlive;

    public override string ToString() =>
        $"index={index}; {(keepAlive ? "keepAlive; " : "")}{base.ToString()}";

    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => previousSibling is null);
        DartRuntimePrimitives.Assert(() => nextSibling is null);
        base.detach();
    }
}

public abstract class RenderSliverMultiBoxAdaptor
    : RenderSliver,
        ContainerRenderObjectMixin<RenderBox, SliverMultiBoxAdaptorParentData>,
        RenderSliverHelpers,
        RenderSliverWithKeepAliveMixin
{
    internal virtual RenderSliverBoxChildManager _childManager { get; private set; } = default!;
    internal virtual DartMap<long, RenderBox> _keepAliveBucket { get; private set; } =
        new DartMap<long, RenderBox>();
    internal virtual List<RenderBox> _debugDanglingKeepAlives { get; set; } = default!;
    internal virtual bool _debugChildIntegrityEnabled { get; set; } = true;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    protected RenderSliverMultiBoxAdaptor(RenderSliverBoxChildManager childManager)
    {
        _childManager = childManager;
        _debugDanglingKeepAlives = new List<RenderBox>();
    }

    public override void setupParentData(RenderObject child)
    {
        if (child.parentData is not SliverMultiBoxAdaptorParentData)
        {
            child.parentData = new SliverMultiBoxAdaptorParentData();
        }
    }

    public virtual RenderSliverBoxChildManager childManager => _childManager;
    public virtual bool debugChildIntegrityEnabled
    {
        get => _debugChildIntegrityEnabled;
        set
        {
            var enabled = value;
            DartRuntimePrimitives.Assert(() =>
            {
                _debugChildIntegrityEnabled = enabled;
                return _debugVerifyChildOrder()
                    && (
                        !_debugChildIntegrityEnabled
                        || (checked((long)_debugDanglingKeepAlives.Count) == 0)
                    );
            });
        }
    }

    public override void adoptChild(RenderObject child)
    {
        base.adoptChild(child);
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        if (!childParentData._keptAlive)
        {
            childManager.didAdoptChild(((RenderBox?)child)!);
        }
    }

    internal virtual bool _debugAssertChildListLocked() =>
        childManager.debugAssertChildListLocked();

    internal virtual bool _debugVerifyChildOrder()
    {
        if (_debugChildIntegrityEnabled)
        {
            RenderBox? child = firstChild;
            long index = default!;
            while (child is not null)
            {
                index = indexOf(child);
                child = childAfter(child);
                DartRuntimePrimitives.Assert(() => (child is null) || (indexOf(child) > index));
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !_keepAliveBucket.containsValue(child));
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() =>
            !EqualityComparer<RenderBox>.Default.Equals(child, after)
        );
        DartRuntimePrimitives.Assert(() =>
            !EqualityComparer<RenderBox>.Default.Equals(child, _firstChild)
        );
        DartRuntimePrimitives.Assert(() =>
            !EqualityComparer<RenderBox>.Default.Equals(child, _lastChild)
        );
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is SliverMultiBoxAdaptorParentData);
        _insertIntoChildList(child, after: after);
        DartRuntimePrimitives.Assert(() => firstChild is not null);
        DartRuntimePrimitives.Assert(() => _debugVerifyChildOrder());
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        if (!childParentData.keptAlive)
        {
            DartRuntimePrimitives.Assert(() => !Equals(child, this));
            DartRuntimePrimitives.Assert(() => !Equals(after, this));
            DartRuntimePrimitives.Assert(() =>
                !EqualityComparer<RenderBox>.Default.Equals(child, after)
            );
            DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
            var childParentDataLocal = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            if (
                !EqualityComparer<RenderBox>.Default.Equals(
                    childParentDataLocal.previousSibling,
                    after
                )
            )
            {
                _removeFromChildList(child);
                _insertIntoChildList(child, after: after);
            }
            // A keyed child can receive a new index while keeping the same
            // physical predecessor. Its child-manager slot still must update.
            childManager.didAdoptChild(child);
            markNeedsLayout();
        }
        else
        {
            if (
                Equals(
                    _keepAliveBucket.GetValueOrDefault(
                        (
                            childParentData.index
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        )
                    ),
                    child
                )
            )
            {
                _keepAliveBucket.remove(
                    (
                        childParentData.index
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                );
            }
            DartRuntimePrimitives.Assert(() =>
            {
                _debugDanglingKeepAlives.Remove(child);
                return true;
            });
            childManager.didAdoptChild(child);
            DartRuntimePrimitives.Assert(() =>
            {
                if (
                    _keepAliveBucket.ContainsKey(
                        (
                            childParentData.index
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        )
                    )
                )
                {
                    _debugDanglingKeepAlives.Add(
                        _keepAliveBucket.GetValueOrDefault(
                            (
                                childParentData.index
                                ?? throw new global::System.NullReferenceException(
                                    "Dart null assertion failed."
                                )
                            )
                        )!
                    );
                }
                return true;
            });
            _keepAliveBucket[
                (
                    childParentData.index
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            ] = child;
        }
    }

    public virtual void remove(RenderBox child)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        if (!childParentData._keptAlive)
        {
            _removeFromChildList(child);
            dropChild(child);
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            Equals(
                _keepAliveBucket.GetValueOrDefault(
                    (
                        childParentData.index
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                ),
                child
            )
        );
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDanglingKeepAlives.Remove(child);
            return true;
        });
        _keepAliveBucket.remove(
            (
                childParentData.index
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        );
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
        _keepAliveBucket.Values.forEach(dropChild);
        _keepAliveBucket.Clear();
    }

    internal virtual void _createOrObtainChild(long index, RenderBox? after)
    {
        invokeLayoutCallback<SliverConstraints>(
            (constraints) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(constraints, this.constraints));
                if (_keepAliveBucket.ContainsKey(index))
                {
                    RenderBox child = _keepAliveBucket.remove(index)!;
                    var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
                    DartRuntimePrimitives.Assert(() => childParentData._keptAlive);
                    dropChild(child);
                    child.parentData = childParentData;
                    insert(child, after: after);
                    childParentData._keptAlive = false;
                }
                else
                {
                    _childManager.createChild(index, after: after);
                }
            }
        );
    }

    internal virtual void _destroyOrCacheChild(RenderBox child)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        if (childParentData.keepAlive)
        {
            DartRuntimePrimitives.Assert(() => !childParentData._keptAlive);
            remove(child);
            _keepAliveBucket[
                (
                    childParentData.index
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            ] = child;
            child.parentData = childParentData;
            base.adoptChild(child);
            childParentData._keptAlive = true;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
            _childManager.removeChild(child);
            DartRuntimePrimitives.Assert(() => child.parent is null);
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (RenderBox childLocal in _keepAliveBucket.Values)
        {
            childLocal.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (RenderBox childLocal in _keepAliveBucket.Values)
        {
            childLocal.detach();
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        _keepAliveBucket.Values.forEach(redepthChild);
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        _keepAliveBucket.Values.forEach(visitor);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override Rect semanticBounds
    {
        get
        {
            if (
                (geometry is not null)
                && !geometry!.visible
                && (firstChild is not null)
                && firstChild!.hasSize
            )
            {
                return firstChild!.paintBounds;
            }
            return base.semanticBounds;
        }
    }

    public virtual bool addInitialChild(long index = 0, double layoutOffset = 0.0)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertChildListLocked());
        DartRuntimePrimitives.Assert(() => firstChild is null);
        _createOrObtainChild(index, after: null);
        if (firstChild is not null)
        {
            DartRuntimePrimitives.Assert(() => Equals(firstChild, lastChild));
            DartRuntimePrimitives.Assert(() => indexOf(firstChild!) == index);
            var firstChildParentData = ((SliverMultiBoxAdaptorParentData?)firstChild!.parentData!)!;
            firstChildParentData.layoutOffset = layoutOffset;
            return true;
        }
        childManager.setDidUnderflow(true);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? insertAndLayoutLeadingChild(
        BoxConstraints childConstraints,
        bool parentUsesSize = false
    )
    {
        DartRuntimePrimitives.Assert(() => _debugAssertChildListLocked());
        long index = indexOf(firstChild!) - 1L;
        _createOrObtainChild(index, after: null);
        if (indexOf(firstChild!) == index)
        {
            firstChild!.layout(childConstraints, parentUsesSize: parentUsesSize);
            return firstChild;
        }
        childManager.setDidUnderflow(true);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? insertAndLayoutChild(
        BoxConstraints childConstraints,
        RenderBox? after,
        bool parentUsesSize = false
    )
    {
        DartRuntimePrimitives.Assert(() => _debugAssertChildListLocked());
        DartRuntimePrimitives.Assert(() => after is not null);
        var previous = DartRuntimePrimitives.RequireReference(after);
        long index = indexOf(previous) + 1L;
        _createOrObtainChild(index, after: previous);
        RenderBox? child = childAfter(previous);
        if ((child is not null) && (indexOf(child) == index))
        {
            child.layout(childConstraints, parentUsesSize: parentUsesSize);
            return child;
        }
        childManager.setDidUnderflow(true);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long calculateLeadingGarbage(long firstIndex)
    {
        RenderBox? walker = firstChild;
        var leadingGarbage = 0L;
        while ((walker is not null) && (indexOf(walker) < firstIndex))
        {
            leadingGarbage += 1L;
            walker = childAfter(walker);
        }
        return leadingGarbage;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long calculateTrailingGarbage(long lastIndex)
    {
        RenderBox? walker = lastChild;
        var trailingGarbage = 0L;
        while ((walker is not null) && (indexOf(walker) > lastIndex))
        {
            trailingGarbage += 1L;
            walker = childBefore(walker);
        }
        return trailingGarbage;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void collectGarbage(long leadingGarbage, long trailingGarbage)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertChildListLocked());
        DartRuntimePrimitives.Assert(() => childCount >= (leadingGarbage + trailingGarbage));
        invokeLayoutCallback<SliverConstraints>(
            (constraints) =>
            {
                while (leadingGarbage > 0L)
                {
                    _destroyOrCacheChild(firstChild!);
                    leadingGarbage -= 1L;
                }
                while (trailingGarbage > 0L)
                {
                    _destroyOrCacheChild(lastChild!);
                    trailingGarbage -= 1L;
                }
                _keepAliveBucket
                    .Values.where(
                        (child) =>
                        {
                            var childParentData = (
                                (SliverMultiBoxAdaptorParentData?)child.parentData!
                            )!;
                            return !childParentData.keepAlive;
                        }
                    )
                    .ToList()
                    .forEach(_childManager.removeChild);
                DartRuntimePrimitives.Assert(() =>
                    _keepAliveBucket
                        .Values.where(
                            (child) =>
                            {
                                var childParentDataLocal = (
                                    (SliverMultiBoxAdaptorParentData?)child.parentData!
                                )!;
                                return !childParentDataLocal.keepAlive;
                            }
                        )
                        .Count() == 0
                );
            }
        );
    }

    public virtual long indexOf(RenderBox child)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.index is not null);
        return (
            childParentData.index
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double paintExtentOf(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => child.hasSize);
        return constraints.axis switch
        {
            Axis.horizontal => child.size.width,
            Axis.vertical => child.size.height,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        RenderBox? child = lastChild;
        var boxResult = BoxHitTestResult.CreateWrap(result);
        while (child is not null)
        {
            if (
                hitTestBoxChild(
                    boxResult,
                    child,
                    mainAxisPosition: mainAxisPosition,
                    crossAxisPosition: crossAxisPosition
                )
            )
            {
                return true;
            }
            child = childBefore(child);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderBox)child;
        return (
                childScrollOffset(__child)
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) - constraints.scrollOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? childScrollOffset(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        return childParentData.layoutOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)child;
        var childParentData = ((SliverMultiBoxAdaptorParentData?)__child.parentData)!;
        return (childParentData.index is not null)
            && !_keepAliveBucket.ContainsKey(
                (
                    childParentData.index
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        if (!paintsChild(__child))
        {
            transform.setZero();
        }
        else
        {
            applyPaintTransformForBoxChild(__child, transform);
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (firstChild is null)
        {
            return;
        }
        Offset mainAxisUnit = default!;
        Offset crossAxisUnit = default!;
        Offset originOffset = default!;
        bool addExtent = default!;
        switch (
            SliverLibrary.applyGrowthDirectionToAxisDirection(
                constraints.axisDirection,
                constraints.growthDirection
            )
        )
        {
            case AxisDirection.up:
            {
                mainAxisUnit = new Offset(0.0, -1.0);
                crossAxisUnit = new Offset(1.0, 0.0);
                originOffset = offset + new Offset(0.0, geometry!.paintExtent);
                addExtent = true;
                break;
            }
            case AxisDirection.right:
            {
                mainAxisUnit = new Offset(1.0, 0.0);
                crossAxisUnit = new Offset(0.0, 1.0);
                originOffset = offset;
                addExtent = false;
                break;
            }
            case AxisDirection.down:
            {
                mainAxisUnit = new Offset(0.0, 1.0);
                crossAxisUnit = new Offset(1.0, 0.0);
                originOffset = offset;
                addExtent = false;
                break;
            }
            case AxisDirection.left:
            {
                mainAxisUnit = new Offset(-1.0, 0.0);
                crossAxisUnit = new Offset(0.0, 1.0);
                originOffset = offset + new Offset(geometry!.paintExtent, 0.0);
                addExtent = true;
                break;
            }
        }
        RenderBox? child = firstChild;
        while (child is not null)
        {
            double mainAxisDelta = childMainAxisPosition(child);
            double crossAxisDelta = childCrossAxisPosition(child);
            var childOffset = new Offset(
                originOffset.dx
                    + (mainAxisUnit.dx * mainAxisDelta)
                    + (crossAxisUnit.dx * crossAxisDelta),
                originOffset.dy
                    + (mainAxisUnit.dy * mainAxisDelta)
                    + (crossAxisUnit.dy * crossAxisDelta)
            );
            if (addExtent)
            {
                childOffset += mainAxisUnit * paintExtentOf(child);
            }
            if (
                (mainAxisDelta < constraints.remainingPaintExtent)
                && ((mainAxisDelta + paintExtentOf(child)) > 0L)
            )
            {
                context.paintChild(child, childOffset);
            }
            child = childAfter(child);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsNode(
                (firstChild is not null)
                    ? $"currently live children: {indexOf(firstChild!)} to {indexOf(lastChild!)}"
                    : "no children current live"
            )
        );
    }

    public virtual bool debugAssertChildListIsNonEmptyAndContiguous()
    {
        DartRuntimePrimitives.Assert(() =>
        {
            DartRuntimePrimitives.Assert(() => firstChild is not null);
            long index = indexOf(firstChild!);
            RenderBox? child = childAfter(firstChild!);
            while (child is not null)
            {
                index += 1L;
                DartRuntimePrimitives.Assert(() => indexOf(child) == index);
                child = childAfter(child);
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderBox? child = firstChild;
            while (true)
            {
                var childParentData = ((SliverMultiBoxAdaptorParentData?)child!.parentData!)!;
                children.Add(
                    ((Diagnosticable)child).toDiagnosticsNode(
                        name: $"child__29592 with index {childParentData.index}"
                    )
                );
                if (Equals(child, lastChild))
                {
                    break;
                }
                child = childParentData.nextSibling;
            }
        }
        if (checked((long)_keepAliveBucket.Count) != 0)
        {
            List<long> indices = (
                (Func<List<long>>)(
                    () =>
                    {
                        var __cascade = _keepAliveBucket.Keys.ToList();
                        __cascade.sort();
                        return __cascade;
                    }
                )
            )();
            foreach (var indexLocal in indices)
            {
                children.Add(
                    (
                        (Diagnosticable)_keepAliveBucket.GetValueOrDefault(indexLocal)!
                    ).toDiagnosticsNode(
                        name: $"child with index {indexLocal} (kept alive but not laid out)",
                        style: DiagnosticsTreeStyle.offstage
                    )
                );
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
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
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"A {GetType()} expected a child of type {typeof(RenderBox)} but received a "
                                + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."
                        ),
                        new ErrorDescription(
                            "RenderObjects expect specific types of children because they "
                                + "coordinate with their children during layout and paint. For "
                                + "example, a RenderSliver cannot be the child of a RenderBox because "
                                + "a RenderSliver does not understand the RenderBox layout protocol."
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {GetType()} that expected a {typeof(RenderBox)} child was created by",
                            debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                        new ErrorSpacer(),
                        new DiagnosticsProperty<object?>(
                            $"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type "
                                + "was created by",
                            child.debugCreator,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                    }
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = (
                    (SliverMultiBoxAdaptorParentData?)_firstChild!.parentData!
                )!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() =>
                _debugUltimatePreviousSiblingOf(after, equals: _firstChild)
            );
            DartRuntimePrimitives.Assert(() =>
                _debugUltimateNextSiblingOf(after, equals: _lastChild)
            );
            var afterParentData = ((SliverMultiBoxAdaptorParentData?)after.parentData!)!;
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
                var childPreviousSiblingParentData = (
                    (SliverMultiBoxAdaptorParentData?)childParentData.previousSibling!.parentData!
                )!;
                var childNextSiblingParentData = (
                    (SliverMultiBoxAdaptorParentData?)childParentData.nextSibling!.parentData!
                )!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach(add);
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() =>
            _debugUltimatePreviousSiblingOf(child, equals: _firstChild)
        );
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = (
                (SliverMultiBoxAdaptorParentData?)childParentData.previousSibling!.parentData!
            )!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = (
                (SliverMultiBoxAdaptorParentData?)childParentData.nextSibling!.parentData!
            )!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;

    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((SliverMultiBoxAdaptorParentData?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _getRightWayUp(SliverConstraints constraints)
    {
        bool reversed = Basic_typesLibrary.axisDirectionIsReversed(constraints.axisDirection);
        return constraints.growthDirection switch
        {
            GrowthDirection.forward => !reversed,
            GrowthDirection.reverse => reversed,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
            }
        );
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
