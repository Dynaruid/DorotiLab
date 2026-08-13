// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/scroll_position_with_single_context.dart
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

public class ScrollPositionWithSingleContext : ScrollPosition, ScrollActivityDelegate
{
    internal virtual double _heldPreviousVelocity { get; set; } = 0.0;
    internal virtual global::Doroti.Generated.Framework.Rendering.ScrollDirection _userScrollDirection { get; set; } = global::Doroti.Generated.Framework.Rendering.ScrollDirection.idle;
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

    public override global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection => ((ScrollContext)this.context).axisDirection;
    public override double setPixels(double newPixels)
    {
        DartRuntimePrimitives.Assert(() => this.activity!.isScrolling);
        return base.setPixels(newPixels);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        if ((other is not ScrollPositionWithSingleContext))
        {
            goIdle();
            return;
        }
        this.activity!.updateDelegate(this);
        _userScrollDirection = ((ScrollPositionWithSingleContext)((ScrollPositionWithSingleContext)other))._userScrollDirection;
        DartRuntimePrimitives.Assert(() => (this._currentDrag is null));
        if ((((ScrollPositionWithSingleContext)((ScrollPositionWithSingleContext)other))._currentDrag is not null))
        {
            _currentDrag = ((ScrollPositionWithSingleContext)((ScrollPositionWithSingleContext)other))._currentDrag;
            this._currentDrag!.updateDelegate(this);
            ((dynamic)other)._currentDrag = null;
        }
    }

    public override void applyNewDimensions()
    {
        base.applyNewDimensions();
        this.context.setCanDrag(this.physics.shouldAcceptUserOffset(this));
    }

    public override void beginActivity(ScrollActivity? newActivity)
    {
        _heldPreviousVelocity = 0.0;
        if ((newActivity is null))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(((ScrollActivity)newActivity).@delegate, this)));
        base.beginActivity(newActivity);
        this._currentDrag?.dispose();
        _currentDrag = null;
        if (!this.activity!.isScrolling)
        {
            updateUserScrollDirection(global::Doroti.Generated.Framework.Rendering.ScrollDirection.idle);
        }
    }

    public virtual void applyUserOffset(double delta)
    {
        updateUserScrollDirection(((delta > 0.0) ? global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward : global::Doroti.Generated.Framework.Rendering.ScrollDirection.reverse));
        setPixels((this.pixels - this.physics.applyPhysicsToUserOffset(this, delta)));
    }

    public virtual void goIdle()
    {
        beginActivity(new IdleScrollActivity(this));
    }

    public virtual void goBallistic(double velocity)
    {
        DartRuntimePrimitives.Assert(() => this.hasPixels);
        global::Doroti.Generated.Framework.Physics.Simulation? simulation__4988 = ((global::Doroti.Generated.Framework.Physics.Simulation?)(object?)this.physics.createBallisticSimulation(this, velocity));
        if ((simulation__4988 is not null))
        {
            beginActivity(new BallisticScrollActivity(this, simulation__4988, ((ScrollContext)this.context).vsync, this.shouldIgnorePointer));
        }
        else
        {
            goIdle();
        }
    }

    public override global::Doroti.Generated.Framework.Rendering.ScrollDirection userScrollDirection => this._userScrollDirection;
    public virtual void updateUserScrollDirection(global::Doroti.Generated.Framework.Rendering.ScrollDirection value)
    {
        if ((object.Equals(this.userScrollDirection, value)))
        {
            return;
        }
        _userScrollDirection = value;
        didUpdateScrollDirection(value);
    }

    public override Future animateTo(double to, Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve)
    {
        if (global::Doroti.Generated.Framework.Physics.UtilsLibrary.nearEqual(to, this.pixels, this.physics.toleranceFor(this).distance))
        {
            jumpTo(to);
            return Future.value();
        }
        var activity__6055 = new DrivenScrollActivity(this, from: this.pixels, to: to, duration: duration, curve: curve, vsync: ((ScrollContext)this.context).vsync);
        beginActivity(activity__6055);
        return ((DrivenScrollActivity)activity__6055).done;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void jumpTo(double pixels)
    {
        goIdle();
        if ((this.pixels != pixels))
        {
            double oldPixels__6377 = this.pixels;
            forcePixels(pixels);
            didStartScroll();
            didUpdateScrollPositionBy((this.pixels - oldPixels__6377));
            didEndScroll();
        }
        goBallistic(0.0);
    }

    public override void pointerScroll(double delta)
    {
        if ((delta == 0.0))
        {
            goBallistic(0.0);
            return;
        }
        double targetPixels__6858 = Math.Min(Math.Max((this.pixels + delta), this.minScrollExtent), this.maxScrollExtent);
        if ((targetPixels__6858 != this.pixels))
        {
            goIdle();
            updateUserScrollDirection(((-delta > 0.0) ? global::Doroti.Generated.Framework.Rendering.ScrollDirection.forward : global::Doroti.Generated.Framework.Rendering.ScrollDirection.reverse));
            double oldPixels__7130 = this.pixels;
            this.isScrollingNotifier.value = true;
            forcePixels(targetPixels__6858);
            didStartScroll();
            didUpdateScrollPositionBy((this.pixels - oldPixels__7130));
            didEndScroll();
            goBallistic(0.0);
        }
    }

    public override void jumpToWithoutSettling(double value)
    {
        goIdle();
        if ((this.pixels != value))
        {
            double oldPixels__7724 = this.pixels;
            forcePixels(value);
            didStartScroll();
            didUpdateScrollPositionBy((this.pixels - oldPixels__7724));
            didEndScroll();
        }
    }

    public override ScrollHoldController hold(global::System.Action holdCancelCallback)
    {
        double previousVelocity__7972 = this.activity!.velocity;
        var holdActivity__8021 = new HoldScrollActivity(@delegate: this, onHoldCanceled: () => holdCancelCallback());
        beginActivity(holdActivity__8021);
        _heldPreviousVelocity = previousVelocity__7972;
        return ((ScrollHoldController)(object?)holdActivity__8021);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Gestures.Drag drag(global::Doroti.Generated.Framework.Gestures.DragStartDetails details, global::System.Action dragCancelCallback)
    {
        var drag__8351 = new ScrollDragController(@delegate: this, details: details, onDragCanceled: () => dragCancelCallback(), carriedVelocity: this.physics.carriedMomentum(this._heldPreviousVelocity), motionStartDistanceThreshold: ((ScrollPhysics)this.physics).dragStartDistanceMotionThreshold);
        beginActivity(new DragScrollActivity(this, drag__8351));
        DartRuntimePrimitives.Assert(() => (this._currentDrag is null));
        _currentDrag = drag__8351;
        return ((global::Doroti.Generated.Framework.Gestures.Drag)(object?)drag__8351);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._currentDrag?.dispose();
        _currentDrag = null;
        base.dispose();
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"{DartRuntimePrimitives.RuntimeType(this.context)}");
        description.Add($"{this.physics}");
        description.Add($"{this.activity}");
        description.Add($"{this.userScrollDirection}");
    }

}
