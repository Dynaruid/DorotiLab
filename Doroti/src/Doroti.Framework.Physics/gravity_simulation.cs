// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/gravity_simulation.dart
using Doroti.Runtime;

namespace Doroti.Framework.Physics;

public class GravitySimulation : Simulation
{
    internal virtual double _x { get; private set; } = default!;
    internal virtual double _v { get; private set; } = default!;
    internal virtual double _a { get; private set; } = default!;
    internal virtual double _end { get; private set; } = default!;

    public GravitySimulation(double acceleration, double distance, double endDistance, double velocity)
    {
        this._a = acceleration;
        this._x = distance;
        this._v = velocity;
        this._end = endDistance;
        System.Diagnostics.Debug.Assert((endDistance >= 0L));
    }

    public override double x(double time) => ((this._x + (this._v * time)) + (((0.5 * this._a) * time) * time));
    public override double dx(double time) => (this._v + (time * this._a));
    public override bool isDone(double time) => (x(time).abs() >= this._end);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "GravitySimulation"))}(g: {this._a.toStringAsFixed(1L)}, x₀: {this._x.toStringAsFixed(1L)}, dx₀: {this._v.toStringAsFixed(1L)}, xₘₐₓ: ±{this._end.toStringAsFixed(1L)})";
}

