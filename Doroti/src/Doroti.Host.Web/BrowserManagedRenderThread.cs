using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Doroti.Host.Web;

/// <summary>Runs the renderer role on a JS-affine thread of the main-owned runtime.</summary>
[SupportedOSPlatform("browser")]
public static partial class BrowserManagedRenderThread
{
    // JSWebWorker is experimental and omitted from the ordinary browser reference
    // assembly. The runtime explicitly supplies RunAsyncVoid for reflection callers.
    // Pin this boundary to the .NET 10 contract; never replace it with Task.Run,
    // which does not install the JS synchronization context needed by Skia/JSImport.
    [DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods,
        "System.Runtime.InteropServices.JavaScript.JSWebWorker", "System.Runtime.InteropServices.JavaScript")]
    [JSExport]
    public static Task StartAsync(string moduleUrl, string sessionToken)
    {
        var worker = typeof(JSHost).Assembly.GetType("System.Runtime.InteropServices.JavaScript.JSWebWorker")
            ?? throw new PlatformNotSupportedException("Doroti main-runtime rendering requires WasmEnableThreads=true.");
        var start = worker.GetMethod("RunAsyncVoid", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new PlatformNotSupportedException("The installed runtime has no supported Doroti JSWebWorker entry point.");
        Func<Task> body = async () =>
        {
            using var module = await JSHost.ImportAsync("doroti-managed-render-thread", moduleUrl);
            await RunRole(sessionToken);
        };
        return (Task)start.Invoke(null, [body, CancellationToken.None])!;
    }

    [JSImport("startSharedRuntimeRole", "doroti-managed-render-thread")]
    private static partial Task RunRole(string sessionToken);

    [JSExport]
    public static int CaptureThreadId() => Environment.CurrentManagedThreadId;
}
