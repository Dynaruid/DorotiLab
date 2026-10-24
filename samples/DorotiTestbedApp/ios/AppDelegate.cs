using Doroti.Host.Maui;
using Foundation;

namespace DorotiTestbedApp.iOS;

[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate
{
    // UIApplication instantiates this through Objective-C rather than a managed new.
    [Preserve]
    public AppDelegate() { }

    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    public override bool FinishedLaunching(UIKit.UIApplication application, NSDictionary? options)
    {
        try { return base.FinishedLaunching(application, options); }
        catch (Exception error)
        {
            // Log before the exception crosses Objective-C and loses its managed stack.
            Console.Error.WriteLine(error);
            throw;
        }
    }

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
