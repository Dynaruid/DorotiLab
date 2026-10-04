using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;

namespace Doroti.Host.Web;

/// <summary>Runs the renderer role on a JS-affine thread of the main-owned runtime.</summary>
[SupportedOSPlatform("browser")]
public static partial class BrowserManagedRenderThread
{
    private static readonly object Gate = new();
    private static readonly Dictionary<string, CancellationTokenSource> Sessions = [];
    // JSWebWorker is experimental and omitted from the ordinary browser reference
    // assembly. The runtime explicitly supplies RunAsyncVoid for reflection callers.
    // Pin this boundary to the .NET 10 contract; never replace it with Task.Run,
    // which does not install the JS synchronization context needed by Skia/JSImport.
    [DynamicDependency(
        DynamicallyAccessedMemberTypes.NonPublicMethods,
        "System.Runtime.InteropServices.JavaScript.JSWebWorker",
        "System.Runtime.InteropServices.JavaScript"
    )]
    [JSExport]
    public static Task StartAsync(string moduleUrl, string sessionToken)
    {
        lock (Gate)
        {
            if (Sessions.ContainsKey(sessionToken)) throw new InvalidOperationException("Duplicate render role token.");
            var cancellation = new CancellationTokenSource();
            Sessions.Add(sessionToken, cancellation);
            return StartCore(moduleUrl, sessionToken, cancellation);
        }
    }

    [JSExport]
    public static Task CancelAsync(string sessionToken)
    {
        lock (Gate) return Sessions.TryGetValue(sessionToken, out var source) ? source.CancelAsync() : Task.CompletedTask;
    }

    private static async Task StartCore(string moduleUrl, string sessionToken, CancellationTokenSource cancellation)
    {
        try
        {
        var worker =
            typeof(JSHost).Assembly.GetType("System.Runtime.InteropServices.JavaScript.JSWebWorker")
            ?? throw new PlatformNotSupportedException(
                "Doroti main-runtime rendering requires WasmEnableThreads=true."
            );
        var start =
            worker.GetMethod("RunAsyncVoid", BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new PlatformNotSupportedException(
                "The installed runtime has no supported Doroti JSWebWorker entry point."
            );
        Func<Task> body = async () =>
        {
            using var module = await JSHost.ImportAsync("doroti-managed-render-thread", moduleUrl, cancellation.Token);
            cancellation.Token.ThrowIfCancellationRequested();
            await RunRole(sessionToken).WaitAsync(cancellation.Token);
        };
        await (Task)start.Invoke(null, [body, cancellation.Token])!;
        }
        finally { lock (Gate) { Sessions.Remove(sessionToken); cancellation.Dispose(); } }
    }

    [JSImport("startSharedRuntimeRole", "doroti-managed-render-thread")]
    private static partial Task RunRole(string sessionToken);

    [JSExport]
    public static int CaptureThreadId() => Environment.CurrentManagedThreadId;
}
