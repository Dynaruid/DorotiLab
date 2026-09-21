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

    public GravitySimulation(
        double acceleration,
        double distance,
        double endDistance,
        double velocity
    )
    {
        _a = acceleration;
        _x = distance;
        _v = velocity;
        _end = endDistance;
        System.Diagnostics.Debug.Assert(endDistance >= 0L);
    }

    public override double x(double time) => _x + (_v * time) + (0.5 * _a * time * time);

    public override double dx(double time) => _v + (time * _a);

    public override bool isDone(double time) => x(time).abs() >= _end;

    public override string ToString() =>
        $"{Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "GravitySimulation")}(g: {_a.toStringAsFixed(1L)}, x₀: {_x.toStringAsFixed(1L)}, dx₀: {_v.toStringAsFixed(1L)}, xₘₐₓ: ±{_end.toStringAsFixed(1L)})";
}
