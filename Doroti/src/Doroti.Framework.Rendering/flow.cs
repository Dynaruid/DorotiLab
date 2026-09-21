// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/flow.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public interface FlowPaintingContext
{
    public Size size { get; }
    public long childCount { get; }
    public Size? getChildSize(long i);
    public void paintChild(long i, Matrix4 transform = default!, double opacity = 1.0);
}

public abstract class FlowDelegate
{
    internal virtual Listenable? _repaint { get; private set; }

    protected FlowDelegate(Listenable? repaint = null)
    {
        _repaint = repaint;
    }

    public virtual Size getSize(BoxConstraints constraints) => constraints.biggest;

    public virtual BoxConstraints getConstraintsForChild(long i, BoxConstraints constraints) =>
        constraints;

    public abstract void paintChildren(FlowPaintingContext context);

    public virtual bool shouldRelayout(FlowDelegate oldDelegate) => false;

    public abstract bool shouldRepaint(FlowDelegate oldDelegate);

    public override string ToString() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "FlowDelegate");
}

public class FlowParentData : ContainerBoxParentData<RenderBox>
{
    internal virtual Matrix4? _transform { get; set; } = default;
}

public class RenderFlow
    : RenderBox,
        ContainerRenderObjectMixin<RenderBox, FlowParentData>,
        RenderBoxContainerDefaultsMixin<RenderBox, FlowParentData>,
        FlowPaintingContext
{
    internal virtual FlowDelegate _delegate { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.hardEdge;
    internal virtual List<RenderBox> _randomAccessChildren { get; private set; } =
        new List<RenderBox>();
    internal virtual List<long> _lastPaintOrder { get; private set; } = new List<long>();
    internal virtual PaintingContext? _paintingContext { get; set; } = default;
    internal virtual Offset? _paintingOffset { get; set; } = default;
    internal virtual LayerHandle<ClipRectLayer> _clipRectLayer { get; private set; } =
        new LayerHandle<ClipRectLayer>();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    public RenderFlow(
        List<RenderBox>? children = null,
        FlowDelegate @delegate = default!,
        Clip clipBehavior = Clip.hardEdge
    )
    {
        _delegate = @delegate;
        _clipBehavior = clipBehavior;
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        ParentData? childParentData = __child.parentData;
        if (childParentData is FlowParentData)
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
        get => _delegate;
        set
        {
            var newDelegate = value;
            if (Equals(_delegate, newDelegate))
            {
                return;
            }
            FlowDelegate oldDelegate = _delegate;
            _delegate = newDelegate;
            if (
                (
                    !Equals(
                        DartRuntimePrimitives.RuntimeType(newDelegate),
                        DartRuntimePrimitives.RuntimeType(oldDelegate)
                    )
                ) || newDelegate.shouldRelayout(oldDelegate)
            )
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
                oldDelegate._repaint?.removeListener(markNeedsPaint);
                newDelegate._repaint?.addListener(markNeedsPaint);
            }
        }
    }
    public virtual Clip clipBehavior
    {
        get => _clipBehavior;
        set
        {
            var __value = value;
            if (!Equals(__value, _clipBehavior))
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
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        _delegate._repaint?.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        _delegate._repaint?.removeListener(markNeedsPaint);
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    internal virtual Size _getSize(BoxConstraints constraints)
    {
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid());
        return constraints.constrain(_delegate.getSize(constraints));
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double widthLocal = _getSize(BoxConstraints.CreateTightForFinite(height: height)).width;
        if (double.IsFinite(widthLocal))
        {
            return widthLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        double heightLocal = _getSize(BoxConstraints.CreateTightForFinite(width: width)).height;
        if (double.IsFinite(heightLocal))
        {
            return heightLocal;
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _getSize(constraints);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        size = _getSize(constraintsLocal);
        var i = 0L;
        _randomAccessChildren.Clear();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            _randomAccessChildren.Add(child);
            BoxConstraints innerConstraints = _delegate.getConstraintsForChild(i, constraintsLocal);
            child.layout(innerConstraints, parentUsesSize: true);
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            childParentData.offset = Offset.zero;
            child = childParentData.nextSibling;
            i += 1L;
        }
    }

    public virtual Size? getChildSize(long i)
    {
        if ((i < 0L) || (i >= checked(_randomAccessChildren.Count)))
        {
            return null;
        }
        return _randomAccessChildren[(int)i].size;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void paintChild(long i, Matrix4? transform = null, double opacity = 1.0)
    {
        transform ??= Matrix4.identity();
        RenderBox child = _randomAccessChildren[(int)i];
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() =>
        {
            if (childParentData._transform is not null)
            {
                throw new FlutterError(
                    "Cannot call paintChild twice for the same child.\n"
                        + $"The flow delegate of type {DartRuntimePrimitives.RuntimeType(_delegate)} attempted to "
                        + $"paint child {i} multiple times, which is not permitted."
                );
            }
            return true;
        });
        _lastPaintOrder.Add(i);
        childParentData._transform = transform;
        if (opacity == 0.0)
        {
            return;
        }
        void painter(PaintingContext context, Offset offset)
        {
            context.paintChild(child, offset);
        }
        if (opacity == 1.0)
        {
            _paintingContext!.pushTransform(
                needsCompositing,
                (
                    _paintingOffset
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                transform,
                painter
            );
        }
        else
        {
            _paintingContext!.pushOpacity(
                (
                    _paintingOffset
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                DorotiUiLibrary.Color.getAlphaFromOpacity(opacity),
                (context, offset) =>
                {
                    context.pushTransform(needsCompositing, offset, transform!, painter);
                }
            );
        }
    }

    internal virtual void _paintWithDelegate(PaintingContext context, Offset offset)
    {
        _lastPaintOrder.Clear();
        _paintingContext = context;
        _paintingOffset = offset;
        foreach (RenderBox child in _randomAccessChildren)
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            childParentData._transform = null;
        }
        try
        {
            _delegate.paintChildren(this);
        }
        finally
        {
            _paintingContext = null;
            _paintingOffset = null;
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _clipRectLayer.layer = context.pushClipRect(
            needsCompositing,
            offset,
            Offset.zero & size,
            _paintWithDelegate,
            clipBehavior: clipBehavior,
            oldLayer: _clipRectLayer.layer
        );
    }

    public override void dispose()
    {
        _clipRectLayer.layer = null;
        base.dispose();
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        List<RenderBox> children = getChildrenAsList();
        for (long i = checked(_lastPaintOrder.Count) - 1L; i >= 0L; --i)
        {
            long childIndex = _lastPaintOrder[(int)i];
            if (childIndex >= checked(children.Count))
            {
                continue;
            }
            RenderBox child = children[(int)childIndex];
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            Matrix4? transformLocal = childParentData._transform;
            if (transformLocal is null)
            {
                continue;
            }
            bool absorbed = result.addWithPaintTransform(
                transform: transformLocal,
                position: position,
                hitTest: (result, position) =>
                {
                    return child.hitTest(result, position: position);
                }
            );
            if (absorbed)
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)(object)child;
        var childParentData = ((FlowParentData?)(object?)__child.parentData!)!;
        if (childParentData._transform is not null)
        {
            transform.multiply(childParentData._transform!);
        }
        base.applyPaintTransform(__child, transform);
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = ((FlowParentData?)(object?)_firstChild!.parentData!)!;
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
            var afterParentData = ((FlowParentData?)(object?)after.parentData!)!;
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
                    (FlowParentData?)(object?)childParentData.previousSibling!.parentData!
                )!;
                var childNextSiblingParentData = (
                    (FlowParentData?)(object?)childParentData.nextSibling!.parentData!
                )!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(() => child.parentData is FlowParentData);
        _insertIntoChildList(child, after: after);
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
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
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
                (FlowParentData?)(object?)childParentData.previousSibling!.parentData!
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
                (FlowParentData?)(object?)childParentData.nextSibling!.parentData!
            )!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;

    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderBox child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(
                    ((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}")
                );
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if (result is not null)
            {
                double result__138852__value138916 = (
                    result
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                );
                return (result__138852__value138916) + childParentData.offset.dy;
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            BaselineOffset candidate = new BaselineOffset(
                child.getDistanceToActualBaseline(baseline)
            ).op_Add(childParentData.offset.dy);
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(
                offset: childParentData.offset,
                position: position,
                hitTest: (result, transformed) =>
                {
                    DartRuntimePrimitives.Assert(() =>
                        Equals(transformed, position - childParentData.offset)
                    );
                    return child!.hitTest(result, position: transformed);
                }
            );
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((FlowParentData?)(object?)child.parentData!)!;
            result.Add(((RenderBox?)(object?)child)!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
