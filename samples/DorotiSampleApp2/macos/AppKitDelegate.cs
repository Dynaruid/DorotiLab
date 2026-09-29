using AppKit;
using Doroti.Host.Maui;
using Foundation;

namespace DorotiSampleApp2.MacOS;

[Register("DorotiAppKitDelegate")]
public sealed class AppKitDelegate : DorotiMacOSMauiApplication
{
    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;

    public override void DidFinishLaunching(NSNotification notification)
    {
        base.DidFinishLaunching(notification);
        NSApplication.SharedApplication.Activate();
    }
}
