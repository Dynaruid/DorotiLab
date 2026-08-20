using AppKit;

namespace Doroti.Validation.AppKitMetalSpike;

public static class MainClass
{
    public static void Main(string[] args)
    {
        NSApplication.Init();
        NSApplication.SharedApplication.Delegate = new AppKitSpikeApplicationDelegate();
        NSApplication.Main(args);
    }
}
