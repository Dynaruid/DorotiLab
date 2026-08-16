namespace Doroti.Target.Windows.Maui;

public static class WindowsMauiTarget
{
    public const string Rid = "win-x64";
    public const string TargetFramework = "net10.0-windows10.0.19041.0";
    public const string NativeViewType = "SkiaSharp.Views.Maui.Handlers.SKGLViewHandler+MauiSKSwapChainPanel";
    public const string GraphicsBackend = "winui3/SKSwapChainPanel/ANGLE-DirectX-Skia";

    public static void EnsureSupported()
    {
        if (!OperatingSystem.IsWindows() || !Environment.Is64BitProcess)
            throw new PlatformNotSupportedException("Doroti.Target.Windows.Maui.win-x64 requires a Windows x64 process.");
    }
}
