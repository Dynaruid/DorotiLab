namespace Doroti.Target.MacCatalyst.Maui;

public static class MacCatalystMauiTarget
{
    public const string Rid = "maccatalyst-arm64";
    public const string TargetFramework = "net10.0-maccatalyst";
    public const string NativeViewType = "Doroti.Host.Maui.DorotiUIKitGraphiteView";
    public const string GraphicsBackend = "UIKit/MTKView/Graphite-Metal";

    public static void EnsureSupported()
    {
        if (!OperatingSystem.IsMacCatalyst())
            throw new PlatformNotSupportedException("Doroti.Target.MacCatalyst.Maui.maccatalyst-arm64 requires Mac Catalyst.");
    }
}
