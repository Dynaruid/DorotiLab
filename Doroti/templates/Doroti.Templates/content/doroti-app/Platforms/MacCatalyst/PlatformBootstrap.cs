using UIKit;

namespace DorotiApp.Platforms.MacCatalyst;

internal static class PlatformBootstrap
{
    internal static void Run(string[] args) => UIApplication.Main(args, null, typeof(AppDelegate));
}
