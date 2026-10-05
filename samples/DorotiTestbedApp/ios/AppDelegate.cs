using Doroti.Host.Maui;
using Foundation;

namespace DorotiTestbedApp.iOS;

[Register("AppDelegate")]
public sealed class AppDelegate : DorotiMauiUIApplicationDelegate
{
    protected override Doroti.Hosting.DorotiApplicationDescriptor CreateApplicationDescriptor() =>
        Doroti.Generated.DorotiBootstrap.Create(Environment.GetCommandLineArgs().Skip(1).ToArray());

    public override bool FinishedLaunching(UIKit.UIApplication application, NSDictionary? options)
    {
        foreach (var name in new[]
        {
            "DOROTI_INPUT_PROBE", "DOROTI_UIKIT_SERVICES_PROBE", "DOROTI_APPLE_FEATURE_PROBE",
            "DOROTI_UIKIT_ROTATION_PROBE", "DOROTI_UIKIT_SHUTDOWN_PROBE", "DOROTI_NAVIGATION_PROBE",
        })
        {
            if (Environment.GetEnvironmentVariable(name) is not { Length: > 0 } outputPath) continue;
            if (!Path.IsPathRooted(outputPath))
                outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            Environment.SetEnvironmentVariable(name, outputPath);
        }
        var result = base.FinishedLaunching(application, options);
        if (Environment.GetEnvironmentVariable("DOROTI_UIKIT_SHUTDOWN_PROBE") is { Length: > 0 } shutdownOutput)
            _ = UIKitShutdownProbe.RunAsync(shutdownOutput);
        if (Environment.GetEnvironmentVariable("DOROTI_APPLE_FEATURE_PROBE") is { Length: > 0 } featureOutput)
            _ = DorotiTestbedApp.Apple.AppleFeatureProbe.RunAsync(featureOutput);
        if (
            Environment.GetEnvironmentVariable("DOROTI_UIKIT_SERVICES_PROBE") is
            { Length: > 0 } output
        )
            _ = DorotiTestbedApp.Apple.UIKitServicesProbe.RunAsync(output);
        if (
            Environment.GetEnvironmentVariable("DOROTI_UIKIT_ACTIVATION_PROBE") is
            { Length: > 0 } link
        )
            _ = DorotiTestbedApp.Apple.UIKitServicesProbe.DeliverLinkAsync(link);
        if (
            Environment.GetEnvironmentVariable("DOROTI_UIKIT_ROTATION_PROBE") is
            { Length: > 0 } rotationOutput
        )
            _ = UIKitRotationProbe.RunAsync(rotationOutput);
        return result;
    }

    protected override void ConfigurePlatform(MauiAppBuilder builder) => _ = builder;
}
