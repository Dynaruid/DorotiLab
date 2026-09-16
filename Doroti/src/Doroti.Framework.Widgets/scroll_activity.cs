// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_activity.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public interface ScrollActivityDelegate
{
    public AxisDirection axisDirection { get; }
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

    public virtual ScrollActivityDelegate @delegate => _delegate;
    public virtual void updateDelegate(ScrollActivityDelegate value)
    {
        DartRuntimePrimitives.Assert(() => !Equals(_delegate, value));
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
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _isDisposed = true;
    }

    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

public class IdleScrollActivity : ScrollActivity
{
    public IdleScrollActivity(ScrollActivityDelegate @delegate) : base(@delegate)
    {
    }

    public override void applyNewDimensions()
    {
        @delegate.goBallistic(0.0);
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
    public virtual Action? onHoldCanceled { get; private set; }

    public HoldScrollActivity(ScrollActivityDelegate @delegate, Action? onHoldCanceled = null) : base(@delegate)
    {
        this.onHoldCanceled = onHoldCanceled;
    }

    public override bool shouldIgnorePointer => false;
    public override bool isScrolling => false;
    public override double velocity => 0.0;
    public virtual void cancel()
    {
        @delegate.goBallistic(0.0);
    }

    public override void dispose()
    {
        onHoldCanceled?.Invoke();
        base.dispose();
    }

}

public class ScrollDragController : Drag
{
    internal virtual ScrollActivityDelegate _delegate { get; set; } = default!;
    public virtual Action? onDragCanceled { get; private set; }
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
    internal virtual object? _lastDetails { get; set; } = default!;

    public ScrollDragController(ScrollActivityDelegate @delegate, DragStartDetails details, Action? onDragCanceled = null, double? carriedVelocity = null, double? motionStartDistanceThreshold = null)
    {
        this.onDragCanceled = onDragCanceled;
        this.carriedVelocity = carriedVelocity;
        this.motionStartDistanceThreshold = motionStartDistanceThreshold;
        _delegate = @delegate;
        _lastDetails = details;
        _retainMomentum = (carriedVelocity is not null) && (DartRuntimePrimitives.RequireValue(carriedVelocity) != 0.0);
        _lastNonStationaryTimestamp = details.sourceTimeStamp;
        _kind = details.kind;
        _offsetSinceLastStop = (motionStartDistanceThreshold is null) ? null : 0.0;
        System.Diagnostics.Debug.Assert((motionStartDistanceThreshold is null) || (DartRuntimePrimitives.RequireValue(motionStartDistanceThreshold) > 0.0));
    }

    public virtual ScrollActivityDelegate @delegate => _delegate;
    internal virtual bool _reversed => Basic_typesLibrary.axisDirectionIsReversed(@delegate.axisDirection);
    public virtual void updateDelegate(ScrollActivityDelegate value)
    {
        DartRuntimePrimitives.Assert(() => !Equals(_delegate, value));
        _delegate = value;
    }

    internal virtual void _maybeLoseMomentum(double offset, Duration? timestamp)
    {
        if (_retainMomentum && (offset == 0.0) && ((timestamp is null) || ((DartRuntimePrimitives.RequireValue(timestamp) - DartRuntimePrimitives.RequireValue(_lastNonStationaryTimestamp)) > momentumRetainStationaryDurationThreshold)))
        {
            _retainMomentum = false;
        }
    }

    internal virtual double _adjustForScrollStartThreshold(double offset, Duration? timestamp)
    {
        if (timestamp is null)
        {
            return offset;
        }
        if (offset == 0.0)
        {
            if ((motionStartDistanceThreshold is not null) && (_offsetSinceLastStop is null) && ((DartRuntimePrimitives.RequireValue(timestamp) - DartRuntimePrimitives.RequireValue(_lastNonStationaryTimestamp)) > motionStoppedDurationThreshold))
            {
                double motionStartDistanceThreshold__value12588 = DartRuntimePrimitives.RequireValue(motionStartDistanceThreshold);
                _offsetSinceLastStop = 0.0;
            }
            return 0.0;
        }
        else
        {
            if (_offsetSinceLastStop is null)
            {
                return offset;
            }
            else
            {
                _offsetSinceLastStop = DartRuntimePrimitives.RequireValue(_offsetSinceLastStop) + offset;
                if (DartRuntimePrimitives.RequireValue(_offsetSinceLastStop).abs() > DartRuntimePrimitives.RequireValue(motionStartDistanceThreshold))
                {
                    _offsetSinceLastStop = null;
                    if (offset.abs() > _bigThresholdBreakDistance)
                    {
                        return offset;
                    }
                    else
                    {
                        return Math.Min(DartRuntimePrimitives.RequireValue(motionStartDistanceThreshold) / 3.0, offset.abs()) * Math.Sign(offset);
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

    public override void update(DragUpdateDetails details)
    {
        DartRuntimePrimitives.Assert(() => details.primaryDelta is not null);
        _lastDetails = details;
        double offset = DartRuntimePrimitives.RequireValue(details.primaryDelta);
        if (offset != 0.0)
        {
            _lastNonStationaryTimestamp = details.sourceTimeStamp;
        }
        _maybeLoseMomentum(offset, details.sourceTimeStamp);
        offset = _adjustForScrollStartThreshold(offset, details.sourceTimeStamp);
        if (offset == 0.0)
        {
            return;
        }
        if (_reversed)
        {
            offset = -offset;
        }
        @delegate.applyUserOffset(offset);
    }

    public override void end(DragEndDetails details)
    {
        DartRuntimePrimitives.Assert(() => details.primaryVelocity is not null);
        double velocity = -DartRuntimePrimitives.RequireValue(details.primaryVelocity);
        if (_reversed)
        {
            velocity = -velocity;
        }
        _lastDetails = details;
        if (_retainMomentum)
        {
            var isFlingingInSameDirection = Math.Sign(velocity) == Math.Sign(DartRuntimePrimitives.RequireValue(carriedVelocity));
            bool isVelocityNotSubstantiallyLessThanCarriedMomentum = velocity.abs() > (DartRuntimePrimitives.RequireValue(carriedVelocity).abs() * momentumRetainVelocityThresholdFactor);
            if (isFlingingInSameDirection && isVelocityNotSubstantiallyLessThanCarriedMomentum)
            {
                velocity += DartRuntimePrimitives.RequireValue(carriedVelocity);
            }
        }
        @delegate.goBallistic(velocity);
    }

    public override void cancel()
    {
        @delegate.goBallistic(0.0);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        _lastDetails = null;
        onDragCanceled?.Invoke();
    }

    public virtual object? lastDetails => _lastDetails;
    public override string ToString() => DiagnosticsLibrary.describeIdentity(this);
}

public class DragScrollActivity : ScrollActivity
{
    internal virtual ScrollDragController? _controller { get; set; } = default;

    public DragScrollActivity(ScrollActivityDelegate @delegate, ScrollDragController controller) : base(@delegate)
    {
        _controller = controller;
    }

    public override void dispatchScrollStartNotification(ScrollMetrics metrics, BuildContext? context)
    {
        object? lastDetailsLocal = _controller!.lastDetails;
        DartRuntimePrimitives.Assert(() => lastDetailsLocal is DragStartDetails);
        new ScrollStartNotification(metrics: metrics, context: context, dragDetails: ((DragStartDetails?)lastDetailsLocal)!).dispatch(context);
    }

    public override void dispatchScrollUpdateNotification(ScrollMetrics metrics, BuildContext context, double scrollDelta)
    {
        object? lastDetailsLocal = _controller!.lastDetails;
        DartRuntimePrimitives.Assert(() => lastDetailsLocal is DragUpdateDetails);
        new ScrollUpdateNotification(metrics: metrics, context: context, scrollDelta: scrollDelta, dragDetails: ((DragUpdateDetails?)lastDetailsLocal)!).dispatch(context);
    }

    public override void dispatchOverscrollNotification(ScrollMetrics metrics, BuildContext context, double overscroll)
    {
        object? lastDetailsLocal = _controller!.lastDetails;
        DartRuntimePrimitives.Assert(() => lastDetailsLocal is DragUpdateDetails);
        new OverscrollNotification(metrics: metrics, context: context, overscroll: overscroll, dragDetails: ((DragUpdateDetails?)lastDetailsLocal)!).dispatch(context);
    }

    public override void dispatchScrollEndNotification(ScrollMetrics metrics, BuildContext context)
    {
        object? lastDetailsLocal = _controller!.lastDetails;
        new ScrollEndNotification(metrics: metrics, context: context, dragDetails: (lastDetailsLocal is DragEndDetails) ? ((DragEndDetails)lastDetailsLocal) : null).dispatch(context);
    }

    public override bool shouldIgnorePointer => DartRuntimePrimitives.ConvertValue<bool>(!Equals(_controller?._kind, PointerDeviceKind.trackpad));
    public override bool isScrolling => true;
    public override double velocity => 0.0;
    public override void dispose()
    {
        _controller = null;
        base.dispose();
    }

    public override string ToString()
    {
        return $"{DiagnosticsLibrary.describeIdentity(this)}({_controller})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BallisticScrollActivity : ScrollActivity
{
    internal virtual AnimationController _controller { get; set; } = default!;
    private bool __field_shouldIgnorePointer = default!;
    public override bool shouldIgnorePointer { get => __field_shouldIgnorePointer; }

    public BallisticScrollActivity(ScrollActivityDelegate @delegate, Physics.Simulation simulation, Scheduler.TickerProvider vsync, bool shouldIgnorePointer) : base(@delegate)
    {
        __field_shouldIgnorePointer = shouldIgnorePointer;
        _controller = ((Func<AnimationController>)(() =>
        {
            var controller = AnimationController.CreateUnbounded(
                debugLabel: objectRuntimeTypeFunctions.objectRuntimeType(this, "BallisticScrollActivity"),
                vsync: vsync);
            controller.addListener(_tick);
            controller.animateWith(simulation).whenComplete(() => { _end(); return default!; });
            return controller;
        }))();
    }

    public override void resetActivity()
    {
        @delegate.goBallistic(velocity);
    }

    public override void applyNewDimensions()
    {
        @delegate.goBallistic(velocity);
    }

    internal virtual void _tick()
    {
        if (!applyMoveTo(_controller.value))
        {
            @delegate.goIdle();
        }
    }

    public virtual bool applyMoveTo(double value)
    {
        return @delegate.setPixels(value).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _end()
    {
        if (!_isDisposed)
        {
            @delegate.goBallistic(0.0);
        }
    }

    public override void dispatchOverscrollNotification(ScrollMetrics metrics, BuildContext context, double overscroll)
    {
        new OverscrollNotification(metrics: metrics, context: context, overscroll: overscroll, velocity: velocity).dispatch(context);
    }

    public override bool isScrolling => true;
    public override double velocity => _controller.velocity;
    public override void dispose()
    {
        _controller.dispose();
        base.dispose();
    }

    public override string ToString()
    {
        return $"{DiagnosticsLibrary.describeIdentity(this)}({_controller})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DrivenScrollActivity : ScrollActivity
{
    internal virtual Completer<object?> _completer { get; private set; } = default!;
    internal virtual AnimationController _controller { get; private set; } = default!;

    public DrivenScrollActivity(ScrollActivityDelegate @delegate, double from, double to, Duration duration, Curve curve, Scheduler.TickerProvider vsync) : base(@delegate)
    {
        System.Diagnostics.Debug.Assert(duration > Duration.zero);
        _completer = new Completer<object?>();
        _controller = AnimationController.CreateUnbounded(
            value: from,
            debugLabel: objectRuntimeTypeFunctions.objectRuntimeType(this, "DrivenScrollActivity"),
            vsync: vsync);
        _controller.addListener(_tick);
        DartRuntimePrimitives.Observe(
            _controller.animateTo(to, duration: duration, curve: curve)
                .whenComplete(() => { _end(); return default!; }),
            "DrivenScrollActivity.animateTo");
    }

    public static DrivenScrollActivity CreateSimulation(ScrollActivityDelegate @delegate, Physics.Simulation simulation, Scheduler.TickerProvider vsync)
    {
        var __instance = new DrivenScrollActivity(@delegate, default!, default!, default!, default!, vsync);
        __instance._completer = new Completer<object?>();
        __instance._controller = ((Func<AnimationController>)(() =>
{
    var __cascade = AnimationController.CreateUnbounded(debugLabel: objectRuntimeTypeFunctions.objectRuntimeType(__instance, "DrivenScrollActivity"), vsync: vsync);
    __cascade.addListener(__instance._tick);
    __cascade.animateWith(simulation).whenComplete(() => { ((Action)__instance._end)(); return default!; });
    return __cascade;
}))();
        return __instance;
    }

    public virtual Future done => _completer.future;
    internal virtual void _tick()
    {
        if (!applyMoveTo(_controller.value))
        {
            @delegate.goIdle();
        }
    }

    public virtual bool applyMoveTo(double value)
    {
        return @delegate.setPixels(value).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _end()
    {
        if (!_isDisposed)
        {
            @delegate.goBallistic(velocity);
        }
    }

    public override void dispatchOverscrollNotification(ScrollMetrics metrics, BuildContext context, double overscroll)
    {
        new OverscrollNotification(metrics: metrics, context: context, overscroll: overscroll, velocity: velocity).dispatch(context);
    }

    public override bool shouldIgnorePointer => true;
    public override bool isScrolling => true;
    public override double velocity => _controller.velocity;
    public override void dispose()
    {
        _completer.complete();
        _controller.dispose();
        base.dispose();
    }

    public override string ToString()
    {
        return $"{DiagnosticsLibrary.describeIdentity(this)}({_controller})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
