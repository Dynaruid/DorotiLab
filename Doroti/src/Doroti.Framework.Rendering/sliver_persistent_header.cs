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
        double shrinkOffset__9262 = Math.Min(scrollOffset, maxExtent);
        if (((this._needsUpdateChild || (this._lastShrinkOffset != shrinkOffset__9262)) || (this._lastOverlapsContent != overlapsContent)))
        {
            invokeLayoutCallback<SliverConstraints>(((Action<SliverConstraints>)((constraints) =>
            {
                DartRuntimePrimitives.Assert(() => (object.Equals(constraints, this.constraints)));
                updateChild(shrinkOffset__9262, overlapsContent);
            })));
            _lastShrinkOffset = shrinkOffset__9262;
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
        var stretchOffset__10137 = 0.0;
        if (((this.stretchConfiguration is not null) && (((SliverConstraints)constraints).scrollOffset == 0.0)))
        {
            stretchOffset__10137 += ((SliverConstraints)constraints).overlap.abs();
        }
        child?.layout(constraints.asBoxConstraints(maxExtent: (Math.Max(this.minExtent, (maxExtent - shrinkOffset__9262)) + stretchOffset__10137)), parentUsesSize: true);
        if (((((this.stretchConfiguration is not null) && (this.stretchConfiguration!.onStretchTrigger is not null)) && (stretchOffset__10137 >= this.stretchConfiguration!.stretchTriggerOffset)) && (this._lastStretchOffset <= this.stretchConfiguration!.stretchTriggerOffset)))
        {
            _ = this.stretchConfiguration!.onStretchTrigger!();
        }
        _lastStretchOffset = stretchOffset__10137;
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
        bool reversed__78998 = global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((SliverConstraints)constraints).axisDirection);
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
        global::Doroti.Ui.Offset paintOffset__80149 = default!;
        global::Doroti.Ui.Offset transformedPosition__80162 = default!;
        switch (((SliverConstraints)constraints).axis)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp__79845)
                    {
                        absolutePosition__80012 = (((RenderBox)child).size.width - absolutePosition__80012);
                        delta__79898 = ((geometry!.paintExtent - ((RenderBox)child).size.width) - delta__79898);
                    }
                    paintOffset__80149 = new global::Doroti.Ui.Offset(delta__79898, crossAxisDelta__79953);
                    transformedPosition__80162 = new global::Doroti.Ui.Offset(absolutePosition__80012, absoluteCrossAxisPosition__80074);
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    if (!rightWayUp__79845)
                    {
                        absolutePosition__80012 = (((RenderBox)child).size.height - absolutePosition__80012);
                        delta__79898 = ((geometry!.paintExtent - ((RenderBox)child).size.height) - delta__79898);
                    }
                    paintOffset__80149 = new global::Doroti.Ui.Offset(crossAxisDelta__79953, delta__79898);
                    transformedPosition__80162 = new global::Doroti.Ui.Offset(absoluteCrossAxisPosition__80074, absolutePosition__80012);
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
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    if (!rightWayUp__81586)
                    {
                        delta__81639 = ((geometry!.paintExtent - ((RenderBox)child).size.width) - delta__81639);
                    }
                    transform.translateByDouble(delta__81639, crossAxisDelta__81694, 0, 1);
                    break;
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
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

public abstract class RenderSliverScrollingPersistentHeader : RenderSliverPersistentHeader
{
    internal virtual double? _childPosition { get; set; } = default;

    protected RenderSliverScrollingPersistentHeader(RenderBox? child = null, OverScrollHeaderStretchConfiguration? stretchConfiguration = null) : base(child: child, stretchConfiguration: stretchConfiguration)
    {
    }

    public virtual double updateGeometry()
    {
        var stretchOffset__14795 = 0.0;
        if ((stretchConfiguration is not null))
        {
            stretchOffset__14795 += ((SliverConstraints)constraints).overlap.abs();
        }
        double maxExtent__14929 = this.maxExtent;
        double paintExtent__14974 = (maxExtent__14929 - ((SliverConstraints)constraints).scrollOffset);
        double cacheExtent__15043 = calculateCacheOffset(constraints, from: 0.0, to: maxExtent__14929);
        geometry = new SliverGeometry(cacheExtent: cacheExtent__15043, scrollExtent: maxExtent__14929, paintOrigin: Math.Min(((SliverConstraints)constraints).overlap, 0.0), paintExtent: Dart_uiLibrary.clampDouble(paintExtent__14974, 0.0, ((SliverConstraints)constraints).remainingPaintExtent), maxPaintExtent: (maxExtent__14929 + stretchOffset__14795), hasVisualOverflow: true);
        return ((stretchOffset__14795 > 0L) ? 0.0 : Math.Min(0.0, (paintExtent__14974 - childExtent)));
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
        SliverConstraints constraints__16899 = this.constraints;
        double maxExtent__16948 = this.maxExtent;
        bool overlapsContent__16991 = (((SliverConstraints)constraints__16899).overlap > 0.0);
        layoutChild(((SliverConstraints)constraints__16899).scrollOffset, maxExtent__16948, overlapsContent: overlapsContent__16991);
        double effectiveRemainingPaintExtent__17141 = Math.Max(0, (((SliverConstraints)constraints__16899).remainingPaintExtent - ((SliverConstraints)constraints__16899).overlap));
        double layoutExtent__17278 = Dart_uiLibrary.clampDouble((maxExtent__16948 - ((SliverConstraints)constraints__16899).scrollOffset), 0.0, effectiveRemainingPaintExtent__17141);
        double stretchOffset__17422 = ((stretchConfiguration is not null) ? ((SliverConstraints)constraints__16899).overlap.abs() : 0.0);
        geometry = new SliverGeometry(scrollExtent: maxExtent__16948, paintOrigin: ((SliverConstraints)constraints__16899).overlap, paintExtent: Math.Min(childExtent, effectiveRemainingPaintExtent__17141), layoutExtent: layoutExtent__17278, maxPaintExtent: (maxExtent__16948 + stretchOffset__17422), maxScrollObstructionExtent: minExtent, cacheExtent: ((layoutExtent__17278 > 0.0) ? (-((SliverConstraints)constraints__16899).cacheOrigin + layoutExtent__17278) : layoutExtent__17278), hasVisualOverflow: true);
    }

    public override double childMainAxisPosition(RenderObject child) => 0.0;
    public override void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        global::Doroti.Ui.Rect? localBounds__18249 = ((descendant is not null) ? MatrixUtils.transformRect(descendant.getTransformTo(this), (rect ?? ((RenderObject)descendant).paintBounds)) : rect);
        global::Doroti.Ui.Rect? newRect__18416 = (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection) switch { global::Doroti.Framework.Painting.AxisDirection.up => Sliver_persistent_headerLibrary._trim(localBounds__18249, bottom: childExtent), global::Doroti.Framework.Painting.AxisDirection.left => Sliver_persistent_headerLibrary._trim(localBounds__18249, right: childExtent), global::Doroti.Framework.Painting.AxisDirection.right => Sliver_persistent_headerLibrary._trim(localBounds__18249, left: 0), global::Doroti.Framework.Painting.AxisDirection.down => Sliver_persistent_headerLibrary._trim(localBounds__18249, top: 0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        base.showOnScreen(descendant: this, rect: newRect__18416, duration: duration, curve: curve);
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
        var stretchOffset__22758 = 0.0;
        if ((stretchConfiguration is not null))
        {
            stretchOffset__22758 += ((SliverConstraints)constraints).overlap.abs();
        }
        double maxExtent__22892 = this.maxExtent;
        double paintExtent__22937 = (maxExtent__22892 - DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset));
        double layoutExtent__23005 = (maxExtent__22892 - ((SliverConstraints)constraints).scrollOffset);
        geometry = new SliverGeometry(scrollExtent: maxExtent__22892, paintOrigin: Math.Min(((SliverConstraints)constraints).overlap, 0.0), paintExtent: Dart_uiLibrary.clampDouble(paintExtent__22937, 0.0, ((SliverConstraints)constraints).remainingPaintExtent), layoutExtent: Dart_uiLibrary.clampDouble(layoutExtent__23005, 0.0, ((SliverConstraints)constraints).remainingPaintExtent), maxPaintExtent: (maxExtent__22892 + stretchOffset__22758), hasVisualOverflow: true);
        return ((stretchOffset__22758 > 0L) ? 0.0 : Math.Min(0.0, (paintExtent__22937 - childExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimation(Duration duration, double endValue, Curve curve)
    {
        DartRuntimePrimitives.Assert(() => (this.vsync is not null));
        AnimationController effectiveController__23786 = _controller ??= ((Func<AnimationController>)(() =>
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
        _animation = effectiveController__23786.drive(new Tween<double>(begin: DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset), end: endValue).chain(new CurveTween(curve: curve)));
    }

    public virtual void updateScrollStartDirection(ScrollDirection direction)
    {
        _lastStartedScrollDirection = direction;
    }

    public virtual void maybeStartSnapAnimation(ScrollDirection direction)
    {
        FloatingHeaderSnapConfiguration? snap__24685 = this.snapConfiguration;
        if ((snap__24685 is null))
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
        _updateAnimation(((FloatingHeaderSnapConfiguration)snap__24685).duration, ((object.Equals(direction, ScrollDirection.forward)) ? 0.0 : maxExtent), ((FloatingHeaderSnapConfiguration)snap__24685).curve);
        this._controller?.forward(from: 0.0);
    }

    public virtual void maybeStopSnapAnimation(ScrollDirection direction)
    {
        this._controller?.stop();
    }

    public override void performLayout()
    {
        SliverConstraints constraints__25394 = this.constraints;
        double maxExtent__25443 = this.maxExtent;
        if (((this._lastActualScrollOffset is not null) && ((((((SliverConstraints)constraints__25394).scrollOffset < DartRuntimePrimitives.RequireValue(this._lastActualScrollOffset))) || ((DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) < maxExtent__25443))))))
        {
            double delta__25873 = (DartRuntimePrimitives.RequireValue(this._lastActualScrollOffset) - ((SliverConstraints)constraints__25394).scrollOffset);
            bool allowFloatingExpansion__25952 = ((object.Equals(((SliverConstraints)constraints__25394).userScrollDirection, ScrollDirection.forward)) || (((this._lastStartedScrollDirection is not null) && (object.Equals(this._lastStartedScrollDirection, ScrollDirection.forward)))));
            if (allowFloatingExpansion__25952)
            {
                if ((DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) > maxExtent__25443))
                {
                    _effectiveScrollOffset = maxExtent__25443;
                }
            }
            else
            {
                if ((delta__25873 > 0.0))
                {
                    delta__25873 = 0.0;
                }
            }
            _effectiveScrollOffset = Dart_uiLibrary.clampDouble((DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) - delta__25873), 0.0, ((SliverConstraints)constraints__25394).scrollOffset);
        }
        else
        {
            _effectiveScrollOffset = ((SliverConstraints)constraints__25394).scrollOffset;
        }
        bool overlapsContent__26809 = (DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset) < ((SliverConstraints)constraints__25394).scrollOffset);
        layoutChild(DartRuntimePrimitives.RequireValue(this._effectiveScrollOffset), maxExtent__25443, overlapsContent: overlapsContent__26809);
        _childPosition = updateGeometry();
        _lastActualScrollOffset = ((SliverConstraints)constraints__25394).scrollOffset;
    }

    public override void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        PersistentHeaderShowOnScreenConfiguration? showOnScreen__27277 = this.showOnScreenConfiguration;
        if ((showOnScreen__27277 is null))
        {
            base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
            return;
        }
        DartRuntimePrimitives.Assert(() => ((child is not null) || (descendant is null)));
        global::Doroti.Ui.Rect? childBounds__27993 = ((descendant is not null) ? MatrixUtils.transformRect(descendant.getTransformTo(child), (rect ?? ((RenderObject)descendant).paintBounds)) : rect);
        double targetExtent__28193 = default!;
        global::Doroti.Ui.Rect? targetRect__28217 = default!;
        switch (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(((SliverConstraints)constraints).axisDirection, ((SliverConstraints)constraints).growthDirection))
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    targetExtent__28193 = (childExtent - ((childBounds__27993?.top ?? 0L)));
                    targetRect__28217 = Sliver_persistent_headerLibrary._trim(childBounds__27993, bottom: childExtent);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    targetExtent__28193 = (childBounds__27993?.right ?? childExtent);
                    targetRect__28217 = Sliver_persistent_headerLibrary._trim(childBounds__27993, left: 0);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    targetExtent__28193 = (childBounds__27993?.bottom ?? childExtent);
                    targetRect__28217 = Sliver_persistent_headerLibrary._trim(childBounds__27993, top: 0);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    targetExtent__28193 = (childExtent - ((childBounds__27993?.left ?? 0L)));
                    targetRect__28217 = Sliver_persistent_headerLibrary._trim(childBounds__27993, right: childExtent);
                    break;
                }
        }
        double effectiveMaxExtent__29036 = Math.Max(childExtent, maxExtent);
        targetExtent__28193 = Dart_uiLibrary.clampDouble(Dart_uiLibrary.clampDouble(targetExtent__28193, ((PersistentHeaderShowOnScreenConfiguration)showOnScreen__27277).minShowOnScreenExtent, ((PersistentHeaderShowOnScreenConfiguration)showOnScreen__27277).maxShowOnScreenExtent), childExtent, effectiveMaxExtent__29036);
        if (((targetExtent__28193 > childExtent) && (!object.Equals(this._controller?.status, AnimationStatus.forward))))
        {
            double targetScrollOffset__29600 = (maxExtent - targetExtent__28193);
            DartRuntimePrimitives.Assert(() => (this.vsync is not null));
            _updateAnimation(duration, targetScrollOffset__29600, curve);
            this._controller?.forward(from: 0.0);
        }
        base.showOnScreen(descendant: ((descendant is null) ? this : child), rect: targetRect__28217, duration: duration, curve: curve);
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
        double minExtent__31298 = this.minExtent;
        double minAllowedExtent__31343 = ((((SliverConstraints)constraints).remainingPaintExtent > minExtent__31298) ? minExtent__31298 : ((SliverConstraints)constraints).remainingPaintExtent);
        double maxExtent__31488 = this.maxExtent;
        double paintExtent__31533 = (maxExtent__31488 - DartRuntimePrimitives.RequireValue(_effectiveScrollOffset));
        double clampedPaintExtent__31601 = Dart_uiLibrary.clampDouble(paintExtent__31533, minAllowedExtent__31343, ((SliverConstraints)constraints).remainingPaintExtent);
        double layoutExtent__31742 = (maxExtent__31488 - ((SliverConstraints)constraints).scrollOffset);
        double stretchOffset__31812 = ((stretchConfiguration is not null) ? ((SliverConstraints)constraints).overlap.abs() : 0.0);
        geometry = new SliverGeometry(scrollExtent: maxExtent__31488, paintOrigin: Math.Min(((SliverConstraints)constraints).overlap, 0.0), paintExtent: clampedPaintExtent__31601, layoutExtent: Dart_uiLibrary.clampDouble(layoutExtent__31742, 0.0, clampedPaintExtent__31601), maxPaintExtent: (maxExtent__31488 + stretchOffset__31812), maxScrollObstructionExtent: minExtent__31298, hasVisualOverflow: true);
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

