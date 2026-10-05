#if IOS && !MACCATALYST
using Foundation;
using UIKit;

namespace Doroti.Host.Maui;

/// <summary>MAUI creates and owns the UIWindow for each connected UIKit scene.</summary>
[Register("DorotiMauiSceneDelegate")]
public sealed class DorotiMauiSceneDelegate : Microsoft.Maui.MauiUISceneDelegate
{
    // UIKit creates scene delegates from the native scene configuration.
    [Preserve]
    public DorotiMauiSceneDelegate() { }

    public override void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions options)
    {
        UIKitApplicationActivation.Connect(options);
        base.WillConnect(scene, session, options);
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
