// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/clamped_simulation.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Physics;

public class ClampedSimulation : Simulation
{
    public virtual Simulation simulation { get; private set; } = default!;
    public virtual double xMin { get; private set; } = default!;
    public virtual double xMax { get; private set; } = default!;
    public virtual double dxMin { get; private set; } = default!;
    public virtual double dxMax { get; private set; } = default!;

    public ClampedSimulation(Simulation simulation, double xMin = double.NegativeInfinity, double xMax = double.PositiveInfinity, double dxMin = double.NegativeInfinity, double dxMax = double.PositiveInfinity)
    {
        this.simulation = simulation;
        this.xMin = xMin;
        this.xMax = xMax;
        this.dxMin = dxMin;
        this.dxMax = dxMax;
        System.Diagnostics.Debug.Assert((xMax >= xMin));
        System.Diagnostics.Debug.Assert((dxMax >= dxMin));
    }

    public override double x(double time) => Dart_uiLibrary.clampDouble(this.simulation.x(time), this.xMin, this.xMax);
    public override double dx(double time) => Dart_uiLibrary.clampDouble(this.simulation.dx(time), this.dxMin, this.dxMax);
    public override bool isDone(double time) => this.simulation.isDone(time);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ClampedSimulation"))}(simulation: {this.simulation}, x: {this.xMin.toStringAsFixed(1L)}..{this.xMax.toStringAsFixed(1L)}, dx: {this.dxMin.toStringAsFixed(1L)}..{this.dxMax.toStringAsFixed(1L)})";
}

