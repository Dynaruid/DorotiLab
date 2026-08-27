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

namespace Doroti.Framework.Rendering;

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
    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "FlowDelegate");
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
        ParentData? childParentData = __child.parentData;
        if ((childParentData is FlowParentData))
        {
            FlowParentData childParentData__8175__as8219 = (FlowParentData)childParentData;
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
            FlowDelegate oldDelegate = this._delegate;
            _delegate = newDelegate;
            if (((!object.Equals(DartRuntimePrimitives.RuntimeType(newDelegate), DartRuntimePrimitives.RuntimeType(oldDelegate))) || newDelegate.shouldRelayout(oldDelegate)))
            {
                markNeedsLayout();
            }
            else
            {
                if (newDelegate.shouldRepaint(oldDelegate))
                {
                    markNeedsPaint();
                }
            }
            if (attached)
            {
                ((FlowDelegate)oldDelegate)._repaint?.removeListener(markNeedsPaint);
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
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.attach(owner);
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        ((FlowDelegate)this._delegate)._repaint?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        ((FlowDelegate)this._delegate)._repaint?.removeListener(markNeedsPaint);
        base.detach();
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            child.detach();
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
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
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
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
        BoxConstraints constraintsLocal = this.constraints;
        size = _getSize(constraintsLocal);
        var i = 0L;
        this._randomAccessChildren.Clear();
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            this._randomAccessChildren.Add(child);
            BoxConstraints innerConstraints = this._delegate.getConstraintsForChild(i, constraintsLocal);
            child.layout(innerConstraints, parentUsesSize: true);
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            childParentData.offset = Offset.zero;
            child = childParentData.nextSibling;
            i += 1L;
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
        RenderBox child = this._randomAccessChildren[(int)(i)];
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((FlowParentData)childParentData)._transform is not null))
                {
                    throw new FlutterError("Cannot call paintChild twice for the same child.\n" + $"The flow delegate of type {DartRuntimePrimitives.RuntimeType(this._delegate)} attempted to " + $"paint child {i} multiple times, which is not permitted.");
                }
                return true;
            });
        this._lastPaintOrder.Add(i);
        childParentData._transform = transform;
        if ((opacity == 0.0))
        {
            return;
        }
        void painter(PaintingContext context, Offset offset)
        {
            context.paintChild(child, offset);
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
        foreach (RenderBox child in this._randomAccessChildren)
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            childParentData._transform = null;
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
        List<RenderBox> children = getChildrenAsList();
        for (long i = (checked((long)(this._lastPaintOrder.Count)) - 1L); (i >= 0L); --i)
        {
            long childIndex = this._lastPaintOrder[(int)(i)];
            if ((childIndex >= checked((long)(children.Count))))
            {
                continue;
            }
            RenderBox child = children[(int)(childIndex)];
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            Matrix4? transformLocal = ((FlowParentData)childParentData)._transform;
            if ((transformLocal is null))
            {
                continue;
            }
            bool absorbed = result.addWithPaintTransform(transform: transformLocal, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, position) =>
            {
                return child.hitTest(result, position: position);
                return default;
            })));
            if (absorbed)
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
        var childParentData = ((FlowParentData?)(object?)__child.parentData!)!;
        if ((((FlowParentData)childParentData)._transform is not null))
        {
            transform.multiply(((FlowParentData)childParentData)._transform!);
        }
        base.applyPaintTransform(__child, transform);
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        while ((childParentData.previousSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.previousSibling, child)));
            child = childParentData.previousSibling!;
            childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        }
        return (object.Equals(child, equals));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        while ((childParentData.nextSibling is not null))
        {
            DartRuntimePrimitives.Assert(() => (!object.Equals(childParentData.nextSibling, child)));
            child = childParentData.nextSibling!;
            childParentData = ((FlowParentData?)(object?)child.parentData!)!;
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
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => (childParentData.nextSibling is null));
        DartRuntimePrimitives.Assert(() => (childParentData.previousSibling is null));
        this._childCount += 1L;
        DartRuntimePrimitives.Assert(() => (this._childCount > 0L));
        if ((after is null))
        {
            childParentData.nextSibling = this._firstChild;
            if ((this._firstChild is not null))
            {
                var firstChildParentData = ((FlowParentData?)(object?)this._firstChild!.parentData!)!;
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
            var afterParentData = ((FlowParentData?)(object?)after.parentData!)!;
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
                var childPreviousSiblingParentData = ((FlowParentData?)(object?)childParentData.previousSibling!.parentData!)!;
                var childNextSiblingParentData = ((FlowParentData?)(object?)childParentData.nextSibling!.parentData!)!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => (object.Equals(afterParentData.nextSibling, child)));
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
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
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
            var childPreviousSiblingParentData = ((FlowParentData?)(object?)childParentData.previousSibling!.parentData!)!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if ((childParentData.nextSibling is null))
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(this._lastChild, child)));
            this._lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = ((FlowParentData?)(object?)childParentData.nextSibling!.parentData!)!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        this._childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
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
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        if ((object.Equals(childParentData.previousSibling, after)))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            redepthChild(child);
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = this._firstChild;
        while ((child is not null))
        {
            visitor(child);
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => this._firstChild;
    public virtual RenderBox? lastChild => this._lastChild;
    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child.parent, this)));
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if ((this.firstChild is not null))
        {
            RenderBox child = this.firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}"));
                if ((object.Equals(child, this.lastChild)))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if ((result is not null))
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return (DartRuntimePrimitives.RequireValue(result__138852__value138916) + childParentData.offset.dy);
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            BaselineOffset candidate = (new BaselineOffset(child.getDistanceToActualBaseline(baseline)).op_Add(childParentData.offset.dy));
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while ((child is not null))
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(offset: childParentData.offset, position: position, hitTest: ((Func<BoxHitTestResult, Offset, bool>)((result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position - childParentData.offset))));
                return child!.hitTest(result, position: transformed);
                return default;
            })));
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            context.paintChild(child, (childParentData.offset + offset));
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while ((child is not null))
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

