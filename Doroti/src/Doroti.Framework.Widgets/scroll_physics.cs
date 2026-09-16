// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_physics.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum ScrollDecelerationRate
{
    normal,
    fast
}

public class ScrollPhysics
{
    public virtual ScrollPhysics? parent { get; private set; }
    internal static global::Doroti.Framework.Physics.SpringDescription _kDefaultSpring = Physics.SpringDescription.CreateWithDampingRatio(mass: 0.5, stiffness: 100.0, ratio: 1.1);

    public ScrollPhysics(ScrollPhysics? parent = null)
    {
        this.parent = parent;
    }

    public virtual ScrollPhysics? buildParent(ScrollPhysics? ancestor) => DartRuntimePrimitives.ConvertValue<ScrollPhysics>((this.parent?.applyTo(ancestor) ?? ancestor));
    public virtual ScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new ScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double applyPhysicsToUserOffset(ScrollMetrics position, double offset)
    {
        return (this.parent?.applyPhysicsToUserOffset(position, offset) ?? offset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool shouldAcceptUserOffset(ScrollMetrics position)
    {
        if (!this.allowUserScrolling)
        {
            return false;
        }
        if ((this.parent is null))
        {
            return ((((ScrollMetrics)position).pixels != 0.0) || (((ScrollMetrics)position).minScrollExtent != ((ScrollMetrics)position).maxScrollExtent));
        }
        return this.parent!.shouldAcceptUserOffset(position);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool recommendDeferredLoading(double velocity, ScrollMetrics metrics, BuildContext context)
    {
        if ((this.parent is null))
        {
            double maxPhysicalPixels = View.of(context).physicalSize.longestSide;
            return (velocity.abs() > maxPhysicalPixels);
        }
        return this.parent!.recommendDeferredLoading(velocity, metrics, context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double applyBoundaryConditions(ScrollMetrics position, double value)
    {
        return (this.parent?.applyBoundaryConditions(position, value) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double adjustPositionForNewDimensions(ScrollMetrics oldPosition, ScrollMetrics newPosition, bool isScrolling, double velocity)
    {
        if ((this.parent is null))
        {
            return ((ScrollMetrics)newPosition).pixels;
        }
        return this.parent!.adjustPositionForNewDimensions(oldPosition: oldPosition, newPosition: newPosition, isScrolling: isScrolling, velocity: velocity);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        return ((global::Doroti.Framework.Physics.Simulation?)this.parent?.createBallisticSimulation(position, velocity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Physics.SpringDescription spring => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Physics.SpringDescription>((this.parent?.spring ?? _kDefaultSpring));
    public virtual global::Doroti.Framework.Physics.Tolerance tolerance
    {
        get
        {
            return ((global::Doroti.Framework.Physics.Tolerance)toleranceFor(new FixedScrollMetrics(minScrollExtent: null, maxScrollExtent: null, pixels: null, viewportDimension: null, axisDirection: AxisDirection.down, devicePixelRatio: WidgetsBinding.instance.window.devicePixelRatio)));
        }
    }
    public virtual global::Doroti.Framework.Physics.Tolerance toleranceFor(ScrollMetrics metrics)
    {
        return (this.parent?.toleranceFor(metrics) ?? new global::Doroti.Framework.Physics.Tolerance(velocity: (1.0 / ((0.05 * ((ScrollMetrics)metrics).devicePixelRatio))), distance: (1.0 / ((ScrollMetrics)metrics).devicePixelRatio)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double minFlingDistance => DartRuntimePrimitives.ConvertValue<double>((this.parent?.minFlingDistance ?? Gestures.ConstantsLibrary.kTouchSlop));
    public virtual double minFlingVelocity => DartRuntimePrimitives.ConvertValue<double>((this.parent?.minFlingVelocity ?? Gestures.ConstantsLibrary.kMinFlingVelocity));
    public virtual double maxFlingVelocity => DartRuntimePrimitives.ConvertValue<double>((this.parent?.maxFlingVelocity ?? Gestures.ConstantsLibrary.kMaxFlingVelocity));
    public virtual double carriedMomentum(double existingVelocity)
    {
        return (this.parent?.carriedMomentum(existingVelocity) ?? 0.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? dragStartDistanceMotionThreshold => this.parent?.dragStartDistanceMotionThreshold;
    public virtual bool allowImplicitScrolling => true;
    public virtual bool allowUserScrolling => true;
    public override string ToString()
    {
        if ((this.parent is null))
        {
            return objectRuntimeTypeFunctions.objectRuntimeType(this, "ScrollPhysics");
        }
        return $"{(objectRuntimeTypeFunctions.objectRuntimeType(this, "ScrollPhysics"))} -> {this.parent}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RangeMaintainingScrollPhysics : ScrollPhysics
{
    public RangeMaintainingScrollPhysics(ScrollPhysics? parent = null) : base(parent: parent)
    {
    }

    public override RangeMaintainingScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new RangeMaintainingScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double adjustPositionForNewDimensions(ScrollMetrics oldPosition, ScrollMetrics newPosition, bool isScrolling, double velocity)
    {
        var maintainOverscroll = true;
        var enforceBoundary = true;
        if ((velocity != 0.0))
        {
            maintainOverscroll = false;
            enforceBoundary = false;
        }
        if ((((((ScrollMetrics)oldPosition).minScrollExtent == ((ScrollMetrics)newPosition).minScrollExtent)) && ((((ScrollMetrics)oldPosition).maxScrollExtent == ((ScrollMetrics)newPosition).maxScrollExtent))))
        {
            maintainOverscroll = false;
        }
        if ((((ScrollMetrics)oldPosition).pixels != ((ScrollMetrics)newPosition).pixels))
        {
            maintainOverscroll = false;
            if ((((double.IsFinite(((ScrollMetrics)oldPosition).minScrollExtent) && double.IsFinite(((ScrollMetrics)oldPosition).maxScrollExtent)) && double.IsFinite(((ScrollMetrics)newPosition).minScrollExtent)) && double.IsFinite(((ScrollMetrics)newPosition).maxScrollExtent)))
            {
                enforceBoundary = false;
            }
        }
        if ((((((ScrollMetrics)oldPosition).pixels < ((ScrollMetrics)oldPosition).minScrollExtent)) || ((((ScrollMetrics)oldPosition).pixels > ((ScrollMetrics)oldPosition).maxScrollExtent))))
        {
            enforceBoundary = false;
        }
        if (maintainOverscroll)
        {
            if (((((ScrollMetrics)oldPosition).pixels < ((ScrollMetrics)oldPosition).minScrollExtent) && (((ScrollMetrics)newPosition).minScrollExtent > ((ScrollMetrics)oldPosition).minScrollExtent)))
            {
                double oldDelta = (((ScrollMetrics)oldPosition).minScrollExtent - ((ScrollMetrics)oldPosition).pixels);
                return (((ScrollMetrics)newPosition).minScrollExtent - oldDelta);
            }
            if (((((ScrollMetrics)oldPosition).pixels > ((ScrollMetrics)oldPosition).maxScrollExtent) && (((ScrollMetrics)newPosition).maxScrollExtent < ((ScrollMetrics)oldPosition).maxScrollExtent)))
            {
                double oldDeltaLocal = (((ScrollMetrics)oldPosition).pixels - ((ScrollMetrics)oldPosition).maxScrollExtent);
                return (((ScrollMetrics)newPosition).maxScrollExtent + oldDeltaLocal);
            }
        }
        double result = base.adjustPositionForNewDimensions(oldPosition: oldPosition, newPosition: newPosition, isScrolling: isScrolling, velocity: velocity);
        if (enforceBoundary)
        {
            result = Dart_uiLibrary.clampDouble(result, ((ScrollMetrics)newPosition).minScrollExtent, ((ScrollMetrics)newPosition).maxScrollExtent);
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class BouncingScrollPhysics : ScrollPhysics
{
    public virtual ScrollDecelerationRate decelerationRate { get; private set; } = default!;

    public BouncingScrollPhysics(ScrollDecelerationRate decelerationRate = ScrollDecelerationRate.normal, ScrollPhysics? parent = null) : base(parent: parent)
    {
        this.decelerationRate = decelerationRate;
    }

    public override BouncingScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new BouncingScrollPhysics(parent: buildParent(ancestor), decelerationRate: this.decelerationRate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double frictionFactor(double overscrollFraction)
    {
        return (Dart_mathLibrary.pow((1L - overscrollFraction), 2L) * (this.decelerationRate switch { ScrollDecelerationRate.fast => 0.26, ScrollDecelerationRate.normal => 0.52, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double applyPhysicsToUserOffset(ScrollMetrics position, double offset)
    {
        DartRuntimePrimitives.Assert(() => (offset != 0.0));
        DartRuntimePrimitives.Assert(() => (((ScrollMetrics)position).minScrollExtent <= ((ScrollMetrics)position).maxScrollExtent));
        if (!((ScrollMetrics)position).outOfRange)
        {
            return offset;
        }
        double overscrollPastStart = Math.Max((((ScrollMetrics)position).minScrollExtent - ((ScrollMetrics)position).pixels), 0.0);
        double overscrollPastEnd = Math.Max((((ScrollMetrics)position).pixels - ((ScrollMetrics)position).maxScrollExtent), 0.0);
        double overscrollPast = Math.Max(overscrollPastStart, overscrollPastEnd);
        bool easing = ((((overscrollPastStart > 0.0) && (offset < 0.0))) || (((overscrollPastEnd > 0.0) && (offset > 0.0))));
        double friction = (easing ? frictionFactor((((overscrollPast - offset.abs())) / ((ScrollMetrics)position).viewportDimension)) : frictionFactor((overscrollPast / ((ScrollMetrics)position).viewportDimension)));
        double direction = Math.Sign(offset);
        if ((easing && (Equals(this.decelerationRate, ScrollDecelerationRate.fast))))
        {
            return (direction * offset.abs());
        }
        return (direction * _applyFriction(overscrollPast, offset.abs(), friction));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _applyFriction(double extentOutside, double absDelta, double gamma)
    {
        DartRuntimePrimitives.Assert(() => (absDelta > 0L));
        var total = 0.0;
        if ((extentOutside > 0L))
        {
            double deltaToLimit = (extentOutside / gamma);
            if ((absDelta < deltaToLimit))
            {
                return (absDelta * gamma);
            }
            total += extentOutside;
            absDelta -= deltaToLimit;
        }
        return (total + absDelta);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double applyBoundaryConditions(ScrollMetrics position, double value) => 0.0;
    public override global::Doroti.Framework.Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        global::Doroti.Framework.Physics.Tolerance toleranceLocal = ((global::Doroti.Framework.Physics.Tolerance)toleranceFor(position));
        if (((velocity.abs() >= ((global::Doroti.Framework.Physics.Tolerance)toleranceLocal).velocity) || ((ScrollMetrics)position).outOfRange))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)new BouncingScrollSimulation(spring: this.spring, position: ((ScrollMetrics)position).pixels, velocity: velocity, leadingExtent: ((ScrollMetrics)position).minScrollExtent, trailingExtent: ((ScrollMetrics)position).maxScrollExtent, tolerance: toleranceLocal, constantDeceleration: (this.decelerationRate switch { ScrollDecelerationRate.fast => 1400, ScrollDecelerationRate.normal => 0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        }
        return ((global::Doroti.Framework.Physics.Simulation?)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double minFlingVelocity => DartRuntimePrimitives.ConvertValue<double>((Gestures.ConstantsLibrary.kMinFlingVelocity * 2.0));
    public override double carriedMomentum(double existingVelocity)
    {
        return (Math.Sign(existingVelocity) * Math.Min((0.000816 * Dart_mathLibrary.pow(existingVelocity.abs(), 1.967).toDouble()), 40000.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? dragStartDistanceMotionThreshold => 3.5;
    public override double maxFlingVelocity => (this.decelerationRate switch { ScrollDecelerationRate.fast => (Gestures.ConstantsLibrary.kMaxFlingVelocity * 8.0), ScrollDecelerationRate.normal => base.maxFlingVelocity, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override global::Doroti.Framework.Physics.SpringDescription spring
    {
        get
        {
            switch (this.decelerationRate)
            {
                case ScrollDecelerationRate.fast:
                    {
                        return Physics.SpringDescription.CreateWithDampingRatio(mass: 0.3, stiffness: 75.0, ratio: 1.3);
                    }
                case ScrollDecelerationRate.normal:
                    {
                        return base.spring;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
}

public class ClampingScrollPhysics : ScrollPhysics
{
    public ClampingScrollPhysics(ScrollPhysics? parent = null) : base(parent: parent)
    {
    }

    public override ClampingScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new ClampingScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double applyBoundaryConditions(ScrollMetrics position, double value)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if ((value == ((ScrollMetrics)position).pixels))
                {
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this.GetType()}.applyBoundaryConditions() was called redundantly."), new global::Doroti.Framework.Foundation.ErrorDescription($"The proposed new position, {value}, is exactly equal to the current position of the " + $"given {DartRuntimePrimitives.RuntimeType(position)}, {((ScrollMetrics)position).pixels}.\n" + "The applyBoundaryConditions method should only be called when the value is " + "going to actually change the pixels, otherwise it is redundant."), new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("The physics object in question was", this, style: DiagnosticsTreeStyle.errorProperty), new global::Doroti.Framework.Foundation.DiagnosticsProperty<ScrollMetrics>("The position object in question was", position, style: DiagnosticsTreeStyle.errorProperty) }));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        if (((value < ((ScrollMetrics)position).pixels) && (((ScrollMetrics)position).pixels <= ((ScrollMetrics)position).minScrollExtent)))
        {
            return (value - ((ScrollMetrics)position).pixels);
        }
        if (((((ScrollMetrics)position).maxScrollExtent <= ((ScrollMetrics)position).pixels) && (((ScrollMetrics)position).pixels < value)))
        {
            return (value - ((ScrollMetrics)position).pixels);
        }
        if (((value < ((ScrollMetrics)position).minScrollExtent) && (((ScrollMetrics)position).minScrollExtent < ((ScrollMetrics)position).pixels)))
        {
            return (value - ((ScrollMetrics)position).minScrollExtent);
        }
        if (((((ScrollMetrics)position).pixels < ((ScrollMetrics)position).maxScrollExtent) && (((ScrollMetrics)position).maxScrollExtent < value)))
        {
            return (value - ((ScrollMetrics)position).maxScrollExtent);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        global::Doroti.Framework.Physics.Tolerance toleranceLocal = ((global::Doroti.Framework.Physics.Tolerance)toleranceFor(position));
        if (((ScrollMetrics)position).outOfRange)
        {
            double? end = default!;
            if ((((ScrollMetrics)position).pixels > ((ScrollMetrics)position).maxScrollExtent))
            {
                end = ((ScrollMetrics)position).maxScrollExtent;
            }
            if ((((ScrollMetrics)position).pixels < ((ScrollMetrics)position).minScrollExtent))
            {
                end = ((ScrollMetrics)position).minScrollExtent;
            }
            DartRuntimePrimitives.Assert(() => (end is not null));
            return ((global::Doroti.Framework.Physics.Simulation?)new global::Doroti.Framework.Physics.ScrollSpringSimulation(this.spring, ((ScrollMetrics)position).pixels, DartRuntimePrimitives.RequireValue(end), Math.Min(0.0, velocity), tolerance: toleranceLocal));
        }
        if ((velocity.abs() < ((global::Doroti.Framework.Physics.Tolerance)toleranceLocal).velocity))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)null);
        }
        if (((velocity > 0.0) && (((ScrollMetrics)position).pixels >= ((ScrollMetrics)position).maxScrollExtent)))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)null);
        }
        if (((velocity < 0.0) && (((ScrollMetrics)position).pixels <= ((ScrollMetrics)position).minScrollExtent)))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)null);
        }
        return ((global::Doroti.Framework.Physics.Simulation?)new ClampingScrollSimulation(position: ((ScrollMetrics)position).pixels, velocity: velocity, tolerance: toleranceLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class AlwaysScrollableScrollPhysics : ScrollPhysics
{
    public AlwaysScrollableScrollPhysics(ScrollPhysics? parent = null) : base(parent: parent)
    {
    }

    public override AlwaysScrollableScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new AlwaysScrollableScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldAcceptUserOffset(ScrollMetrics position) => true;
}

public class NeverScrollableScrollPhysics : ScrollPhysics
{
    public NeverScrollableScrollPhysics(ScrollPhysics? parent = null) : base(parent: parent)
    {
    }

    public override NeverScrollableScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new NeverScrollableScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool allowUserScrolling => false;
    public override bool allowImplicitScrolling => false;
}

