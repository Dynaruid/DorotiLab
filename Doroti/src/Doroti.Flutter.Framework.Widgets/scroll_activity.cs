// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/scroll_activity.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public interface ScrollActivityDelegate
{
    public global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection { get; }
    public double setPixels(double pixels);
    public void applyUserOffset(double delta);
    public void goIdle();
    public void goBallistic(double velocity);
}

public abstract class ScrollActivity
{
    internal virtual ScrollActivityDelegate _delegate { get; set; } = default!;
    internal virtual bool _isDisposed { get; set; } = false;

    protected ScrollActivity(ScrollActivityDelegate _delegate)
    {
        this._delegate = _delegate;
    }

    public virtual ScrollActivityDelegate @delegate => this._delegate;
    public virtual void updateDelegate(ScrollActivityDelegate value)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._delegate, value)));
        _delegate = value;
    }

    public virtual void resetActivity()
    {
    }

    public virtual void dispatchScrollStartNotification(ScrollMetrics metrics, BuildContext? context)
    {
        new ScrollStartNotification(metrics: metrics, context: context).dispatch(context);
    }

    public virtual void dispatchScrollUpdateNotification(ScrollMetrics metrics, BuildContext context, double scrollDelta)
    {
        new ScrollUpdateNotification(metrics: metrics, context: context, scrollDelta: scrollDelta).dispatch(context);
    }

    public virtual void dispatchOverscrollNotification(ScrollMetrics metrics, BuildContext context, double overscroll)
    {
        new OverscrollNotification(metrics: metrics, context: context, overscroll: overscroll).dispatch(context);
    }

    public virtual void dispatchScrollEndNotification(ScrollMetrics metrics, BuildContext context)
    {
        new ScrollEndNotification(metrics: metrics, context: context).dispatch(context);
    }

    public virtual void applyNewDimensions()
    {
    }

    public abstract bool shouldIgnorePointer { get; }
    public abstract bool isScrolling { get; }
    public abstract double velocity { get; }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _isDisposed = true;
    }

    public override string ToString() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
}

public class IdleScrollActivity : ScrollActivity
{
    public IdleScrollActivity(ScrollActivityDelegate @delegate) : base(@delegate)
    {
    }

    public override void applyNewDimensions()
    {
        this.@delegate.goBallistic(0.0);
    }

    public override bool shouldIgnorePointer => false;
    public override bool isScrolling => false;
    public override double velocity => 0.0;
}

public interface ScrollHoldController
{
    public void cancel();
}

public class HoldScrollActivity : ScrollActivity, ScrollHoldController
{
    public virtual global::System.Action? onHoldCanceled { get; private set; }

    public HoldScrollActivity(ScrollActivityDelegate @delegate, global::System.Action? onHoldCanceled = null) : base(@delegate)
    {
        this.onHoldCanceled = onHoldCanceled;
    }

    public override bool shouldIgnorePointer => false;
    public override bool isScrolling => false;
    public override double velocity => 0.0;
    public virtual void cancel()
    {
        this.@delegate.goBallistic(0.0);
    }

    public override void dispose()
    {
        this.onHoldCanceled?.Invoke();
        base.dispose();
    }

}

public class ScrollDragController : global::Doroti.Generated.Framework.Gestures.Drag
{
    internal virtual ScrollActivityDelegate _delegate { get; set; } = default!;
    public virtual global::System.Action? onDragCanceled { get; private set; }
    public virtual double? carriedVelocity { get; private set; }
    public virtual double? motionStartDistanceThreshold { get; private set; }
    internal virtual Duration? _lastNonStationaryTimestamp { get; set; } = default;
    internal virtual bool _retainMomentum { get; set; } = default!;
    internal virtual double? _offsetSinceLastStop { get; set; } = default;
    public static Duration momentumRetainStationaryDurationThreshold = Duration.Create(milliseconds: 20L);
    public const double momentumRetainVelocityThresholdFactor = 0.5;
    public static Duration motionStoppedDurationThreshold = Duration.Create(milliseconds: 50L);
    internal const double _bigThresholdBreakDistance = 24.0;
    internal virtual PointerDeviceKind? _kind { get; private set; }
    internal virtual dynamic _lastDetails { get; set; } = default!;

    public ScrollDragController(ScrollActivityDelegate @delegate, global::Doroti.Generated.Framework.Gestures.DragStartDetails details, global::System.Action? onDragCanceled = null, double? carriedVelocity = null, double? motionStartDistanceThreshold = null)
    {
        this.onDragCanceled = onDragCanceled;
        this.carriedVelocity = carriedVelocity;
        this.motionStartDistanceThreshold = motionStartDistanceThreshold;
        this._delegate = @delegate;
        this._lastDetails = details;
        this._retainMomentum = ((carriedVelocity is not null) && (DartRuntimePrimitives.RequireValue(carriedVelocity) != 0.0));
        this._lastNonStationaryTimestamp = ((global::Doroti.Generated.Framework.Gestures.DragStartDetails)details).sourceTimeStamp;
        this._kind = ((global::Doroti.Generated.Framework.Gestures.DragStartDetails)details).kind;
        this._offsetSinceLastStop = ((motionStartDistanceThreshold is null) ? null : 0.0);
        System.Diagnostics.Debug.Assert(((motionStartDistanceThreshold is null) || (DartRuntimePrimitives.RequireValue(motionStartDistanceThreshold) > 0.0)));
    }

    public virtual ScrollActivityDelegate @delegate => this._delegate;
    internal virtual bool _reversed => global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionIsReversed(((ScrollActivityDelegate)this.@delegate).axisDirection);
    public virtual void updateDelegate(ScrollActivityDelegate value)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._delegate, value)));
        _delegate = value;
    }

    internal virtual void _maybeLoseMomentum(double offset, Duration? timestamp)
    {
        if (((this._retainMomentum && (offset == 0.0)) && (((timestamp is null) || ((DartRuntimePrimitives.RequireValue(timestamp) - DartRuntimePrimitives.RequireValue(this._lastNonStationaryTimestamp)) > momentumRetainStationaryDurationThreshold)))))
        {
            _retainMomentum = false;
        }
    }

    internal virtual double _adjustForScrollStartThreshold(double offset, Duration? timestamp)
    {
        if ((timestamp is null))
        {
            return offset;
        }
        if ((offset == 0.0))
        {
            if ((((this.motionStartDistanceThreshold is not null) && (this._offsetSinceLastStop is null)) && ((DartRuntimePrimitives.RequireValue(timestamp) - DartRuntimePrimitives.RequireValue(this._lastNonStationaryTimestamp)) > motionStoppedDurationThreshold)))
            {
                double motionStartDistanceThreshold__value12588 = DartRuntimePrimitives.RequireValue(motionStartDistanceThreshold);
                _offsetSinceLastStop = 0.0;
            }
            return 0.0;
        }
        else
        {
            if ((this._offsetSinceLastStop is null))
            {
                return offset;
            }
            else
            {
                _offsetSinceLastStop = (DartRuntimePrimitives.RequireValue(this._offsetSinceLastStop) + offset);
                if ((DartRuntimePrimitives.RequireValue(this._offsetSinceLastStop).abs() > DartRuntimePrimitives.RequireValue(this.motionStartDistanceThreshold)))
                {
                    _offsetSinceLastStop = null;
                    if ((offset.abs() > _bigThresholdBreakDistance))
                    {
                        return offset;
                    }
                    else
                    {
                        return (Math.Min((DartRuntimePrimitives.RequireValue(this.motionStartDistanceThreshold) / 3.0), offset.abs()) * Math.Sign(offset));
                    }
                }
                else
                {
                    return 0.0;
                }
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void update(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).primaryDelta is not null));
        _lastDetails = details;
        double offset__14109 = DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).primaryDelta);
        if ((offset__14109 != 0.0))
        {
            _lastNonStationaryTimestamp = ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp;
        }
        _maybeLoseMomentum(offset__14109, ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp);
        offset__14109 = _adjustForScrollStartThreshold(offset__14109, ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)details).sourceTimeStamp);
        if ((offset__14109 == 0.0))
        {
            return;
        }
        if (this._reversed)
        {
            offset__14109 = -offset__14109;
        }
        this.@delegate.applyUserOffset(offset__14109);
    }

    public override void end(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).primaryVelocity is not null));
        double velocity__15012 = -DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Gestures.DragEndDetails)details).primaryVelocity);
        if (this._reversed)
        {
            velocity__15012 = -velocity__15012;
        }
        _lastDetails = details;
        if (this._retainMomentum)
        {
            var isFlingingInSameDirection__15237 = (Math.Sign(velocity__15012) == Math.Sign(DartRuntimePrimitives.RequireValue(this.carriedVelocity)));
            bool isVelocityNotSubstantiallyLessThanCarriedMomentum__15448 = (velocity__15012.abs() > (DartRuntimePrimitives.RequireValue(this.carriedVelocity).abs() * momentumRetainVelocityThresholdFactor));
            if ((isFlingingInSameDirection__15237 && isVelocityNotSubstantiallyLessThanCarriedMomentum__15448))
            {
                velocity__15012 += DartRuntimePrimitives.RequireValue(this.carriedVelocity);
            }
        }
        this.@delegate.goBallistic(velocity__15012);
    }

    public override void cancel()
    {
        this.@delegate.goBallistic(0.0);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _lastDetails = null;
        this.onDragCanceled?.Invoke();
    }

    public virtual dynamic lastDetails => this._lastDetails;
    public override string ToString() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
}

public class DragScrollActivity : ScrollActivity
{
    internal virtual ScrollDragController? _controller { get; set; } = default;

    public DragScrollActivity(ScrollActivityDelegate @delegate, ScrollDragController controller) : base(@delegate)
    {
        this._controller = controller;
    }

    public override void dispatchScrollStartNotification(ScrollMetrics metrics, BuildContext? context)
    {
        dynamic lastDetails__16999 = this._controller!.lastDetails;
        DartRuntimePrimitives.Assert(() => (lastDetails__16999 is global::Doroti.Generated.Framework.Gestures.DragStartDetails));
        new ScrollStartNotification(metrics: metrics, context: context, dragDetails: ((global::Doroti.Generated.Framework.Gestures.DragStartDetails?)(object?)lastDetails__16999)!).dispatch(context);
    }

    public override void dispatchScrollUpdateNotification(ScrollMetrics metrics, BuildContext context, double scrollDelta)
    {
        dynamic lastDetails__17397 = this._controller!.lastDetails;
        DartRuntimePrimitives.Assert(() => (lastDetails__17397 is global::Doroti.Generated.Framework.Gestures.DragUpdateDetails));
        new ScrollUpdateNotification(metrics: metrics, context: context, scrollDelta: scrollDelta, dragDetails: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails?)(object?)lastDetails__17397)!).dispatch(context);
    }

    public override void dispatchOverscrollNotification(ScrollMetrics metrics, BuildContext context, double overscroll)
    {
        dynamic lastDetails__17827 = this._controller!.lastDetails;
        DartRuntimePrimitives.Assert(() => (lastDetails__17827 is global::Doroti.Generated.Framework.Gestures.DragUpdateDetails));
        new OverscrollNotification(metrics: metrics, context: context, overscroll: overscroll, dragDetails: ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails?)(object?)lastDetails__17827)!).dispatch(context);
    }

    public override void dispatchScrollEndNotification(ScrollMetrics metrics, BuildContext context)
    {
        dynamic lastDetails__18302 = this._controller!.lastDetails;
        new ScrollEndNotification(metrics: metrics, context: context, dragDetails: ((lastDetails__18302 is global::Doroti.Generated.Framework.Gestures.DragEndDetails) ? ((global::Doroti.Generated.Framework.Gestures.DragEndDetails)lastDetails__18302) : null)).dispatch(context);
    }

    public override bool shouldIgnorePointer => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this._controller?._kind, PointerDeviceKind.trackpad)));
    public override bool isScrolling => true;
    public override double velocity => 0.0;
    public override void dispose()
    {
        _controller = null;
        base.dispose();
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({this._controller})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BallisticScrollActivity : ScrollActivity
{
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _controller { get; set; } = default!;
    private bool __field_shouldIgnorePointer = default!;
    public override bool shouldIgnorePointer { get => __field_shouldIgnorePointer; }

    public BallisticScrollActivity(ScrollActivityDelegate @delegate, global::Doroti.Generated.Framework.Physics.Simulation simulation, global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync, bool shouldIgnorePointer) : base(@delegate)
    {
        this.__field_shouldIgnorePointer = shouldIgnorePointer;
    }

    public override void resetActivity()
    {
        this.@delegate.goBallistic(this.velocity);
    }

    public override void applyNewDimensions()
    {
        this.@delegate.goBallistic(this.velocity);
    }

    internal virtual void _tick()
    {
        if (!applyMoveTo(((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).value))
        {
            this.@delegate.goIdle();
        }
    }

    public virtual bool applyMoveTo(double value)
    {
        return (this.@delegate.setPixels(value).abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _end()
    {
        if (!this._isDisposed)
        {
            this.@delegate.goBallistic(0.0);
        }
    }

    public override void dispatchOverscrollNotification(ScrollMetrics metrics, BuildContext context, double overscroll)
    {
        new OverscrollNotification(metrics: metrics, context: context, overscroll: overscroll, velocity: this.velocity).dispatch(context);
    }

    public override bool isScrolling => true;
    public override double velocity => ((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).velocity;
    public override void dispose()
    {
        this._controller.dispose();
        base.dispose();
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({this._controller})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DrivenScrollActivity : ScrollActivity
{
    internal virtual Completer<object?> _completer { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _controller { get; private set; } = default!;

    public DrivenScrollActivity(ScrollActivityDelegate @delegate, double from, double to, Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve, global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync) : base(@delegate)
    {
        System.Diagnostics.Debug.Assert((duration > Duration.zero));
    }

    public static DrivenScrollActivity CreateSimulation(ScrollActivityDelegate @delegate, global::Doroti.Generated.Framework.Physics.Simulation simulation, global::Doroti.Generated.Framework.Scheduler.TickerProvider vsync)
    {
        var __instance = new DrivenScrollActivity(default!, default!, default!, default!, default!, default!);
        __instance._completer = new Completer<object?>();
        __instance._controller = ((Func<global::Doroti.Generated.Framework.Animation.AnimationController>)(() =>
{            var __cascade = global::Doroti.Generated.Framework.Animation.AnimationController.CreateUnbounded(debugLabel: global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(__instance, "DrivenScrollActivity"), vsync: vsync);
            __cascade.addListener(() => __instance._tick());
            __cascade.animateWith(simulation).whenComplete(() => { ((Action)__instance._end)(); return default!; });
            return __cascade;        }))();
        return __instance;
    }

    public virtual Future done => this._completer.future;
    internal virtual void _tick()
    {
        if (!applyMoveTo(((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).value))
        {
            this.@delegate.goIdle();
        }
    }

    public virtual bool applyMoveTo(double value)
    {
        return (this.@delegate.setPixels(value).abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _end()
    {
        if (!this._isDisposed)
        {
            this.@delegate.goBallistic(this.velocity);
        }
    }

    public override void dispatchOverscrollNotification(ScrollMetrics metrics, BuildContext context, double overscroll)
    {
        new OverscrollNotification(metrics: metrics, context: context, overscroll: overscroll, velocity: this.velocity).dispatch(context);
    }

    public override bool shouldIgnorePointer => true;
    public override bool isScrolling => true;
    public override double velocity => ((global::Doroti.Generated.Framework.Animation.AnimationController)this._controller).velocity;
    public override void dispose()
    {
        this._completer.complete();
        this._controller.dispose();
        base.dispose();
    }

    public override string ToString()
    {
        return $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({this._controller})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

