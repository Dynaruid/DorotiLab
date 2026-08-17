using Android.App;
using Android.Runtime;
using Doroti.Host.Maui;
using Doroti.Hosting;

namespace DorotiTemplateApp.Platforms.Android;

[Application]
public sealed class MainApplication(IntPtr handle, JniHandleOwnership ownership)
    : DorotiMauiAndroidApplication(handle, ownership)
{
    protected override DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create([]);

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
