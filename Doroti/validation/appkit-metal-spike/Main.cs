using AppKit;

namespace Doroti.Validation.AppKitMetalSpike;

public static class MainClass
{
    public static void Main(string[] args)
    {
        if (Environment.GetEnvironmentVariable("DOROTI_APPKIT_SPIKE_CONTRACT") == "1")
        {
            Environment.ExitCode = GraphiteMetalContract.Run();
            return;
        }
        NSApplication.Init();
        NSApplication.SharedApplication.Delegate = new AppKitSpikeApplicationDelegate();
        NSApplication.Main(args);
    }
}
