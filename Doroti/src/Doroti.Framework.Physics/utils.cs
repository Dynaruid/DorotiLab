// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/utils.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Physics;

public static partial class UtilsLibrary
{
    public static bool nearEqual(double? a, double? b, double epsilon)
    {
        DartRuntimePrimitives.Assert(() => (epsilon >= 0.0));
        if (((a is null) || (b is null)))
        {
            return (a == b);
        }
        return ((((DartRuntimePrimitives.RequireValue(a) > ((DartRuntimePrimitives.RequireValue(b) - epsilon)))) && ((DartRuntimePrimitives.RequireValue(a) < ((DartRuntimePrimitives.RequireValue(b) + epsilon))))) || (DartRuntimePrimitives.RequireValue(a) == DartRuntimePrimitives.RequireValue(b)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class UtilsLibrary
{
    public static bool nearZero(double a, double epsilon) => UtilsLibrary.nearEqual(a, 0.0, epsilon);
}

