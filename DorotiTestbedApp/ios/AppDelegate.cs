using Doroti.Host.Maui;
using Foundation;

namespace DorotiTestbedApp.iOS;

[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate
{
    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    public override bool FinishedLaunching(UIKit.UIApplication application, NSDictionary? launchOptions)
    {
        var result = base.FinishedLaunching(application, launchOptions);
        _ = PlatformViewEvidence.CaptureAsync();
        _ = UIKitBlurCalibration.RunAsync();
        return result;
    }

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
