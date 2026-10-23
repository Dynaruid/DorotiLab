#if MACCATALYST
using Doroti.Host.Maui;
using Foundation;

namespace DorotiTestbedApp.MacCatalyst;

[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate
{
    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    public override bool FinishedLaunching(UIKit.UIApplication application, NSDictionary? options)
    {
        var textOutput = Environment.GetEnvironmentVariable("DOROTI_APPLE_TEXT_EDITING_PROBE");
        if (!string.IsNullOrEmpty(textOutput)) File.AppendAllText(textOutput + ".launch", "before Maui launch\n");
        var result = base.FinishedLaunching(application, options);
        if (!string.IsNullOrEmpty(textOutput))
        {
            File.AppendAllText(textOutput + ".launch", "after Maui launch\n");
            _ = DorotiTestbedApp.Apple.AppleFeatureProbe.RunTextEditingAsync(textOutput);
        }
        if (Environment.GetEnvironmentVariable("DOROTI_APPLE_FEATURE_PROBE") is { Length: > 0 } featureOutput)
            _ = DorotiTestbedApp.Apple.AppleFeatureProbe.RunAsync(featureOutput);
        if (Environment.GetEnvironmentVariable("DOROTI_UIKIT_SERVICES_PROBE") is { Length: > 0 } output)
            _ = DorotiTestbedApp.Apple.UIKitServicesProbe.RunAsync(output);
        if (Environment.GetEnvironmentVariable("DOROTI_UIKIT_ACTIVATION_PROBE") is { Length: > 0 } link)
            _ = DorotiTestbedApp.Apple.UIKitServicesProbe.DeliverLinkAsync(link);
        return result;
    }

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
#endif
