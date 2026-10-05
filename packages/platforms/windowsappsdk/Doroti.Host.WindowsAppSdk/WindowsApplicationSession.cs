using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>Application execution outlives every native loop, including the primary window.</summary>
internal sealed class WindowsApplicationSession : IDisposable
{
    internal DorotiApplicationDispatcher Owner { get; } = new();
    internal DorotiHostSession Session { get; }
    internal DorotiManagedPluginRegistry? ManagedPlugins { get; private set; }
    internal DorotiCapabilityLifetime Capabilities { get; } = new();
    private readonly HashSet<object> _services = new(ReferenceEqualityComparer.Instance);
    internal void OwnService<T>(T service) where T : class
    { lock (_services) if (_services.Add(service)) Capabilities.Own(service); }
    private long _nextView;
    private int _disposed;
    internal WindowsApplicationSession(DorotiApplicationDescriptor descriptor)
    {
        DorotiHostSession? session = null;
        try
        {
            Owner.InvokeAsync(() =>
            {
                ManagedPlugins = new DorotiManagedPluginRegistry(Capabilities);
                OwnService(ManagedPlugins);
                ManagedPlugins.Configure(descriptor.ManagedPluginRegistrations ?? []);
                session = new(descriptor.EntrypointFactory(), coordinatorOwnsFailureCleanup: true, applicationDispatcher: Owner);
                session.Start(deferFrameworkBootstrap: true);
            }).AsTask().GetAwaiter().GetResult();
        }
        catch
        {
            try
            {
                Owner.InvokeAsync(() => Doroti.Runtime.DorotiCleanup.Run(() => session?.Dispose(), Capabilities.Dispose))
                    .AsTask().GetAwaiter().GetResult();
            }
            catch (Exception cleanup) { System.Diagnostics.Trace.TraceError(cleanup.ToString()); }
            finally { Owner.DisposeAsync().AsTask().GetAwaiter().GetResult(); }
            throw;
        }
        Session = session!;
    }
    internal ulong AllocateViewId() => checked((ulong)Interlocked.Increment(ref _nextView));
    internal void Attach(Action callback) => Owner.InvokeAsync(callback).AsTask().GetAwaiter().GetResult();
    internal void Detach(DorotiView view) => Attach(() => Session.DetachView(view));
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        try { Attach(() => Doroti.Runtime.DorotiCleanup.Run(Session.Dispose, Capabilities.Dispose)); }
        finally { Owner.DisposeAsync().AsTask().GetAwaiter().GetResult(); }
    }
}
