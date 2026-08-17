// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/scroll_simulation.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public class BouncingScrollSimulation : global::Doroti.Framework.Physics.Simulation
{
    public const double maxSpringTransferVelocity = 5000.0;
    public virtual double leadingExtent { get; private set; } = default!;
    public virtual double trailingExtent { get; private set; } = default!;
    public virtual global::Doroti.Framework.Physics.SpringDescription spring { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Physics.FrictionSimulation _frictionSimulation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Physics.Simulation _springSimulation { get; set; } = default!;
    internal virtual double _springTime { get; set; } = default!;
    internal virtual double _timeOffset { get; set; } = 0.0;

    public BouncingScrollSimulation(double position, double velocity, double leadingExtent, double trailingExtent, global::Doroti.Framework.Physics.SpringDescription spring, double constantDeceleration = 0, global::Doroti.Framework.Physics.Tolerance tolerance = default!) : base(tolerance: tolerance ?? global::Doroti.Framework.Physics.Tolerance.defaultTolerance)
    {
        this.leadingExtent = leadingExtent;
        this.trailingExtent = trailingExtent;
        this.spring = spring;
        System.Diagnostics.Debug.Assert((leadingExtent <= trailingExtent));
        if (position < leadingExtent)
        {
            this._springSimulation = _underscrollSimulation(position, velocity);
            this._springTime = double.NegativeInfinity;
        }
        else if (position > trailingExtent)
        {
            this._springSimulation = _overscrollSimulation(position, velocity);
            this._springTime = double.NegativeInfinity;
        }
        else
        {
            this._frictionSimulation = new global::Doroti.Framework.Physics.FrictionSimulation(
                0.135, position, velocity, constantDeceleration: constantDeceleration);
            var finalX = this._frictionSimulation.finalX;
            if (velocity > 0.0 && finalX > trailingExtent)
            {
                this._springTime = this._frictionSimulation.timeAtX(trailingExtent);
                this._springSimulation = _overscrollSimulation(
                    trailingExtent,
                    Math.Min(this._frictionSimulation.dx(this._springTime), maxSpringTransferVelocity));
                System.Diagnostics.Debug.Assert(double.IsFinite(this._springTime));
            }
            else if (velocity < 0.0 && finalX < leadingExtent)
            {
                this._springTime = this._frictionSimulation.timeAtX(leadingExtent);
                this._springSimulation = _underscrollSimulation(
                    leadingExtent,
                    Math.Min(this._frictionSimulation.dx(this._springTime), maxSpringTransferVelocity));
                System.Diagnostics.Debug.Assert(double.IsFinite(this._springTime));
            }
            else
            {
                this._springTime = double.PositiveInfinity;
            }
        }
    }

    internal virtual global::Doroti.Framework.Physics.Simulation _underscrollSimulation(double x, double dx)
    {
        return ((global::Doroti.Framework.Physics.Simulation)(object?)new global::Doroti.Framework.Physics.ScrollSpringSimulation(this.spring, x, this.leadingExtent, dx));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Physics.Simulation _overscrollSimulation(double x, double dx)
    {
        return ((global::Doroti.Framework.Physics.Simulation)(object?)new global::Doroti.Framework.Physics.ScrollSpringSimulation(this.spring, x, this.trailingExtent, dx));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Physics.Simulation _simulation(double time)
    {
        global::Doroti.Framework.Physics.Simulation simulation__4144 = default!;
        if ((time > this._springTime))
        {
            _timeOffset = (double.IsFinite(this._springTime) ? this._springTime : 0.0);
            simulation__4144 = this._springSimulation;
        }
        else
        {
            _timeOffset = 0.0;
            simulation__4144 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Physics.Simulation>(this._frictionSimulation);
        }
        return ((Func<global::Doroti.Framework.Physics.Simulation>)(() =>
{            var __cascade = simulation__4144;
            __cascade.tolerance = this.tolerance;
            return __cascade;        }))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double x(double time) => _simulation(time).x((time - this._timeOffset));
    public override double dx(double time) => _simulation(time).dx((time - this._timeOffset));
    public override bool isDone(double time) => _simulation(time).isDone((time - this._timeOffset));
    public override string ToString()
    {
        return $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "BouncingScrollSimulation"))}(leadingExtent: {this.leadingExtent}, trailingExtent: {this.trailingExtent})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ClampingScrollSimulation : global::Doroti.Framework.Physics.Simulation
{
    public virtual double position { get; private set; } = default!;
    public virtual double velocity { get; private set; } = default!;
    public virtual double friction { get; private set; } = default!;
    internal virtual double _duration { get; set; } = default!;
    internal virtual double _distance { get; set; } = default!;
    internal static double _kDecelerationRate = (global::Doroti.Runtime.Dart_mathLibrary.log(0.78) / global::Doroti.Runtime.Dart_mathLibrary.log(0.9));
    internal const double _kInflexion = 0.35;
    internal static double _physicalCoeff = (((9.80665 * 39.37) * 160.0) * 0.84);

    public ClampingScrollSimulation(double position, double velocity, double friction = 0.015, global::Doroti.Framework.Physics.Tolerance tolerance = default!) : base(tolerance: tolerance ?? global::Doroti.Framework.Physics.Tolerance.defaultTolerance)
    {
        this.position = position;
        this.velocity = velocity;
        this.friction = friction;
        this._duration = _flingDuration();
        this._distance = _flingDistance();
    }

    internal virtual double _flingDuration()
    {
        double referenceVelocity__8333 = ((this.friction * _physicalCoeff) / _kInflexion);
        var androidDuration__8485 = ((double)global::Doroti.Runtime.Dart_mathLibrary.pow((this.velocity.abs() / referenceVelocity__8333), (1L / ((_kDecelerationRate - 1.0)))));
        return ((_kDecelerationRate * _kInflexion) * androidDuration__8485);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _flingDistance()
    {
        double distance__8933 = ((this.velocity * this._duration) / _kDecelerationRate);
        DartRuntimePrimitives.Assert(() =>
            {
                double referenceVelocity__9182 = ((this.friction * _physicalCoeff) / _kInflexion);
                double logVelocity__9262 = global::Doroti.Runtime.Dart_mathLibrary.log((this.velocity.abs() / referenceVelocity__9182));
                double distanceAgain__9341 = ((this.friction * _physicalCoeff) * global::Doroti.Runtime.Dart_mathLibrary.exp(((logVelocity__9262 * _kDecelerationRate) / ((_kDecelerationRate - 1.0)))));
                return (((distance__8933.abs() - distanceAgain__9341)).abs() < ((global::Doroti.Framework.Physics.Tolerance)this.tolerance).distance);
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return distance__8933;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double x(double time)
    {
        double t__9653 = Dart_uiLibrary.clampDouble((time / this._duration), 0.0, 1.0);
        return (this.position + (this._distance * ((1.0 - global::Doroti.Runtime.Dart_mathLibrary.pow((1.0 - t__9653), _kDecelerationRate)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double dx(double time)
    {
        double t__9840 = Dart_uiLibrary.clampDouble((time / this._duration), 0.0, 1.0);
        return (this.velocity * global::Doroti.Runtime.Dart_mathLibrary.pow((1.0 - t__9840), (_kDecelerationRate - 1.0)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool isDone(double time)
    {
        return (time >= this._duration);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
