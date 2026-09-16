// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/app_version.dart
namespace Doroti.Framework.Services;

public static partial class App_versionLibrary
{
    public static string? appBuildName = Environment.GetEnvironmentVariable("FLUTTER_BUILD_NAME") is not null ? Environment.GetEnvironmentVariable("FLUTTER_BUILD_NAME") : null;
}

public static partial class App_versionLibrary
{
    public static string? appBuildNumber = Environment.GetEnvironmentVariable("FLUTTER_BUILD_NUMBER") is not null ? Environment.GetEnvironmentVariable("FLUTTER_BUILD_NUMBER") : null;
}

