// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/physics/utils.dart
using Doroti.Runtime;

namespace Doroti.Framework.Physics;

public static partial class UtilsLibrary
{
    public static bool nearEqual(double? a, double? b, double epsilon)
    {
        DartRuntimePrimitives.Assert(() => epsilon >= 0.0);
        if ((a is null) || (b is null))
        {
            return a == b;
        }
        return (
                (
                    a
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
                    > (
                        b
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) - epsilon
                && (
                    a
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
                    < (
                        b
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    ) + epsilon
            )
            || (
                (
                    a
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
                == (
                    b
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class UtilsLibrary
{
    public static bool nearZero(double a, double epsilon) => nearEqual(a, 0.0, epsilon);
}
