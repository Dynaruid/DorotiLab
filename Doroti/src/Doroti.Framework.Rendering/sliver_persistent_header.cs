// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_persistent_header.dart
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

public static partial class Sliver_persistent_headerLibrary
{
    internal static Rect? _trim(Rect? original, double? top = null, double right = double.PositiveInfinity, double bottom = double.PositiveInfinity, double? left = null)
    {
        double __top = top ?? -double.PositiveInfinity;
        double __left = left ?? -double.PositiveInfinity;
        return original?.intersect(global::Doroti.Ui.Rect.fromLTRB(__left, __top, right, bottom));
    }
}

public class OverScrollHeaderStretchConfiguration
{
    public virtual double stretchTriggerOffset { get; private set; } = default!;
    public virtual Func<Future>? onStretchTrigger { get; private set; }

    public OverScrollHeaderStretchConfiguration(double stretchTriggerOffset = 100.0, Func<Future>? onStretchTrigger = null)
    {
        this.stretchTriggerOffset = stretchTriggerOffset;
        this.onStretchTrigger = onStretchTrigger;
    }

}

public class PersistentHeaderShowOnScreenConfiguration
{
    public virtual double minShowOnScreenExtent { get; private set; } = default!;
    public virtual double maxShowOnScreenExtent { get; private set; } = default!;

    public PersistentHeaderShowOnScreenConfiguration(double minShowOnScreenExtent = double.NegativeInfinity, double maxShowOnScreenExtent = double.PositiveInfinity)
    {
        this.minShowOnScreenExtent = minShowOnScreenExtent;
        this.maxShowOnScreenExtent = maxShowOnScreenExtent;
        System.Diagnostics.Debug.Assert((minShowOnScreenExtent <= maxShowOnScreenExtent));
    }

}

public abstract class RenderSliverPersistentHeader : RenderSliver, RenderObjectWithChildMixin<RenderBox>, RenderSliverHelpers
{
    internal virtual double _lastStretchOffset { get; set; } = default!;
    internal virtual bool _needsUpdateChild { get; set; } = true;
    internal virtual double _lastShrinkOffset { get; set; } = 0.0;
    internal virtual bool _lastOverlapsContent { get; set; } = false;
    public virtual OverScrollHeaderStretchConfiguration? stretchConfiguration { get; set; } = default;
    public virtual RenderBox? _child { get; set; } = default;

    protected RenderSliverPersistentHeader(RenderBox? child = null, OverScrollHeaderStretchConfiguration? stretchConfiguration = null)
    {
        this.stretchConfiguration = stretchConfiguration;
    }

    public abstract double maxExtent { get; }
    public abstract double minExtent { get; }
    public virtual double childExtent
    {
        get
        {
            if ((child is null))
            {
                return 0.0;
            }
            DartRuntimePrimitives.Assert(() => child!.hasSize);
            return (((SliverConstraints)constraints).axis switch { global::Doroti.Framework.Painting.Axis.vertical => child!.size.height, global::Doroti.Framework.Painting.Axis.horizontal => child!.size.width, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    public virtual double lastShrinkOffset => this._lastShrinkOffset;
    public virtual bool lastOverlapsContent => this._lastOverlapsContent;
    public virtual void updateChild(double shrinkOffset, bool overlapsContent)
    {
    }

    public override void markNeedsLayout()
    {
        _needsUpdateChild = true;
        base.markNeedsLayout();
    }

    public virtual void layoutChild(double scrollOffset, double maxExtent, bool overlapsContent = false)
    {
        double shrinkOffset = Math.Min(scrollOffset, maxExtent);
        if (((this._needsUpdateChild || (this._lastShrinkOffset != shrinkOffset)) || (this._lastOverlapsContent != overlapsContent)))
        {
            invokeLayoutCallback<SliverConstraints>(((Action<SliverConstraints>)((constraints) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(constraints, this.constraints)));
                updateChild(shrinkOffset, overlapsContent);
            })));
            _lastShrinkOffset = shrinkOffset;
            _lastOverlapsContent = overlapsContent;
            _needsUpdateChild = false;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.minExtent <= maxExtent))
                {
                    return true;
                }
                throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"The maxExtent for this {this.GetType()} is less than its minExtent."), new DoubleProperty("The specified maxExtent was", maxExtent), new DoubleProperty("The specified minExtent was", this.minExtent) });
            });
        var stretchOffset = 0.0;
        if (((this.stretchConfiguration is not null) && (((SliverConstraints)constraints).scrollOffset == 0.0)))
        {
            stretchOffset += ((SliverConstraints)constraints).overlap.abs();
        }
        child?.layout(constraints.asBoxConstraints(maxExtent: (Math.Max(this.minExtent, (maxExtent - shrinkOffset)) + stretchOffset)), parentUsesSize: true);
        if (((((this.stretchConfiguration is not null) && (this.stretchConfiguration!.onStretchTrigger is not null)) && (stretchOffset >= this.stretchConfiguration!.stretchTriggerOffset)) && (this._lastStretchOffset <= this.stretchConfiguration!.stretchTriggerOffset)))
        {
            _ = this.stretchConfiguration!.onStretchTrigger!();
        }
        _lastStretchOffset = stretchOffset;
    }

    public override double childMainAxisPosition(RenderObject child) => base.childMainAxisPosition(child);
    public override bool hitTestChildren(SliverHitTestResult result, double mainAxisPosition, double crossAxisPosition)
    {
        DartRuntimePrimitives.Assert(() => (geometry!.hitTestExtent > 0.0));
        if ((child is not null))
        {
            return hitTestBoxChild(BoxHitTestResult.CreateWrap(result), child!, mainAxisPosition: mainAxisPosition, crossAxisPosition: crossAxisPosition);
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(child, this.child)));
        applyPaintTransformForBoxChild(((RenderBox?)(object?)child)!, transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if (((child is not null) && geometry!.visible))
        {
            offset += (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => new global::Doroti.Ui.Offset(0.0, ((geometry!.paintExtent - childMainAxisPosition(child!)) - this.childExtent)), global::Doroti.Framework.Painting.AxisDirection.left => new global::Doroti.Ui.Offset(((geometry!.paintExtent - childMainAxisPosition(child!)) - this.childExtent), 0.0), global::Doroti.Framework.Painting.AxisDirection.right => new global::Doroti.Ui.Offset(childMainAxisPosition(child!), 0.0), global::Doroti.Framework.Painting.AxisDirection.down => new global::Doroti.Ui.Offset(0.0, childMainAxisPosition(child!)), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            context.paintChild(child!, offset);
        }
    }

    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.addTagForChildren(RenderViewport.excludeFromScrolling);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("maxExtent", (() => this.maxExtent)));
        properties.add(new DoubleProperty("child position", (() => childMainAxisPosition(child!))));
    }

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
    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        this._child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        this._child?.detach();
    }

    public override void redepthChildren()
    {
        if ((this._child is not null))
        {
            redepthChild(this._child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if ((this._child is not null))
        {
            visitor(this._child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<DiagnosticsNode>());
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

public abstract class RenderSliverScrollingPersistentHeader : RenderSliverPersistentHeader
{
    internal virtual double? _childPosition { get; set; } = default;

    protected RenderSliverScrollingPersistentHeader(RenderBox? child = null, OverScrollHeaderStretchConfiguration? stretchConfiguration = null) : base(child: child, stretchConfiguration: stretchConfiguration)
    {
    }

    public virtual double updateGeometry()
    {
        var stretchOffset = 0.0;
        if ((stretchConfiguration is not null))
        {
            stretchOffset += ((SliverConstraints)constraints).overlap.abs();
        }
        double maxExtentLocal = this.maxExtent;
        double paintExtentLocal = (maxExtentLocal - ((SliverConstraints)constraints).scrollOffset);
        double cacheExtentLocal = calculateCacheOffset(constraints, from: 0.0, to: maxExtentLocal);
        geometry = new SliverGeometry(cacheExtent: cacheExtentLocal, scrollExtent: maxExtentLocal, paintOrigin: Math.Min(((SliverConstraints)constraints).overlap, 0.0), paintExtent: Dart_uiLibrary.clampDouble(paintExtentLocal, 0.0, ((SliverConstraints)constraints).remainingPaintExtent), maxPaintExtent: (maxExtentLocal + stretchOffset), hasVisualOverflow: true);
        return ((stretchOffset > 0L) ? 0.0 : Math.Min(0.0, (paintExtentLocal - childExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        layoutChild(((SliverConstraints)constraints).scrollOffset, maxExtent);
        _childPosition = updateGeometry();
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child, this.child)));
        DartRuntimePrimitives.Assert(() => (this._childPosition is not null));
        return DartRuntimePrimitives.RequireValue(this._childPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public abstract class RenderSliverPinnedPersistentHeader : RenderSliverPersistentHeader
{
    public virtual PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration { get; set; } = default;

    protected RenderSliverPinnedPersistentHeader(RenderBox? child = null, OverScrollHeaderStretchConfiguration? stretchConfiguration = null, PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = default!) : base(child: child, stretchConfiguration: stretchConfiguration)
    {
        PersistentHeaderShowOnScreenConfiguration? __showOnScreenConfiguration = showOnScreenConfiguration ?? new PersistentHeaderShowOnScreenConfiguration();
        this.showOnScreenConfiguration = __showOnScreenConfiguration;
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = this.constraints;
        double maxExtentLocal = this.maxExtent;
        bool overlapsContentLocal = (((SliverConstraints)constraintsLocal).overlap > 0.0);
        layoutChild(((SliverConstraints)constraintsLocal).scrollOffset, maxExtentLocal, overlapsContent: overlapsContentLocal);
        double effectiveRemainingPaintExtent = Math.Max(0, (((SliverConstraints)constraintsLocal).remainingPaintExtent - ((SliverConstraints)constraintsLocal).overlap));
        double layoutExtentLocal = Dart_uiLibrary.clampDouble((maxExtentLocal - ((SliverConstraints)constraintsLocal).scrollOffset), 0.0, effectiveRemainingPaintExtent);
        double stretchOffset = ((stretchConfiguration is not null) ? ((SliverConstraints)constraintsLocal).overlap.abs() : 0.0);
        geometry = new SliverGeometry(scrollExtent: maxExtentLocal, paintOrigin: ((SliverConstraints)constraintsLocal).overlap, paintExtent: Math.Min(childExtent, effectiveRemainingPaintExtent), layoutExtent: layoutExtentLocal, maxPaintExtent: (maxExtentLocal + stretchOffset), maxScrollObstructionExtent: minExtent, cacheExtent: ((layoutExtentLocal > 0.0) ? (-((SliverConstraints)constraintsLocal).cacheOrigin + layoutExtentLocal) : layoutExtentLocal), hasVisualOverflow: true);
    }

    public override double childMainAxisPosition(RenderObject child) => 0.0;
    public override void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        global::Doroti.Ui.Rect? localBounds = ((descendant is not null) ? MatrixUtils.transformRect(descendant.getTransformTo(this), (rect ?? ((RenderObject)descendant).paintBounds)) : rect);
        global::Doroti.Ui.Rect? newRect = (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => Sliver_persistent_headerLibrary._trim(localBounds, bottom: childExtent), global::Doroti.Framework.Painting.AxisDirection.left => Sliver_persistent_headerLibrary._trim(localBounds, right: childExtent), global::Doroti.Framework.Painting.AxisDirection.right => Sliver_persistent_headerLibrary._trim(localBounds, left: 0), global::Doroti.Framework.Painting.AxisDirection.down => Sliver_persistent_headerLibrary._trim(localBounds, top: 0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        base.showOnScreen(descendant: this, rect: newRect, duration: duration, curve: curve);
    }

}

public class FloatingHeaderSnapConfiguration
{
    public virtual Curve curve { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;

    public FloatingHeaderSnapConfiguration(Curve curve = default!, Duration? duration = null)
    {
        Curve __curve = curve ?? Curves.ease;
        Duration __duration = duration ?? Duration.Create(milliseconds: 300);
        this.curve = __curve;
        this.duration = __duration;
    }

}

public abstract class RenderSliverFloatingPersistentHeader : RenderSliverPersistentHeader
{
    internal virtual AnimationController? _controller { get; set; } = default;
    internal virtual Animation<double> _animation { get; set; } = default!;
    internal virtual double? _lastActualScrollOffset { get; set; } = default;
    internal virtual double? _effectiveScrollOffset { get; set; } = default;
    internal virtual ScrollDirection? _lastStartedScrollDirection { get; set; } = default;
    internal virtual double? _childPosition { get; set; } = default;
    internal virtual TickerProvider? _vsync { get; set; } = default;
    public virtual FloatingHeaderSnapConfiguration? snapConfiguration { get; set; } = default;
    public virtual PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration { get; set; } = default;

    protected RenderSliverFloatingPersistentHeader(RenderBox? child = null, TickerProvider? vsync = null, FloatingHeaderSnapConfiguration? snapConfiguration = null, OverScrollHeaderStretchConfiguration? stretchConfiguration = null, PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = default!) : base(child: child, stretchConfiguration: stretchConfiguration)
    {
        this.snapConfiguration = snapConfiguration;
        this.showOnScreenConfiguration = showOnScreenConfiguration;
        this._vsync = vsync;
    }

    public override void detach()
    {
        this._controller?.dispose();
        _controller = null;
        base.detach();
    }

    public virtual TickerProvider? vsync
    {
        get => this._vsync;
        set
        {
            var __value = value;
            if ((object.Equals(__value, this._vsync)))
            {
                return;
            }
            _vsync = __value;
            if ((__value is null))
            {
                this._controller?.dispose();
                _controller = null;
            }
            else
            {
                this._controller?.resync(__value);
            }
        }
    }
    public virtual double updateGeometry()
    {
        var stretchOffset = 0.0;
        if ((stretchConfiguration is not null))
        {
            stretchOffset += ((SliverConstraints)constraints).overlap.abs();
        }
        double maxExtentLocal = this.maxExtent;
        double paintExtentLocal = (maxExtentLocal - DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset));
        double layoutExtentLocal = (maxExtentLocal - ((SliverConstraints)constraints).scrollOffset);
        geometry = new SliverGeometry(scrollExtent: maxExtentLocal, paintOrigin: Math.Min(((SliverConstraints)constraints).overlap, 0.0), paintExtent: Dart_uiLibrary.clampDouble(paintExtentLocal, 0.0, ((SliverConstraints)constraints).remainingPaintExtent), layoutExtent: Dart_uiLibrary.clampDouble(layoutExtentLocal, 0.0, ((SliverConstraints)constraints).remainingPaintExtent), maxPaintExtent: (maxExtentLocal + stretchOffset), hasVisualOverflow: true);
        return ((stretchOffset > 0L) ? 0.0 : Math.Min(0.0, (paintExtentLocal - childExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimation(Duration duration, double endValue, Curve curve)
    {
        DartRuntimePrimitives.Assert(() => (this.vsync is not null));
        AnimationController effectiveController = _controller ??= ((Func<AnimationController>)(() =>
{
    var __cascade = new AnimationController(vsync: this.vsync!, duration: duration);
    __cascade.addListener((() =>
    {
        if ((this._effectiveScrollOffset == this._animation.value))
        {
            return;
        }
        _effectiveScrollOffset = this._animation.value;
        markNeedsLayout();
    }));
    return __cascade;
}))();
        _animation = effectiveController.drive(new Tween<double>(begin: DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset), end: endValue).chain(new CurveTween(curve: curve)));
    }

    public virtual void updateScrollStartDirection(ScrollDirection direction)
    {
        _lastStartedScrollDirection = direction;
    }

    public virtual void maybeStartSnapAnimation(ScrollDirection direction)
    {
        FloatingHeaderSnapConfiguration? snap = this.snapConfiguration;
        if ((snap is null))
        {
            return;
        }
        if (((object.Equals(direction, ScrollDirection.forward)) && (DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) <= 0.0)))
        {
            return;
        }
        if (((object.Equals(direction, ScrollDirection.reverse)) && (DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) >= maxExtent)))
        {
            return;
        }
        _updateAnimation(((FloatingHeaderSnapConfiguration)snap).duration, ((object.Equals(direction, ScrollDirection.forward)) ? 0.0 : maxExtent), ((FloatingHeaderSnapConfiguration)snap).curve);
        this._controller?.forward(from: 0.0);
    }

    public virtual void maybeStopSnapAnimation(ScrollDirection direction)
    {
        this._controller?.stop();
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = this.constraints;
        double maxExtentLocal = this.maxExtent;
        if (((this._lastActualScrollOffset is not null) && ((((((SliverConstraints)constraintsLocal).scrollOffset < DartRuntimePrimitives.RequireValue(this._lastActualScrollOffset))) || ((DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) < maxExtentLocal))))))
        {
            double delta = (DartRuntimePrimitives.RequireValue(this._lastActualScrollOffset) - ((SliverConstraints)constraintsLocal).scrollOffset);
            bool allowFloatingExpansion = ((object.Equals(((SliverConstraints)constraintsLocal).userScrollDirection, ScrollDirection.forward)) || (((this._lastStartedScrollDirection is not null) && (object.Equals(this._lastStartedScrollDirection, ScrollDirection.forward)))));
            if (allowFloatingExpansion)
            {
                if ((DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) > maxExtentLocal))
                {
                    _effectiveScrollOffset = maxExtentLocal;
                }
            }
            else
            {
                if ((delta > 0.0))
                {
                    delta = 0.0;
                }
            }
            _effectiveScrollOffset = Dart_uiLibrary.clampDouble((DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) - delta), 0.0, ((SliverConstraints)constraintsLocal).scrollOffset);
        }
        else
        {
            _effectiveScrollOffset = ((SliverConstraints)constraintsLocal).scrollOffset;
        }
        bool overlapsContentLocal = (DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) < ((SliverConstraints)constraintsLocal).scrollOffset);
        layoutChild(DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset), maxExtentLocal, overlapsContent: overlapsContentLocal);
        _childPosition = updateGeometry();
        _lastActualScrollOffset = ((SliverConstraints)constraintsLocal).scrollOffset;
    }

    public override void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        PersistentHeaderShowOnScreenConfiguration? showOnScreenLocal = this.showOnScreenConfiguration;
        if ((showOnScreenLocal is null))
        {
            base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
            return;
        }
        DartRuntimePrimitives.Assert(() => ((child is not null) || (descendant is null)));
        global::Doroti.Ui.Rect? childBounds = ((descendant is not null) ? MatrixUtils.transformRect(descendant.getTransformTo(child), (rect ?? ((RenderObject)descendant).paintBounds)) : rect);
        double targetExtent = default!;
        global::Doroti.Ui.Rect? targetRect = default!;
        switch (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection))
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    targetExtent = (childExtent - ((childBounds?.top ?? 0L)));
                    targetRect = Sliver_persistent_headerLibrary._trim(childBounds, bottom: childExtent);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    targetExtent = (childBounds?.right ?? childExtent);
                    targetRect = Sliver_persistent_headerLibrary._trim(childBounds, left: 0);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    targetExtent = (childBounds?.bottom ?? childExtent);
                    targetRect = Sliver_persistent_headerLibrary._trim(childBounds, top: 0);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    targetExtent = (childExtent - ((childBounds?.left ?? 0L)));
                    targetRect = Sliver_persistent_headerLibrary._trim(childBounds, right: childExtent);
                    break;
                }
        }
        double effectiveMaxExtent = Math.Max(childExtent, maxExtent);
        targetExtent = Dart_uiLibrary.clampDouble(Dart_uiLibrary.clampDouble(targetExtent, ((PersistentHeaderShowOnScreenConfiguration)showOnScreenLocal).minShowOnScreenExtent, ((PersistentHeaderShowOnScreenConfiguration)showOnScreenLocal).maxShowOnScreenExtent), childExtent, effectiveMaxExtent);
        if (((targetExtent > childExtent) && (!object.Equals(this._controller?.status, AnimationStatus.forward))))
        {
            double targetScrollOffset = (maxExtent - targetExtent);
            DartRuntimePrimitives.Assert(() => (this.vsync is not null));
            _updateAnimation(duration, targetScrollOffset, curve);
            this._controller?.forward(from: 0.0);
        }
        base.showOnScreen(descendant: ((descendant is null) ? this : child), rect: targetRect, duration: duration, curve: curve);
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child, this.child)));
        return (this._childPosition ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("effective scroll offset", this._effectiveScrollOffset));
    }

}

public abstract class RenderSliverFloatingPinnedPersistentHeader : RenderSliverFloatingPersistentHeader
{
    protected RenderSliverFloatingPinnedPersistentHeader(RenderBox? child = null, TickerProvider? vsync = null, FloatingHeaderSnapConfiguration? snapConfiguration = null, OverScrollHeaderStretchConfiguration? stretchConfiguration = null, PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = null) : base(child: child, vsync: vsync, snapConfiguration: snapConfiguration, stretchConfiguration: stretchConfiguration, showOnScreenConfiguration: showOnScreenConfiguration)
    {
    }

    public override double updateGeometry()
    {
        double minExtentLocal = this.minExtent;
        double minAllowedExtent = ((((SliverConstraints)constraints).remainingPaintExtent > minExtentLocal) ? minExtentLocal : ((SliverConstraints)constraints).remainingPaintExtent);
        double maxExtentLocal = this.maxExtent;
        double paintExtentLocal = (maxExtentLocal - DartRuntimePrimitives.RequireValue(_effectiveScrollOffset));
        double clampedPaintExtent = Dart_uiLibrary.clampDouble(paintExtentLocal, minAllowedExtent, ((SliverConstraints)constraints).remainingPaintExtent);
        double layoutExtentLocal = (maxExtentLocal - ((SliverConstraints)constraints).scrollOffset);
        double stretchOffset = ((stretchConfiguration is not null) ? ((SliverConstraints)constraints).overlap.abs() : 0.0);
        geometry = new SliverGeometry(scrollExtent: maxExtentLocal, paintOrigin: Math.Min(((SliverConstraints)constraints).overlap, 0.0), paintExtent: clampedPaintExtent, layoutExtent: Dart_uiLibrary.clampDouble(layoutExtentLocal, 0.0, clampedPaintExtent), maxPaintExtent: (maxExtentLocal + stretchOffset), maxScrollObstructionExtent: minExtentLocal, hasVisualOverflow: true);
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

