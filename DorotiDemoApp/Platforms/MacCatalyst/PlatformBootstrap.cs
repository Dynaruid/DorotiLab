using UIKit;

namespace DorotiDemoApp.Platforms.MacCatalyst;

internal static class PlatformBootstrap
{
    internal static void Run(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}
