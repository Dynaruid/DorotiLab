using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Doroti.Host.Web;
using SkiaSharp;
using SkiaSharp.Internals;

internal static class BrowserSkiaHandleLockRegression
{
    public static void Run()
    {
        var registryLock = new BrowserSkiaHandleLock();
        // Handle lookup holds an upgradeable section while a new wrapper
        // registers with a write section. Registration can look up a child too.
        registryLock.EnterUpgradeableReadLock();
        registryLock.EnterWriteLock();
        registryLock.EnterReadLock();
        registryLock.ExitReadLock();
        registryLock.ExitWriteLock();
        registryLock.ExitUpgradeableReadLock();

        // Run in an isolated process: Skia caches the factory's first lock.
        PlatformLock.Factory = () => registryLock;
        using var ready = new CountdownEvent(3);
        using var start = new ManualResetEventSlim();
        var errors = new ConcurrentQueue<Exception>();
        var workers = Enumerable.Range(0, 3).Select(_ => new Thread(() =>
        {
            ready.Signal();
            start.Wait();
            try
            {
                using var surface = SKSurface.Create(new SKImageInfo(32, 32));
                for (var i = 0; i < 1000; i++)
                {
                    surface.Canvas.Clear(SKColors.CornflowerBlue);
                    using var image = surface.Snapshot();
                    using var shader = image.ToShader();
                    using var paint = new SKPaint { Shader = shader };
                    surface.Canvas.DrawRect(0, 0, 32, 32, paint);
                    if (i % 4 == 0) LeaveForFinalizer(surface);
                }
            }
            catch (Exception error) { errors.Enqueue(error); }
        }) { IsBackground = true }).ToArray();
        foreach (var worker in workers) worker.Start();
        if (!ready.Wait(TimeSpan.FromSeconds(10))) throw new Exception("Skia registry workers did not start.");
        start.Set();
        for (var i = 0; i < 10; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(10);
        }
        foreach (var worker in workers)
            if (!worker.Join(TimeSpan.FromSeconds(20))) throw new Exception("Skia handle registry deadlocked.");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        if (errors.TryDequeue(out var failure)) throw new Exception("Concurrent Skia registry access failed.", failure);
        Console.WriteLine("Browser Skia handles: PASS; nested registry access and 3000 concurrent image/shader lifetimes with finalization.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void LeaveForFinalizer(SKSurface surface) => _ = surface.Snapshot();
}
