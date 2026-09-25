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
        // UIKit requires multi-scene adoption even to destroy the only scene.
        // Until the Desktop factory supports more windows, reject native new-window
        // requests before MAUI allocates another independent main-window manager.
        if (
            IPlatformApplication.Current?.Application
                is DorotiMauiApplication { UsesDesktop: true } app
            && app.Windows.Count > 0
        )
        {
            UIApplication.SharedApplication.RequestSceneSessionDestruction(
                session,
                null,
                error => DorotiMauiSurface.WriteFailure(new NSErrorException(error))
            );
            return;
        }
        base.WillConnect(scene, session, connectionOptions);
    }
}
#endif
