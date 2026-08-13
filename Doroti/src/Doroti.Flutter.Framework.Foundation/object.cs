// <doroti-reviewed-framework-source />
#nullable enable
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/foundation/object.dart
using System;
using Doroti.Flutter.Runtime;

namespace Doroti.Generated.Framework.Foundation;

public static partial class objectRuntimeTypeFunctions
{
    public static string objectRuntimeType(object? @object, string optimizedValue)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                optimizedValue = DartRuntimePrimitives.RuntimeTypeName(@object);
                return true;
            });
        return optimizedValue;
    }
}

