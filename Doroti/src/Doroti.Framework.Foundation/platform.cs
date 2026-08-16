// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/foundation/platform.dart and _platform_io.dart
using Doroti.Ui;

namespace Doroti.Framework.Foundation;

public enum TargetPlatform
{
    android,
    fuchsia,
    iOS,
    linux,
    macOS,
    windows,
}

public static class PlatformLibrary
{
    private static TargetPlatform? _debugDefaultTargetPlatformOverride;

    public static TargetPlatform defaultTargetPlatform => _debugDefaultTargetPlatformOverride ??
        PlatformEnvironmentContext.current.operatingSystem switch
        {
            HostOperatingSystem.android => TargetPlatform.android,
            HostOperatingSystem.fuchsia => TargetPlatform.fuchsia,
            HostOperatingSystem.iOS => TargetPlatform.iOS,
            HostOperatingSystem.linux => TargetPlatform.linux,
            HostOperatingSystem.macOS => TargetPlatform.macOS,
            HostOperatingSystem.windows => TargetPlatform.windows,
            _ => throw new DorotiCapabilityException(
                DorotiCapabilityIds.PlatformEnvironment,
                null,
                DartUiInvocation.Managed("dart:io#Platform.operatingSystem"),
                "the host platform is not represented by Flutter TargetPlatform"),
        };

    public static TargetPlatform? debugDefaultTargetPlatformOverride
    {
        get => _debugDefaultTargetPlatformOverride;
        set
        {
            if (!ConstantsLibrary.kDebugMode)
            {
                throw new FlutterError("Cannot modify debugDefaultTargetPlatformOverride in non-debug builds.");
            }
            _debugDefaultTargetPlatformOverride = value;
        }
    }
}
