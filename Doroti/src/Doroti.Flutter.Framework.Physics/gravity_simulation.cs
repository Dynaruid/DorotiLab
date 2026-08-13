// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/gravity_simulation.dart
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
    public override string ToString() => $"{(global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "GravitySimulation"))}(g: {this._a.toStringAsFixed(1L)}, x₀: {this._x.toStringAsFixed(1L)}, dx₀: {this._v.toStringAsFixed(1L)}, xₘₐₓ: ±{this._end.toStringAsFixed(1L)})";
}

