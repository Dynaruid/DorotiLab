// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_multi_box_adaptor.dart
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

public interface RenderSliverBoxChildManager
{
    void createChild(long index, RenderBox? after);
    void removeChild(RenderBox child);
    double estimateMaxScrollOffset(SliverConstraints constraints, long? firstIndex = null, long? lastIndex = null, double? leadingScrollOffset = null, double? trailingScrollOffset = null);
    long childCount { get; }
    long? estimatedChildCount => null;
    void didAdoptChild(RenderBox child);
    void setDidUnderflow(bool value);
    void didStartLayout()
    {
    }

    void didFinishLayout()
    {
    }

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

public class SliverMultiBoxAdaptorParentData : SliverLogicalParentData, ContainerParentDataMixin<RenderBox>, KeepAliveParentDataMixin
{
    public virtual long? index { get; set; } = default;
    internal virtual bool _keptAlive { get; set; } = false;
    public virtual RenderBox? previousSibling { get; set; } = default;
    public virtual RenderBox? nextSibling { get; set; } = default;
    public virtual bool keepAlive { get; set; } = false;

    public virtual bool keptAlive => this._keptAlive;
    public override string ToString() => $"index={this.index}; {(keepAlive ? "keepAlive; " : "")}{base.ToString()}";
    public override void detach()
    {
        DartRuntimePrimitives.Assert(() => (this.previousSibling is null));
        DartRuntimePrimitives.Assert(() => (this.nextSibling is null));
        base.detach();
    }

}

public abstract class RenderSliverMultiBoxAdaptor : RenderSliver, ContainerRenderObjectMixin<RenderBox, SliverMultiBoxAdaptorParentData>, RenderSliverHelpers, RenderSliverWithKeepAliveMixin
{
    internal virtual RenderSliverBoxChildManager _childManager { get; private set; } = default!;
    internal virtual DartMap<long, RenderBox> _keepAliveBucket { get; private set; } = new DartMap<long, RenderBox>();
    internal virtual List<RenderBox> _debugDanglingKeepAlives { get; set; } = default!;
    internal virtual bool _debugChildIntegrityEnabled { get; set; } = true;
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    protected RenderSliverMultiBoxAdaptor(RenderSliverBoxChildManager childManager)
    {
        this._childManager = childManager;
        this._debugDanglingKeepAlives = new List<RenderBox>();
    }

    public override void setupParentData(RenderObject child)
    {
        if ((((RenderObject)child).parentData is not SliverMultiBoxAdaptorParentData))
        {
            child.parentData = new SliverMultiBoxAdaptorParentData();
        }
    }

    public virtual RenderSliverBoxChildManager childManager => this._childManager;
    public virtual bool debugChildIntegrityEnabled
    {
        get => this._debugChildIntegrityEnabled;
        set
        {
            var enabled = value;
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugChildIntegrityEnabled = enabled;
                    return (_debugVerifyChildOrder() && ((!this._debugChildIntegrityEnabled || (checked((long)(this._debugDanglingKeepAlives.Count)) == 0))));
                });
        }
    }
    public override void adoptChild(RenderObject child)
    {
        base.adoptChild(child);
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)((RenderObject)child).parentData!)!;
        if (!((SliverMultiBoxAdaptorParentData)childParentData)._keptAlive)
        {
            this.childManager.didAdoptChild(((RenderBox?)(object?)child)!);
        }
    }

    internal virtual bool _debugAssertChildListLocked() => this.childManager.debugAssertChildListLocked();
    internal virtual bool _debugVerifyChildOrder()
    {
        if (this._debugChildIntegrityEnabled)
        {
            RenderBox? child = firstChild;
            long index = default!;
            while ((child is not null))
            {
                index = indexOf(child);
                child = childAfter(child);
                DartRuntimePrimitives.Assert(() => ((child is null) || (indexOf(child) > index)));
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !this._keepAliveBucket.containsValue(child));
        DartRuntimePrimitives.Assert(() => !object.Equals(child, this));
        DartRuntimePrimitives.Assert(() => !object.Equals(after, this));
        DartRuntimePrimitives.Assert(() => !EqualityComparer<RenderBox>.Default.Equals(child, after));
        DartRuntimePrimitives.Assert(() => !EqualityComparer<RenderBox>.Default.Equals(child, this._firstChild));
        DartRuntimePrimitives.Assert(() => !EqualityComparer<RenderBox>.Default.Equals(child, this._lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => (child.parentData is SliverMultiBoxAdaptorParentData));
        _insertIntoChildList(child, after: after);
        DartRuntimePrimitives.Assert(() => (firstChild is not null));
        DartRuntimePrimitives.Assert(() => _debugVerifyChildOrder());
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        if (!((SliverMultiBoxAdaptorParentData)childParentData).keptAlive)
        {
            DartRuntimePrimitives.Assert(() => !object.Equals(child, this));
            DartRuntimePrimitives.Assert(() => !object.Equals(after, this));
            DartRuntimePrimitives.Assert(() => !EqualityComparer<RenderBox>.Default.Equals(child, after));
            DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
            var childParentDataLocal = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            if (EqualityComparer<RenderBox>.Default.Equals(childParentDataLocal.previousSibling, after))
            {
                return;
            }
            _removeFromChildList(child);
            _insertIntoChildList(child, after: after);
            markNeedsLayout();
            this.childManager.didAdoptChild(child);
            markNeedsLayout();
        }
        else
        {
            if ((object.Equals(this._keepAliveBucket.GetValueOrDefault(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index)), child)))
            {
                this._keepAliveBucket.remove(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index));
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    this._debugDanglingKeepAlives.Remove(child);
                    return true;
                });
            this.childManager.didAdoptChild(child);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (this._keepAliveBucket.ContainsKey(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index)))
                    {
                        this._debugDanglingKeepAlives.Add(this._keepAliveBucket.GetValueOrDefault(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index))!);
                    }
                    return true;
                });
            this._keepAliveBucket[DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index)] = child;
        }
    }

    public virtual void remove(RenderBox child)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        if (!((SliverMultiBoxAdaptorParentData)childParentData)._keptAlive)
        {
            _removeFromChildList(child);
            dropChild(child);
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._keepAliveBucket.GetValueOrDefault(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index)), child)));
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugDanglingKeepAlives.Remove(child);
                return true;
            });
        this._keepAliveBucket.remove(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index));
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
        this._keepAliveBucket.Values.forEach(dropChild);
        this._keepAliveBucket.Clear();
    }

    internal virtual void _createOrObtainChild(long index, RenderBox? after)
    {
        invokeLayoutCallback<SliverConstraints>(((Action<SliverConstraints>)((constraints) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(constraints, this.constraints)));
            if (this._keepAliveBucket.ContainsKey(index))
            {
                RenderBox child = this._keepAliveBucket.remove(index)!;
                var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
                DartRuntimePrimitives.Assert(() => ((SliverMultiBoxAdaptorParentData)childParentData)._keptAlive);
                dropChild(child);
                child.parentData = childParentData;
                insert(child, after: after);
                childParentData._keptAlive = false;
            }
            else
            {
                this._childManager.createChild(index, after: after);
            }
        })));
    }

    internal virtual void _destroyOrCacheChild(RenderBox child)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        if (childParentData.keepAlive)
        {
            DartRuntimePrimitives.Assert(() => !((SliverMultiBoxAdaptorParentData)childParentData)._keptAlive);
            remove(child);
            this._keepAliveBucket[DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index)] = child;
            child.parentData = childParentData;
            base.adoptChild(child);
            childParentData._keptAlive = true;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
            this._childManager.removeChild(child);
            DartRuntimePrimitives.Assert(() => (child.parent is null));
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (RenderBox childLocal in this._keepAliveBucket.Values)
        {
            childLocal.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        foreach (RenderBox childLocal in this._keepAliveBucket.Values)
        {
            childLocal.detach();
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        this._keepAliveBucket.Values.forEach(redepthChild);
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        this._keepAliveBucket.Values.forEach(visitor);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override Rect semanticBounds
    {
        get
        {
            if (((((geometry is not null) && !geometry!.visible) && (firstChild is not null)) && firstChild!.hasSize))
            {
                return firstChild!.paintBounds;
            }
            return base.semanticBounds;
            return default!;
        }
    }
    public virtual bool addInitialChild(long index = 0, double layoutOffset = 0.0)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertChildListLocked());
        DartRuntimePrimitives.Assert(() => (firstChild is null));
        _createOrObtainChild(index, after: null);
        if ((firstChild is not null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(firstChild, lastChild)));
            DartRuntimePrimitives.Assert(() => (indexOf(firstChild!) == index));
            var firstChildParentData = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
            firstChildParentData.layoutOffset = layoutOffset;
            return true;
        }
        this.childManager.setDidUnderflow(true);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? insertAndLayoutLeadingChild(BoxConstraints childConstraints, bool parentUsesSize = false)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertChildListLocked());
        long index = (indexOf(firstChild!) - 1L);
        _createOrObtainChild(index, after: null);
        if ((indexOf(firstChild!) == index))
        {
            firstChild!.layout(childConstraints, parentUsesSize: parentUsesSize);
            return firstChild;
        }
        this.childManager.setDidUnderflow(true);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? insertAndLayoutChild(BoxConstraints childConstraints, RenderBox? after, bool parentUsesSize = false)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertChildListLocked());
        DartRuntimePrimitives.Assert(() => (after is not null));
        long index = (indexOf(after!) + 1L);
        _createOrObtainChild(index, after: after);
        RenderBox? child = childAfter(after);
        if (((child is not null) && (indexOf(child) == index)))
        {
            child.layout(childConstraints, parentUsesSize: parentUsesSize);
            return child;
        }
        this.childManager.setDidUnderflow(true);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long calculateLeadingGarbage(long firstIndex)
    {
        RenderBox? walker = firstChild;
        var leadingGarbage = 0L;
        while (((walker is not null) && (indexOf(walker) < firstIndex)))
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
        while (((walker is not null) && (indexOf(walker) > lastIndex)))
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
        DartRuntimePrimitives.Assert(() => (childCount >= (leadingGarbage + trailingGarbage)));
        invokeLayoutCallback<SliverConstraints>(((Action<SliverConstraints>)((constraints) =>
        {
            while ((leadingGarbage > 0L))
            {
                _destroyOrCacheChild(firstChild!);
                leadingGarbage -= 1L;
            }
            while ((trailingGarbage > 0L))
            {
                _destroyOrCacheChild(lastChild!);
                trailingGarbage -= 1L;
            }
            this._keepAliveBucket.Values.where(((child) =>
            {
                var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
                return !childParentData.keepAlive;
                return default;
            })).ToList().forEach(((RenderSliverBoxChildManager)this._childManager).removeChild);
            DartRuntimePrimitives.Assert(() => (this._keepAliveBucket.Values.where(((child) =>
            {
                var childParentDataLocal = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
                return !childParentDataLocal.keepAlive;
                return default;
            })).Count() == 0));
        })));
    }

    public virtual long indexOf(RenderBox child)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (((SliverMultiBoxAdaptorParentData)childParentData).index is not null));
        return DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData).index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double paintExtentOf(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => ((RenderBox)child).hasSize);
        return (((SliverConstraints)constraints).axis switch { global::Doroti.Framework.Painting.Axis.horizontal => ((RenderBox)child).size.width, global::Doroti.Framework.Painting.Axis.vertical => ((RenderBox)child).size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        RenderBox? child = lastChild;
        var boxResult = BoxHitTestResult.CreateWrap(result);
        while ((child is not null))
        {
            if (hitTestBoxChild(boxResult, child, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition))
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
        var __child = (RenderBox)(object)child;
        return (DartRuntimePrimitives.RequireValue(childScrollOffset(__child)) - ((SliverConstraints)constraints).scrollOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? childScrollOffset(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)((RenderObject)child).parentData!)!;
        return childParentData.layoutOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)__child.parentData)!;
        return ((childParentData.index is not null) && !this._keepAliveBucket.ContainsKey(DartRuntimePrimitives.RequireValue(childParentData.index)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
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
        if ((firstChild is null))
        {
            return;
        }
        global::Doroti.Ui.Offset mainAxisUnit = default!;
        global::Doroti.Ui.Offset crossAxisUnit = default!;
        global::Doroti.Ui.Offset originOffset = default!;
        bool addExtent = default!;
        switch (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection))
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    mainAxisUnit = new global::Doroti.Ui.Offset(0.0, -1.0);
                    crossAxisUnit = new global::Doroti.Ui.Offset(1.0, 0.0);
                    originOffset = (offset + new global::Doroti.Ui.Offset(0.0, geometry!.paintExtent));
                    addExtent = true;
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    mainAxisUnit = new global::Doroti.Ui.Offset(1.0, 0.0);
                    crossAxisUnit = new global::Doroti.Ui.Offset(0.0, 1.0);
                    originOffset = offset;
                    addExtent = false;
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    mainAxisUnit = new global::Doroti.Ui.Offset(0.0, 1.0);
                    crossAxisUnit = new global::Doroti.Ui.Offset(1.0, 0.0);
                    originOffset = offset;
                    addExtent = false;
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    mainAxisUnit = new global::Doroti.Ui.Offset(-1.0, 0.0);
                    crossAxisUnit = new global::Doroti.Ui.Offset(0.0, 1.0);
                    originOffset = (offset + new global::Doroti.Ui.Offset(geometry!.paintExtent, 0.0));
                    addExtent = true;
                    break;
                }
        }
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            double mainAxisDelta = childMainAxisPosition(child);
            double crossAxisDelta = childCrossAxisPosition(child);
            var childOffset = new global::Doroti.Ui.Offset(((originOffset.dx + (mainAxisUnit.dx * mainAxisDelta)) + (crossAxisUnit.dx * crossAxisDelta)), ((originOffset.dy + (mainAxisUnit.dy * mainAxisDelta)) + (crossAxisUnit.dy * crossAxisDelta)));
            if (addExtent)
            {
                childOffset += (mainAxisUnit * paintExtentOf(child));
            }
            if (((mainAxisDelta < ((SliverConstraints)constraints).remainingPaintExtent) && ((mainAxisDelta + paintExtentOf(child)) > 0L)))
            {
                context.paintChild(child, childOffset);
            }
            child = childAfter(child);
        }
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsNode(((firstChild is not null) ? $"currently live children: {indexOf(firstChild!)} to {indexOf(lastChild!)}" : "no children current live")));
    }

    public virtual bool debugAssertChildListIsNonEmptyAndContiguous()
    {
        DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() => (firstChild is not null));
                long index = indexOf(firstChild!);
                RenderBox? child = childAfter(firstChild!);
                while ((child is not null))
                {
                    index += 1L;
                    DartRuntimePrimitives.Assert(() => (indexOf(child) == index));
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
        if ((firstChild is not null))
        {
            RenderBox? child = firstChild;
            while (true)
            {
                var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child!.parentData!)!;
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__29592 with index {((SliverMultiBoxAdaptorParentData)childParentData).index}"));
                if ((object.Equals(child, lastChild)))
                {
                    break;
                }
                child = childParentData.nextSibling;
            }
        }
        if ((checked((long)(this._keepAliveBucket.Count)) != 0))
        {
            List<long> indices = ((Func<List<long>>)(() =>
{
    var __cascade = this._keepAliveBucket.Keys.ToList();
    __cascade.sort();
    return __cascade;
}))();
            foreach (var indexLocal in indices)
            {
                children.Add(((Diagnosticable)this._keepAliveBucket.GetValueOrDefault(indexLocal)!).toDiagnosticsNode(name: $"child with index {indexLocal} (kept alive but not laid out)", style: DiagnosticsTreeStyle.offstage));
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((SliverMultiBoxAdaptorParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData = ((SliverMultiBoxAdaptorParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((SliverMultiBoxAdaptorParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((SliverMultiBoxAdaptorParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
            }
        }
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
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((SliverMultiBoxAdaptorParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((SliverMultiBoxAdaptorParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _getRightWayUp(SliverConstraints constraints)
    {
        bool reversed = global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).axisDirection);
        return (((SliverConstraints)constraints).growthDirection switch { GrowthDirection.forward => !reversed, GrowthDirection.reverse => reversed, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestBoxChild(BoxHitTestResult result, RenderBox child, double mainAxisPosition, double crossAxisPosition)
    {
        bool rightWayUp = _getRightWayUp(constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        double absolutePosition = (mainAxisPosition - delta);
        double absoluteCrossAxisPosition = (crossAxisPosition - crossAxisDelta);
        global::Doroti.Ui.Offset paintOffsetLocal = default!;
        global::Doroti.Ui.Offset transformedPosition = default!;
        switch (((SliverConstraints)constraints).axis)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp)
                    {
                        absolutePosition = (((RenderBox)child).size.width - absolutePosition);
                        delta = ((geometry!.paintExtent - ((RenderBox)child).size.width) - delta);
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
                        delta = ((geometry!.paintExtent - ((RenderBox)child).size.height) - delta);
                    }
                    paintOffsetLocal = new global::Doroti.Ui.Offset(crossAxisDelta, delta);
                    transformedPosition = new global::Doroti.Ui.Offset(absoluteCrossAxisPosition, absolutePosition);
                    break;
                }
        }
        return result.addWithOutOfBandPosition(paintOffset: paintOffsetLocal, hitTest: ((Func<BoxHitTestResult, bool>)((result) =>
        {
            return child.hitTest(result, position: transformedPosition);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyPaintTransformForBoxChild(RenderBox child, Matrix4 transform)
    {
        bool rightWayUp = _getRightWayUp(constraints);
        double delta = childMainAxisPosition(child);
        double crossAxisDelta = childCrossAxisPosition(child);
        switch (((SliverConstraints)constraints).axis)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp)
                    {
                        delta = ((geometry!.paintExtent - ((RenderBox)child).size.width) - delta);
                    }
                    transform.translateByDouble(delta, crossAxisDelta, 0, 1);
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    if (!rightWayUp)
                    {
                        delta = ((geometry!.paintExtent - ((RenderBox)child).size.height) - delta);
                    }
                    transform.translateByDouble(crossAxisDelta, delta, 0, 1);
                    break;
                }
        }
    }

}
