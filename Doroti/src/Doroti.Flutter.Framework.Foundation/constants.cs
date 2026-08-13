// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/constants.dart
using Doroti.Flutter.Runtime;

namespace Doroti.Generated.Framework.Foundation;

public static class ConstantsLibrary
{
    public static bool kReleaseMode => FoundationRuntimePorts.kReleaseMode;

    public static bool kProfileMode => FoundationRuntimePorts.kProfileMode;

    public static bool kDebugMode => !kReleaseMode && !kProfileMode;

    public const double precisionErrorTolerance = 1e-10;

    public const bool kIsWeb = false;

    public const bool kIsWasm = false;
}
