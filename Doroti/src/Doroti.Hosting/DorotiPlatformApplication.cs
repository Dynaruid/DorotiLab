using Doroti.Ui;

namespace Doroti.Hosting;

public sealed record DorotiProcessContext(Guid ApplicationId, DorotiLaunchContext LaunchContext);

public sealed record DorotiPlatformViewRequest(
    Guid ApplicationId,
    long Generation,
    DorotiViewConfiguration Configuration,
    PlatformDispatcher Dispatcher,
    bool AdoptPrimary
);

public interface IDorotiPlatformProvider
{
    string Id { get; }
    ValueTask<IDorotiPreparedPlatform> PrepareProcessAsync(
        DorotiProcessContext context, CancellationToken cancellationToken);
}

public interface IDorotiPreparedPlatform : IAsyncDisposable
{
    IDorotiApplicationDispatcher ApplicationDispatcher { get; }
    void RegisterApplicationCapabilities(DorotiCapabilityLifetime lifetime);
    ValueTask<IDorotiPlatformViewLease> CreateViewAsync(
        DorotiPlatformViewRequest request,
        IDorotiCapabilityRegistrar capabilities,
        CancellationToken cancellationToken);
}

/// <summary>The native factory owns a view, never a widget root or framework session.</summary>
public interface IDorotiPlatformViewLease : IAsyncDisposable
{
    ulong ViewId { get; }
    ValueTask DrainAsync(CancellationToken cancellationToken);
}

public enum DorotiPlatformApplicationState
{
    Created,
    PreparingProcess,
    ProcessPrepared,
    ConfiguringApplication,
    CreatingView,
    ViewAttached,
    FirstFrameSubmitted,
    Stopping,
    Stopped,
    Failed,
}

/// <summary>Stores factories without executing startup or plugin constructors.</summary>
public sealed class DorotiLaunchPlan
{
    public DorotiLaunchPlan(IDorotiPlatformProvider provider, DorotiLaunchContext launchContext,
        Func<DorotiApplicationDescriptor> configureApplication, TimeSpan? drainTimeout = null)
    {
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        ArgumentNullException.ThrowIfNull(launchContext);
        LaunchContext = launchContext with { Arguments = Array.AsReadOnly(launchContext.Arguments.ToArray()) };
        ConfigureApplication = configureApplication ?? throw new ArgumentNullException(nameof(configureApplication));
        DrainTimeout = drainTimeout ?? TimeSpan.FromSeconds(30);
        if (DrainTimeout <= TimeSpan.Zero || DrainTimeout > TimeSpan.FromMinutes(20))
            throw new ArgumentOutOfRangeException(nameof(drainTimeout));
    }

    public IDorotiPlatformProvider Provider { get; }
    public DorotiLaunchContext LaunchContext { get; }
    public TimeSpan DrainTimeout { get; }
    internal Func<DorotiApplicationDescriptor> ConfigureApplication { get; }

    public static DorotiLaunchPlan Create<TStartup>(IDorotiPlatformProvider provider,
        DorotiLaunchContext context, IEnumerable<Func<IDorotiNativePluginHandler>>? nativePluginHandlerFactories = null)
        where TStartup : IDorotiApplicationStartup, new()
    {
        ArgumentNullException.ThrowIfNull(context);
        var snapshot = context with { Arguments = Array.AsReadOnly(context.Arguments.ToArray()) };
        var factories = nativePluginHandlerFactories?.ToArray();
        return new(provider, snapshot, () => DorotiApplicationFactory.Create<TStartup>(snapshot,
            nativePluginHandlerFactories: factories));
    }
}

public sealed class DorotiPlatformDrainException(ulong viewId, Exception error)
    : InvalidOperationException($"View {viewId} failed to drain. Its native, capability and process leases are retained; retry StopAsync.", error)
{
    public ulong ViewId { get; } = viewId;
}

/// <summary>Staged process preparation, one application session, and independently drained views.</summary>
public sealed class DorotiPlatformApplication(DorotiLaunchPlan plan) : IAsyncDisposable
{
    private sealed class AttachedView(IDorotiPlatformViewLease native, DorotiViewCapabilities capabilities, long generation)
    {
        public IDorotiPlatformViewLease Native { get; } = native;
        public DorotiViewCapabilities Capabilities { get; } = capabilities;
        public DorotiView? View { get; set; }
        public bool Detached { get; set; }
        public long Generation { get; } = generation;
        public Task? NativeDrain { get; set; }
    }

    private readonly DorotiLaunchPlan _plan = plan ?? throw new ArgumentNullException(nameof(plan));
    private readonly object _gate = new();
    private readonly SemaphoreSlim _viewsGate = new(1);
    private readonly CancellationTokenSource _lifetime = new();
    private readonly DorotiCapabilityLifetime _applicationCapabilities = new();
    private readonly List<AttachedView> _views = [];
    private readonly Guid _applicationId = Guid.NewGuid();
    private IDorotiPreparedPlatform? _process;
    private DorotiApplicationDescriptor? _descriptor;
    private DorotiHostSession? _session;
    private DorotiManagedPluginRegistry? _managedPlugins;
    private Task? _start;
    private Task? _stop;
    private long _generation;
    private DorotiPlatformApplicationState _state;

    public Guid ApplicationId => _applicationId;
    public DorotiPlatformApplicationState State { get { lock (_gate) return _state; } }
    public event Action<DorotiPlatformApplicationState>? StageChanged;

    private void Stage(DorotiPlatformApplicationState state)
    {
        lock (_gate) _state = state;
        StageChanged?.Invoke(state);
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        lock (_gate)
        {
            if (_start is not null || _state != DorotiPlatformApplicationState.Created)
                throw new InvalidOperationException("An application can start exactly once; restart uses a new application generation.");
            var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _start = completion.Task;
            _ = CompleteStartAsync(completion, cancellationToken);
            return _start;
        }
    }

    private async Task CompleteStartAsync(TaskCompletionSource completion, CancellationToken cancellationToken)
    {
        try { await StartCoreAsync(cancellationToken); completion.TrySetResult(); }
        catch (OperationCanceledException canceled) { completion.TrySetCanceled(canceled.CancellationToken); }
        catch (Exception failure) { completion.TrySetException(failure); }
    }

    private async Task StartCoreAsync(CancellationToken cancellationToken)
    {
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        try
        {
            Stage(DorotiPlatformApplicationState.PreparingProcess);
            _process = await _plan.Provider.PrepareProcessAsync(new(_applicationId, _plan.LaunchContext), linked.Token);
            linked.Token.ThrowIfCancellationRequested();
            Stage(DorotiPlatformApplicationState.ProcessPrepared);
            Stage(DorotiPlatformApplicationState.ConfiguringApplication);
            _descriptor = await _process.ApplicationDispatcher.InvokeAsync(() =>
            {
                var descriptor = _plan.ConfigureApplication();
                foreach (var handler in descriptor.NativePluginHandlers) _applicationCapabilities.Own(handler);
                return descriptor;
            }, linked.Token);
            await _process.ApplicationDispatcher.InvokeAsync(() =>
            {
                _managedPlugins = _applicationCapabilities.Own(new DorotiManagedPluginRegistry(_applicationCapabilities));
                foreach (var registration in _descriptor.ManagedPluginRegistrations ?? []) registration.Register(_managedPlugins);
                _managedPlugins.Seal();
                _process.RegisterApplicationCapabilities(_applicationCapabilities);
                _session = new DorotiHostSession(_descriptor.EntrypointFactory(), coordinatorOwnsFailureCleanup: true,
                    applicationDispatcher: _process.ApplicationDispatcher, applicationId: _applicationId);
                _session.Start(deferFrameworkBootstrap: true);
            }, linked.Token);
            await CreateViewCoreAsync(adoptPrimary: true, linked.Token);
        }
        catch (Exception startup)
        {
            try { await CleanupAsync(); }
            catch (Exception cleanup)
            {
                Stage(DorotiPlatformApplicationState.Failed);
                throw new AggregateException(startup, cleanup);
            }
            Stage(DorotiPlatformApplicationState.Failed);
            throw;
        }
    }

    public async Task<ulong> CreateViewAsync(CancellationToken cancellationToken = default)
    {
        if (State is not (DorotiPlatformApplicationState.ViewAttached or DorotiPlatformApplicationState.FirstFrameSubmitted))
            throw new InvalidOperationException("The application must be running before creating an additional view.");
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _lifetime.Token);
        return await CreateViewCoreAsync(adoptPrimary: false, linked.Token);
    }

    private async Task<ulong> CreateViewCoreAsync(bool adoptPrimary, CancellationToken cancellationToken)
    {
        await _viewsGate.WaitAsync(cancellationToken);
        var capabilities = new DorotiViewCapabilities(_plan.Provider.Id, _applicationCapabilities);
        AttachedView? attached = null;
        try
        {
            capabilities.Register<IDorotiManagedPluginInvoker>(DorotiCapabilityIds.ManagedPlugins,
                _managedPlugins ?? throw new InvalidOperationException("Managed plugins are not configured."), DorotiCapabilityOwnership.Borrowed);
            cancellationToken.ThrowIfCancellationRequested();
            var process = _process ?? throw new InvalidOperationException("The process is not prepared.");
            var session = _session ?? throw new InvalidOperationException("The application is not configured.");
            if (adoptPrimary) Stage(DorotiPlatformApplicationState.CreatingView);
            var request = new DorotiPlatformViewRequest(_applicationId, ++_generation,
                _descriptor!.ViewConfiguration, session.dispatcher, adoptPrimary);
            var native = await process.CreateViewAsync(request, capabilities.Registrar, cancellationToken);
            attached = new(native, capabilities, request.Generation);
            lock (_gate) _views.Add(attached);
            cancellationToken.ThrowIfCancellationRequested();
            await process.ApplicationDispatcher.InvokeAsync(() =>
            {
                // RegisterView seals the limited registrar before framework bootstrap/attachment.
                attached.View = session.dispatcher.RegisterView(native.ViewId, capabilities, request.Generation);
                session.AttachView(attached.View);
            }, cancellationToken);
            if (adoptPrimary) Stage(DorotiPlatformApplicationState.ViewAttached);
            return native.ViewId;
        }
        catch (Exception failure)
        {
            try
            {
                // A failed Bootstrap may have created framework resources before
                // throwing. Unmount those while the native/capability leases live.
                if (_session?.state == DorotiHostSessionState.shutDown)
                    await _process!.ApplicationDispatcher.InvokeAsync(_session.ShutdownFramework);
                if (attached is null) capabilities.Dispose();
                else await ReleaseViewAsync(attached);
            }
            catch (Exception cleanup) { throw new AggregateException(failure, cleanup); }
            throw;
        }
        finally { _viewsGate.Release(); }
    }

    public void MarkFirstFrameSubmitted(ulong viewId, long generation)
    {
        lock (_gate)
        {
            if (_lifetime.IsCancellationRequested || _session is null ||
                !_views.Any(view => view.View?.viewId == viewId && view.Generation == generation && !view.Detached))
                throw new InvalidOperationException("A frame submission must belong to a live attached view generation.");
            _state = DorotiPlatformApplicationState.FirstFrameSubmitted;
        }
        StageChanged?.Invoke(DorotiPlatformApplicationState.FirstFrameSubmitted);
    }

    public async Task CloseViewAsync(ulong viewId, CancellationToken cancellationToken = default)
    {
        await _viewsGate.WaitAsync(cancellationToken);
        try
        {
            AttachedView attached;
            lock (_gate) attached = _views.SingleOrDefault(view => view.Native.ViewId == viewId)
                ?? throw new InvalidOperationException("The view is not attached to this application.");
            await ReleaseViewAsync(attached);
        }
        finally { _viewsGate.Release(); }
    }

    private async Task ReleaseViewAsync(AttachedView attached)
    {
        if (attached.View is not null && !attached.Detached)
        {
            await _process!.ApplicationDispatcher.InvokeAsync(() =>
            {
                _session!.DetachView(attached.View);
                attached.View.QuiesceCallbacks();
            });
            lock (_gate) attached.Detached = true;
        }
        try
        {
            if (attached.View is not null)
                await attached.View.WaitForInvocationDrainAsync().WaitAsync(_plan.DrainTimeout);
            // A timeout retains an in-progress native drain. Retry joins that
            // same operation instead of starting a second consumer drain.
            attached.NativeDrain ??= attached.Native.DrainAsync(CancellationToken.None).AsTask();
            try { await attached.NativeDrain.WaitAsync(_plan.DrainTimeout); }
            catch
            {
                if (attached.NativeDrain.IsCompleted) attached.NativeDrain = null;
                throw;
            }
        }
        catch (Exception error) { throw new DorotiPlatformDrainException(attached.Native.ViewId, error); }
        await _process!.ApplicationDispatcher.InvokeAsync(() =>
        {
            if (attached.View is not null) attached.View.Dispose();
            else attached.Capabilities.Dispose();
        });
        await attached.Native.DisposeAsync();
        lock (_gate) _views.Remove(attached);
    }

    public Task StopAsync()
    {
        lock (_gate)
        {
            if (_stop is not null && !_stop.IsFaulted) return _stop;
            var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _stop = completion.Task;
            Exception? cancellationFailure = null;
            try { _lifetime.Cancel(); }
            catch (Exception failure) { cancellationFailure = failure; }
            _ = CompleteStopAsync(completion, cancellationFailure);
            return _stop;
        }
    }

    private async Task CompleteStopAsync(TaskCompletionSource completion, Exception? cancellationFailure)
    {
        try
        {
            await StopCoreAsync();
            if (cancellationFailure is not null) throw cancellationFailure;
            completion.TrySetResult();
        }
        catch (Exception failure) { completion.TrySetException(failure); }
    }

    private async Task StopCoreAsync()
    {
        if (_start is not null)
        {
            try { await _start; }
            catch { /* The original startup error remains on StartAsync. Retry retained cleanup below. */ }
        }
        Stage(DorotiPlatformApplicationState.Stopping);
        try
        {
            await CleanupAsync();
            Stage(DorotiPlatformApplicationState.Stopped);
        }
        catch
        {
            Stage(DorotiPlatformApplicationState.Failed);
            throw;
        }
    }

    private async Task CleanupAsync()
    {
        await _viewsGate.WaitAsync();
        try
        {
            if (_session is not null)
                await _process!.ApplicationDispatcher.InvokeAsync(_session.ShutdownFramework);
            AttachedView[] views;
            lock (_gate) views = _views.ToArray();
            foreach (var view in views.Reverse()) await ReleaseViewAsync(view);
            if (_session is not null)
            {
                await _process!.ApplicationDispatcher.InvokeAsync(_session.Dispose);
                _session = null;
            }
            if (_process is not null)
                await _process.ApplicationDispatcher.InvokeAsync(_applicationCapabilities.Dispose);
            else _applicationCapabilities.Dispose();
            if (_process is not null)
            {
                await _process.DisposeAsync();
                _process = null;
            }
        }
        finally { _viewsGate.Release(); }
    }

    public ValueTask DisposeAsync() => new(StopAsync());
}
