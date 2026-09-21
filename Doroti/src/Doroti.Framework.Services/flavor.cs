// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/flavor.dart
namespace Doroti.Framework.Services;

public static partial class FlavorLibrary
{
    public static string? appFlavor =
        (Environment.GetEnvironmentVariable("FLUTTER_APP_FLAVOR") != "")
            ? Environment.GetEnvironmentVariable("FLUTTER_APP_FLAVOR")
            : null;
}
