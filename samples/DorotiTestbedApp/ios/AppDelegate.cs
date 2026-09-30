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
        if (
            Environment.GetEnvironmentVariable("DOROTI_INPUT_PROBE") is { Length: > 0 } inputOutput
            && !System.IO.Path.IsPathRooted(inputOutput)
        )
            Environment.SetEnvironmentVariable(
                "DOROTI_INPUT_PROBE",
                System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    inputOutput
                )
            );
        var result = base.FinishedLaunching(application, options);
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
