// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/single_child_scroll_view.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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

    public SingleChildScrollView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.vertical, bool reverse = false, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, bool? primary = null, ScrollPhysics? physics = null, ScrollController? controller = null, Widget? child = null, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, Clip clipBehavior = Clip.hardEdge, global::Doroti.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque, string? restorationId = null, ScrollViewKeyboardDismissBehavior? keyboardDismissBehavior = null) : base(key: key)
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
        System.Diagnostics.Debug.Assert(!(((controller is not null) && ((primary ?? false)))));
    }

    internal virtual global::Doroti.Framework.Painting.AxisDirection _getDirection(BuildContext context)
    {
        return global::Doroti.Framework.Widgets.BasicLibrary.getAxisDirectionFromAxisReverseAndDirectionality(context, this.scrollDirection, this.reverse);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        global::Doroti.Framework.Painting.AxisDirection axisDirection__10892 = _getDirection(context);
        Widget? contents__10944 = this.child;
        if ((this.padding is not null))
        {
            contents__10944 = DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: this.padding!, child: contents__10944));
        }
        bool effectivePrimary__11072 = (this.primary ?? ((this.controller is null) && PrimaryScrollController.shouldInherit(context, this.scrollDirection)));
        ScrollController? scrollController__11234 = (effectivePrimary__11072 ? PrimaryScrollController.maybeOf(context) : this.controller);
        Widget scrollable__11355 = ((Widget)(object?)new Scrollable(dragStartBehavior: this.dragStartBehavior, axisDirection: axisDirection__10892, controller: scrollController__11234, physics: this.physics, restorationId: this.restorationId, clipBehavior: this.clipBehavior, hitTestBehavior: this.hitTestBehavior, viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Framework.Rendering.ViewportOffset, Widget>)((context, offset) => {
return ((Widget)(object?)new _SingleChildViewport__single_child_scroll_view(axisDirection: axisDirection__10892, offset: offset, clipBehavior: this.clipBehavior, child: contents__10944));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        ScrollViewKeyboardDismissBehavior effectiveKeyboardDismissBehavior__11941 = ((this.keyboardDismissBehavior ?? (ScrollViewKeyboardDismissBehavior)ScrollConfiguration.of(context).getKeyboardDismissBehavior(context)));
        if ((object.Equals(effectiveKeyboardDismissBehavior__11941, ScrollViewKeyboardDismissBehavior.onDrag)))
        {
            scrollable__11355 = DartRuntimePrimitives.ConvertValue<Widget>(new NotificationListener<ScrollUpdateNotification>(child: scrollable__11355, onNotification: ((global::System.Func<ScrollUpdateNotification, bool>?)((notification) => {
FocusScopeNode currentScope__12368 = ((FocusScopeNode)(object?)FocusScope.of(context));
if ((((((ScrollUpdateNotification)notification).dragDetails is not null) && !currentScope__12368.hasPrimaryFocus) && currentScope__12368.hasFocus))
{
    FocusManager.instance.primaryFocus?.unfocus();
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        }
        return ((effectivePrimary__11072 && (scrollController__11234 is not null)) ? PrimaryScrollController.CreateNone(child: scrollable__11355) : scrollable__11355);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _SingleChildViewport__single_child_scroll_view : SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.ViewportOffset offset { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;

    internal _SingleChildViewport__single_child_scroll_view(global::Doroti.Framework.Painting.AxisDirection axisDirection = global::Doroti.Framework.Painting.AxisDirection.down, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, Widget? child = null, Clip clipBehavior = default!) : base(child: child)
    {
        this.axisDirection = axisDirection;
        this.offset = offset;
        this.clipBehavior = clipBehavior;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSingleChildViewport__single_child_scroll_view(axisDirection: this.axisDirection, offset: this.offset, clipBehavior: this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSingleChildViewport__single_child_scroll_view)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSingleChildViewport__single_child_scroll_view>)(() =>
{            var __cascade = __renderObject;
            __cascade.axisDirection = this.axisDirection;
            __cascade.offset = this.offset;
            __cascade.clipBehavior = this.clipBehavior;
            return __cascade;        }))());
    }

    public override SingleChildRenderObjectElement createElement()
    {
        return ((SingleChildRenderObjectElement)(object?)new _SingleChildViewportElement__single_child_scroll_view(this));
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
        _notificationTree = new _NotificationNode__framework(this._parent?._notificationTree, this);
    }

    public virtual bool onNotification(Notification notification)
    {
        if ((notification is ViewportNotificationMixin))
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

    internal _RenderSingleChildViewport__single_child_scroll_view(global::Doroti.Framework.Painting.AxisDirection axisDirection = global::Doroti.Framework.Painting.AxisDirection.down, global::Doroti.Framework.Rendering.ViewportOffset offset = default!, global::Doroti.Framework.Rendering.RenderBox? child = null, Clip clipBehavior = default!)
    {
        this._axisDirection = axisDirection;
        this._offset = offset;
        this._clipBehavior = clipBehavior;
    }

    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection
    {
        get => this._axisDirection;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._axisDirection)))
            {
                return;
            }
            _axisDirection = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Painting.Axis axis => global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this.axisDirection);
    public virtual global::Doroti.Framework.Rendering.ViewportOffset offset
    {
        get => this._offset;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._offset)))
            {
                return;
            }
            if (this.attached)
            {
                this._offset.removeListener(() => this._hasScrolled());
            }
            _offset = __value;
            if (this.attached)
            {
                this._offset.addListener(() => this._hasScrolled());
            }
            markNeedsLayout();
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
    internal virtual void _hasScrolled()
    {
        markNeedsPaint();
        markNeedsSemanticsUpdate();
    }

    public override void setupParentData(global::Doroti.Framework.Rendering.RenderObject child)
    {
        if (false)
        {
            ((dynamic)child).parentData = new global::Doroti.Framework.Rendering.ParentData();
        }
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        this._child?.attach(owner);
        this._offset.addListener(() => this._hasScrolled());
    }

    public override void detach()
    {
        this._offset.removeListener(() => this._hasScrolled());
        base.detach();
        this._child?.detach();
    }

    public override bool isRepaintBoundary => true;
    internal virtual double _viewportExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.hasSize);
            return (this.axis switch { global::Doroti.Framework.Painting.Axis.horizontal => this.size.width, global::Doroti.Framework.Painting.Axis.vertical => this.size.height, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    internal virtual double _minScrollExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.hasSize);
            return 0.0;
            return default!;
        }
    }
    internal virtual double _maxScrollExtent
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.hasSize);
            if ((this.child is null))
            {
                return 0.0;
            }
            return Math.Max(0.0, (this.axis switch { global::Doroti.Framework.Painting.Axis.horizontal => (this.child!.size.width - this.size.width), global::Doroti.Framework.Painting.Axis.vertical => (this.child!.size.height - this.size.height), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            return default!;
        }
    }
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _getInnerConstraints(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return (this.axis switch { global::Doroti.Framework.Painting.Axis.horizontal => constraints.heightConstraints(), global::Doroti.Framework.Painting.Axis.vertical => constraints.widthConstraints(), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        return (this.child?.getMinIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        return (this.child?.getMaxIntrinsicWidth(height) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return (this.child?.getMinIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        return (this.child?.getMaxIntrinsicHeight(width) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        if ((this.child is null))
        {
            return ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).smallest;
        }
        global::Doroti.Ui.Size childSize__17693 = ((global::Doroti.Ui.Size)(object?)this.child!.getDryLayout(_getInnerConstraints(constraints)));
        return constraints.constrain(childSize__17693);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.BoxConstraints constraints__17873 = this.constraints;
        if ((this.child is null))
        {
            size = ((global::Doroti.Framework.Rendering.BoxConstraints)constraints__17873).smallest;
        }
        else
        {
            this.child!.layout(_getInnerConstraints(constraints__17873), parentUsesSize: true);
            size = constraints__17873.constrain(this.child!.size);
        }
        if (((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).hasPixels)
        {
            if ((((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels > this._maxScrollExtent))
            {
                this.offset.correctBy((this._maxScrollExtent - ((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels));
            }
            else
            {
                if ((((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels < this._minScrollExtent))
                {
                    this.offset.correctBy((this._minScrollExtent - ((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels));
                }
            }
        }
        this.offset.applyViewportDimension(this._viewportExtent);
        this.offset.applyContentDimensions(this._minScrollExtent, this._maxScrollExtent);
    }

    internal virtual global::Doroti.Ui.Offset _paintOffset => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Offset>(_paintOffsetForPosition(((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels));
    internal virtual global::Doroti.Ui.Offset _paintOffsetForPosition(double position)
    {
        return (this.axisDirection switch { global::Doroti.Framework.Painting.AxisDirection.up => new global::Doroti.Ui.Offset(0.0, ((position - this.child!.size.height) + this.size.height)), global::Doroti.Framework.Painting.AxisDirection.left => new global::Doroti.Ui.Offset(((position - this.child!.size.width) + this.size.width), 0.0), global::Doroti.Framework.Painting.AxisDirection.right => new global::Doroti.Ui.Offset(-position, 0.0), global::Doroti.Framework.Painting.AxisDirection.down => new global::Doroti.Ui.Offset(0.0, -position), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldClipAtPaintOffset(Offset paintOffset)
    {
        DartRuntimePrimitives.Assert(() => (this.child is not null));
        switch (this.clipBehavior)
        {
            case Clip.none:
                {
                    return false;
                }
            case Clip.hardEdge:
            case Clip.antiAlias:
            case Clip.antiAliasWithSaveLayer:
                {
                    return ((((paintOffset.dx < 0L) || (paintOffset.dy < 0L)) || ((paintOffset.dx + this.child!.size.width) > this.size.width)) || ((paintOffset.dy + this.child!.size.height) > this.size.height));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if ((this.child is not null))
        {
            global::Doroti.Ui.Offset paintOffset__19510 = ((global::Doroti.Ui.Offset)(object?)this._paintOffset);
            void paintContents(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
            {
                context.paintChild(this.child!, (offset + paintOffset__19510));
            }
            if (_shouldClipAtPaintOffset(paintOffset__19510))
            {
                this._clipRectLayer.layer = context.pushClipRect(this.needsCompositing, offset, (Offset.zero & this.size), (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)paintContents, clipBehavior: this.clipBehavior, oldLayer: ((global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.ClipRectLayer>)this._clipRectLayer).layer);
            }
            else
            {
                this._clipRectLayer.layer = null;
                paintContents(context, offset);
            }
        }
    }

    public override void dispose()
    {
        this._clipRectLayer.layer = null;
        base.dispose();
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        global::Doroti.Ui.Offset paintOffset__20347 = ((global::Doroti.Ui.Offset)(object?)this._paintOffset);
        transform.translateByDouble(paintOffset__20347.dx, paintOffset__20347.dy, 0, 1);
    }

    public override Rect? describeApproximatePaintClip(global::Doroti.Framework.Rendering.RenderObject child)
    {
        if (((child is not null) && _shouldClipAtPaintOffset(this._paintOffset)))
        {
            return (Offset.zero & this.size);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if ((this.child is not null))
        {
            return result.addWithPaintOffset(offset: this._paintOffset, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) => {
DartRuntimePrimitives.Assert(() => (object.Equals(transformed, (position + -this._paintOffset))));
return this.child!.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Rendering.RevealedOffset getOffsetToReveal(global::Doroti.Framework.Rendering.RenderObject target, double alignment, Rect? rect = null, global::Doroti.Framework.Painting.Axis? axis = null)
    {
        axis = this.axis;
        rect ??= ((global::Doroti.Framework.Rendering.RenderObject)target).paintBounds;
        if ((target is not global::Doroti.Framework.Rendering.RenderBox))
        {
            return new global::Doroti.Framework.Rendering.RevealedOffset(offset: ((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels, rect: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect)));
        }
        global::Doroti.Framework.Rendering.RenderBox targetBox__21521 = ((global::Doroti.Framework.Rendering.RenderBox)target);
        Matrix4 transform__21559 = ((Matrix4)(object?)targetBox__21521.getTransformTo(this.child));
        global::Doroti.Ui.Rect bounds__21619 = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform__21559, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rect))));
        global::Doroti.Ui.Size contentSize__21687 = ((global::Doroti.Ui.Size)(object?)this.child!.size);
        var (mainAxisExtent__21740, leadingScrollOffset__21769, targetMainAxisExtent__21803) = (this.axisDirection switch { global::Doroti.Framework.Painting.AxisDirection.up => (((double, double, double))((this.size.height, (contentSize__21687.height - bounds__21619.bottom), bounds__21619.height))), global::Doroti.Framework.Painting.AxisDirection.left => (((double, double, double))((this.size.width, (contentSize__21687.width - bounds__21619.right), bounds__21619.width))), global::Doroti.Framework.Painting.AxisDirection.right => (((double, double, double))((this.size.width, bounds__21619.left, bounds__21619.width))), global::Doroti.Framework.Painting.AxisDirection.down => (((double, double, double))((this.size.height, bounds__21619.top, bounds__21619.height))), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        double targetOffset__22205 = (leadingScrollOffset__21769 - (((mainAxisExtent__21740 - targetMainAxisExtent__21803)) * alignment));
        global::Doroti.Ui.Rect targetRect__22318 = ((global::Doroti.Ui.Rect)(object?)bounds__21619.shift(_paintOffsetForPosition(targetOffset__22205)));
        return new global::Doroti.Framework.Rendering.RevealedOffset(offset: targetOffset__22205, rect: targetRect__22318);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void showOnScreen(global::Doroti.Framework.Rendering.RenderObject? descendant = null, Rect? rect = null, Duration duration = default, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        if (!((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).allowImplicitScrolling)
        {
            base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
            return;
        }
        global::Doroti.Ui.Rect? newRect__22822 = ((global::Doroti.Ui.Rect?)(object?)_showInViewport(descendant, rect, this, this.offset, duration, curve));
        base.showOnScreen(rect: newRect__22822, duration: duration, curve: curve);
    }

    private static Rect? _showInViewport(global::Doroti.Framework.Rendering.RenderObject? descendant, Rect? rect, _RenderSingleChildViewport__single_child_scroll_view viewport, global::Doroti.Framework.Rendering.ViewportOffset offset, Duration duration, global::Doroti.Framework.Animation.Curve curve) { if (descendant is null) return rect; var leading = viewport.getOffsetToReveal(descendant, 0.0, rect: rect); var trailing = viewport.getOffsetToReveal(descendant, 1.0, rect: rect); var target = global::Doroti.Framework.Rendering.RevealedOffset.clampOffset(leading, trailing, offset.pixels); if (target is null) return rect ?? descendant.paintBounds; _ = offset.moveTo(target.offset, duration: duration, curve: curve); return target.rect; }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Offset>("offset", this._paintOffset));
    }

    public override Rect? describeSemanticsClip(global::Doroti.Framework.Rendering.RenderObject? child)
    {
        double remainingOffset__23372 = (this._maxScrollExtent - ((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels);
        switch (this.axisDirection)
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    return global::Doroti.Ui.Rect.fromLTRB(this.semanticBounds.left, (this.semanticBounds.top - remainingOffset__23372), this.semanticBounds.right, (this.semanticBounds.bottom + ((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels));
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    return global::Doroti.Ui.Rect.fromLTRB((this.semanticBounds.left - ((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels), this.semanticBounds.top, (this.semanticBounds.right + remainingOffset__23372), this.semanticBounds.bottom);
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    return global::Doroti.Ui.Rect.fromLTRB(this.semanticBounds.left, (this.semanticBounds.top - ((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels), this.semanticBounds.right, (this.semanticBounds.bottom + remainingOffset__23372));
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    return global::Doroti.Ui.Rect.fromLTRB((this.semanticBounds.left - remainingOffset__23372), this.semanticBounds.top, (this.semanticBounds.right + ((global::Doroti.Framework.Rendering.ViewportOffset)this.offset).pixels), this.semanticBounds.bottom);
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
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.ErrorSpacer(), new global::Doroti.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((dynamic)child).debugCreator, style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? child
    {
        get => this._child;
        set
        {
            var __value = value;
            if ((this._child is not null))
            {
                dropChild(this._child!);
            }
            this._child = __value;
            if ((this._child is not null))
            {
                adoptChild(this._child!);
            }
        }
    }
    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(global::System.Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<global::Doroti.Framework.Foundation.DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
