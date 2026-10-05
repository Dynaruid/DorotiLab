using Doroti.Ui;

namespace Doroti.Hosting;

/// <summary>One application root and service lifetime for views on a provider's GUI owner.</summary>
public sealed class DorotiSharedHostSession : IDisposable
{
    public DorotiHostSession Session { get; }
    public DorotiManagedPluginRegistry? ManagedPlugins { get; private set; }
    public DorotiCapabilityLifetime Capabilities { get; } = new();
    private readonly HashSet<object> _services = new(ReferenceEqualityComparer.Instance);
    private long _nextView;
    private bool _started;
    private bool _disposed;
    public DorotiSharedHostSession(IDorotiViewEntrypoint entrypoint, IDorotiApplicationDispatcher? applicationDispatcher = null) =>
        Session = new(entrypoint, coordinatorOwnsFailureCleanup: true, applicationDispatcher: applicationDispatcher);
    public DorotiSharedHostSession(DorotiApplicationDescriptor descriptor, IDorotiApplicationDispatcher? applicationDispatcher = null) : this(descriptor.EntrypointFactory(), applicationDispatcher)
    {
        try
        {
            ManagedPlugins = new(Capabilities);
            OwnService(ManagedPlugins);
            ManagedPlugins.Configure(descriptor.ManagedPluginRegistrations ?? []);
        }
        catch
        {
            try { Dispose(); } catch (Exception cleanup) { System.Diagnostics.Trace.TraceError(cleanup.ToString()); }
            throw;
        }
    }
    public void RegisterManagedPlugins(DorotiViewCapabilities capabilities)
    {
        if (ManagedPlugins is not null)
            capabilities.Register<IDorotiManagedPluginInvoker>(DorotiCapabilityIds.ManagedPlugins, ManagedPlugins, DorotiCapabilityOwnership.Borrowed);
    }
    public ulong AllocateViewId() => checked((ulong)Interlocked.Increment(ref _nextView));
    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_started) return;
        Session.Start(deferFrameworkBootstrap: true);
        _started = true;
    }
    public void OwnService<T>(T service) where T : class
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_services.Add(service)) Capabilities.Own(service);
    }
    public void Dispose()
    {
        if (_disposed) return;
        Doroti.Runtime.DorotiCleanup.Run(Session.Dispose, Capabilities.Dispose);
        _disposed = true;
        _services.Clear();
    }
}
