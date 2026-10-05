using Doroti.Ui;

namespace Doroti.Desktop;

public sealed partial class DorotiWindowManager(
    IWindowHostFactory factory,
    WindowLifetimePolicy lifetimePolicy = WindowLifetimePolicy.OnLastWindowClosed
) : IWindowService, IWindowingHostCapability
{
    private readonly IWindowHostFactory _factory =
        factory ?? throw new ArgumentNullException(nameof(factory));
    private readonly Dictionary<WindowId, DorotiWindowController> _windows = [];
    private readonly Dictionary<WindowId, DorotiWindowController> _initializingWindows = [];
    private readonly HashSet<Task> _initializationWork = [];
    private readonly SemaphoreSlim _creation = new(1);
    private readonly object _gate = new();
    private readonly SemaphoreSlim _exitLock = new(1);
    private bool _exiting;
    private bool _exitCompleted;
    private DorotiWindowController? _exitCandidate;
    public WindowManagerCapabilities Capabilities => _factory.Capabilities;
    public WindowLifetimePolicy LifetimePolicy { get; } = lifetimePolicy;
    public WindowId? MainWindowId { get; private set; }
    public event Action<DorotiWindowController>? WindowCreated;
    public event Action<DorotiWindowController>? WindowClosed;
    public event Action? ExitRequested;
    public event Action<DorotiWindowController, Exception>? InitializationFailed;

    /// <summary>Joins startup hooks, including hooks whose windows have already closed.
    /// Providers await this before releasing the application dispatcher or terminating the OS loop.</summary>
    public async Task WaitForInitializationAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            Task[] work;
            lock (_gate) work = _initializationWork.ToArray();
            if (work.Length == 0) return;
            await Task.WhenAll(work).WaitAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task TrackInitializationAsync(Task work)
    {
        try { await work.ConfigureAwait(false); }
        catch { /* The controller reports startup failures; this tracks lifetime completion. */ }
        finally { lock (_gate) _initializationWork.Remove(work); }
    }

    public IReadOnlyList<DorotiWindowController> GetWindows()
    {
        lock (_gate)
            return Array.AsReadOnly(_windows.Values.ToArray());
    }

    public bool TryGetWindow(WindowId id, out DorotiWindowController? window)
    {
        lock (_gate)
            return _windows.TryGetValue(id, out window);
    }

    public Task<DorotiWindowController> CreateWindowAsync(
        WindowCreateOptions options,
        CancellationToken cancellationToken = default
    ) => CreateAsync(options, false, cancellationToken);

    public Task<DorotiWindowController> CreateMainWindowAsync(
        WindowCreateOptions options,
        CancellationToken cancellationToken = default
    ) => CreateAsync(options, true, cancellationToken);

    /// <summary>Closes current windows through their cancellable close policy, then exits an Explicit lifetime.</summary>
    public async Task<bool> RequestExitAsync(CancellationToken cancellationToken = default)
    {
        await _exitLock.WaitAsync(cancellationToken);
        var completed = false;
        try
        {
            if (_exitCompleted) { completed = true; return true; }
            await _creation.WaitAsync(cancellationToken);
            try { lock (_gate) _exiting = true; }
            finally { _creation.Release(); }
            foreach (var window in GetWindows())
                if (!await window.CloseAsync(cancellationToken)) return false;
            completed = GetWindows().Count == 0;
            // Serialized exit callers join the same completed application lifetime.
            // Set before notifying so a failing subscriber cannot trigger a second exit.
            _exitCompleted = completed;
            if (completed && LifetimePolicy == WindowLifetimePolicy.Explicit) ExitRequested?.Invoke();
            return completed;
        }
        finally
        {
            if (!completed) { lock (_gate) _exiting = false; }
            _exitLock.Release();
        }
    }

    private async Task<DorotiWindowController> CreateAsync(
        WindowCreateOptions request,
        bool main,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.OwnerWindowId is { } explicitOwner && request.Options.OwnerWindowId is { } optionOwner && explicitOwner != optionOwner)
            throw new ArgumentException("Conflicting window owners.", nameof(request));
        request = request with { Options = request.Options with { OwnerWindowId = request.OwnerWindowId ?? request.Options.OwnerWindowId } };
        request.Options.Validate();
        if (request.Options.Kind != WindowKind.Regular && request.Options.OwnerWindowId is null)
            throw new ArgumentException("Owned window kinds require an explicit owner.", nameof(request));
        if (request.Options.OwnerWindowId is { } owner && (!TryGetWindow(owner, out var owningWindow) || owningWindow!.State.Closed || owningWindow.IsClosing))
            throw new InvalidOperationException("The owning window is not registered or is closed.");
        _factory.Evaluate(request).ThrowIfUnsupported();
        await _creation.WaitAsync(cancellationToken);
        DorotiWindowController? controller = null;
        IWindowHost? host = null;
        try
        {
            lock (_gate)
            {
                if (request.Options.OwnerWindowId is { } parent && (!_windows.TryGetValue(parent, out var owning) || owning.State.Closed || owning.IsClosing))
                    throw new InvalidOperationException("The owner closed while this window was being queued.");
                if (_exiting) throw new InvalidOperationException("The desktop application is exiting.");
                if (main && MainWindowId is not null)
                    throw new InvalidOperationException("Main window has already been assigned.");
                if (
                    (!main && !Capabilities.CanCreateAdditionalWindows)
                    || (Capabilities.MaximumWindows is { } max && _windows.Count >= max)
                )
                    throw new NotSupportedException(
                        "This host cannot create an additional native window."
                    );
            }
            var id = new WindowId(Guid.NewGuid());
            host = await _factory.CreateAsync(id, request, cancellationToken);
            controller = new(id, host, this, request.Options);
            lock (_gate)
            {
                _initializingWindows.Add(id, controller);
                _initializationWork.Add(controller.InitializationWork);
            }
            _ = TrackInitializationAsync(controller.InitializationWork);
            await controller.InitializeAsync(request, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            lock (_gate)
            {
                if (controller.State.Closed)
                    throw new ObjectDisposedException(
                        nameof(DorotiWindowController),
                        "Native window closed during creation."
                    );
                _initializingWindows.Remove(id);
                _windows.Add(id, controller);
                _exitCandidate = null;
                if (main)
                    MainWindowId = id;
            }
        }
        catch (Exception error)
        {
            if (controller is not null)
            {
                lock (_gate) _initializingWindows.Remove(controller.Id);
                try { await controller.AbortAsync(error); }
                finally { controller.FailBeforeStartup(error); }
            }
            else if (host is not null)
                await host.DisposeAsync();
            throw;
        }
        finally
        {
            _creation.Release();
            TryNotifyExit();
        }
        Notify(WindowCreated, controller);
        PublishWindow(controller);
        controller.Start(request);
        return controller;
    }

    internal void Remove(DorotiWindowController controller)
    {
        lock (_gate)
        {
            if (!_windows.Remove(controller.Id))
                return;
            if (!_windows.Values.Any(window => window.Kind == WindowKind.Regular))
                _exitCandidate = controller;
        }
        Notify(WindowClosed, controller);
        PublishWindow(controller);
        TryNotifyExit();
    }

    private void TryNotifyExit()
    {
        DorotiWindowController? controller;
        lock (_gate)
        {
            if (_windows.Values.Any(window => window.Kind == WindowKind.Regular) || _creation.CurrentCount == 0)
                return;
            controller = _exitCandidate;
            _exitCandidate = null;
            if (controller is not null && LifetimePolicy == WindowLifetimePolicy.OnLastWindowClosed) _exiting = true;
        }
        if (controller is not null && LifetimePolicy == WindowLifetimePolicy.OnLastWindowClosed)
            foreach (var callback in ExitRequested?.GetInvocationList() ?? [])
                try
                {
                    ((Action)callback)();
                }
                catch (Exception error)
                {
                    ReportFailure(controller, error);
                }
    }

    private void Notify(
        Action<DorotiWindowController>? callbacks,
        DorotiWindowController controller
    )
    {
        foreach (var callback in callbacks?.GetInvocationList() ?? [])
            try
            {
                ((Action<DorotiWindowController>)callback)(controller);
            }
            catch (Exception error)
            {
                ReportFailure(controller, error);
            }
    }

    internal void ReportFailure(DorotiWindowController controller, Exception error)
    {
        foreach (var callback in InitializationFailed?.GetInvocationList() ?? [])
            try
            {
                ((Action<DorotiWindowController, Exception>)callback)(controller, error);
            }
            catch
            { /* Diagnostic subscribers do not own native lifetime. */
            }
    }
}
