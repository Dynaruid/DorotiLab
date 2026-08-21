namespace Doroti.Target.Windows.Maui;

public static class WindowsMauiTarget
{
    public const string Rid = "win-x64";
    public const string TargetFramework = "net10.0-windows10.0.19041.0";
    public const string NativeViewType = "Microsoft.UI.Xaml.Controls.SwapChainPanel";
    public const string GraphicsBackend = "winui3/SwapChainPanel/Doroti-owned-D3D12-Skia";

    public static void EnsureSupported()
    {
        if (!OperatingSystem.IsWindows() || !Environment.Is64BitProcess)
            throw new PlatformNotSupportedException("Doroti.Target.Windows.Maui.win-x64 requires a Windows x64 process.");
    }
}
