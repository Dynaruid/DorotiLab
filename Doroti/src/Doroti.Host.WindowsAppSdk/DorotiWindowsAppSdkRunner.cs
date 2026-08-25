using Doroti.Hosting;

namespace Doroti.Host.WindowsAppSdk;

public static class DorotiWindowsAppSdkRunner
{
    public static int Run(DorotiApplicationDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException(
                "Doroti.Host.WindowsAppSdk can only launch on Windows.");

        var adapter = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_ADAPTER");
        if (!string.IsNullOrWhiteSpace(adapter) &&
            !adapter.Equals("WinRtComposition", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Unsupported Windows App SDK adapter '{adapter}'. Expected WinRtComposition.");
        }

        throw new NotSupportedException(
            "The WinRtComposition backend is planned but not implemented. " +
            "Select the Maui backend with -WindowsBackend Maui when a runnable Windows host is required.");
    }
}
