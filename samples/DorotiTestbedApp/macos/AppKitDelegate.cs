#if MACOS
using AppKit;
using Doroti.Host.Maui;
using Foundation;

namespace DorotiTestbedApp.MacOS;

[Register("DorotiTestbedAppKitDelegate")]
public sealed class AppKitDelegate : DorotiMacOSMauiApplication
{
    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;

    public override void DidFinishLaunching(NSNotification notification)
    {
        base.DidFinishLaunching(notification);
        NSApplication.SharedApplication.Activate();
        if (Environment.GetEnvironmentVariable("DOROTI_MACOS_FRAME_PROBE") is { Length: > 0 } frameOutput)
            _ = AppKitFrameLifecycleProbe.RunAsync(frameOutput);
        if (Environment.GetEnvironmentVariable("DOROTI_MACOS_SERVICES_PROBE") is { Length: > 0 } output)
            _ = AppKitServicesProbe.RunAsync(output);
        if (Environment.GetEnvironmentVariable("DOROTI_MACOS_AUTOMATION") is { Length: > 0 } directory)
            _ = AppKitAutomation.RunAsync(directory);
    }
}
#endif
