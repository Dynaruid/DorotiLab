using Doroti.Host.Maui;
using Foundation;

namespace DorotiDemoApp.Platforms.MacCatalyst;

[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate<Program>
{
    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
