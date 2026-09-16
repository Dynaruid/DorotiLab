// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_simulation.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class BouncingScrollSimulation : Physics.Simulation
{
    public const double maxSpringTransferVelocity = 5000.0;
    public virtual double leadingExtent { get; private set; } = default!;
    public virtual double trailingExtent { get; private set; } = default!;
    public virtual Physics.SpringDescription spring { get; private set; } = default!;
    internal virtual Physics.FrictionSimulation _frictionSimulation { get; set; } = default!;
    internal virtual Physics.Simulation _springSimulation { get; set; } = default!;
    internal virtual double _springTime { get; set; } = default!;
    internal virtual double _timeOffset { get; set; } = 0.0;

    public BouncingScrollSimulation(double position, double velocity, double leadingExtent, double trailingExtent, Physics.SpringDescription spring, double constantDeceleration = 0, Physics.Tolerance tolerance = default!) : base(tolerance: tolerance ?? Physics.Tolerance.defaultTolerance)
    {
        this.leadingExtent = leadingExtent;
        this.trailingExtent = trailingExtent;
        this.spring = spring;
        System.Diagnostics.Debug.Assert(leadingExtent <= trailingExtent);
        if (position < leadingExtent)
        {
            _springSimulation = _underscrollSimulation(position, velocity);
            _springTime = double.NegativeInfinity;
        }
        else if (position > trailingExtent)
        {
            _springSimulation = _overscrollSimulation(position, velocity);
            _springTime = double.NegativeInfinity;
        }
        else
        {
            _frictionSimulation = new Physics.FrictionSimulation(
                0.135, position, velocity, constantDeceleration: constantDeceleration);
            var finalX = _frictionSimulation.finalX;
            if (velocity > 0.0 && finalX > trailingExtent)
            {
                _springTime = _frictionSimulation.timeAtX(trailingExtent);
                _springSimulation = _overscrollSimulation(
                    trailingExtent,
                    Math.Min(_frictionSimulation.dx(_springTime), maxSpringTransferVelocity));
                System.Diagnostics.Debug.Assert(double.IsFinite(_springTime));
            }
            else if (velocity < 0.0 && finalX < leadingExtent)
            {
                _springTime = _frictionSimulation.timeAtX(leadingExtent);
                _springSimulation = _underscrollSimulation(
                    leadingExtent,
                    Math.Min(_frictionSimulation.dx(_springTime), maxSpringTransferVelocity));
                System.Diagnostics.Debug.Assert(double.IsFinite(_springTime));
            }
            else
            {
                _springTime = double.PositiveInfinity;
            }
        }
    }

    internal virtual Physics.Simulation _underscrollSimulation(double x, double dx)
    {
        return new Physics.ScrollSpringSimulation(spring, x, leadingExtent, dx);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Physics.Simulation _overscrollSimulation(double x, double dx)
    {
        return new Physics.ScrollSpringSimulation(spring, x, trailingExtent, dx);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Physics.Simulation _simulation(double time)
    {
        Physics.Simulation simulation = default!;
        if (time > _springTime)
        {
            _timeOffset = double.IsFinite(_springTime) ? _springTime : 0.0;
            simulation = _springSimulation;
        }
        else
        {
            _timeOffset = 0.0;
            simulation = DartRuntimePrimitives.ConvertValue<Physics.Simulation>(_frictionSimulation);
        }
        return ((Func<Physics.Simulation>)(() =>
{
    var __cascade = simulation;
    __cascade.tolerance = tolerance;
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double x(double time) => _simulation(time).x(time - _timeOffset);
    public override double dx(double time) => _simulation(time).dx(time - _timeOffset);
    public override bool isDone(double time) => _simulation(time).isDone(time - _timeOffset);
    public override string ToString()
    {
        return $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "BouncingScrollSimulation")}(leadingExtent: {leadingExtent}, trailingExtent: {trailingExtent})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ClampingScrollSimulation : Physics.Simulation
{
    public virtual double position { get; private set; } = default!;
    public virtual double velocity { get; private set; } = default!;
    public virtual double friction { get; private set; } = default!;
    internal virtual double _duration { get; set; } = default!;
    internal virtual double _distance { get; set; } = default!;
    internal static double _kDecelerationRate = Dart_mathLibrary.log(0.78) / Dart_mathLibrary.log(0.9);
    internal const double _kInflexion = 0.35;
    internal static double _physicalCoeff = 9.80665 * 39.37 * 160.0 * 0.84;

    public ClampingScrollSimulation(double position, double velocity, double friction = 0.015, Physics.Tolerance tolerance = default!) : base(tolerance: tolerance ?? Physics.Tolerance.defaultTolerance)
    {
        this.position = position;
        this.velocity = velocity;
        this.friction = friction;
        _duration = _flingDuration();
        _distance = _flingDistance();
    }

    internal virtual double _flingDuration()
    {
        double referenceVelocity = friction * _physicalCoeff / _kInflexion;
        var androidDuration = (double)Dart_mathLibrary.pow(velocity.abs() / referenceVelocity, 1L / (_kDecelerationRate - 1.0));
        return _kDecelerationRate * _kInflexion * androidDuration;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _flingDistance()
    {
        double distanceLocal = velocity * _duration / _kDecelerationRate;
        DartRuntimePrimitives.Assert(() =>
            {
                double referenceVelocity = friction * _physicalCoeff / _kInflexion;
                double logVelocity = Dart_mathLibrary.log(velocity.abs() / referenceVelocity);
                double distanceAgain = friction * _physicalCoeff * Dart_mathLibrary.exp(logVelocity * _kDecelerationRate / (_kDecelerationRate - 1.0));
                return (distanceLocal.abs() - distanceAgain).abs() < tolerance.distance;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return distanceLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double x(double time)
    {
        double t = Dart_uiLibrary.clampDouble(time / _duration, 0.0, 1.0);
        return position + (_distance * (1.0 - Dart_mathLibrary.pow(1.0 - t, _kDecelerationRate)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time)
    {
        double t = Dart_uiLibrary.clampDouble(time / _duration, 0.0, 1.0);
        return velocity * Dart_mathLibrary.pow(1.0 - t, _kDecelerationRate - 1.0);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time)
    {
        return time >= _duration;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
