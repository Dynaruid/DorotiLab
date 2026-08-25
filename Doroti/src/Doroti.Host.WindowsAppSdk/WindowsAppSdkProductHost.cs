using Doroti.Skia.Rendering;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

internal enum WindowsAppSdkAdapterKind
{
    FlutterEmbedder,
    ArmNLegacy,
}

internal static class WindowsAppSdkAdapterSelection
{
    internal const string EnvironmentVariable = "DOROTI_WINDOWS_ADAPTER";

    internal static WindowsAppSdkAdapterKind Resolve()
    {
        var value = Environment.GetEnvironmentVariable(EnvironmentVariable);
        if (string.IsNullOrWhiteSpace(value) ||
            value.Equals("FlutterEmbedder", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("flutter", StringComparison.OrdinalIgnoreCase))
            return WindowsAppSdkAdapterKind.FlutterEmbedder;
        if (value.Equals("ArmNLegacy", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("arm-n", StringComparison.OrdinalIgnoreCase))
            return WindowsAppSdkAdapterKind.ArmNLegacy;
        if (value.Equals("MauiRollback", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("maui", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "MAUI rollback is a separate WinUI entrypoint. Use doroti.ps1 with " +
                "-WindowsAdapter MauiRollback so the windows/DorotiDemoApp.Windows.csproj runner is selected.");
        throw new InvalidOperationException(
            $"Unsupported {EnvironmentVariable} value '{value}'. " +
            "Expected FlutterEmbedder, ArmNLegacy, or MauiRollback through doroti.ps1.");
    }
}

internal interface IWindowsAppSdkProductHost :
    IViewHostCapability,
    IFrameHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    ITextInputHostCapability,
    IPlatformEnvironmentHostCapability,
    IPlatformServicesHostCapability,
    ISkiaSceneRendererHost
{
    void AttachRenderer(SkiaSceneRenderer renderer);
    int RunMessageLoop();
    void WriteDiagnostics(SkiaFrameDiagnostics diagnostics);
    void ApplyLeftResizeSmokeStep(int step);
}
