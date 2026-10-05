using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using Doroti.Ui;

namespace Doroti.Host.Web;

[SupportedOSPlatform("browser")]
public static partial class BrowserHotReload
{
    // Async exports work with the threaded runtime's default JS interop policy.
    [JSExport]
    public static Task<string> ReadStatusAsync() => Task.FromResult(DorotiDevelopmentHost.Status);

    [JSExport]
    public static Task<bool> PrepareAsync(string runtimeId, string requestId) =>
        Task.FromResult(DorotiDevelopmentHost.Prepare(runtimeId, requestId));
}
