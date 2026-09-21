// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/refresh.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class RefreshLibrary
{
    internal static double _kActivityIndicatorRadius = 14.0;
}

public static partial class RefreshLibrary
{
    internal static double _kActivityIndicatorMargin = 16.0;
}

internal class _CupertinoSliverRefresh__refresh : SingleChildRenderObjectWidget
{
    public virtual double refreshIndicatorLayoutExtent { get; private set; } = default!;
    public virtual bool hasLayoutExtent { get; private set; } = default!;

    internal _CupertinoSliverRefresh__refresh(
        double refreshIndicatorLayoutExtent = 0.0,
        bool hasLayoutExtent = false,
        Widget? child = null
    )
        : base(child: child)
    {
        this.refreshIndicatorLayoutExtent = refreshIndicatorLayoutExtent;
        this.hasLayoutExtent = hasLayoutExtent;
        System.Diagnostics.Debug.Assert(refreshIndicatorLayoutExtent >= 0.0);
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderCupertinoSliverRefresh__refresh(
            refreshIndicatorExtent: refreshIndicatorLayoutExtent,
            hasLayoutExtent: hasLayoutExtent
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoSliverRefresh__refresh)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderCupertinoSliverRefresh__refresh>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.refreshIndicatorLayoutExtent = refreshIndicatorLayoutExtent;
                        __cascade.hasLayoutExtent = hasLayoutExtent;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

public class _RenderCupertinoSliverRefresh__refresh
    : RenderSliver,
        RenderObjectWithChildMixin<RenderBox>
{
    internal virtual double _refreshIndicatorExtent { get; set; } = default!;
    internal virtual bool _hasLayoutExtent { get; set; } = default!;
    public virtual double layoutExtentOffsetCompensation { get; set; } = 0.0;
    public virtual RenderBox? _child { get; set; } = default;

    internal _RenderCupertinoSliverRefresh__refresh(
        double refreshIndicatorExtent,
        bool hasLayoutExtent,
        RenderBox? child = null
    )
    {
        _refreshIndicatorExtent = refreshIndicatorExtent;
        _hasLayoutExtent = hasLayoutExtent;
        System.Diagnostics.Debug.Assert(refreshIndicatorExtent >= 0.0);
        this.child = child;
    }

    public virtual double refreshIndicatorLayoutExtent
    {
        get => _refreshIndicatorExtent;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => __value >= 0.0);
            if (__value == _refreshIndicatorExtent)
            {
                return;
            }
            _refreshIndicatorExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual bool hasLayoutExtent
    {
        get => _hasLayoutExtent;
        set
        {
            var __value = value;
            if (__value == _hasLayoutExtent)
            {
                return;
            }
            _hasLayoutExtent = __value;
            markNeedsLayout();
        }
    }

    public override void performLayout()
    {
        SliverConstraints constraintsLocal = constraints;
        DartRuntimePrimitives.Assert(() =>
            Equals(constraintsLocal.axisDirection, AxisDirection.down)
        );
        DartRuntimePrimitives.Assert(() =>
            Equals(constraintsLocal.growthDirection, GrowthDirection.forward)
        );
        double layoutExtentLocal = (_hasLayoutExtent ? 1.0 : 0.0) * _refreshIndicatorExtent;
        if (layoutExtentLocal != layoutExtentOffsetCompensation)
        {
            geometry = new SliverGeometry(
                scrollOffsetCorrection: layoutExtentLocal - layoutExtentOffsetCompensation
            );
            layoutExtentOffsetCompensation = layoutExtentLocal;
            return;
        }
        bool active = (constraintsLocal.overlap < 0.0) || (layoutExtentLocal > 0.0);
        double overscrolledExtent =
            (constraintsLocal.overlap < 0.0) ? constraintsLocal.overlap.abs() : 0.0;
        child!.layout(
            constraintsLocal.asBoxConstraints(maxExtent: layoutExtentLocal + overscrolledExtent),
            parentUsesSize: true
        );
        if (active)
        {
            geometry = new SliverGeometry(
                scrollExtent: layoutExtentLocal,
                paintOrigin: -overscrolledExtent - constraintsLocal.scrollOffset,
                paintExtent: Math.Max(
                    Math.Max(child!.size.height, layoutExtentLocal) - constraintsLocal.scrollOffset,
                    0.0
                ),
                maxPaintExtent: Math.Max(
                    Math.Max(child!.size.height, layoutExtentLocal) - constraintsLocal.scrollOffset,
                    0.0
                ),
                layoutExtent: Math.Max(layoutExtentLocal - constraintsLocal.scrollOffset, 0.0)
            );
        }
        else
        {
            geometry = SliverGeometry.zero;
        }
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        if ((constraints.overlap < 0.0) || ((constraints.scrollOffset + child!.size.height) > 0L))
        {
            context.paintChild(child!, offset);
        }
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform) { }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not RenderBox)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
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
                    )
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum RefreshIndicatorMode
{
    inactive,
    drag,
    armed,
    refresh,
    done,
}

public delegate Widget RefreshControlIndicatorBuilder(
    BuildContext context,
    RefreshIndicatorMode refreshState,
    double pulledExtent,
    double refreshTriggerPullDistance,
    double refreshIndicatorExtent
);

public delegate Future RefreshCallback();

public class CupertinoSliverRefreshControl : StatefulWidget
{
    public virtual double refreshTriggerPullDistance { get; private set; } = default!;
    public virtual double refreshIndicatorExtent { get; private set; } = default!;
    public virtual Func<BuildContext, RefreshIndicatorMode, double, double, double, Widget>? builder
    {
        get;
        private set;
    }
    public virtual Func<Future>? onRefresh { get; private set; }
    internal const double _defaultRefreshTriggerPullDistance = 100.0;
    internal const double _defaultRefreshIndicatorExtent = 60.0;

    public CupertinoSliverRefreshControl(
        Key? key = null,
        double? refreshTriggerPullDistance = null,
        double? refreshIndicatorExtent = null,
        Func<BuildContext, RefreshIndicatorMode, double, double, double, Widget>? builder =
            default!,
        Func<Future>? onRefresh = null
    )
        : base(key: key)
    {
        double __refreshTriggerPullDistance =
            refreshTriggerPullDistance ?? _defaultRefreshTriggerPullDistance;
        double __refreshIndicatorExtent = refreshIndicatorExtent ?? _defaultRefreshIndicatorExtent;
        Func<BuildContext, RefreshIndicatorMode, double, double, double, Widget>? __builder =
            builder ?? buildRefreshIndicator;
        this.refreshTriggerPullDistance = __refreshTriggerPullDistance;
        this.refreshIndicatorExtent = __refreshIndicatorExtent;
        this.builder = __builder;
        this.onRefresh = onRefresh;
        System.Diagnostics.Debug.Assert(__refreshTriggerPullDistance > 0.0);
        System.Diagnostics.Debug.Assert(__refreshIndicatorExtent >= 0.0);
        System.Diagnostics.Debug.Assert(__refreshTriggerPullDistance >= __refreshIndicatorExtent);
    }

    public static RefreshIndicatorMode state(BuildContext context)
    {
        _CupertinoSliverRefreshControlState__refresh stateLocal =
            context.findAncestorStateOfType<_CupertinoSliverRefreshControlState__refresh>()!;
        return stateLocal.refreshState;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Widget buildRefreshIndicator(
        BuildContext context,
        RefreshIndicatorMode refreshState,
        double pulledExtent,
        double refreshTriggerPullDistance,
        double refreshIndicatorExtent
    )
    {
        double percentageComplete = Dart_uiLibrary.clampDouble(
            pulledExtent / refreshTriggerPullDistance,
            0.0,
            1.0
        );
        return new Center(
            child: new Stack(
                clipBehavior: Clip.none,
                children: new List<Widget>
                {
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new Positioned(
                            top: RefreshLibrary._kActivityIndicatorMargin,
                            left: 0.0,
                            right: 0.0,
                            child: _buildIndicatorForRefreshState(
                                refreshState,
                                RefreshLibrary._kActivityIndicatorRadius,
                                percentageComplete
                            )
                        )
                    ),
                }
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static Widget _buildIndicatorForRefreshState(
        RefreshIndicatorMode refreshState,
        double radius,
        double percentageComplete
    )
    {
        switch (refreshState)
        {
            case RefreshIndicatorMode.drag:
            {
                Curve opacityCurve = new Interval(0.0, 0.35, curve: Curves.easeInOut);
                return new Opacity(
                    opacity: opacityCurve.transform(percentageComplete),
                    child: CupertinoActivityIndicator.CreatePartiallyRevealed(
                        radius: radius,
                        progress: percentageComplete
                    )
                );
            }
            case RefreshIndicatorMode.armed:
            case RefreshIndicatorMode.refresh:
            {
                return new CupertinoActivityIndicator(radius: radius);
            }
            case RefreshIndicatorMode.done:
            {
                return new CupertinoActivityIndicator(radius: radius * percentageComplete);
            }
            case RefreshIndicatorMode.inactive:
            {
                return SizedBox.CreateShrink();
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _CupertinoSliverRefreshControlState__refresh()
        );
}

internal class _CupertinoSliverRefreshControlState__refresh : State<CupertinoSliverRefreshControl>
{
    internal const double _inactiveResetOverscrollFraction = 0.1;
    public virtual RefreshIndicatorMode refreshState { get; set; } = default!;
    public virtual Future? refreshTask { get; set; } = default;
    public virtual double latestIndicatorBoxExtent { get; set; } = 0.0;
    public virtual bool hasSliverLayoutExtent { get; set; } = false;

    public override void initState()
    {
        base.initState();
        refreshState = RefreshIndicatorMode.inactive;
    }

    public virtual RefreshIndicatorMode transitionNextState()
    {
        RefreshIndicatorMode nextState = default!;
        void goToDone()
        {
            nextState = RefreshIndicatorMode.done;
            if (
                Equals(
                    Scheduler.SchedulerBinding.instance.schedulerPhase,
                    Scheduler.SchedulerPhase.idle
                )
            )
            {
                setState(() =>
                {
                    _ = hasSliverLayoutExtent = false;
                });
            }
            else
            {
                Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                    (timestamp) =>
                    {
                        setState(() =>
                        {
                            _ = hasSliverLayoutExtent = false;
                        });
                    },
                    debugLabel: "Refresh.goToDone"
                );
            }
        }
        switch (refreshState)
        {
            case RefreshIndicatorMode.inactive:
            {
                if (latestIndicatorBoxExtent <= 0L)
                {
                    return RefreshIndicatorMode.inactive;
                }
                else
                {
                    nextState = RefreshIndicatorMode.drag;
                }
                goto case RefreshIndicatorMode.drag;
            }
            case RefreshIndicatorMode.drag:
            {
                if (latestIndicatorBoxExtent == 0L)
                {
                    return RefreshIndicatorMode.inactive;
                }
                else
                {
                    if (latestIndicatorBoxExtent < widget.refreshTriggerPullDistance)
                    {
                        return RefreshIndicatorMode.drag;
                    }
                    else
                    {
                        if (widget.onRefresh is not null)
                        {
                            DartRuntimePrimitives.Ignore(HapticFeedback.mediumImpact());
                            Scheduler.SchedulerBinding.instance.addPostFrameCallback(
                                (timestamp) =>
                                {
                                    DartRuntimePrimitives.Ignore(
                                        refreshTask = (
                                            (Func<Future>)(
                                                () =>
                                                {
                                                    var __cascade = widget.onRefresh!();
                                                    __cascade.whenComplete(() =>
                                                    {
                                                        if (mounted)
                                                        {
                                                            setState(() =>
                                                            {
                                                                _ = refreshTask = null;
                                                            });
                                                            refreshState = transitionNextState();
                                                        }
                                                    });
                                                    return __cascade;
                                                }
                                            )
                                        )()
                                    );
                                    setState(() =>
                                    {
                                        _ = hasSliverLayoutExtent = true;
                                    });
                                },
                                debugLabel: "Refresh.transition"
                            );
                        }
                        return RefreshIndicatorMode.armed;
                    }
                }
            }
            case RefreshIndicatorMode.armed:
            {
                if (Equals(refreshState, RefreshIndicatorMode.armed) && (refreshTask is null))
                {
                    goToDone();
                    goto case RefreshIndicatorMode.done;
                }
                if (latestIndicatorBoxExtent > widget.refreshIndicatorExtent)
                {
                    return RefreshIndicatorMode.armed;
                }
                else
                {
                    nextState = RefreshIndicatorMode.refresh;
                }
                goto case RefreshIndicatorMode.refresh;
            }
            case RefreshIndicatorMode.refresh:
            {
                if (refreshTask is not null)
                {
                    return RefreshIndicatorMode.refresh;
                }
                else
                {
                    goToDone();
                }
                goto case RefreshIndicatorMode.done;
            }
            case RefreshIndicatorMode.done:
            {
                if (
                    latestIndicatorBoxExtent
                    > (widget.refreshTriggerPullDistance * _inactiveResetOverscrollFraction)
                )
                {
                    return RefreshIndicatorMode.done;
                }
                else
                {
                    nextState = RefreshIndicatorMode.inactive;
                }
                break;
            }
        }
        return nextState;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new _CupertinoSliverRefresh__refresh(
            refreshIndicatorLayoutExtent: widget.refreshIndicatorExtent,
            hasLayoutExtent: hasSliverLayoutExtent,
            child: new LayoutBuilder(
                builder: (context, constraints) =>
                {
                    latestIndicatorBoxExtent = constraints.maxHeight;
                    refreshState = transitionNextState();
                    if ((widget.builder is not null) && (latestIndicatorBoxExtent > 0L))
                    {
                        return widget.builder!(
                            context,
                            refreshState,
                            latestIndicatorBoxExtent,
                            widget.refreshTriggerPullDistance,
                            widget.refreshIndicatorExtent
                        );
                    }
                    return new LimitedBox(
                        maxWidth: 0.0,
                        maxHeight: 0.0,
                        child: SizedBox.CreateExpand()
                    );
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
