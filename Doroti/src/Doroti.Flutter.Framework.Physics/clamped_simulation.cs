// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/clamped_simulation.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Physics;

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
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ClampedSimulation"))}(simulation: {this.simulation}, x: {this.xMin.toStringAsFixed(1L)}..{this.xMax.toStringAsFixed(1L)}, dx: {this.dxMin.toStringAsFixed(1L)}..{this.dxMax.toStringAsFixed(1L)})";
}

