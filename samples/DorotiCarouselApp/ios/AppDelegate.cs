using Doroti.Host.Maui;
using Foundation;

namespace DorotiCarouselApp.iOS;

[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate
{
    // UIApplication creates this through Objective-C rather than a managed new.
    [Preserve]
    public AppDelegate() { }

    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
