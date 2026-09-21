// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_physics.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum ScrollDecelerationRate
{
    normal,
    fast,
}

public class ScrollPhysics
{
    public virtual ScrollPhysics? parent { get; private set; }
    internal static Physics.SpringDescription _kDefaultSpring =
        Physics.SpringDescription.CreateWithDampingRatio(mass: 0.5, stiffness: 100.0, ratio: 1.1);

    public ScrollPhysics(ScrollPhysics? parent = null)
    {
        this.parent = parent;
    }

    public virtual ScrollPhysics? buildParent(ScrollPhysics? ancestor) =>
        DartRuntimePrimitives.ConvertValue<ScrollPhysics>(parent?.applyTo(ancestor) ?? ancestor);

    public virtual ScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new ScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double applyPhysicsToUserOffset(ScrollMetrics position, double offset)
    {
        return parent?.applyPhysicsToUserOffset(position, offset) ?? offset;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool shouldAcceptUserOffset(ScrollMetrics position)
    {
        if (!allowUserScrolling)
        {
            return false;
        }
        if (parent is null)
        {
            return (position.pixels != 0.0)
                || (position.minScrollExtent != position.maxScrollExtent);
        }
        return parent!.shouldAcceptUserOffset(position);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual bool recommendDeferredLoading(
        double velocity,
        ScrollMetrics metrics,
        BuildContext context
    )
    {
        if (parent is null)
        {
            double maxPhysicalPixels = View.of(context).physicalSize.longestSide;
            return velocity.abs() > maxPhysicalPixels;
        }
        return parent!.recommendDeferredLoading(velocity, metrics, context);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double applyBoundaryConditions(ScrollMetrics position, double value)
    {
        return parent?.applyBoundaryConditions(position, value) ?? 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double adjustPositionForNewDimensions(
        ScrollMetrics oldPosition,
        ScrollMetrics newPosition,
        bool isScrolling,
        double velocity
    )
    {
        if (parent is null)
        {
            return newPosition.pixels;
        }
        return parent!.adjustPositionForNewDimensions(
            oldPosition: oldPosition,
            newPosition: newPosition,
            isScrolling: isScrolling,
            velocity: velocity
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Physics.Simulation? createBallisticSimulation(
        ScrollMetrics position,
        double velocity
    )
    {
        return parent?.createBallisticSimulation(position, velocity);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Physics.SpringDescription spring =>
        DartRuntimePrimitives.ConvertValue<Physics.SpringDescription>(
            parent?.spring ?? _kDefaultSpring
        );
    public virtual Physics.Tolerance tolerance
    {
        get
        {
            return toleranceFor(
                new FixedScrollMetrics(
                    minScrollExtent: null,
                    maxScrollExtent: null,
                    pixels: null,
                    viewportDimension: null,
                    axisDirection: AxisDirection.down,
                    devicePixelRatio: WidgetsBinding.instance.window.devicePixelRatio
                )
            );
        }
    }

    public virtual Physics.Tolerance toleranceFor(ScrollMetrics metrics)
    {
        return parent?.toleranceFor(metrics)
            ?? new Physics.Tolerance(
                velocity: 1.0 / (0.05 * metrics.devicePixelRatio),
                distance: 1.0 / metrics.devicePixelRatio
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double minFlingDistance =>
        DartRuntimePrimitives.ConvertValue<double>(
            parent?.minFlingDistance ?? Gestures.ConstantsLibrary.kTouchSlop
        );
    public virtual double minFlingVelocity =>
        DartRuntimePrimitives.ConvertValue<double>(
            parent?.minFlingVelocity ?? Gestures.ConstantsLibrary.kMinFlingVelocity
        );
    public virtual double maxFlingVelocity =>
        DartRuntimePrimitives.ConvertValue<double>(
            parent?.maxFlingVelocity ?? Gestures.ConstantsLibrary.kMaxFlingVelocity
        );

    public virtual double carriedMomentum(double existingVelocity)
    {
        return parent?.carriedMomentum(existingVelocity) ?? 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double? dragStartDistanceMotionThreshold =>
        parent?.dragStartDistanceMotionThreshold;
    public virtual bool allowImplicitScrolling => true;
    public virtual bool allowUserScrolling => true;

    public override string ToString()
    {
        if (parent is null)
        {
            return objectRuntimeTypeFunctions.objectRuntimeType(this, "ScrollPhysics");
        }
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ScrollPhysics")} -> {parent}";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class RangeMaintainingScrollPhysics : ScrollPhysics
{
    public RangeMaintainingScrollPhysics(ScrollPhysics? parent = null)
        : base(parent: parent) { }

    public override RangeMaintainingScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new RangeMaintainingScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double adjustPositionForNewDimensions(
        ScrollMetrics oldPosition,
        ScrollMetrics newPosition,
        bool isScrolling,
        double velocity
    )
    {
        var maintainOverscroll = true;
        var enforceBoundary = true;
        if (velocity != 0.0)
        {
            maintainOverscroll = false;
            enforceBoundary = false;
        }
        if (
            oldPosition.minScrollExtent == newPosition.minScrollExtent
            && oldPosition.maxScrollExtent == newPosition.maxScrollExtent
        )
        {
            maintainOverscroll = false;
        }
        if (oldPosition.pixels != newPosition.pixels)
        {
            maintainOverscroll = false;
            if (
                double.IsFinite(oldPosition.minScrollExtent)
                && double.IsFinite(oldPosition.maxScrollExtent)
                && double.IsFinite(newPosition.minScrollExtent)
                && double.IsFinite(newPosition.maxScrollExtent)
            )
            {
                enforceBoundary = false;
            }
        }
        if (
            oldPosition.pixels < oldPosition.minScrollExtent
            || oldPosition.pixels > oldPosition.maxScrollExtent
        )
        {
            enforceBoundary = false;
        }
        if (maintainOverscroll)
        {
            if (
                (oldPosition.pixels < oldPosition.minScrollExtent)
                && (newPosition.minScrollExtent > oldPosition.minScrollExtent)
            )
            {
                double oldDelta = oldPosition.minScrollExtent - oldPosition.pixels;
                return newPosition.minScrollExtent - oldDelta;
            }
            if (
                (oldPosition.pixels > oldPosition.maxScrollExtent)
                && (newPosition.maxScrollExtent < oldPosition.maxScrollExtent)
            )
            {
                double oldDeltaLocal = oldPosition.pixels - oldPosition.maxScrollExtent;
                return newPosition.maxScrollExtent + oldDeltaLocal;
            }
        }
        double result = base.adjustPositionForNewDimensions(
            oldPosition: oldPosition,
            newPosition: newPosition,
            isScrolling: isScrolling,
            velocity: velocity
        );
        if (enforceBoundary)
        {
            result = DorotiUiLibrary.clampDouble(
                result,
                newPosition.minScrollExtent,
                newPosition.maxScrollExtent
            );
        }
        return result;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class BouncingScrollPhysics : ScrollPhysics
{
    public virtual ScrollDecelerationRate decelerationRate { get; private set; } = default!;

    public BouncingScrollPhysics(
        ScrollDecelerationRate decelerationRate = ScrollDecelerationRate.normal,
        ScrollPhysics? parent = null
    )
        : base(parent: parent)
    {
        this.decelerationRate = decelerationRate;
    }

    public override BouncingScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new BouncingScrollPhysics(
            parent: buildParent(ancestor),
            decelerationRate: decelerationRate
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual double frictionFactor(double overscrollFraction)
    {
        return Dart_mathLibrary.pow(1L - overscrollFraction, 2L)
            * (
                decelerationRate switch
                {
                    ScrollDecelerationRate.fast => 0.26,
                    ScrollDecelerationRate.normal => 0.52,
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                }
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double applyPhysicsToUserOffset(ScrollMetrics position, double offset)
    {
        DartRuntimePrimitives.Assert(() => offset != 0.0);
        DartRuntimePrimitives.Assert(() => position.minScrollExtent <= position.maxScrollExtent);
        if (!position.outOfRange)
        {
            return offset;
        }
        double overscrollPastStart = Math.Max(position.minScrollExtent - position.pixels, 0.0);
        double overscrollPastEnd = Math.Max(position.pixels - position.maxScrollExtent, 0.0);
        double overscrollPast = Math.Max(overscrollPastStart, overscrollPastEnd);
        bool easing =
            ((overscrollPastStart > 0.0) && (offset < 0.0))
            || ((overscrollPastEnd > 0.0) && (offset > 0.0));
        double friction = easing
            ? frictionFactor((overscrollPast - offset.abs()) / position.viewportDimension)
            : frictionFactor(overscrollPast / position.viewportDimension);
        double direction = Math.Sign(offset);
        if (easing && Equals(decelerationRate, ScrollDecelerationRate.fast))
        {
            return direction * offset.abs();
        }
        return direction * _applyFriction(overscrollPast, offset.abs(), friction);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static double _applyFriction(double extentOutside, double absDelta, double gamma)
    {
        DartRuntimePrimitives.Assert(() => absDelta > 0L);
        var total = 0.0;
        if (extentOutside > 0L)
        {
            double deltaToLimit = extentOutside / gamma;
            if (absDelta < deltaToLimit)
            {
                return absDelta * gamma;
            }
            total += extentOutside;
            absDelta -= deltaToLimit;
        }
        return total + absDelta;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double applyBoundaryConditions(ScrollMetrics position, double value) => 0.0;

    public override Physics.Simulation? createBallisticSimulation(
        ScrollMetrics position,
        double velocity
    )
    {
        Physics.Tolerance toleranceLocal = toleranceFor(position);
        if ((velocity.abs() >= toleranceLocal.velocity) || position.outOfRange)
        {
            return (Physics.Simulation?)
                new BouncingScrollSimulation(
                    spring: spring,
                    position: position.pixels,
                    velocity: velocity,
                    leadingExtent: position.minScrollExtent,
                    trailingExtent: position.maxScrollExtent,
                    tolerance: toleranceLocal,
                    constantDeceleration: decelerationRate switch
                    {
                        ScrollDecelerationRate.fast => 1400,
                        ScrollDecelerationRate.normal => 0,
                        _ => throw new InvalidOperationException(
                            "Switch expression did not handle the supplied value."
                        ),
                    }
                );
        }
        return null;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double minFlingVelocity =>
        DartRuntimePrimitives.ConvertValue<double>(
            Gestures.ConstantsLibrary.kMinFlingVelocity * 2.0
        );

    public override double carriedMomentum(double existingVelocity)
    {
        return Math.Sign(existingVelocity)
            * Math.Min(
                0.000816 * Dart_mathLibrary.pow(existingVelocity.abs(), 1.967).toDouble(),
                40000.0
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? dragStartDistanceMotionThreshold => 3.5;
    public override double maxFlingVelocity =>
        decelerationRate switch
        {
            ScrollDecelerationRate.fast => Gestures.ConstantsLibrary.kMaxFlingVelocity * 8.0,
            ScrollDecelerationRate.normal => base.maxFlingVelocity,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
    public override Physics.SpringDescription spring
    {
        get
        {
            switch (decelerationRate)
            {
                case ScrollDecelerationRate.fast:
                {
                    return Physics.SpringDescription.CreateWithDampingRatio(
                        mass: 0.3,
                        stiffness: 75.0,
                        ratio: 1.3
                    );
                }
                case ScrollDecelerationRate.normal:
                {
                    return base.spring;
                }
                default:
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    );
            }
        }
    }
}

public class ClampingScrollPhysics : ScrollPhysics
{
    public ClampingScrollPhysics(ScrollPhysics? parent = null)
        : base(parent: parent) { }

    public override ClampingScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new ClampingScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double applyBoundaryConditions(ScrollMetrics position, double value)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (value == position.pixels)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                $"{GetType()}.applyBoundaryConditions() was called redundantly."
                            ),
                            new ErrorDescription(
                                $"The proposed new position, {value}, is exactly equal to the current position of the "
                                    + $"given {DartRuntimePrimitives.RuntimeType(position)}, {position.pixels}.\n"
                                    + "The applyBoundaryConditions method should only be called when the value is "
                                    + "going to actually change the pixels, otherwise it is redundant."
                            ),
                            new DiagnosticsProperty<ScrollPhysics>(
                                "The physics object in question was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                            new DiagnosticsProperty<ScrollMetrics>(
                                "The position object in question was",
                                position,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        if ((value < position.pixels) && (position.pixels <= position.minScrollExtent))
        {
            return value - position.pixels;
        }
        if ((position.maxScrollExtent <= position.pixels) && (position.pixels < value))
        {
            return value - position.pixels;
        }
        if ((value < position.minScrollExtent) && (position.minScrollExtent < position.pixels))
        {
            return value - position.minScrollExtent;
        }
        if ((position.pixels < position.maxScrollExtent) && (position.maxScrollExtent < value))
        {
            return value - position.maxScrollExtent;
        }
        return 0.0;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Physics.Simulation? createBallisticSimulation(
        ScrollMetrics position,
        double velocity
    )
    {
        Physics.Tolerance toleranceLocal = toleranceFor(position);
        if (position.outOfRange)
        {
            double? end = default!;
            if (position.pixels > position.maxScrollExtent)
            {
                end = position.maxScrollExtent;
            }
            if (position.pixels < position.minScrollExtent)
            {
                end = position.minScrollExtent;
            }
            DartRuntimePrimitives.Assert(() => end is not null);
            return (Physics.Simulation?)
                new Physics.ScrollSpringSimulation(
                    spring,
                    position.pixels,
                    (
                        end
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    ),
                    Math.Min(0.0, velocity),
                    tolerance: toleranceLocal
                );
        }
        if (velocity.abs() < toleranceLocal.velocity)
        {
            return null;
        }
        if ((velocity > 0.0) && (position.pixels >= position.maxScrollExtent))
        {
            return null;
        }
        if ((velocity < 0.0) && (position.pixels <= position.minScrollExtent))
        {
            return null;
        }
        return (Physics.Simulation?)
            new ClampingScrollSimulation(
                position: position.pixels,
                velocity: velocity,
                tolerance: toleranceLocal
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class AlwaysScrollableScrollPhysics : ScrollPhysics
{
    public AlwaysScrollableScrollPhysics(ScrollPhysics? parent = null)
        : base(parent: parent) { }

    public override AlwaysScrollableScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new AlwaysScrollableScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool shouldAcceptUserOffset(ScrollMetrics position) => true;
}

public class NeverScrollableScrollPhysics : ScrollPhysics
{
    public NeverScrollableScrollPhysics(ScrollPhysics? parent = null)
        : base(parent: parent) { }

    public override NeverScrollableScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new NeverScrollableScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool allowUserScrolling => false;
    public override bool allowImplicitScrolling => false;
}
