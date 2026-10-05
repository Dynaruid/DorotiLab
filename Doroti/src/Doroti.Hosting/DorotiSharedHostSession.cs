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
    private bool _stopping;
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
        ObjectDisposedException.ThrowIf(_disposed || _stopping, this);
        if (ManagedPlugins is not null)
            capabilities.Register<IDorotiManagedPluginInvoker>(DorotiCapabilityIds.ManagedPlugins, ManagedPlugins, DorotiCapabilityOwnership.Borrowed);
    }
    public ulong AllocateViewId()
    {
        ObjectDisposedException.ThrowIf(_disposed || _stopping, this);
        return checked((ulong)Interlocked.Increment(ref _nextView));
    }
    /// <summary>Unmount one branch and revoke its callbacks before waiting on GPU or typed service consumers.</summary>
    public void BeginViewClose(DorotiView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        if (!ReferenceEquals(view.platformDispatcher, Session.dispatcher))
            throw new InvalidOperationException("The view belongs to another application.");
        Session.DetachView(view);
        view.QuiesceCallbacks();
    }
    public void Start()
    {
        ObjectDisposedException.ThrowIf(_disposed || _stopping, this);
        if (_started) return;
        Session.Start(deferFrameworkBootstrap: true);
        _started = true;
    }
    public void OwnService<T>(T service) where T : class
    {
        ObjectDisposedException.ThrowIf(_disposed || _stopping, this);
        if (_services.Add(service)) Capabilities.Own(service);
    }
    public void Dispose()
    {
        if (_disposed) return;
        _stopping = true;
        Doroti.Runtime.DorotiCleanup.Run(Session.Dispose, Capabilities.Dispose);
        _disposed = true;
        _services.Clear();
    }
}
