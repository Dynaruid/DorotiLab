#if MACOS
using AppKit;
using Doroti.Host.Maui;
using Foundation;
using System.Text.Json;

namespace DorotiDemoApp.MacOS;

[Register("DorotiDemoAppKitDelegate")]
public sealed class AppKitDelegate : DorotiMacOSMauiApplication
{
    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;

    public override void DidFinishLaunching(NSNotification notification)
    {
        base.DidFinishLaunching(notification);
        NSApplication.SharedApplication.Activate();
        _ = CaptureNativeBridgeEvidenceAsync();
    }

    private static async Task CaptureNativeBridgeEvidenceAsync()
    {
        var path = Environment.GetEnvironmentVariable("DOROTI_NATIVE_BRIDGE_EVIDENCE");
        if (string.IsNullOrWhiteSpace(path)) return;
        try
        {
            var bridge = new DorotiNativePlatformBridge();
            var platform = bridge.PlatformInfo();
            var echo = bridge.Echo("appkit-echo");
            var mainThreadEcho = await bridge.EchoOnUiThreadAsync("appkit-main-thread");
            var json = JsonSerializer.Serialize(new
            {
                platform,
                echo,
                mainThreadEcho,
                callbackOnMainThread = true,
            }, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            TryExitAfterEvidence();
        }
        catch (Exception exception)
        {
            File.WriteAllText(path + ".exception.txt", exception.ToString());
        }
    }

    private static void TryExitAfterEvidence()
    {
        var surfacePath = Environment.GetEnvironmentVariable("DOROTI_MAUI_EVIDENCE");
        if (string.Equals(Environment.GetEnvironmentVariable("DOROTI_EXIT_AFTER_EVIDENCE"), "1", StringComparison.Ordinal) &&
            !string.IsNullOrWhiteSpace(surfacePath) && File.Exists(surfacePath))
        {
            Environment.Exit(0);
        }
    }
}
#endif
