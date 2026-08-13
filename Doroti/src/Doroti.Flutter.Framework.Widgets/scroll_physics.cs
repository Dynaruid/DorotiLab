// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/scroll_physics.dart
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

public enum ScrollDecelerationRate
{
    normal,
    fast
}

public class ScrollPhysics
{
    public virtual ScrollPhysics? parent { get; private set; }
    internal static global::Doroti.Generated.Framework.Physics.SpringDescription _kDefaultSpring = global::Doroti.Generated.Framework.Physics.SpringDescription.CreateWithDampingRatio(mass: 0.5, stiffness: 100.0, ratio: 1.1);

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
            double maxPhysicalPixels__10857 = View.of(context).physicalSize.longestSide;
            return (velocity.abs() > maxPhysicalPixels__10857);
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

    public virtual global::Doroti.Generated.Framework.Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        return ((global::Doroti.Generated.Framework.Physics.Simulation?)(object?)this.parent?.createBallisticSimulation(position, velocity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Physics.SpringDescription spring => DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Physics.SpringDescription>((this.parent?.spring ?? _kDefaultSpring));
    public virtual global::Doroti.Generated.Framework.Physics.Tolerance tolerance
    {
        get
        {
            return ((global::Doroti.Generated.Framework.Physics.Tolerance)(object?)toleranceFor(new FixedScrollMetrics(minScrollExtent: ((double)(object)null), maxScrollExtent: ((double)(object)null), pixels: ((double)(object)null), viewportDimension: ((double)(object)null), axisDirection: global::Doroti.Generated.Framework.Painting.AxisDirection.down, devicePixelRatio: WidgetsBinding.instance.window.devicePixelRatio)));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Physics.Tolerance toleranceFor(ScrollMetrics metrics)
    {
        return (this.parent?.toleranceFor(metrics) ?? new global::Doroti.Generated.Framework.Physics.Tolerance(velocity: (1.0 / ((0.05 * ((ScrollMetrics)metrics).devicePixelRatio))), distance: (1.0 / ((ScrollMetrics)metrics).devicePixelRatio)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double minFlingDistance => DartRuntimePrimitives.ConvertValue<double>((this.parent?.minFlingDistance ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kTouchSlop));
    public virtual double minFlingVelocity => DartRuntimePrimitives.ConvertValue<double>((this.parent?.minFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity));
    public virtual double maxFlingVelocity => DartRuntimePrimitives.ConvertValue<double>((this.parent?.maxFlingVelocity ?? global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity));
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
            return global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ScrollPhysics");
        }
        return $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ScrollPhysics"))} -> {this.parent}";
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
        var maintainOverscroll__25208 = true;
        var enforceBoundary__25243 = true;
        if ((velocity != 0.0))
        {
            maintainOverscroll__25208 = false;
            enforceBoundary__25243 = false;
        }
        if ((((((ScrollMetrics)oldPosition).minScrollExtent == ((ScrollMetrics)newPosition).minScrollExtent)) && ((((ScrollMetrics)oldPosition).maxScrollExtent == ((ScrollMetrics)newPosition).maxScrollExtent))))
        {
            maintainOverscroll__25208 = false;
        }
        if ((((ScrollMetrics)oldPosition).pixels != ((ScrollMetrics)newPosition).pixels))
        {
            maintainOverscroll__25208 = false;
            if ((((double.IsFinite(((ScrollMetrics)oldPosition).minScrollExtent) && double.IsFinite(((ScrollMetrics)oldPosition).maxScrollExtent)) && double.IsFinite(((ScrollMetrics)newPosition).minScrollExtent)) && double.IsFinite(((ScrollMetrics)newPosition).maxScrollExtent)))
            {
                enforceBoundary__25243 = false;
            }
        }
        if ((((((ScrollMetrics)oldPosition).pixels < ((ScrollMetrics)oldPosition).minScrollExtent)) || ((((ScrollMetrics)oldPosition).pixels > ((ScrollMetrics)oldPosition).maxScrollExtent))))
        {
            enforceBoundary__25243 = false;
        }
        if (maintainOverscroll__25208)
        {
            if (((((ScrollMetrics)oldPosition).pixels < ((ScrollMetrics)oldPosition).minScrollExtent) && (((ScrollMetrics)newPosition).minScrollExtent > ((ScrollMetrics)oldPosition).minScrollExtent)))
            {
                double oldDelta__27527 = (((ScrollMetrics)oldPosition).minScrollExtent - ((ScrollMetrics)oldPosition).pixels);
                return (((ScrollMetrics)newPosition).minScrollExtent - oldDelta__27527);
            }
            if (((((ScrollMetrics)oldPosition).pixels > ((ScrollMetrics)oldPosition).maxScrollExtent) && (((ScrollMetrics)newPosition).maxScrollExtent < ((ScrollMetrics)oldPosition).maxScrollExtent)))
            {
                double oldDelta__27805 = (((ScrollMetrics)oldPosition).pixels - ((ScrollMetrics)oldPosition).maxScrollExtent);
                return (((ScrollMetrics)newPosition).maxScrollExtent + oldDelta__27805);
            }
        }
        double result__28014 = base.adjustPositionForNewDimensions(oldPosition: oldPosition, newPosition: newPosition, isScrolling: isScrolling, velocity: velocity);
        if (enforceBoundary__25243)
        {
            result__28014 = Dart_uiLibrary.clampDouble(result__28014, ((ScrollMetrics)newPosition).minScrollExtent, ((ScrollMetrics)newPosition).maxScrollExtent);
        }
        return result__28014;
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
        return (global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow((1L - overscrollFraction), 2L) * (this.decelerationRate switch { ScrollDecelerationRate.fast => 0.26, ScrollDecelerationRate.normal => 0.52, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
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
        double overscrollPastStart__31103 = Math.Max((((ScrollMetrics)position).minScrollExtent - ((ScrollMetrics)position).pixels), 0.0);
        double overscrollPastEnd__31201 = Math.Max((((ScrollMetrics)position).pixels - ((ScrollMetrics)position).maxScrollExtent), 0.0);
        double overscrollPast__31297 = Math.Max(overscrollPastStart__31103, overscrollPastEnd__31201);
        bool easing__31379 = ((((overscrollPastStart__31103 > 0.0) && (offset < 0.0))) || (((overscrollPastEnd__31201 > 0.0) && (offset > 0.0))));
        double friction__31504 = (easing__31379 ? frictionFactor((((overscrollPast__31297 - offset.abs())) / ((ScrollMetrics)position).viewportDimension)) : frictionFactor((overscrollPast__31297 / ((ScrollMetrics)position).viewportDimension)));
        double direction__31772 = Math.Sign(offset);
        if ((easing__31379 && (object.Equals(this.decelerationRate, ScrollDecelerationRate.fast))))
        {
            return (direction__31772 * offset.abs());
        }
        return (direction__31772 * BouncingScrollPhysics._applyFriction(overscrollPast__31297, offset.abs(), friction__31504));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static double _applyFriction(double extentOutside, double absDelta, double gamma)
    {
        DartRuntimePrimitives.Assert(() => (absDelta > 0L));
        var total__32116 = 0.0;
        if ((extentOutside > 0L))
        {
            double deltaToLimit__32177 = (extentOutside / gamma);
            if ((absDelta < deltaToLimit__32177))
            {
                return (absDelta * gamma);
            }
            total__32116 += extentOutside;
            absDelta -= deltaToLimit__32177;
        }
        return (total__32116 + absDelta);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double applyBoundaryConditions(ScrollMetrics position, double value) => 0.0;
    public override global::Doroti.Generated.Framework.Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        global::Doroti.Generated.Framework.Physics.Tolerance tolerance__32602 = ((global::Doroti.Generated.Framework.Physics.Tolerance)(object?)toleranceFor(position));
        if (((velocity.abs() >= ((global::Doroti.Generated.Framework.Physics.Tolerance)tolerance__32602).velocity) || ((ScrollMetrics)position).outOfRange))
        {
            return ((global::Doroti.Generated.Framework.Physics.Simulation?)(object?)new BouncingScrollSimulation(spring: this.spring, position: ((ScrollMetrics)position).pixels, velocity: velocity, leadingExtent: ((ScrollMetrics)position).minScrollExtent, trailingExtent: ((ScrollMetrics)position).maxScrollExtent, tolerance: tolerance__32602, constantDeceleration: (this.decelerationRate switch { ScrollDecelerationRate.fast => 1400, ScrollDecelerationRate.normal => 0, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") })));
        }
        return ((global::Doroti.Generated.Framework.Physics.Simulation)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double minFlingVelocity => DartRuntimePrimitives.ConvertValue<double>((global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity * 2.0));
    public override double carriedMomentum(double existingVelocity)
    {
        return (Math.Sign(existingVelocity) * Math.Min((0.000816 * global::Doroti.Flutter.Runtime.Dart_mathLibrary.pow(existingVelocity.abs(), 1.967).toDouble()), 40000.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? dragStartDistanceMotionThreshold => 3.5;
    public override double maxFlingVelocity => (this.decelerationRate switch { ScrollDecelerationRate.fast => (global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity * 8.0), ScrollDecelerationRate.normal => base.maxFlingVelocity, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
    public override global::Doroti.Generated.Framework.Physics.SpringDescription spring
    {
        get
        {
            switch (this.decelerationRate)
            {
                case ScrollDecelerationRate.fast:
                    {
                        return global::Doroti.Generated.Framework.Physics.SpringDescription.CreateWithDampingRatio(mass: 0.3, stiffness: 75.0, ratio: 1.3);
                    }
                case ScrollDecelerationRate.normal:
                    {
                        return base.spring;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            return default!;
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
                    throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this.GetType()}.applyBoundaryConditions() was called redundantly."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"The proposed new position, {value}, is exactly equal to the current position of the " + $"given {DartRuntimePrimitives.RuntimeType(position)}, {((ScrollMetrics)position).pixels}.\n" + "The applyBoundaryConditions method should only be called when the value is " + "going to actually change the pixels, otherwise it is redundant."), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("The physics object in question was", this, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty), new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ScrollMetrics>("The position object in question was", position, style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.errorProperty) }));
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

    public override global::Doroti.Generated.Framework.Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        global::Doroti.Generated.Framework.Physics.Tolerance tolerance__37981 = ((global::Doroti.Generated.Framework.Physics.Tolerance)(object?)toleranceFor(position));
        if (((ScrollMetrics)position).outOfRange)
        {
            double? end__38062 = default!;
            if ((((ScrollMetrics)position).pixels > ((ScrollMetrics)position).maxScrollExtent))
            {
                end__38062 = ((ScrollMetrics)position).maxScrollExtent;
            }
            if ((((ScrollMetrics)position).pixels < ((ScrollMetrics)position).minScrollExtent))
            {
                end__38062 = ((ScrollMetrics)position).minScrollExtent;
            }
            DartRuntimePrimitives.Assert(() => (end__38062 is not null));
            return ((global::Doroti.Generated.Framework.Physics.Simulation?)(object?)new global::Doroti.Generated.Framework.Physics.ScrollSpringSimulation(this.spring, ((ScrollMetrics)position).pixels, DartRuntimePrimitives.RequireValue(end__38062), Math.Min(0.0, velocity), tolerance: tolerance__37981));
        }
        if ((velocity.abs() < ((global::Doroti.Generated.Framework.Physics.Tolerance)tolerance__37981).velocity))
        {
            return ((global::Doroti.Generated.Framework.Physics.Simulation)(object)null);
        }
        if (((velocity > 0.0) && (((ScrollMetrics)position).pixels >= ((ScrollMetrics)position).maxScrollExtent)))
        {
            return ((global::Doroti.Generated.Framework.Physics.Simulation)(object)null);
        }
        if (((velocity < 0.0) && (((ScrollMetrics)position).pixels <= ((ScrollMetrics)position).minScrollExtent)))
        {
            return ((global::Doroti.Generated.Framework.Physics.Simulation)(object)null);
        }
        return ((global::Doroti.Generated.Framework.Physics.Simulation?)(object?)new ClampingScrollSimulation(position: ((ScrollMetrics)position).pixels, velocity: velocity, tolerance: tolerance__37981));
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

