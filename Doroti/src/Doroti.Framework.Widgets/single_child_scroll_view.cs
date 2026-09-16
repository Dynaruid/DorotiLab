// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/single_child_scroll_view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class SingleChildScrollView : StatelessWidget
{
    public virtual global::Doroti.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual ScrollController? controller { get; private set; }
    public virtual bool? primary { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual Widget? child { get; private set; }
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior { get; private set; }

    public SingleChildScrollView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.vertical, bool reverse = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool? primary = null, ScrollPhysics? physics = null, ScrollController? controller = null, Widget? child = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = DragStartBehavior.start, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, string? restorationId = null, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null) : base(key: key)
    {
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.padding = padding;
        this.primary = primary;
        this.physics = physics;
        this.controller = controller;
        this.child = child;
        this.dragStartBehavior = dragStartBehavior;
        this.clipBehavior = clipBehavior;
        this.hitTestBehavior = hitTestBehavior;
        this.restorationId = restorationId;
        this.keyboardDismissBehavior = keyboardDismissBehavior;
        System.Diagnostics.Debug.Assert(!((controller is not null) && (primary ?? false)));
    }

    internal virtual global::Doroti.Framework.Painting.AxisDirection _getDirection(BuildContext context)
    {
        return BasicLibrary.getAxisDirectionFromAxisReverseAndDirectionality(context, scrollDirection, reverse);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        global::Doroti.Framework.Painting.AxisDirection axisDirectionLocal = _getDirection(context);
        Widget? contents = child;
        if (padding is not null)
        {
            contents = DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: padding!, child: contents));
        }
        bool effectivePrimary = primary ?? ((controller is null) && PrimaryScrollController.shouldInherit(context, scrollDirection));
        ScrollController? scrollController = effectivePrimary ? PrimaryScrollController.maybeOf(context) : controller;
        Widget scrollable = new Scrollable(dragStartBehavior: dragStartBehavior, axisDirection: axisDirectionLocal, controller: scrollController, physics: physics, restorationId: restorationId, clipBehavior: clipBehavior, hitTestBehavior: hitTestBehavior, viewportBuilder: (context, offset) =>
        {
            return new _SingleChildViewport__single_child_scroll_view(axisDirection: axisDirectionLocal, offset: offset, clipBehavior: clipBehavior, child: contents);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        ScrollViewKeyboardDismissBehavior effectiveKeyboardDismissBehavior = keyboardDismissBehavior ?? ScrollConfiguration.of(context).getKeyboardDismissBehavior(context);
        if (Equals(effectiveKeyboardDismissBehavior, ScrollViewKeyboardDismissBehavior.onDrag))
        {
            scrollable = DartRuntimePrimitives.ConvertValue<Widget>(new NotificationListener<ScrollUpdateNotification>(child: scrollable, onNotification: (notification) =>
            {
                FocusScopeNode currentScope = FocusScope.of(context);
                if ((notification.dragDetails is not null) && !currentScope.hasPrimaryFocus && currentScope.hasFocus)
                {
                    FocusManager.instance.primaryFocus?.unfocus();
                }
                return false;
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
        return (effectivePrimary && (scrollController is not null)) ? PrimaryScrollController.CreateNone(child: scrollable) : scrollable;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _SingleChildViewport__single_child_scroll_view : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.ViewportOffset offset { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    internal _SingleChildViewport__single_child_scroll_view(global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, Widget? child = null, Clip clipBehavior = default!) : base(child: child)
    {
        this.axisDirection = axisDirection;
        this.offset = offset;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSingleChildViewport__single_child_scroll_view(axisDirection: axisDirection, offset: offset, clipBehavior: clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSingleChildViewport__single_child_scroll_view)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSingleChildViewport__single_child_scroll_view>)(() =>
{
    var __cascade = __renderObject;
    __cascade.axisDirection = axisDirection;
    __cascade.offset = offset;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))());
    }

    public override SingleChildRenderObjectElement createElement()
    {
        return new _SingleChildViewportElement__single_child_scroll_view(this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SingleChildViewportElement__single_child_scroll_view : SingleChildRenderObjectElement, NotifiableElementMixin, ViewportElementMixin
{

    internal _SingleChildViewportElement__single_child_scroll_view(_SingleChildViewport__single_child_scroll_view widget) : base(widget)
    {
    }

    public override void attachNotificationTree()
    {
        _notificationTree = new _NotificationNode__framework(_parent?._notificationTree, this);
    }

    public virtual bool onNotification(Notification notification)
    {
        if (notification is ViewportNotificationMixin)
        {
            ((ViewportNotificationMixin)notification)._depth += 1L;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _RenderSingleChildViewport__single_child_scroll_view : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.RenderObjectWithChildMixin<global::Doroti.Framework.Rendering.RenderBox>
{
    internal virtual global::Doroti.Framework.Painting.AxisDirection _axisDirection { get; set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.ViewportOffset _offset { get; set; } = default!;
    internal virtual Clip _clipBehavior { get; set; } = Clip.none;
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer> _clipRectLayer { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer>();
    public virtual RenderBox? _child { get; set; } = default;

    internal _RenderSingleChildViewport__single_child_scroll_view(global::Doroti.Framework.Painting.AxisDirection axisDirection = AxisDirection.down, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, global::Doroti.Framework.Rendering.RenderBox? child = null, Clip clipBehavior = default!)
    {
        _axisDirection = axisDirection;
        _offset = offset;
        _clipBehavior = clipBehavior;
    }

    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection
    {
        get => _axisDirection;
        set
        {
            var __value = value;
            if (Equals(__value, _axisDirection))
            {
                return;
            }
            _axisDirection = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.Axis axis => Basic_typesLibrary.axisDirectionToAxis(axisDirection);
    public virtual global::Doroti.Framework.Rendering.ViewportOffset offset
    {
        get => _offset;
        set
        {
            var __value = value;
            if (Equals(__value, _offset))
            {
                return;
            }
            if (attached)
            {
                _offset.removeListener(_hasScrolled);
            }
            _offset = __value;
            if (attached)
            {
                _offset.addListener(_hasScrolled);
            }
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.Clip clipBehavior
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
    internal virtual void _hasScrolled()
    {
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        _child?.attach(owner);
        _offset.addListener(_hasScrolled);
    }

    public override void detach()
    {
        _offset.removeListener(_hasScrolled);
        base.detach();
        _child?.detach();
    }

    public override bool isRepaintBoundary => true;
    internal virtual double _viewportExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            return axis switch { Axis.horizontal => size.width, Axis.vertical => size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        }
    }
    internal virtual double _minScrollExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            return 0.0;
        }
    }
    internal virtual double _maxScrollExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => hasSize);
            if (child is null)
            {
                return 0.0;
            }
            return Math.Max(0.0, axis switch { Axis.horizontal => child!.size.width - size.width, Axis.vertical => child!.size.height - size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
    }
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _getInnerConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return axis switch { Axis.horizontal => constraints.heightConstraints(), Axis.vertical => constraints.widthConstraints(), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return child?.getMinIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return child?.getMaxIntrinsicWidth(height) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return child?.getMinIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return child?.getMaxIntrinsicHeight(width) ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        if (child is null)
        {
            return constraints.smallest;
        }
        global::Doroti.Ui.Size childSize = child!.getDryLayout(_getInnerConstraints(constraints));
        return constraints.constrain(childSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = constraints;
        if (child is null)
        {
            size = constraintsLocal.smallest;
        }
        else
        {
            child!.layout(_getInnerConstraints(constraintsLocal), parentUsesSize: true);
            size = constraintsLocal.constrain(child!.size);
        }
        if (offset.hasPixels)
        {
            if (offset.pixels > _maxScrollExtent)
            {
                offset.correctBy(_maxScrollExtent - offset.pixels);
            }
            else
            {
                if (offset.pixels < _minScrollExtent)
                {
                    offset.correctBy(_minScrollExtent - offset.pixels);
                }
            }
        }
        offset.applyViewportDimension(_viewportExtent);
        offset.applyContentDimensions(_minScrollExtent, _maxScrollExtent);
    }

    internal virtual global::Doroti.Ui.Offset _paintOffset => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_paintOffsetForPosition(offset.pixels));
    internal virtual global::Doroti.Ui.Offset _paintOffsetForPosition(double position)
    {
        return axisDirection switch { AxisDirection.up => new global::Doroti.Ui.Offset(0.0, position - child!.size.height + size.height), AxisDirection.left => new global::Doroti.Ui.Offset(position - child!.size.width + size.width, 0.0), AxisDirection.right => new global::Doroti.Ui.Offset(-position, 0.0), AxisDirection.down => new global::Doroti.Ui.Offset(0.0, -position), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldClipAtPaintOffset(Offset paintOffset)
    {
        DartRuntimePrimitives.Assert(() => child is not null);
        switch (clipBehavior)
        {
            case Clip.none:
                {
                    return false;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    return (paintOffset.dx < 0L) || (paintOffset.dy < 0L) || ((paintOffset.dx + child!.size.width) > size.width) || ((paintOffset.dy + child!.size.height) > size.height);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (child is not null)
        {
            global::Doroti.Ui.Offset paintOffset = _paintOffset;
            void paintContents(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
            {
                context.paintChild(child!, offset + paintOffset);
            }
            if (_shouldClipAtPaintOffset(paintOffset))
            {
                _clipRectLayer.layer = context.pushClipRect(needsCompositing, offset, Offset.zero & size, paintContents, clipBehavior: clipBehavior, oldLayer: _clipRectLayer.layer);
            }
            else
            {
                _clipRectLayer.layer = null;
                paintContents(context, offset);
            }
        }
    }

    public override void dispose()
    {
        _clipRectLayer.layer = null;
        base.dispose();
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        global::Doroti.Ui.Offset paintOffset = _paintOffset;
        transform.translateByDouble(paintOffset.dx, paintOffset.dy, 0, 1);
    }

    public override Rect? describeApproximatePaintClip(global::Doroti.Framework.Rendering.RenderObject child)
    {
        if ((child is not null) && _shouldClipAtPaintOffset(_paintOffset))
        {
            return Offset.zero & size;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (child is not null)
        {
            return result.addWithPaintOffset(offset: _paintOffset, position: position, hitTest: (result, transformed) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(transformed, position + -_paintOffset));
                return child!.hitTest(result, position: transformed);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RevealedOffset getOffsetToReveal(global::Doroti.Framework.Rendering.RenderObject target, double alignment, Rect? rect = null, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        axis = this.axis;
        rect ??= target.paintBounds;
        if (target is not RenderBox)
        {
            return new global::Doroti.Framework.Rendering.RevealedOffset(offset: offset.pixels, rect: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        }
        global::Doroti.Framework.Rendering.RenderBox targetBox = (global::Doroti.Framework.Rendering.RenderBox)target;
        Matrix4 transform = targetBox.getTransformTo(child);
        global::Doroti.Ui.Rect bounds = MatrixUtils.transformRect(transform, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        global::Doroti.Ui.Size contentSize = child!.size;
        var (mainAxisExtent, leadingScrollOffset, targetMainAxisExtent) = axisDirection switch { AxisDirection.up => (size.height, contentSize.height - bounds.bottom, bounds.height), AxisDirection.left => (size.width, contentSize.width - bounds.right, bounds.width), AxisDirection.right => (size.width, bounds.left, bounds.width), AxisDirection.down => (size.height, bounds.top, bounds.height), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        double targetOffset = leadingScrollOffset - ((mainAxisExtent - targetMainAxisExtent) * alignment);
        global::Doroti.Ui.Rect targetRect = bounds.shift(_paintOffsetForPosition(targetOffset));
        return new global::Doroti.Framework.Rendering.RevealedOffset(offset: targetOffset, rect: targetRect);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void showOnScreen(global::Doroti.Framework.Rendering.RenderObject? descendant = null, Rect? rect = null, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        if (!offset.allowImplicitScrolling)
        {
            base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
            return;
        }
        global::Doroti.Ui.Rect? newRect = _showInViewport(descendant, rect, this, offset, duration, curve);
        base.showOnScreen(rect: newRect, duration: duration, curve: curve);
    }

    private static Rect? _showInViewport(global::Doroti.Framework.Rendering.RenderObject? descendant, Rect? rect, _RenderSingleChildViewport__single_child_scroll_view viewport, global::Doroti.Framework.Rendering.ViewportOffset offset, Duration duration, global::Doroti.Framework.Animation.Curve curve) { if (descendant is null) return rect; var leading = viewport.getOffsetToReveal(descendant, 0.0, rect: rect); var trailing = viewport.getOffsetToReveal(descendant, 1.0, rect: rect); var target = RevealedOffset.clampOffset(leading, trailing, offset.pixels); if (target is null) return rect ?? descendant.paintBounds; _ = offset.moveTo(target.offset, duration: duration, curve: curve); return target.rect; }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", _paintOffset));
    }

    public override Rect? describeSemanticsClip(global::Doroti.Framework.Rendering.RenderObject? child)
    {
        double remainingOffset = _maxScrollExtent - offset.pixels;
        switch (axisDirection)
        {
            case AxisDirection.up:
                {
                    return Rect.fromLTRB(semanticBounds.left, semanticBounds.top - remainingOffset, semanticBounds.right, semanticBounds.bottom + offset.pixels);
                }
            case AxisDirection.right:
                {
                    return Rect.fromLTRB(semanticBounds.left - offset.pixels, semanticBounds.top, semanticBounds.right + remainingOffset, semanticBounds.bottom);
                }
            case AxisDirection.down:
                {
                    return Rect.fromLTRB(semanticBounds.left, semanticBounds.top - offset.pixels, semanticBounds.right, semanticBounds.bottom + remainingOffset);
                }
            case AxisDirection.left:
                {
                    return Rect.fromLTRB(semanticBounds.left - remainingOffset, semanticBounds.top, semanticBounds.right + offset.pixels, semanticBounds.bottom);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (child is not RenderBox)
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {GetType()} that expected a {typeof(RenderBox)} child was created by", debugCreator, style: DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", child.debugCreator, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? child
    {
        get => _child;
        set
        {
            var __value = value;
            if (_child is not null)
            {
                dropChild(_child!);
            }
            _child = __value;
            if (_child is not null)
            {
                adoptChild(_child!);
            }
        }
    }
    public override void redepthChildren()
    {
        if (_child is not null)
        {
            redepthChild(_child!);
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return (child is not null) ? new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { ((Diagnosticable)child!).toDiagnosticsNode(name: "child") } : new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
