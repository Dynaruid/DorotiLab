using Doroti.Target.Web;

namespace Doroti.Validation.G7WebBuild;

internal static class Program
{
    [System.Runtime.Versioning.SupportedOSPlatform("browser")]
    private static void Main()
    {
        using var target = new BrowserWasmTarget();
        if (target.Rid != "browser-wasm" || target.Manifest.ManagedCallbackAbi != "doroti.browser-managed-callbacks/v1")
            throw new InvalidDataException("The browser target package manifest is inconsistent.");
        Console.WriteLine($"Doroti G7 Web build probe: PASS ({target.Rid}; {target.GraphicsBackend})");
    }
}
