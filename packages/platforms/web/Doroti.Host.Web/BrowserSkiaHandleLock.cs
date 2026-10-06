using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using SkiaSharp.Internals;

namespace Doroti.Host.Web;

// The handle registry is shared by the render owner and Mono's finalizer thread.
// ReaderWriterLockSlim's ownership bookkeeping can fail under threaded Safari
// WASM, throwing from SKNativeObject.Finalize and stopping the entire runtime.
// SkiaSharp provides this factory specifically for host-specific registry locks.
internal sealed class BrowserSkiaHandleLock : IPlatformLock
{
    private readonly object _gate = new();

    // Serialize every registry operation. A lookup can construct a wrapper and
    // register its handle inside the same section, so this must be reentrant.
    // Use an object Monitor rather than System.Threading.Lock.
    public void EnterReadLock() => Monitor.Enter(_gate);
    public void ExitReadLock() => Monitor.Exit(_gate);
    public void EnterWriteLock() => Monitor.Enter(_gate);
    public void ExitWriteLock() => Monitor.Exit(_gate);
    public void EnterUpgradeableReadLock() => Monitor.Enter(_gate);
    public void ExitUpgradeableReadLock() => Monitor.Exit(_gate);

    [ModuleInitializer]
    [SuppressMessage("Usage", "CA2255", Justification = "The browser host must select the handle lock before app or font initialization touches SkiaSharp.")]
    internal static void Configure()
    {
        // Install before any app/font initialization creates the registry.
        // Loading this host in a desktop test must not change native Skia locks.
        if (OperatingSystem.IsBrowser()
            && PlatformLock.Factory == (Func<IPlatformLock>)PlatformLock.DefaultFactory)
            PlatformLock.Factory = static () => new BrowserSkiaHandleLock();
    }
}
