// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/utils.dart
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

