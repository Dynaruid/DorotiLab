// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/refresh.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

public static partial class RefreshLibrary
{
    internal static double _kActivityIndicatorRadius = 14.0;
}

public static partial class RefreshLibrary
{
    internal static double _kActivityIndicatorMargin = 16.0;
}

internal class _CupertinoSliverRefresh__refresh : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual double refreshIndicatorLayoutExtent { get; private set; } = default!;
    public virtual bool hasLayoutExtent { get; private set; } = default!;

    internal _CupertinoSliverRefresh__refresh(double refreshIndicatorLayoutExtent = 0.0, bool hasLayoutExtent = false, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(child: child)
    {
        this.refreshIndicatorLayoutExtent = refreshIndicatorLayoutExtent;
        this.hasLayoutExtent = hasLayoutExtent;
        System.Diagnostics.Debug.Assert((refreshIndicatorLayoutExtent >= 0.0));
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderCupertinoSliverRefresh__refresh(refreshIndicatorExtent: this.refreshIndicatorLayoutExtent, hasLayoutExtent: this.hasLayoutExtent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderCupertinoSliverRefresh__refresh)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderCupertinoSliverRefresh__refresh>)(() =>
{            var __cascade = __renderObject;
            __cascade.refreshIndicatorLayoutExtent = this.refreshIndicatorLayoutExtent;
            __cascade.hasLayoutExtent = this.hasLayoutExtent;
            return __cascade;        }))());
    }

}

public class _RenderCupertinoSliverRefresh__refresh : global::Doroti.Generated.Framework.Rendering.RenderSliver, global::Doroti.Generated.Framework.Rendering.RenderObjectWithChildMixin<global::Doroti.Generated.Framework.Rendering.RenderBox>
{
    internal virtual double _refreshIndicatorExtent { get; set; } = default!;
    internal virtual bool _hasLayoutExtent { get; set; } = default!;
    public virtual double layoutExtentOffsetCompensation { get; set; } = 0.0;
    public virtual RenderBox? _child { get; set; } = default;

    internal _RenderCupertinoSliverRefresh__refresh(double refreshIndicatorExtent, bool hasLayoutExtent, global::Doroti.Generated.Framework.Rendering.RenderBox? child = null)
    {
        this._refreshIndicatorExtent = refreshIndicatorExtent;
        this._hasLayoutExtent = hasLayoutExtent;
        System.Diagnostics.Debug.Assert((refreshIndicatorExtent >= 0.0));
        ((dynamic)this).child = child;
    }

    public virtual double refreshIndicatorLayoutExtent
    {
        get => this._refreshIndicatorExtent;
        set
        {
            var __value = value;
            DartRuntimePrimitives.Assert(() => (__value >= 0.0));
            if ((__value == this._refreshIndicatorExtent))
            {
                return;
            }
            _refreshIndicatorExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual bool hasLayoutExtent
    {
        get => this._hasLayoutExtent;
        set
        {
            var __value = value;
            if ((__value == this._hasLayoutExtent))
            {
                return;
            }
            _hasLayoutExtent = __value;
            markNeedsLayout();
        }
    }
    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.SliverConstraints constraints__3818 = this.constraints;
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.down)));
        DartRuntimePrimitives.Assert(() => (object.Equals(((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).growthDirection, global::Doroti.Generated.Framework.Rendering.GrowthDirection.forward)));
        double layoutExtent__4123 = (((this._hasLayoutExtent ? 1.0 : 0.0)) * this._refreshIndicatorExtent);
        if ((layoutExtent__4123 != this.layoutExtentOffsetCompensation))
        {
            geometry = new global::Doroti.Generated.Framework.Rendering.SliverGeometry(scrollOffsetCorrection: (layoutExtent__4123 - this.layoutExtentOffsetCompensation));
            layoutExtentOffsetCompensation = layoutExtent__4123;
            return;
        }
        bool active__4956 = ((((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).overlap < 0.0) || (layoutExtent__4123 > 0.0));
        double overscrolledExtent__5031 = ((((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).overlap < 0.0) ? ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).overlap.abs() : 0.0);
        this.child!.layout(constraints__3818.asBoxConstraints(maxExtent: (layoutExtent__4123 + overscrolledExtent__5031)), parentUsesSize: true);
        if (active__4956)
        {
            geometry = new global::Doroti.Generated.Framework.Rendering.SliverGeometry(scrollExtent: layoutExtent__4123, paintOrigin: (-overscrolledExtent__5031 - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).scrollOffset), paintExtent: Math.Max((Math.Max(this.child!.size.height, layoutExtent__4123) - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).scrollOffset), 0.0), maxPaintExtent: Math.Max((Math.Max(this.child!.size.height, layoutExtent__4123) - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).scrollOffset), 0.0), layoutExtent: Math.Max((layoutExtent__4123 - ((global::Doroti.Generated.Framework.Rendering.SliverConstraints)constraints__3818).scrollOffset), 0.0));
        }
        else
        {
            geometry = global::Doroti.Generated.Framework.Rendering.SliverGeometry.zero;
        }
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (((((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).overlap < 0.0) || ((((global::Doroti.Generated.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset + this.child!.size.height) > 0L)))
        {
            context.paintChild(this.child!, offset);
        }
    }

    public override void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
    }

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((child is not RenderBox))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"A {this.GetType()} expected a child of type {typeof(RenderBox)} but received a " + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription("RenderObjects expect specific types of children because they " + "coordinate with their children during layout and paint. For " + "example, a RenderSliver cannot be the child of a RenderBox because " + "a RenderSliver does not understand the RenderBox layout protocol."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {this.GetType()} that expected a {typeof(RenderBox)} child was created by", this.debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer(), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<object?>($"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type " + "was created by", ((dynamic)child).debugCreator, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
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
        ((dynamic)this._child)?.attach(owner);
    }

    public override void detach()
    {
        base.detach();
        ((dynamic)this._child)?.detach();
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

    public override List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        return ((this.child is not null) ? new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { ((Diagnosticable)this.child!).toDiagnosticsNode(name: "child") } : new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum RefreshIndicatorMode
{
    inactive,
    drag,
    armed,
    refresh,
    done
}

public delegate global::Doroti.Generated.Framework.Widgets.Widget RefreshControlIndicatorBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, RefreshIndicatorMode refreshState, double pulledExtent, double refreshTriggerPullDistance, double refreshIndicatorExtent);

public delegate Future RefreshCallback();

public class CupertinoSliverRefreshControl : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual double refreshTriggerPullDistance { get; private set; } = default!;
    public virtual double refreshIndicatorExtent { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, RefreshIndicatorMode, double, double, double, global::Doroti.Generated.Framework.Widgets.Widget>? builder { get; private set; }
    public virtual global::System.Func<Future>? onRefresh { get; private set; }
    internal const double _defaultRefreshTriggerPullDistance = 100.0;
    internal const double _defaultRefreshIndicatorExtent = 60.0;

    public CupertinoSliverRefreshControl(global::Doroti.Generated.Framework.Foundation.Key? key = null, double? refreshTriggerPullDistance = null, double? refreshIndicatorExtent = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, RefreshIndicatorMode, double, double, double, global::Doroti.Generated.Framework.Widgets.Widget>? builder = default!, global::System.Func<Future>? onRefresh = null) : base(key: key)
    {
        double __refreshTriggerPullDistance = refreshTriggerPullDistance ?? _defaultRefreshTriggerPullDistance;
        double __refreshIndicatorExtent = refreshIndicatorExtent ?? _defaultRefreshIndicatorExtent;
        global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, RefreshIndicatorMode, double, double, double, global::Doroti.Generated.Framework.Widgets.Widget>? __builder = builder ?? buildRefreshIndicator;
        this.refreshTriggerPullDistance = __refreshTriggerPullDistance;
        this.refreshIndicatorExtent = __refreshIndicatorExtent;
        this.builder = __builder;
        this.onRefresh = onRefresh;
        System.Diagnostics.Debug.Assert((__refreshTriggerPullDistance > 0.0));
        System.Diagnostics.Debug.Assert((__refreshIndicatorExtent >= 0.0));
        System.Diagnostics.Debug.Assert((__refreshTriggerPullDistance >= __refreshIndicatorExtent));
    }

    public static RefreshIndicatorMode state(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        _CupertinoSliverRefreshControlState__refresh state__15140 = context.findAncestorStateOfType<_CupertinoSliverRefreshControlState__refresh>()!;
        return ((_CupertinoSliverRefreshControlState__refresh)state__15140).refreshState;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.Widget buildRefreshIndicator(global::Doroti.Generated.Framework.Widgets.BuildContext context, RefreshIndicatorMode refreshState, double pulledExtent, double refreshTriggerPullDistance, double refreshIndicatorExtent)
    {
        double percentageComplete__16035 = Dart_uiLibrary.clampDouble((pulledExtent / refreshTriggerPullDistance), 0.0, 1.0);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Center(child: new global::Doroti.Generated.Framework.Widgets.Stack(clipBehavior: Clip.none, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(top: RefreshLibrary._kActivityIndicatorMargin, left: 0.0, right: 0.0, child: CupertinoSliverRefreshControl._buildIndicatorForRefreshState(refreshState, RefreshLibrary._kActivityIndicatorRadius, percentageComplete__16035))) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Widgets.Widget _buildIndicatorForRefreshState(RefreshIndicatorMode refreshState, double radius, double percentageComplete)
    {
        switch (refreshState)
        {
            case RefreshIndicatorMode.drag:
                {
                    global::Doroti.Generated.Framework.Animation.Curve opacityCurve__17620 = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0.0, 0.35, curve: global::Doroti.Generated.Framework.Animation.Curves.easeInOut));
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Opacity(opacity: opacityCurve__17620.transform(percentageComplete), child: CupertinoActivityIndicator.CreatePartiallyRevealed(radius: radius, progress: percentageComplete)));
                }
            case RefreshIndicatorMode.armed:
            case RefreshIndicatorMode.refresh:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoActivityIndicator(radius: radius));
                }
            case RefreshIndicatorMode.done:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoActivityIndicator(radius: (radius * percentageComplete)));
                }
            case RefreshIndicatorMode.inactive:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSliverRefreshControlState__refresh());
}

internal class _CupertinoSliverRefreshControlState__refresh : global::Doroti.Generated.Framework.Widgets.State<CupertinoSliverRefreshControl>
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
        RefreshIndicatorMode nextState__19800 = default!;
        void goToDone()
        {
            nextState__19800 = RefreshIndicatorMode.done;
            if ((object.Equals(global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.schedulerPhase, global::Doroti.Generated.Framework.Scheduler.SchedulerPhase.idle)))
            {
                setState(((global::System.Action)(() => { _ = hasSliverLayoutExtent = false; })));
            }
            else
            {
                global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
setState(((global::System.Action)(() => { _ = hasSliverLayoutExtent = false; })));
})), debugLabel: "Refresh.goToDone");
            }
        }
        switch (this.refreshState)
        {
            case RefreshIndicatorMode.inactive:
                {
                    if ((this.latestIndicatorBoxExtent <= 0L))
                    {
                        return RefreshIndicatorMode.inactive;
                    }
                    else
                    {
                        nextState__19800 = RefreshIndicatorMode.drag;
                    }
                    goto case RefreshIndicatorMode.drag;
                }
            case RefreshIndicatorMode.drag:
                {
                    if ((this.latestIndicatorBoxExtent == 0L))
                    {
                        return RefreshIndicatorMode.inactive;
                    }
                    else
                    {
                        if ((this.latestIndicatorBoxExtent < ((CupertinoSliverRefreshControl)this.widget).refreshTriggerPullDistance))
                        {
                            return RefreshIndicatorMode.drag;
                        }
                        else
                        {
                            if ((((CupertinoSliverRefreshControl)this.widget).onRefresh is not null))
                            {
                                DartRuntimePrimitives.Ignore(HapticFeedback.mediumImpact());
                                global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback(((global::System.Action<Duration>)((timestamp) => {
DartRuntimePrimitives.Ignore(refreshTask = ((Func<Future>)(() =>
{            var __cascade = ((CupertinoSliverRefreshControl)this.widget).onRefresh!();
            __cascade.whenComplete((() => {
if (this.mounted)
{
    setState(((global::System.Action)(() => { _ = refreshTask = null; })));
    refreshState = transitionNextState();
}
}));
            return __cascade;        }))());
setState(((global::System.Action)(() => { _ = hasSliverLayoutExtent = true; })));
})), debugLabel: "Refresh.transition");
                            }
                            return RefreshIndicatorMode.armed;
                        }
                    }
                    break;
                }
            case RefreshIndicatorMode.armed:
                {
                    if (((object.Equals(this.refreshState, RefreshIndicatorMode.armed)) && (this.refreshTask is null)))
                    {
                        goToDone();
                        goto case RefreshIndicatorMode.done;
                    }
                    if ((this.latestIndicatorBoxExtent > ((CupertinoSliverRefreshControl)this.widget).refreshIndicatorExtent))
                    {
                        return RefreshIndicatorMode.armed;
                    }
                    else
                    {
                        nextState__19800 = RefreshIndicatorMode.refresh;
                    }
                    goto case RefreshIndicatorMode.refresh;
                }
            case RefreshIndicatorMode.refresh:
                {
                    if ((this.refreshTask is not null))
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
                    if ((this.latestIndicatorBoxExtent > (((CupertinoSliverRefreshControl)this.widget).refreshTriggerPullDistance * _inactiveResetOverscrollFraction)))
                    {
                        return RefreshIndicatorMode.done;
                    }
                    else
                    {
                        nextState__19800 = RefreshIndicatorMode.inactive;
                    }
                    break;
                }
        }
        return nextState__19800;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _CupertinoSliverRefresh__refresh(refreshIndicatorLayoutExtent: ((CupertinoSliverRefreshControl)this.widget).refreshIndicatorExtent, hasLayoutExtent: this.hasSliverLayoutExtent, child: new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
latestIndicatorBoxExtent = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight;
refreshState = transitionNextState();
if (((((CupertinoSliverRefreshControl)this.widget).builder is not null) && (this.latestIndicatorBoxExtent > 0L)))
{
    return ((CupertinoSliverRefreshControl)this.widget).builder!(context, this.refreshState, this.latestIndicatorBoxExtent, ((CupertinoSliverRefreshControl)this.widget).refreshTriggerPullDistance, ((CupertinoSliverRefreshControl)this.widget).refreshIndicatorExtent);
}
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LimitedBox(maxWidth: 0.0, maxHeight: 0.0, child: global::Doroti.Generated.Framework.Widgets.SizedBox.CreateExpand()));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
