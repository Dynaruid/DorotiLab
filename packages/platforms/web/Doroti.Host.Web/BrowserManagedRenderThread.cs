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
    // Pin this boundary to the qualified Mono contract; never replace it with Task.Run,
    // which does not install the JS synchronization context needed by Skia/JSImport.
    [FeatureSwitchDefinition("Doroti.Web.Threading")]
    private static bool ThreadingEnabled => AppContext.TryGetSwitch("Doroti.Web.Threading", out var enabled) && enabled;

    [JSExport]
    public static Task StartAsync(string moduleUrl, string sessionToken)
    {
        if (!ThreadingEnabled)
            throw new PlatformNotSupportedException("The compatible runtime cannot create a shared managed render thread.");
        Trace("start-request");
        CancellationTokenSource cancellation;
        lock (Gate)
        {
            if (Sessions.ContainsKey(sessionToken)) throw new InvalidOperationException("Duplicate render role token.");
            cancellation = new CancellationTokenSource();
            Sessions.Add(sessionToken, cancellation);
        }
        return StartCore(moduleUrl, sessionToken, cancellation);
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
            Trace("entry-resolve");
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
                Trace("owner-enter");
                using var module = await JSHost.ImportAsync("doroti-managed-render-thread", moduleUrl, cancellation.Token);
                Trace("role-imported");
                cancellation.Token.ThrowIfCancellationRequested();
                await RunRole(sessionToken).WaitAsync(cancellation.Token);
            };
            Trace("worker-start");
            var lifetime = (Task)start.Invoke(null, [body, cancellation.Token])!;
            Trace("worker-started");
            await lifetime;
        }
        finally { lock (Gate) { Sessions.Remove(sessionToken); cancellation.Dispose(); } }
    }

    private static void Trace(string stage)
    {
        if (Environment.GetEnvironmentVariable("DOROTI_STAGE_TRACE") == "1")
            Console.WriteLine($"Doroti managed render: {stage}; thread={Environment.CurrentManagedThreadId}");
    }

    [JSImport("startSharedRuntimeRole", "doroti-managed-render-thread")]
    private static partial Task RunRole(string sessionToken);

    [JSExport]
    public static int CaptureThreadId() => Environment.CurrentManagedThreadId;
}
