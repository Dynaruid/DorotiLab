// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/simulation.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Physics;

public abstract class Simulation
{
    public virtual Tolerance tolerance { get; set; } = default!;

    protected Simulation(Tolerance tolerance = default!)
    {
        Tolerance __tolerance = tolerance ?? Tolerance.defaultTolerance;
        this.tolerance = __tolerance;
    }

    public abstract double x(double time);
    public abstract double dx(double time);
    public abstract bool isDone(double time);
    public override string ToString() => global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Simulation");
}

