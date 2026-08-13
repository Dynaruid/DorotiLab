// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_multi_box_adaptor.dart
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
        var childParentData__10828 = ((SliverMultiBoxAdaptorParentData?)(object?)((RenderObject)child).parentData!)!;
        if (!((SliverMultiBoxAdaptorParentData)childParentData__10828)._keptAlive)
        {
            this.childManager.didAdoptChild(((RenderBox?)(object?)child)!);
        }
    }

    internal virtual bool _debugAssertChildListLocked() => this.childManager.debugAssertChildListLocked();
    internal virtual bool _debugVerifyChildOrder()
    {
        if (this._debugChildIntegrityEnabled)
        {
            RenderBox? child__11300 = firstChild;
            long index__11330 = default!;
            while ((child__11300 is not null))
            {
                index__11330 = indexOf(child__11300);
                child__11300 = childAfter(child__11300);
                DartRuntimePrimitives.Assert(() => ((child__11300 is null) || (indexOf(child__11300) > index__11330)));
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
        var childParentData__12269 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        if (!((SliverMultiBoxAdaptorParentData)childParentData__12269).keptAlive)
        {
            DartRuntimePrimitives.Assert(() => !object.Equals(child, this));
            DartRuntimePrimitives.Assert(() => !object.Equals(after, this));
            DartRuntimePrimitives.Assert(() => !EqualityComparer<RenderBox>.Default.Equals(child, after));
            DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
            var childParentData__181479 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            if (EqualityComparer<RenderBox>.Default.Equals(childParentData__181479.previousSibling, after))
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
            if ((object.Equals(this._keepAliveBucket.GetValueOrDefault(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__12269).index)), child)))
            {
                this._keepAliveBucket.remove(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__12269).index));
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    this._debugDanglingKeepAlives.Remove(child);
                    return true;
                });
            this.childManager.didAdoptChild(child);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (this._keepAliveBucket.ContainsKey(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__12269).index)))
                    {
                        this._debugDanglingKeepAlives.Add(this._keepAliveBucket.GetValueOrDefault(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__12269).index))!);
                    }
                    return true;
                });
            this._keepAliveBucket[DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__12269).index)] = child;
        }
    }

    public virtual void remove(RenderBox child)
    {
        var childParentData__13751 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        if (!((SliverMultiBoxAdaptorParentData)childParentData__13751)._keptAlive)
        {
            _removeFromChildList(child);
            dropChild(child);
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(this._keepAliveBucket.GetValueOrDefault(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__13751).index)), child)));
        DartRuntimePrimitives.Assert(() =>
            {
                this._debugDanglingKeepAlives.Remove(child);
                return true;
            });
        this._keepAliveBucket.remove(DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__13751).index));
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child__180623 = this._firstChild;
        while ((child__180623 is not null))
        {
            var childParentData__180684 = ((SliverMultiBoxAdaptorParentData?)(object?)child__180623.parentData!)!;
            RenderBox? next__180762 = childParentData__180684.nextSibling;
            childParentData__180684.previousSibling = null;
            childParentData__180684.nextSibling = null;
            dropChild(child__180623);
            child__180623 = next__180762;
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
                RenderBox child__14548 = this._keepAliveBucket.remove(index)!;
                var childParentData__14603 = ((SliverMultiBoxAdaptorParentData?)(object?)child__14548.parentData!)!;
                DartRuntimePrimitives.Assert(() => ((SliverMultiBoxAdaptorParentData)childParentData__14603)._keptAlive);
                dropChild(child__14548);
                child__14548.parentData = childParentData__14603;
                insert(child__14548, after: after);
                childParentData__14603._keptAlive = false;
            }
            else
            {
                this._childManager.createChild(index, after: after);
            }
        })));
    }

    internal virtual void _destroyOrCacheChild(RenderBox child)
    {
        var childParentData__15019 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        if (childParentData__15019.keepAlive)
        {
            DartRuntimePrimitives.Assert(() => !((SliverMultiBoxAdaptorParentData)childParentData__15019)._keptAlive);
            remove(child);
            this._keepAliveBucket[DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__15019).index)] = child;
            child.parentData = childParentData__15019;
            base.adoptChild(child);
            childParentData__15019._keptAlive = true;
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
        RenderBox? child__181803 = this._firstChild;
        while ((child__181803 is not null))
        {
            child__181803.attach(owner);
            var childParentData__181891 = ((SliverMultiBoxAdaptorParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
        foreach (RenderBox child__15597 in this._keepAliveBucket.Values)
        {
            child__15597.attach(owner);
        }
    }

    public override void detach()
    {
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((SliverMultiBoxAdaptorParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
        foreach (RenderBox child__15746 in this._keepAliveBucket.Values)
        {
            child__15746.detach();
        }
    }

    public override void redepthChildren()
    {
        RenderBox? child__182311 = this._firstChild;
        while ((child__182311 is not null))
        {
            redepthChild(child__182311);
            var childParentData__182399 = ((SliverMultiBoxAdaptorParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
        this._keepAliveBucket.Values.forEach(redepthChild);
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((SliverMultiBoxAdaptorParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
        this._keepAliveBucket.Values.forEach(visitor);
    }

    public override void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((SliverMultiBoxAdaptorParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
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
            var firstChildParentData__17856 = ((SliverMultiBoxAdaptorParentData?)(object?)firstChild!.parentData!)!;
            firstChildParentData__17856.layoutOffset = layoutOffset;
            return true;
        }
        this.childManager.setDidUnderflow(true);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? insertAndLayoutLeadingChild(BoxConstraints childConstraints, bool parentUsesSize = false)
    {
        DartRuntimePrimitives.Assert(() => _debugAssertChildListLocked());
        long index__18962 = (indexOf(firstChild!) - 1L);
        _createOrObtainChild(index__18962, after: null);
        if ((indexOf(firstChild!) == index__18962))
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
        long index__20042 = (indexOf(after!) + 1L);
        _createOrObtainChild(index__20042, after: after);
        RenderBox? child__20139 = childAfter(after);
        if (((child__20139 is not null) && (indexOf(child__20139) == index__20042)))
        {
            child__20139.layout(childConstraints, parentUsesSize: parentUsesSize);
            return child__20139;
        }
        this.childManager.setDidUnderflow(true);
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long calculateLeadingGarbage(long firstIndex)
    {
        RenderBox? walker__20865 = firstChild;
        var leadingGarbage__20894 = 0L;
        while (((walker__20865 is not null) && (indexOf(walker__20865) < firstIndex)))
        {
            leadingGarbage__20894 += 1L;
            walker__20865 = childAfter(walker__20865);
        }
        return leadingGarbage__20894;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long calculateTrailingGarbage(long lastIndex)
    {
        RenderBox? walker__21561 = lastChild;
        var trailingGarbage__21589 = 0L;
        while (((walker__21561 is not null) && (indexOf(walker__21561) > lastIndex)))
        {
            trailingGarbage__21589 += 1L;
            walker__21561 = childBefore(walker__21561);
        }
        return trailingGarbage__21589;
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
                var childParentData__23296 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
                return !childParentData__23296.keepAlive;
                return default;
            })).ToList().forEach(((RenderSliverBoxChildManager)this._childManager).removeChild);
            DartRuntimePrimitives.Assert(() => (this._keepAliveBucket.Values.where(((child) =>
            {
                var childParentData__23583 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
                return !childParentData__23583.keepAlive;
                return default;
            })).Count() == 0));
        })));
    }

    public virtual long indexOf(RenderBox child)
    {
        var childParentData__23926 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (((SliverMultiBoxAdaptorParentData)childParentData__23926).index is not null));
        return DartRuntimePrimitives.RequireValue(((SliverMultiBoxAdaptorParentData)childParentData__23926).index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double paintExtentOf(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => ((RenderBox)child).hasSize);
        return (((SliverConstraints)constraints).axis switch { global::Doroti.Generated.Framework.Painting.Axis.horizontal => ((RenderBox)child).size.width, global::Doroti.Generated.Framework.Painting.Axis.vertical => ((RenderBox)child).size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        RenderBox? child__24623 = lastChild;
        var boxResult__24652 = BoxHitTestResult.CreateWrap(result);
        while ((child__24623 is not null))
        {
            if (hitTestBoxChild(boxResult__24652, child__24623, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition))
            {
                return true;
            }
            child__24623 = childBefore(child__24623);
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
        var childParentData__25215 = ((SliverMultiBoxAdaptorParentData?)(object?)((RenderObject)child).parentData!)!;
        return childParentData__25215.layoutOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool paintsChild(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        var childParentData__25393 = ((SliverMultiBoxAdaptorParentData?)(object?)__child.parentData)!;
        return ((childParentData__25393.index is not null) && !this._keepAliveBucket.ContainsKey(DartRuntimePrimitives.RequireValue(childParentData__25393.index)));
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
        global::Doroti.Flutter.Ui.Offset mainAxisUnit__26623 = default!;
        global::Doroti.Flutter.Ui.Offset crossAxisUnit__26637 = default!;
        global::Doroti.Flutter.Ui.Offset originOffset__26652 = default!;
        bool addExtent__26681 = default!;
        switch (global::Doroti.Generated.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection))
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
                {
                    mainAxisUnit__26623 = new global::Doroti.Flutter.Ui.Offset(0.0, -1.0);
                    crossAxisUnit__26637 = new global::Doroti.Flutter.Ui.Offset(1.0, 0.0);
                    originOffset__26652 = (offset + new global::Doroti.Flutter.Ui.Offset(0.0, geometry!.paintExtent));
                    addExtent__26681 = true;
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
                {
                    mainAxisUnit__26623 = new global::Doroti.Flutter.Ui.Offset(1.0, 0.0);
                    crossAxisUnit__26637 = new global::Doroti.Flutter.Ui.Offset(0.0, 1.0);
                    originOffset__26652 = offset;
                    addExtent__26681 = false;
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                {
                    mainAxisUnit__26623 = new global::Doroti.Flutter.Ui.Offset(0.0, 1.0);
                    crossAxisUnit__26637 = new global::Doroti.Flutter.Ui.Offset(1.0, 0.0);
                    originOffset__26652 = offset;
                    addExtent__26681 = false;
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                {
                    mainAxisUnit__26623 = new global::Doroti.Flutter.Ui.Offset(-1.0, 0.0);
                    crossAxisUnit__26637 = new global::Doroti.Flutter.Ui.Offset(0.0, 1.0);
                    originOffset__26652 = (offset + new global::Doroti.Flutter.Ui.Offset(geometry!.paintExtent, 0.0));
                    addExtent__26681 = true;
                    break;
                }
        }
        RenderBox? child__27648 = firstChild;
        while ((child__27648 is not null))
        {
            double mainAxisDelta__27715 = childMainAxisPosition(child__27648);
            double crossAxisDelta__27780 = childCrossAxisPosition(child__27648);
            var childOffset__27838 = new global::Doroti.Flutter.Ui.Offset(((originOffset__26652.dx + (mainAxisUnit__26623.dx * mainAxisDelta__27715)) + (crossAxisUnit__26637.dx * crossAxisDelta__27780)), ((originOffset__26652.dy + (mainAxisUnit__26623.dy * mainAxisDelta__27715)) + (crossAxisUnit__26637.dy * crossAxisDelta__27780)));
            if (addExtent__26681)
            {
                childOffset__27838 += (mainAxisUnit__26623 * paintExtentOf(child__27648));
            }
            if (((mainAxisDelta__27715 < ((SliverConstraints)constraints).remainingPaintExtent) && ((mainAxisDelta__27715 + paintExtentOf(child__27648)) > 0L)))
            {
                context.paintChild(child__27648, childOffset__27838);
            }
            child__27648 = childAfter(child__27648);
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
                long index__29176 = indexOf(firstChild!);
                RenderBox? child__29223 = childAfter(firstChild!);
                while ((child__29223 is not null))
                {
                    index__29176 += 1L;
                    DartRuntimePrimitives.Assert(() => (indexOf(child__29223) == index__29176));
                    child__29223 = childAfter(child__29223);
                }
                return true;
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children__29513 = new List<DiagnosticsNode>();
        if ((firstChild is not null))
        {
            RenderBox? child__29592 = firstChild;
            while (true)
            {
                var childParentData__29647 = ((SliverMultiBoxAdaptorParentData?)(object?)child__29592!.parentData!)!;
                children__29513.Add(((Diagnosticable)child__29592).toDiagnosticsNode(name: $"child__29592 with index {((SliverMultiBoxAdaptorParentData)childParentData__29647).index}"));
                if ((object.Equals(child__29592, lastChild)))
                {
                    break;
                }
                child__29592 = childParentData__29647.nextSibling;
            }
        }
        if ((checked((long)(this._keepAliveBucket.Count)) != 0))
        {
            List<long> indices__29999 = ((Func<List<long>>)(() =>
{
    var __cascade = this._keepAliveBucket.Keys.ToList();
    __cascade.sort();
    return __cascade;
}))();
            foreach (var index__30066 in indices__29999)
            {
                children__29513.Add(((Diagnosticable)this._keepAliveBucket.GetValueOrDefault(index__30066)!).toDiagnosticsNode(name: $"child with index {index__30066} (kept alive but not laid out)", style: DiagnosticsTreeStyle.offstage));
            }
        }
        return children__29513;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((SliverMultiBoxAdaptorParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((SliverMultiBoxAdaptorParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((SliverMultiBoxAdaptorParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((SliverMultiBoxAdaptorParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
                childPreviousSiblingParentData__177424.nextSibling = child;
                childNextSiblingParentData__177547.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData__176766.nextSibling, child)));
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
        var childParentData__179226 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((SliverMultiBoxAdaptorParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((SliverMultiBoxAdaptorParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
            childNextSiblingParentData__179965.previousSibling = childParentData__179226.previousSibling;
        }
        childParentData__179226.previousSibling = null;
        childParentData__179226.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
        return childParentData__183356.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _getRightWayUp(SliverConstraints constraints)
    {
        bool reversed__78998 = global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).axisDirection);
        return (((SliverConstraints)constraints).growthDirection switch { GrowthDirection.forward => !reversed__78998, GrowthDirection.reverse => reversed__78998, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool hitTestBoxChild(BoxHitTestResult result, RenderBox child, double mainAxisPosition, double crossAxisPosition)
    {
        bool rightWayUp__79845 = _getRightWayUp(constraints);
        double delta__79898 = childMainAxisPosition(child);
        double crossAxisDelta__79953 = childCrossAxisPosition(child);
        double absolutePosition__80012 = (mainAxisPosition - delta__79898);
        double absoluteCrossAxisPosition__80074 = (crossAxisPosition - crossAxisDelta__79953);
        global::Doroti.Flutter.Ui.Offset paintOffset__80149 = default!;
        global::Doroti.Flutter.Ui.Offset transformedPosition__80162 = default!;
        switch (((SliverConstraints)constraints).axis)
        {
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp__79845)
                    {
                        absolutePosition__80012 = (((RenderBox)child).size.width - absolutePosition__80012);
                        delta__79898 = ((geometry!.paintExtent - ((RenderBox)child).size.width) - delta__79898);
                    }
                    paintOffset__80149 = new global::Doroti.Flutter.Ui.Offset(delta__79898, crossAxisDelta__79953);
                    transformedPosition__80162 = new global::Doroti.Flutter.Ui.Offset(absolutePosition__80012, absoluteCrossAxisPosition__80074);
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    if (!rightWayUp__79845)
                    {
                        absolutePosition__80012 = (((RenderBox)child).size.height - absolutePosition__80012);
                        delta__79898 = ((geometry!.paintExtent - ((RenderBox)child).size.height) - delta__79898);
                    }
                    paintOffset__80149 = new global::Doroti.Flutter.Ui.Offset(crossAxisDelta__79953, delta__79898);
                    transformedPosition__80162 = new global::Doroti.Flutter.Ui.Offset(absoluteCrossAxisPosition__80074, absolutePosition__80012);
                    break;
                }
        }
        return result.addWithOutOfBandPosition(paintOffset: paintOffset__80149, hitTest: ((Func<BoxHitTestResult, bool>)((result) =>
        {
            return child.hitTest(result, position: transformedPosition__80162);
            return default;
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void applyPaintTransformForBoxChild(RenderBox child, Matrix4 transform)
    {
        bool rightWayUp__81586 = _getRightWayUp(constraints);
        double delta__81639 = childMainAxisPosition(child);
        double crossAxisDelta__81694 = childCrossAxisPosition(child);
        switch (((SliverConstraints)constraints).axis)
        {
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp__81586)
                    {
                        delta__81639 = ((geometry!.paintExtent - ((RenderBox)child).size.width) - delta__81639);
                    }
                    transform.translateByDouble(delta__81639, crossAxisDelta__81694, 0, 1);
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    if (!rightWayUp__81586)
                    {
                        delta__81639 = ((geometry!.paintExtent - ((RenderBox)child).size.height) - delta__81639);
                    }
                    transform.translateByDouble(crossAxisDelta__81694, delta__81639, 0, 1);
                    break;
                }
        }
    }

}
