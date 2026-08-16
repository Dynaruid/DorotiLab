// <doroti-reviewed-framework-source />
#nullable enable
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/physics/tolerance.dart
using System;
using Doroti.Runtime;

namespace Doroti.Framework.Physics;

public class Tolerance
{
    private const double _epsilonDefault = 0.001;
    public static readonly Tolerance defaultTolerance = new Tolerance();
    public double distance { get; }
    public double time { get; }
    public double velocity { get; }

    public Tolerance(double distance = _epsilonDefault, double time = _epsilonDefault, double velocity = _epsilonDefault)
    {
        this.distance = distance;
        this.time = time;
        this.velocity = velocity;
    }

    public override string ToString() => $"{Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "Tolerance")}(distance: ±{distance}, time: ±{time}, velocity: ±{velocity})";
}
