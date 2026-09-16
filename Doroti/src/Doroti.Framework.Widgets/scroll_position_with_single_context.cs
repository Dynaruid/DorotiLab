// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_position_with_single_context.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public class ScrollPositionWithSingleContext : ScrollPosition, ScrollActivityDelegate
{
    internal virtual double _heldPreviousVelocity { get; set; } = 0.0;
    internal virtual ScrollDirection _userScrollDirection { get; set; } = ScrollDirection.idle;
    internal virtual ScrollDragController? _currentDrag { get; set; } = default;

    public ScrollPositionWithSingleContext(ScrollPhysics physics, ScrollContext context, double? initialPixels = 0.0, bool keepScrollOffset = true, ScrollPosition? oldPosition = null, string? debugLabel = null) : base(physics: physics, context: context, keepScrollOffset: keepScrollOffset, oldPosition: oldPosition, debugLabel: debugLabel)
    {
        if (!hasPixels && initialPixels is not null)
        {
            correctPixels(initialPixels.Value);
        }
        if (activity is null)
        {
            goIdle();
        }
    }

    public override AxisDirection axisDirection => context.axisDirection;
    public override double setPixels(double newPixels)
    {
        DartRuntimePrimitives.Assert(() => activity!.isScrolling);
        return base.setPixels(newPixels);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        if (other is not ScrollPositionWithSingleContext)
        {
            goIdle();
            return;
        }
        activity!.updateDelegate(this);
        _userScrollDirection = ((ScrollPositionWithSingleContext)other)._userScrollDirection;
        DartRuntimePrimitives.Assert(() => _currentDrag is null);
        if (((ScrollPositionWithSingleContext)other)._currentDrag is not null)
        {
            _currentDrag = ((ScrollPositionWithSingleContext)other)._currentDrag;
            _currentDrag!.updateDelegate(this);
            ((ScrollPositionWithSingleContext)other)._currentDrag = null;
        }
    }

    public override void applyNewDimensions()
    {
        base.applyNewDimensions();
        context.setCanDrag(physics.shouldAcceptUserOffset(this));
    }

    public override void beginActivity(ScrollActivity? newActivity)
    {
        _heldPreviousVelocity = 0.0;
        if (newActivity is null)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => Equals(newActivity.@delegate, this));
        base.beginActivity(newActivity);
        _currentDrag?.dispose();
        _currentDrag = null;
        if (!activity!.isScrolling)
        {
            updateUserScrollDirection(ScrollDirection.idle);
        }
    }

    public virtual void applyUserOffset(double delta)
    {
        updateUserScrollDirection((delta > 0.0) ? ScrollDirection.forward : ScrollDirection.reverse);
        setPixels(pixels - physics.applyPhysicsToUserOffset(this, delta));
    }

    public virtual void goIdle()
    {
        beginActivity(new IdleScrollActivity(this));
    }

    public virtual void goBallistic(double velocity)
    {
        DartRuntimePrimitives.Assert(() => hasPixels);
        Physics.Simulation? simulation = physics.createBallisticSimulation(this, velocity);
        if (simulation is not null)
        {
            beginActivity(new BallisticScrollActivity(this, simulation, context.vsync, shouldIgnorePointer));
        }
        else
        {
            goIdle();
        }
    }

    public override ScrollDirection userScrollDirection => _userScrollDirection;
    public virtual void updateUserScrollDirection(ScrollDirection value)
    {
        if (Equals(userScrollDirection, value))
        {
            return;
        }
        _userScrollDirection = value;
        didUpdateScrollDirection(value);
    }

    public override Future animateTo(double to, Duration duration, Curve curve)
    {
        if (Physics.UtilsLibrary.nearEqual(to, pixels, physics.toleranceFor(this).distance))
        {
            jumpTo(to);
            return Future.value();
        }
        var activity = new DrivenScrollActivity(this, from: pixels, to: to, duration: duration, curve: curve, vsync: context.vsync);
        beginActivity(activity);
        return activity.done;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void jumpTo(double pixels)
    {
        goIdle();
        if (this.pixels != pixels)
        {
            double oldPixels = this.pixels;
            forcePixels(pixels);
            didStartScroll();
            didUpdateScrollPositionBy(this.pixels - oldPixels);
            didEndScroll();
        }
        goBallistic(0.0);
    }

    public override void pointerScroll(double delta)
    {
        if (delta == 0.0)
        {
            goBallistic(0.0);
            return;
        }
        double targetPixels = Math.Min(Math.Max(pixels + delta, minScrollExtent), maxScrollExtent);
        if (targetPixels != pixels)
        {
            goIdle();
            updateUserScrollDirection((-delta > 0.0) ? ScrollDirection.forward : ScrollDirection.reverse);
            double oldPixels = pixels;
            isScrollingNotifier.value = true;
            forcePixels(targetPixels);
            didStartScroll();
            didUpdateScrollPositionBy(pixels - oldPixels);
            didEndScroll();
            goBallistic(0.0);
        }
    }

    public override void jumpToWithoutSettling(double value)
    {
        goIdle();
        if (pixels != value)
        {
            double oldPixels = pixels;
            forcePixels(value);
            didStartScroll();
            didUpdateScrollPositionBy(pixels - oldPixels);
            didEndScroll();
        }
    }

    public override ScrollHoldController hold(Action holdCancelCallback)
    {
        double previousVelocity = activity!.velocity;
        var holdActivity = new HoldScrollActivity(@delegate: this, onHoldCanceled: () => holdCancelCallback());
        beginActivity(holdActivity);
        _heldPreviousVelocity = previousVelocity;
        return holdActivity;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Drag drag(DragStartDetails details, Action dragCancelCallback)
    {
        var dragLocal = new ScrollDragController(@delegate: this, details: details, onDragCanceled: () => dragCancelCallback(), carriedVelocity: physics.carriedMomentum(_heldPreviousVelocity), motionStartDistanceThreshold: physics.dragStartDistanceMotionThreshold);
        beginActivity(new DragScrollActivity(this, dragLocal));
        DartRuntimePrimitives.Assert(() => _currentDrag is null);
        _currentDrag = dragLocal;
        return dragLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _currentDrag?.dispose();
        _currentDrag = null;
        base.dispose();
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"{DartRuntimePrimitives.RuntimeType(context)}");
        description.Add($"{physics}");
        description.Add($"{activity}");
        description.Add($"{userScrollDirection}");
    }

}
