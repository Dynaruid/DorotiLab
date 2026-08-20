using AppKit;
using Foundation;
using Microsoft.Maui.Platforms.MacOS.Platform;

namespace Doroti.Validation.AppKitMetalSpike;

[Register("DorotiAppKitSpikeApplicationDelegate")]
public sealed class AppKitSpikeApplicationDelegate : MacOSMauiApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void DidFinishLaunching(NSNotification notification)
    {
        base.DidFinishLaunching(notification);
        NSApplication.SharedApplication.Activate();
    }
}
