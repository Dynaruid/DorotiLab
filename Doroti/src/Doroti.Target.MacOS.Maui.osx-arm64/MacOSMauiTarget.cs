namespace Doroti.Target.MacOS.Maui;

public static class MacOSMauiTarget
{
    public const string TargetFramework = "net10.0-macos";
    public const string RuntimeIdentifier = "osx-arm64";
    public const string NativeEntryKind = "AppKit-Main";
    public const string GraphicsBackend = "AppKit/MTKView/Metal-Skia";
}
