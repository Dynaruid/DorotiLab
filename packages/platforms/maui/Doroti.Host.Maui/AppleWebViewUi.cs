#if MACOS || IOS || MACCATALYST
using Doroti.Hosting;

namespace Doroti.Host.Maui;

internal static class AppleWebViewUi
{
    internal static IPlatformViewDispatcher Dispatcher =>
#if MACOS
        new AppKitPlatformViewDispatcher();
#else
        new UIKitPlatformViewDispatcher();
#endif

    internal static void VerifyThread()
    {
#if MACOS
        AppKitPlatformViewDispatcher.VerifyThread();
#else
        UIKitPlatformViewDispatcher.VerifyThread();
#endif
    }
}
#endif
