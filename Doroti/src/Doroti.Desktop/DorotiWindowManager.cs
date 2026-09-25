using System.Runtime.CompilerServices;
using Doroti.Hosting;

namespace Doroti.Desktop;

public sealed class DorotiWindowManager(
    IWindowHostFactory factory,
    WindowLifetimePolicy lifetimePolicy = WindowLifetimePolicy.OnLastWindowClosed
)
{
    private readonly IWindowHostFactory _factory =
        factory ?? throw new ArgumentNullException(nameof(factory));
    private readonly Dictionary<WindowId, DorotiWindowController> _windows = [];
    private readonly ConditionalWeakTable<IDorotiViewEntrypoint, object> _usedContent = new();
    private readonly SemaphoreSlim _creation = new(1);
    private readonly object _gate = new();
    private DorotiWindowController? _exitCandidate;
    public WindowManagerCapabilities Capabilities => _factory.Capabilities;
    public WindowLifetimePolicy LifetimePolicy { get; } = lifetimePolicy;
    public WindowId? MainWindowId { get; private set; }
    public event Action<DorotiWindowController>? WindowCreated;
    public event Action<DorotiWindowController>? WindowClosed;
    public event Action? ExitRequested;
    public event Action<DorotiWindowController, Exception>? InitializationFailed;

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

    private async Task<DorotiWindowController> CreateAsync(
        WindowCreateOptions request,
        bool main,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Content);
        request.Options.Validate();
        if (request.OwnerWindowId is not null)
            throw new NotSupportedException("Owned windows are not implemented in this version.");
        _factory.Evaluate(request).ThrowIfUnsupported();
        await _creation.WaitAsync(cancellationToken);
        DorotiWindowController? controller = null;
        IWindowHost? host = null;
        try
        {
            lock (_gate)
            {
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
            var id = WindowId.New();
            host = await _factory.CreateAsync(id, request, cancellationToken);
            controller = new(id, host, this, request.Options);
            var content = request.Content.Create(new(controller, this));
            if (_usedContent.TryGetValue(content, out _))
                throw new InvalidOperationException(
                    "Content factory reused an entrypoint instance."
                );
            _usedContent.Add(content, new());
            await controller.InitializeAsync(request, content, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            lock (_gate)
            {
                if (controller.State.Closed)
                    throw new ObjectDisposedException(
                        nameof(DorotiWindowController),
                        "Native window closed during creation."
                    );
                _windows.Add(id, controller);
                _exitCandidate = null;
                if (main)
                    MainWindowId = id;
            }
        }
        catch (Exception error)
        {
            if (controller is not null)
                await controller.AbortAsync(error);
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
        controller.Start(request);
        return controller;
    }

    internal void Remove(DorotiWindowController controller)
    {
        lock (_gate)
        {
            if (!_windows.Remove(controller.Id))
                return;
            if (_windows.Count == 0)
                _exitCandidate = controller;
        }
        Notify(WindowClosed, controller);
        TryNotifyExit();
    }

    private void TryNotifyExit()
    {
        DorotiWindowController? controller;
        lock (_gate)
        {
            if (_windows.Count != 0 || _creation.CurrentCount == 0)
                return;
            controller = _exitCandidate;
            _exitCandidate = null;
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
