// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/flow.dart
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

public interface FlowPaintingContext
{
    public global::Doroti.Ui.Size size { get; }
    public long childCount { get; }
    public global::Doroti.Ui.Size? getChildSize(long i);
    public void paintChild(long i, Matrix4 transform = default!, double opacity = 1.0);
}

public abstract class FlowDelegate
{
    internal virtual Listenable? _repaint { get; private set; }

    protected FlowDelegate(Listenable? repaint = null)
    {
        this._repaint = repaint;
    }

    public virtual global::Doroti.Ui.Size getSize(BoxConstraints constraints) => ((BoxConstraints)constraints).biggest;
    public virtual BoxConstraints getConstraintsForChild(long i, BoxConstraints constraints) => constraints;
    public abstract void paintChildren(FlowPaintingContext context);
    public virtual bool shouldRelayout(FlowDelegate oldDelegate) => false;
    public abstract bool shouldRepaint(FlowDelegate oldDelegate);
    public override string ToString() => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FlowDelegate");
}

public class FlowParentData : ContainerBoxParentData<RenderBox>
{
    internal virtual Matrix4? _transform { get; set; } = default;

}

public class RenderFlow : RenderBox, ContainerRenderObjectMixin<RenderBox, FlowParentData>, RenderBoxContainerDefaultsMixin<RenderBox, FlowParentData>, FlowPaintingContext
{
    internal virtual FlowDelegate _delegate { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual List<RenderBox> _randomAccessChildren { get; private set; } = new List<RenderBox>();
    internal virtual List<long> _lastPaintOrder { get; private set; } = new List<long>();
    internal virtual PaintingContext? _paintingContext { get; set; } = default;
    internal virtual Offset? _paintingOffset { get; set; } = default;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } = new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderFlow(List<RenderBox>? children = null, FlowDelegate @delegate = default!, Clip clipBehavior = Clip.hardEdge)
    {
        this._delegate = @delegate;
        this._clipBehavior = clipBehavior;
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        ParentData? childParentData__8175 = __child.parentData;
        if ((childParentData__8175 is FlowParentData))
        {
            FlowParentData childParentData__8175__as8219 = (FlowParentData)childParentData__8175;
            childParentData__8175__as8219._transform = null;
        }
        else
        {
            __child.parentData = new FlowParentData();
        }
    }

    public virtual FlowDelegate @delegate
    {
        get => this._delegate;
        set
        {
            var newDelegate = value;
            if ((object.Equals(this._delegate, newDelegate)))
            {
                return;
            }
            FlowDelegate oldDelegate__8960 = this._delegate;
            _delegate = newDelegate;
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate__8960))) || newDelegate.shouldRelayout(oldDelegate__8960)))
            {
                markNeedsLayout();
            }
            else
            {
                if (newDelegate.shouldRepaint(oldDelegate__8960))
                {
                    markNeedsPaint();
                }
            }
            if (attached)
            {
                ((FlowDelegate)oldDelegate__8960)._repaint?.removeListener(markNeedsPaint);
                ((FlowDelegate)newDelegate)._repaint?.addListener(markNeedsPaint);
            }
        }
    }
    public virtual global::Doroti.Ui.Clip clipBehavior
    {
        get => this._clipBehavior;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._clipBehavior)))
            {
                _clipBehavior = __value;
                markNeedsPaint();
                markNeedsSemanticsUpdate();
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
            var childParentData__181891 = ((FlowParentData?)(object?)child__181803.parentData!)!;
            child__181803 = childParentData__181891.nextSibling;
        }
        ((FlowDelegate)this._delegate)._repaint?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        ((FlowDelegate)this._delegate)._repaint?.removeListener(markNeedsPaint);
        base.detach();
        RenderBox? child__182065 = this._firstChild;
        while ((child__182065 is not null))
        {
            child__182065.detach();
            var childParentData__182148 = ((FlowParentData?)(object?)child__182065.parentData!)!;
            child__182065 = childParentData__182148.nextSibling;
        }
    }

    internal virtual global::Doroti.Ui.Size _getSize(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        return constraints.constrain(this._delegate.getSize(constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isRepaintBoundary => true;
    public override double computeMinIntrinsicWidth(double height)
    {
        double width__10539 = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(width__10539))
        {
            return width__10539;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double width__10763 = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(width__10763))
        {
            return width__10763;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double height__10987 = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(height__10987))
        {
            return height__10987;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double height__11213 = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(height__11213))
        {
            return height__11213;
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
        BoxConstraints constraints__11549 = this.constraints;
        size = _getSize(constraints__11549);
        var i__11623 = 0L;
        this._randomAccessChildren.Clear();
        RenderBox? child__11680 = firstChild;
        while ((child__11680 is not null))
        {
            this._randomAccessChildren.Add(child__11680);
            BoxConstraints innerConstraints__11795 = this._delegate.getConstraintsForChild(i__11623, constraints__11549);
            child__11680.layout(innerConstraints__11795, parentUsesSize: true);
            var childParentData__11936 = ((FlowParentData?)(object?)child__11680.parentData!)!;
            childParentData__11936.offset = Offset.zero;
            child__11680 = childParentData__11936.nextSibling;
            i__11623 += 1L;
        }
    }

    public virtual global::Doroti.Ui.Size? getChildSize(long i)
    {
        if (((i < 0L) || (i >= checked((long)(this._randomAccessChildren.Count)))))
        {
            return null;
        }
        return this._randomAccessChildren[(int)(i)].size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void paintChild(long i, Matrix4? transform = null, double opacity = 1.0)
    {
        transform ??= Matrix4.identity();
        RenderBox child__12707 = this._randomAccessChildren[(int)(i)];
        var childParentData__12751 = ((FlowParentData?)(object?)child__12707.parentData!)!;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((FlowParentData)childParentData__12751)._transform is not null))
                {
                    throw new FlutterError("Cannot call paintChild twice for the same child.\n" + $"The flow delegate of type {DartRuntimePrimitives.RuntimeType(this._delegate)} attempted to " + $"paint child {i} multiple times, which is not permitted.");
                }
                return true;
            });
        this._lastPaintOrder.Add(i);
        childParentData__12751._transform = transform;
        if ((opacity == 0.0))
        {
            return;
        }
        void painter(PaintingContext context, Offset offset)
        {
            context.paintChild(child__12707, offset);
        }
        if ((opacity == 1.0))
        {
            this._paintingContext!.pushTransform(needsCompositing, DartRuntimePrimitives.RequireValue(this._paintingOffset), transform, (Action<PaintingContext, Offset>)painter);
        }
        else
        {
            this._paintingContext!.pushOpacity(DartRuntimePrimitives.RequireValue(this._paintingOffset), Dart_uiLibrary.Color.getAlphaFromOpacity(opacity), ((Action<PaintingContext, Offset>)((context, offset) =>
            {
                context.pushTransform(needsCompositing, offset, transform!, (Action<PaintingContext, Offset>)painter);
            })));
        }
    }

    internal virtual void _paintWithDelegate(PaintingContext context, Offset offset)
    {
        this._lastPaintOrder.Clear();
        _paintingContext = context;
        _paintingOffset = offset;
        foreach (RenderBox child__14089 in this._randomAccessChildren)
        {
            var childParentData__14135 = ((FlowParentData?)(object?)child__14089.parentData!)!;
            childParentData__14135._transform = null;
        }
        try
        {
            this._delegate.paintChildren(this);
        }
        finally
        {
            _paintingContext = null;
            _paintingOffset = null;
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        this._clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, (Offset.zero & size), (Action<PaintingContext, Offset>)this._paintWithDelegate, clipBehavior: this.clipBehavior, oldLayer: ((LayerHandle<ClipRectLayer>)this._clipRectLayer).layer);
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        base.dispose();
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        List<RenderBox> children__14951 = getChildrenAsList();
        for (long i__14996 = (checked((long)(this._lastPaintOrder.Count)) - 1L); (i__14996 >= 0L); --i__14996)
        {
            long childIndex__15059 = this._lastPaintOrder[(int)(i__14996)];
            if ((childIndex__15059 >= checked((long)(children__14951.Count))))
            {
                continue;
            }
            RenderBox child__15183 = children__14951[(int)(childIndex__15059)];
            var childParentData__15225 = ((FlowParentData?)(object?)child__15183.parentData!)!;
            Matrix4? transform__15301 = ((FlowParentData)childParentData__15225)._transform;
            if ((transform__15301 is null))
            {
                continue;
            }
            bool absorbed__15415 = result.addWithPaintTransform(transform: transform__15301, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, position) =>
            {
                return child__15183.hitTest(result, position: position);
                return default;
            })));
            if (absorbed__15415)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        var childParentData__15823 = ((FlowParentData?)(object?)__child.parentData!)!;
        if ((((FlowParentData)childParentData__15823)._transform is not null))
        {
            transform.multiply(((FlowParentData)childParentData__15823)._transform!);
        }
        base.applyPaintTransform(__child, transform);
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173585 = ((FlowParentData?)(object?)child.parentData!)!;
        while ((childParentData__173585.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173585.previousSibling, child)));
            child = childParentData__173585.previousSibling!;
            childParentData__173585 = ((FlowParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData__173981 = ((FlowParentData?)(object?)child.parentData!)!;
        while ((childParentData__173981.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData__173981.nextSibling, child)));
            child = childParentData__173981.nextSibling!;
            childParentData__173981 = ((FlowParentData?)(object?)child.parentData!)!;
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
        var childParentData__175971 = ((FlowParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData__175971.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData__175971.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData__175971.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData__176343 = ((FlowParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData__176766 = ((FlowParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData__177424 = ((FlowParentData?)(object?)childParentData__175971.previousSibling!.parentData!)!;
                var childNextSiblingParentData__177547 = ((FlowParentData?)(object?)childParentData__175971.nextSibling!.parentData!)!;
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
        DartRuntimePrimitives.Assert(() => (child.parentData is FlowParentData));
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
        var childParentData__179226 = ((FlowParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData__179613 = ((FlowParentData?)(object?)childParentData__179226.previousSibling!.parentData!)!;
            childPreviousSiblingParentData__179613.nextSibling = childParentData__179226.nextSibling;
        }
        if ((childParentData__179226.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData__179226.previousSibling;
        }
        else
        {
            var childNextSiblingParentData__179965 = ((FlowParentData?)(object?)childParentData__179226.nextSibling!.parentData!)!;
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
            var childParentData__180684 = ((FlowParentData?)(object?)child__180623.parentData!)!;
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
        var childParentData__181479 = ((FlowParentData?)(object?)child.parentData!)!;
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
            var childParentData__182399 = ((FlowParentData?)(object?)child__182311.parentData!)!;
            child__182311 = childParentData__182399.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child__182587 = this._firstChild;
        while ((child__182587 is not null))
        {
            visitor(child__182587);
            var childParentData__182670 = ((FlowParentData?)(object?)child__182587.parentData!)!;
            child__182587 = childParentData__182670.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183103 = ((FlowParentData?)(object?)child.parentData!)!;
        return childParentData__183103.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData__183356 = ((FlowParentData?)(object?)child.parentData!)!;
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
                children__183528.Add(((Diagnosticable)child__183606).toDiagnosticsNode(name: $"child__183606 {count__183637}"));
                if ((object.Equals(child__183606, this.lastChild)))
                {
                    break;
                }
                count__183637 += 1L;
                var childParentData__183833 = ((FlowParentData?)(object?)child__183606.parentData!)!;
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
            var childParentData__138777 = ((FlowParentData?)(object?)child__138717.parentData!)!;
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
            var childParentData__139488 = ((FlowParentData?)(object?)child__139428.parentData!)!;
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
            var childParentData__140418 = ((FlowParentData?)(object?)child__140279.parentData!)!;
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
            var childParentData__141300 = ((FlowParentData?)(object?)child__141240.parentData!)!;
            context.paintChild(child__141240, (childParentData__141300.offset + offset));
            child__141240 = childParentData__141300.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result__141793 = new List<RenderBox>();
        RenderBox? child__141832 = firstChild;
        while ((child__141832 is not null))
        {
            var childParentData__141892 = ((FlowParentData?)(object?)child__141832.parentData!)!;
            result__141793.Add(((RenderBox?)(object?)child__141832)!);
            child__141832 = childParentData__141892.nextSibling;
        }
        return result__141793;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

