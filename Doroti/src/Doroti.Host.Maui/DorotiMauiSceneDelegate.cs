#if IOS && !MACCATALYST
using Foundation;

namespace Doroti.Host.Maui;

/// <summary>MAUI creates and owns the UIWindow for each connected UIKit scene.</summary>
[Register("DorotiMauiSceneDelegate")]
public sealed class DorotiMauiSceneDelegate : Microsoft.Maui.MauiUISceneDelegate { }
#endif
