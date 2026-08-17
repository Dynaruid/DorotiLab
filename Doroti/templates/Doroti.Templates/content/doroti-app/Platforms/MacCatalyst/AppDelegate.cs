using Doroti.Host.Maui;
using Foundation;

namespace DorotiTemplateApp.Platforms.MacCatalyst;

[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate<Program>
{
    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
