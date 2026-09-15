// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/flutter_version.dart
namespace Doroti.Framework.Services;

public abstract class FlutterVersion
{
    public static string? version = (Environment.GetEnvironmentVariable("FLUTTER_VERSION") is not null ? Environment.GetEnvironmentVariable("FLUTTER_VERSION") : null);
    public static string? channel = (Environment.GetEnvironmentVariable("FLUTTER_CHANNEL") is not null ? Environment.GetEnvironmentVariable("FLUTTER_CHANNEL") : null);
    public static string? gitUrl = (Environment.GetEnvironmentVariable("FLUTTER_GIT_URL") is not null ? Environment.GetEnvironmentVariable("FLUTTER_GIT_URL") : null);
    public static string? frameworkRevision = (Environment.GetEnvironmentVariable("FLUTTER_FRAMEWORK_REVISION") is not null ? Environment.GetEnvironmentVariable("FLUTTER_FRAMEWORK_REVISION") : null);
    public static string? engineRevision = (Environment.GetEnvironmentVariable("FLUTTER_ENGINE_REVISION") is not null ? Environment.GetEnvironmentVariable("FLUTTER_ENGINE_REVISION") : null);
    public static string? dartVersion = (Environment.GetEnvironmentVariable("FLUTTER_DART_VERSION") is not null ? Environment.GetEnvironmentVariable("FLUTTER_DART_VERSION") : null);

    protected FlutterVersion()
    {
    }

}

