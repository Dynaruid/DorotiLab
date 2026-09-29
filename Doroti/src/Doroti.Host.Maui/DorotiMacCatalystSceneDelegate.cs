#if MACCATALYST
using Foundation;
using UIKit;

namespace Doroti.Host.Maui;

/// <summary>Explicit scene registration for UIKit geometry and destruction APIs.</summary>
[Register("DorotiMacCatalystSceneDelegate")]
public sealed class DorotiMacCatalystSceneDelegate : Microsoft.Maui.MauiUISceneDelegate
{
    public override void WillConnect(
        UIScene scene,
        UISceneSession session,
        UISceneConnectionOptions connectionOptions
    )
    {
        UIKitApplicationActivation.Connect(connectionOptions, IPlatformApplication.Current?.Application?.Windows.Count == 0);
        // Native New Window/restored sessions have no Desktop content factory.
        // Reject those sessions before MAUI can allocate a second main manager.
        if (IPlatformApplication.Current?.Application is DorotiMauiApplication { UsesDesktop: true } app
            && OperatingSystem.IsMacCatalystVersionAtLeast(16)
            && app.Windows.Count > 0 && !MacCatalystDesktopWindowHost.HasPendingScene)
        {
            UIApplication.SharedApplication.RequestSceneSessionDestruction(session, null,
                error => DorotiMauiSurface.WriteFailure(new NSErrorException(error)));
            return;
        }
        base.WillConnect(scene, session, connectionOptions);
    }
    public override bool OpenUrl(UIScene scene, NSSet<UIOpenUrlContext> contexts)
    {
        var handled = base.OpenUrl(scene, contexts);
        foreach (var context in contexts) UIKitApplicationActivation.Deliver(context.Url, false, false);
        return handled || contexts.Count > 0;
    }
    public override bool ContinueUserActivity(UIScene scene, NSUserActivity activity)
    {
        UIKitApplicationActivation.Continue(activity, false);
        return base.ContinueUserActivity(scene, activity) || activity.ActivityType == NSUserActivityType.BrowsingWeb;
    }
}
#endif
