using Doroti.Hosting;
using Doroti.Ui;

internal static class SharedSessionCloseRegression
{
    private sealed class Root : IDorotiViewEntrypoint
    {
        internal int Shutdowns;
        public void Bootstrap(PlatformDispatcher dispatcher) { }
        public void AttachView(DorotiView view) { }
        public void DetachView(DorotiView view) { }
        public void Shutdown() => Shutdowns++;
    }
    private sealed class ReentrantService(Action dispose) : IDisposable
    {
        internal int Disposals;
        public void Dispose() { Disposals++; dispose(); }
    }
    internal static void Run()
    {
        var root = new Root();
        using var shared = new DorotiSharedHostSession(root);
        shared.Start();
        DorotiView Create()
        {
            var id = shared.AllocateViewId();
            return shared.Session.dispatcher.RegisterView(id, new DorotiViewCapabilities("shared-close", shared.Capabilities)
                .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, new Doroti.Testing.TestHost(new Size(100, 100), 1, viewId: id)));
        }
        var first = Create();
        var second = Create();
        shared.Session.AttachView(first); shared.Session.AttachView(second);
        var callbacks = 0;
        shared.BeginViewClose(first);
        Check.True(first.InvocationLifetime.IsCancellationRequested, "Detached view retained callback admission.");
        first.DispatchPlatformEvent(() => callbacks++);
        second.DispatchPlatformEvent(() => callbacks++);
        Check.True(callbacks == 1 && root.Shutdowns == 0, "Detaching primary stopped the surviving branch.");
        first.Dispose();
        shared.BeginViewClose(second); second.Dispose();
        var service = new ReentrantService(() =>
        {
            Check.Throws<ObjectDisposedException>(() => shared.AllocateViewId());
            Check.Throws<ObjectDisposedException>(() => shared.OwnService(new object()));
            Check.Throws<ObjectDisposedException>(shared.Start);
        });
        shared.OwnService(service);
        shared.Dispose(); shared.Dispose();
        Check.True(root.Shutdowns == 1 && service.Disposals == 1, "Shared shutdown was not exactly once.");
        Console.WriteLine("PASS: shared native-session branch detachment revokes callbacks, preserves survivor work and rejects reentrant view/service registration during final shutdown.");
    }
}
