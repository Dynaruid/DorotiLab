// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/sliver_persistent_header.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Rendering;

public static partial class Sliver_persistent_headerLibrary
{
    internal static Rect? _trim(
        Rect? original,
        double? top = null,
        double right = double.PositiveInfinity,
        double bottom = double.PositiveInfinity,
        double? left = null
    )
    {
        double __top = top ?? -double.PositiveInfinity;
        double __left = left ?? -double.PositiveInfinity;
        return original?.intersect(Rect.fromLTRB(__left, __top, right, bottom));
    }
}

public class OverScrollHeaderStretchConfiguration
{
    public virtual double stretchTriggerOffset { get; private set; } = default!;
    public virtual Func<Future>? onStretchTrigger { get; private set; }

    public OverScrollHeaderStretchConfiguration(
        double stretchTriggerOffset = 100.0,
        Func<Future>? onStretchTrigger = null
    )
    {
        this.stretchTriggerOffset = stretchTriggerOffset;
        this.onStretchTrigger = onStretchTrigger;
    }
}

public class PersistentHeaderShowOnScreenConfiguration
{
    public virtual double minShowOnScreenExtent { get; private set; } = default!;
    public virtual double maxShowOnScreenExtent { get; private set; } = default!;

    public PersistentHeaderShowOnScreenConfiguration(
        double minShowOnScreenExtent = double.NegativeInfinity,
        double maxShowOnScreenExtent = double.PositiveInfinity
    )
    {
        this.minShowOnScreenExtent = minShowOnScreenExtent;
        this.maxShowOnScreenExtent = maxShowOnScreenExtent;
        System.Diagnostics.Debug.Assert(minShowOnScreenExtent <= maxShowOnScreenExtent);
    }
}

public abstract class RenderSliverPersistentHeader
    : RenderSliver,
        RenderObjectWithChildMixin<RenderBox>,
        RenderSliverHelpers
{
    internal virtual double _lastStretchOffset { get; set; } = default!;
    internal virtual bool _needsUpdateChild { get; set; } = true;
    internal virtual double _lastShrinkOffset { get; set; } = 0.0;
    internal virtual bool _lastOverlapsContent { get; set; } = false;
    public virtual OverScrollHeaderStretchConfiguration? stretchConfiguration { get; set; } =
        default;
    public virtual RenderBox? _child { get; set; } = default;

    protected RenderSliverPersistentHeader(
        RenderBox? child = null,
        OverScrollHeaderStretchConfiguration? stretchConfiguration = null
    )
    {
        this.stretchConfiguration = stretchConfiguration;
    }

    public abstract double maxExtent { get; }
    public abstract double minExtent { get; }
    public virtual double childExtent
    {
        get
        {
            if (child is null)
            {
                return 0.0;
            }
            DartRuntimePrimitives.Assert(() => child!.hasSize);
            return constraints.axis switch
            {
                Axis.vertical => child!.size.height,
                Axis.horizontal => child!.size.width,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
        }
    }
    public virtual double lastShrinkOffset => _lastShrinkOffset;
    public virtual bool lastOverlapsContent => _lastOverlapsContent;

    public virtual void updateChild(double shrinkOffset, bool overlapsContent) { }

    public override void markNeedsLayout()
    {
        _needsUpdateChild = true;
        base.markNeedsLayout();
    }

    public virtual void layoutChild(
        double scrollOffset,
        double maxExtent,
        bool overlapsContent = false
    )
    {
        double shrinkOffset = Math.Min(scrollOffset, maxExtent);
        if (
            _needsUpdateChild
            || (_lastShrinkOffset != shrinkOffset)
            || (_lastOverlapsContent != overlapsContent)
        )
        {
            invokeLayoutCallback<SliverConstraints>(
                (constraints) =>
                {
                    DartRuntimePrimitives.Assert(() => Equals(constraints, this.constraints));
                    updateChild(shrinkOffset, overlapsContent);
                }
            );
            _lastShrinkOffset = shrinkOffset;
            _lastOverlapsContent = overlapsContent;
            _needsUpdateChild = false;
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (minExtent <= maxExtent)
            {
                return true;
            }
            throw new FlutterError(
                new List<DiagnosticsNode>
                {
                    new ErrorSummary(
                        $"The maxExtent for this {GetType()} is less than its minExtent."
                    ),
                    new DoubleProperty("The specified maxExtent was", maxExtent),
                    new DoubleProperty("The specified minExtent was", minExtent),
                }
            );
        });
        var stretchOffset = 0.0;
        if ((stretchConfiguration is not null) && (constraints.scrollOffset == 0.0))
        {
            stretchOffset += constraints.overlap.abs();
        }
        child?.layout(
            constraints.asBoxConstraints(
                maxExtent: Math.Max(minExtent, maxExtent - shrinkOffset) + stretchOffset
            ),
            parentUsesSize: true
        );
        if (
            (stretchConfiguration is not null)
            && (stretchConfiguration!.onStretchTrigger is not null)
            && (stretchOffset >= stretchConfiguration!.stretchTriggerOffset)
            && (_lastStretchOffset <= stretchConfiguration!.stretchTriggerOffset)
        )
        {
            _ = stretchConfiguration!.onStretchTrigger!();
        }
        _lastStretchOffset = stretchOffset;
    }

    public override double childMainAxisPosition(RenderObject child) =>
        base.childMainAxisPosition(child);

    public override bool hitTestChildren(
        SliverHitTestResult result,
        double mainAxisPosition,
        double crossAxisPosition
    )
    {
        DartRuntimePrimitives.Assert(() => geometry!.hitTestExtent > 0.0);
        if (child is not null)
        {
            return hitTestBoxChild(
                BoxHitTestResult.CreateWrap(result),
                child!,
                mainAxisPosition: mainAxisPosition,
                crossAxisPosition: crossAxisPosition
            );
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => Equals(child, this.child));
        applyPaintTransformForBoxChild(((RenderBox?)(object?)child)!, transform);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((child is not null) && geometry!.visible)
        {
            offset += SliverLibrary.applyGrowthDirectionToAxisDirection(
                constraints.axisDirection,
                constraints.growthDirection
            ) switch
            {
                AxisDirection.up => new Offset(
                    0.0,
                    geometry!.paintExtent - childMainAxisPosition(child!) - childExtent
                ),
                AxisDirection.left => new Offset(
                    geometry!.paintExtent - childMainAxisPosition(child!) - childExtent,
                    0.0
                ),
                AxisDirection.right => new Offset(childMainAxisPosition(child!), 0.0),
                AxisDirection.down => new Offset(0.0, childMainAxisPosition(child!)),
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            context.paintChild(child!, offset);
        }
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.addTagForChildren(RenderViewport.excludeFromScrolling);
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("maxExtent", () => maxExtent));
        properties.add(new DoubleProperty("child position", () => childMainAxisPosition(child!)));
    }

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

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        _child?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        _child?.detach();
    }

    public override void redepthChildren()
    {
        if (_child is not null)
        {
            redepthChild(_child!);
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child!);
        }
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return (child is not null)
            ? new List<DiagnosticsNode>
            {
                ((Diagnosticable)child!).toDiagnosticsNode(name: "child"),
            }
            : new List<DiagnosticsNode>();
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

public abstract class RenderSliverScrollingPersistentHeader : RenderSliverPersistentHeader
{
    internal virtual double? _childPosition { get; set; } = default;

    protected RenderSliverScrollingPersistentHeader(
        RenderBox? child = null,
        OverScrollHeaderStretchConfiguration? stretchConfiguration = null
    )
        : base(child: child, stretchConfiguration: stretchConfiguration) { }

    public virtual double updateGeometry()
    {
        var stretchOffset = 0.0;
        if (stretchConfiguration is not null)
        {
            stretchOffset += constraints.overlap.abs();
        }
        double maxExtentLocal = maxExtent;
        double paintExtentLocal = maxExtentLocal - constraints.scrollOffset;
        double cacheExtentLocal = calculateCacheOffset(constraints, from: 0.0, to: maxExtentLocal);
        geometry = new SliverGeometry(
            cacheExtent: cacheExtentLocal,
            scrollExtent: maxExtentLocal,
            paintOrigin: Math.Min(constraints.overlap, 0.0),
            paintExtent: Dart_uiLibrary.clampDouble(
                paintExtentLocal,
                0.0,
                constraints.remainingPaintExtent
            ),
            maxPaintExtent: maxExtentLocal + stretchOffset,
            hasVisualOverflow: true
        );
        return (stretchOffset > 0L) ? 0.0 : Math.Min(0.0, paintExtentLocal - childExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        layoutChild(constraints.scrollOffset, maxExtent);
        _childPosition = updateGeometry();
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => Equals(__child, this.child));
        DartRuntimePrimitives.Assert(() => _childPosition is not null);
        return (
            _childPosition
            ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class RenderSliverPinnedPersistentHeader : RenderSliverPersistentHeader
{
    public virtual PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration { get; set; } =
        default;

    protected RenderSliverPinnedPersistentHeader(
        RenderBox? child = null,
        OverScrollHeaderStretchConfiguration? stretchConfiguration = null,
        PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = default!
    )
        : base(child: child, stretchConfiguration: stretchConfiguration)
    {
        PersistentHeaderShowOnScreenConfiguration? __showOnScreenConfiguration =
            showOnScreenConfiguration ?? new PersistentHeaderShowOnScreenConfiguration();
        this.showOnScreenConfiguration = __showOnScreenConfiguration;
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = constraints;
        double maxExtentLocal = maxExtent;
        bool overlapsContentLocal = constraintsLocal.overlap > 0.0;
        layoutChild(
            constraintsLocal.scrollOffset,
            maxExtentLocal,
            overlapsContent: overlapsContentLocal
        );
        double effectiveRemainingPaintExtent = Math.Max(
            0,
            constraintsLocal.remainingPaintExtent - constraintsLocal.overlap
        );
        double layoutExtentLocal = Dart_uiLibrary.clampDouble(
            maxExtentLocal - constraintsLocal.scrollOffset,
            0.0,
            effectiveRemainingPaintExtent
        );
        double stretchOffset =
            (stretchConfiguration is not null) ? constraintsLocal.overlap.abs() : 0.0;
        geometry = new SliverGeometry(
            scrollExtent: maxExtentLocal,
            paintOrigin: constraintsLocal.overlap,
            paintExtent: Math.Min(childExtent, effectiveRemainingPaintExtent),
            layoutExtent: layoutExtentLocal,
            maxPaintExtent: maxExtentLocal + stretchOffset,
            maxScrollObstructionExtent: minExtent,
            cacheExtent: (layoutExtentLocal > 0.0)
                ? (-constraintsLocal.cacheOrigin + layoutExtentLocal)
                : layoutExtentLocal,
            hasVisualOverflow: true
        );
    }

    public override double childMainAxisPosition(RenderObject child) => 0.0;

    public override void showOnScreen(
        RenderObject? descendant = null,
        Rect? rect = null,
        Duration duration = default,
        Curve curve = default!
    )
    {
        Rect? localBounds =
            (descendant is not null)
                ? MatrixUtils.transformRect(
                    descendant.getTransformTo(this),
                    rect ?? descendant.paintBounds
                )
                : rect;
        Rect? newRect = SliverLibrary.applyGrowthDirectionToAxisDirection(
            constraints.axisDirection,
            constraints.growthDirection
        ) switch
        {
            AxisDirection.up => Sliver_persistent_headerLibrary._trim(
                localBounds,
                bottom: childExtent
            ),
            AxisDirection.left => Sliver_persistent_headerLibrary._trim(
                localBounds,
                right: childExtent
            ),
            AxisDirection.right => Sliver_persistent_headerLibrary._trim(localBounds, left: 0),
            AxisDirection.down => Sliver_persistent_headerLibrary._trim(localBounds, top: 0),
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
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
    public virtual PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration { get; set; } =
        default;

    protected RenderSliverFloatingPersistentHeader(
        RenderBox? child = null,
        TickerProvider? vsync = null,
        FloatingHeaderSnapConfiguration? snapConfiguration = null,
        OverScrollHeaderStretchConfiguration? stretchConfiguration = null,
        PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = default!
    )
        : base(child: child, stretchConfiguration: stretchConfiguration)
    {
        this.snapConfiguration = snapConfiguration;
        this.showOnScreenConfiguration = showOnScreenConfiguration;
        _vsync = vsync;
    }

    public override void detach()
    {
        _controller?.dispose();
        _controller = null;
        base.detach();
    }

    public virtual TickerProvider? vsync
    {
        get => _vsync;
        set
        {
            var __value = value;
            if (Equals(__value, _vsync))
            {
                return;
            }
            _vsync = __value;
            if (__value is null)
            {
                _controller?.dispose();
                _controller = null;
            }
            else
            {
                _controller?.resync(__value);
            }
        }
    }

    public virtual double updateGeometry()
    {
        var stretchOffset = 0.0;
        if (stretchConfiguration is not null)
        {
            stretchOffset += constraints.overlap.abs();
        }
        double maxExtentLocal = maxExtent;
        double paintExtentLocal =
            maxExtentLocal
            - (
                _effectiveScrollOffset
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        double layoutExtentLocal = maxExtentLocal - constraints.scrollOffset;
        geometry = new SliverGeometry(
            scrollExtent: maxExtentLocal,
            paintOrigin: Math.Min(constraints.overlap, 0.0),
            paintExtent: Dart_uiLibrary.clampDouble(
                paintExtentLocal,
                0.0,
                constraints.remainingPaintExtent
            ),
            layoutExtent: Dart_uiLibrary.clampDouble(
                layoutExtentLocal,
                0.0,
                constraints.remainingPaintExtent
            ),
            maxPaintExtent: maxExtentLocal + stretchOffset,
            hasVisualOverflow: true
        );
        return (stretchOffset > 0L) ? 0.0 : Math.Min(0.0, paintExtentLocal - childExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimation(Duration duration, double endValue, Curve curve)
    {
        DartRuntimePrimitives.Assert(() => vsync is not null);
        AnimationController effectiveController = _controller ??= (
            (Func<AnimationController>)(
                () =>
                {
                    var __cascade = new AnimationController(vsync: vsync!, duration: duration);
                    __cascade.addListener(() =>
                    {
                        if (_effectiveScrollOffset == _animation.value)
                        {
                            return;
                        }
                        _effectiveScrollOffset = _animation.value;
                        markNeedsLayout();
                    });
                    return __cascade;
                }
            )
        )();
        _animation = effectiveController.drive(
            new Tween<double>(
                begin: (
                    _effectiveScrollOffset
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ),
                end: endValue
            ).chain(new CurveTween(curve: curve))
        );
    }

    public virtual void updateScrollStartDirection(ScrollDirection direction)
    {
        _lastStartedScrollDirection = direction;
    }

    public virtual void maybeStartSnapAnimation(ScrollDirection direction)
    {
        FloatingHeaderSnapConfiguration? snap = snapConfiguration;
        if (snap is null)
        {
            return;
        }
        if (
            Equals(direction, ScrollDirection.forward)
            && (
                (
                    _effectiveScrollOffset
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) <= 0.0
            )
        )
        {
            return;
        }
        if (
            Equals(direction, ScrollDirection.reverse)
            && (
                (
                    _effectiveScrollOffset
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) >= maxExtent
            )
        )
        {
            return;
        }
        _updateAnimation(
            snap.duration,
            Equals(direction, ScrollDirection.forward) ? 0.0 : maxExtent,
            snap.curve
        );
        _controller?.forward(from: 0.0);
    }

    public virtual void maybeStopSnapAnimation(ScrollDirection direction)
    {
        _controller?.stop();
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = constraints;
        double maxExtentLocal = maxExtent;
        if (
            (_lastActualScrollOffset is not null)
            && (
                constraintsLocal.scrollOffset
                    < (
                        _lastActualScrollOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                || (
                    _effectiveScrollOffset
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) < maxExtentLocal
            )
        )
        {
            double delta =
                (
                    _lastActualScrollOffset
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) - constraintsLocal.scrollOffset;
            bool allowFloatingExpansion =
                Equals(constraintsLocal.userScrollDirection, ScrollDirection.forward)
                || (
                    (_lastStartedScrollDirection is not null)
                    && Equals(_lastStartedScrollDirection, ScrollDirection.forward)
                );
            if (allowFloatingExpansion)
            {
                if (
                    (
                        _effectiveScrollOffset
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) > maxExtentLocal
                )
                {
                    _effectiveScrollOffset = maxExtentLocal;
                }
            }
            else
            {
                if (delta > 0.0)
                {
                    delta = 0.0;
                }
            }
            _effectiveScrollOffset = Dart_uiLibrary.clampDouble(
                (
                    _effectiveScrollOffset
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) - delta,
                0.0,
                constraintsLocal.scrollOffset
            );
        }
        else
        {
            _effectiveScrollOffset = constraintsLocal.scrollOffset;
        }
        bool overlapsContentLocal =
            (
                _effectiveScrollOffset
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) < constraintsLocal.scrollOffset;
        layoutChild(
            (
                _effectiveScrollOffset
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ),
            maxExtentLocal,
            overlapsContent: overlapsContentLocal
        );
        _childPosition = updateGeometry();
        _lastActualScrollOffset = constraintsLocal.scrollOffset;
    }

    public override void showOnScreen(
        RenderObject? descendant = null,
        Rect? rect = null,
        Duration duration = default,
        Curve curve = default!
    )
    {
        PersistentHeaderShowOnScreenConfiguration? showOnScreenLocal = showOnScreenConfiguration;
        if (showOnScreenLocal is null)
        {
            base.showOnScreen(descendant: descendant, rect: rect, duration: duration, curve: curve);
            return;
        }
        DartRuntimePrimitives.Assert(() => (child is not null) || (descendant is null));
        Rect? childBounds =
            (descendant is not null)
                ? MatrixUtils.transformRect(
                    descendant.getTransformTo(child),
                    rect ?? descendant.paintBounds
                )
                : rect;
        double targetExtent = default!;
        Rect? targetRect = default!;
        switch (
            SliverLibrary.applyGrowthDirectionToAxisDirection(
                constraints.axisDirection,
                constraints.growthDirection
            )
        )
        {
            case AxisDirection.up:
            {
                targetExtent = childExtent - (childBounds?.top ?? 0L);
                targetRect = Sliver_persistent_headerLibrary._trim(
                    childBounds,
                    bottom: childExtent
                );
                break;
            }
            case AxisDirection.right:
            {
                targetExtent = childBounds?.right ?? childExtent;
                targetRect = Sliver_persistent_headerLibrary._trim(childBounds, left: 0);
                break;
            }
            case AxisDirection.down:
            {
                targetExtent = childBounds?.bottom ?? childExtent;
                targetRect = Sliver_persistent_headerLibrary._trim(childBounds, top: 0);
                break;
            }
            case AxisDirection.left:
            {
                targetExtent = childExtent - (childBounds?.left ?? 0L);
                targetRect = Sliver_persistent_headerLibrary._trim(childBounds, right: childExtent);
                break;
            }
        }
        double effectiveMaxExtent = Math.Max(childExtent, maxExtent);
        targetExtent = Dart_uiLibrary.clampDouble(
            Dart_uiLibrary.clampDouble(
                targetExtent,
                showOnScreenLocal.minShowOnScreenExtent,
                showOnScreenLocal.maxShowOnScreenExtent
            ),
            childExtent,
            effectiveMaxExtent
        );
        if ((targetExtent > childExtent) && (!Equals(_controller?.status, AnimationStatus.forward)))
        {
            double targetScrollOffset = maxExtent - targetExtent;
            DartRuntimePrimitives.Assert(() => vsync is not null);
            _updateAnimation(duration, targetScrollOffset, curve);
            _controller?.forward(from: 0.0);
        }
        base.showOnScreen(
            descendant: (descendant is null) ? this : child,
            rect: targetRect,
            duration: duration,
            curve: curve
        );
    }

    public override double childMainAxisPosition(RenderObject child)
    {
        var __child = (RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => Equals(__child, this.child));
        return _childPosition ?? 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DoubleProperty("effective scroll offset", _effectiveScrollOffset));
    }
}

public abstract class RenderSliverFloatingPinnedPersistentHeader
    : RenderSliverFloatingPersistentHeader
{
    protected RenderSliverFloatingPinnedPersistentHeader(
        RenderBox? child = null,
        TickerProvider? vsync = null,
        FloatingHeaderSnapConfiguration? snapConfiguration = null,
        OverScrollHeaderStretchConfiguration? stretchConfiguration = null,
        PersistentHeaderShowOnScreenConfiguration? showOnScreenConfiguration = null
    )
        : base(
            child: child,
            vsync: vsync,
            snapConfiguration: snapConfiguration,
            stretchConfiguration: stretchConfiguration,
            showOnScreenConfiguration: showOnScreenConfiguration
        ) { }

    public override double updateGeometry()
    {
        double minExtentLocal = minExtent;
        double minAllowedExtent =
            (constraints.remainingPaintExtent > minExtentLocal)
                ? minExtentLocal
                : constraints.remainingPaintExtent;
        double maxExtentLocal = maxExtent;
        double paintExtentLocal =
            maxExtentLocal
            - (
                _effectiveScrollOffset
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
        double clampedPaintExtent = Dart_uiLibrary.clampDouble(
            paintExtentLocal,
            minAllowedExtent,
            constraints.remainingPaintExtent
        );
        double layoutExtentLocal = maxExtentLocal - constraints.scrollOffset;
        double stretchOffset = (stretchConfiguration is not null) ? constraints.overlap.abs() : 0.0;
        geometry = new SliverGeometry(
            scrollExtent: maxExtentLocal,
            paintOrigin: Math.Min(constraints.overlap, 0.0),
            paintExtent: clampedPaintExtent,
            layoutExtent: Dart_uiLibrary.clampDouble(layoutExtentLocal, 0.0, clampedPaintExtent),
            maxPaintExtent: maxExtentLocal + stretchOffset,
            maxScrollObstructionExtent: minExtentLocal,
            hasVisualOverflow: true
        );
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
