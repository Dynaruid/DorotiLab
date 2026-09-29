using Doroti.Desktop;
using Doroti.Hosting;

namespace Doroti.Host.WindowsAppSdk;

internal sealed class WindowsDesktopWindowFactory(DorotiApplicationDescriptor descriptor,
    string presenter, WindowsAppSdkDesktopWindowHost main, DorotiApplicationBoundary application) : IWindowHostFactory
{
    private readonly DorotiApplicationBoundary _application = application.Retain();
    private bool _mainAllocated;
    private readonly List<Task> _nativeLoops = [];
    private readonly List<DorotiWindowController> _controllers = [];
    private readonly List<IWindowHost> _additionalHosts = [];
    private readonly TaskCompletionSource _exit = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public WindowManagerCapabilities Capabilities { get; } = new(true, null);
    public WindowEvaluation Evaluate(WindowCreateOptions options) => main.Evaluate(options);
    public ValueTask<IWindowHost> CreateAsync(WindowId id, WindowCreateOptions options, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!_mainAllocated) { _mainAllocated = true; return ValueTask.FromResult<IWindowHost>(main); }
        lock (_nativeLoops) _nativeLoops.RemoveAll(task => task.IsCompletedSuccessfully);
        lock (_additionalHosts) _additionalHosts.RemoveAll(host => host.State.Closed &&
            host is ThreadedWindowHost threaded && threaded.NativeLoop.IsCompletedSuccessfully);
        lock (_controllers) _controllers.RemoveAll(controller => controller.State.Closed && controller.InitializationWork.IsCompletedSuccessfully);
        var host = new ThreadedWindowHost(descriptor, presenter, id, options.Options, _application.Retain());
        lock (_additionalHosts) _additionalHosts.Add(host);
        lock (_nativeLoops) _nativeLoops.Add(host.NativeLoop);
        return ValueTask.FromResult<IWindowHost>(host);
    }
    public void Exit() => _exit.TrySetResult();
    public void Track(DorotiWindowController controller) { lock (_controllers) _controllers.Add(controller); }
    public void Abort()
    {
        IWindowHost[] hosts;
        lock (_additionalHosts) hosts = _additionalHosts.ToArray();
        try
        {
            Task.WhenAll(hosts.Select(host => host.DisposeAsync().AsTask()))
                .WaitAsync(TimeSpan.FromSeconds(30)).GetAwaiter().GetResult();
        }
        finally { _application.Dispose(); }
    }
    public void WaitForExit()
    {
        try { WaitForExitCore(); }
        finally { _application.Dispose(); }
    }
    private void WaitForExitCore()
    {
        _exit.Task.GetAwaiter().GetResult();
        Task[] loops;
        lock (_nativeLoops) loops = _nativeLoops.ToArray();
        Task.WhenAll(loops).GetAwaiter().GetResult();
        DorotiWindowController[] controllers;
        lock (_controllers) controllers = _controllers.ToArray();
        Task.WhenAll(controllers.Select(controller => controller.InitializationWork))
            .WaitAsync(TimeSpan.FromSeconds(30)).GetAwaiter().GetResult();
    }

    private sealed class ThreadedWindowHost(DorotiApplicationDescriptor descriptor, string presenter,
        WindowId id, WindowOptions options, DorotiApplicationBoundary application) : IWindowHost
    {
        private readonly TaskCompletionSource<WindowsAppSdkDesktopWindowHost> _initialized = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource _loop = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private WindowsAppSdkDesktopWindowHost? _host;
        private bool _started;
        public Task NativeLoop => _loop.Task;
        public WindowCapabilities Capabilities => _host?.Capabilities ?? new((_, _) => WindowEvaluation.Supported);
        public WindowState State { get; private set; } = new(null, options.Size, 1, false, false,
            WindowPresentationState.Normal, options.Appearance, new(options.Appearance), new(0, Ui.ViewPadding.zero, 0, 0, 0));
        public Task ReadyToShow => _ready.Task;
        public event Action<WindowState>? StateChanged;
        public event Action? CloseRequested;
        public event Action? Closed;

        public Task InitializeAsync(WindowOptions windowOptions, DesktopWindowContext context,
            IDorotiViewEntrypoint content, CancellationToken cancellationToken)
        {
            if (_started) throw new InvalidOperationException("Window already initialized.");
            cancellationToken.ThrowIfCancellationRequested();
            _started = true;
            var thread = new Thread(() =>
            {
                Exception? failure = null;
                try
                {
                    using var contextDispatcher = new Ui.PlatformDispatcher();
                    using var isolated = contextDispatcher.EnterScope();
                    var host = new WindowsAppSdkDesktopWindowHost();
                    _host = host;
                    host.StateChanged += state => { State = state; StateChanged?.Invoke(state); };
                    host.CloseRequested += () => CloseRequested?.Invoke();
                    host.Closed += () => { State = host.State; Closed?.Invoke(); };
                    host.InitializeAsync(windowOptions, context, content, cancellationToken).GetAwaiter().GetResult();
                    _ = ObserveReady(host.ReadyToShow);
                    _initialized.TrySetResult(host);
                    var configuration = DesktopApplication.ToViewConfiguration(windowOptions, WindowLifetimePolicy.OnLastWindowClosed);
                    if (descriptor.ViewConfiguration.Navigation is { } navigation)
                        configuration = configuration with { Navigation = navigation with
                        { ProtocolScheme = null, RestorationId = null } };
                    DorotiWindowsAppSdkRunner.RunAdditionalWindow(descriptor with { ViewConfiguration = configuration }, presenter, host, application);
                }
                catch (Exception error)
                {
                    failure = error;
                    _initialized.TrySetException(error);
                    _ready.TrySetException(error);
                    try { if (_host is { } host) host.CompleteShutdown(error); }
                    catch (Exception shutdown) { failure = new AggregateException(error, shutdown); }
                }
                finally
                {
                    try { application.Dispose(); }
                    catch (Exception cleanup) { failure = failure is null ? cleanup : new AggregateException(failure, cleanup); }
                    if (failure is null) _loop.TrySetResult(); else _loop.TrySetException(failure);
                }
            }) { IsBackground = false, Name = "Doroti window " + id };
            thread.SetApartmentState(ApartmentState.STA);
            // A new window must not inherit the caller's active dispatcher/timer scope.
            try
            {
                if (ExecutionContext.IsFlowSuppressed()) thread.Start();
                else { using (ExecutionContext.SuppressFlow()) thread.Start(); }
            }
            catch (Exception error)
            {
                application.Dispose();
                _initialized.TrySetException(error); _ready.TrySetException(error); _loop.TrySetException(error);
                throw;
            }
            return _initialized.Task;
        }
        private async Task ObserveReady(Task ready)
        {
            try { await ready; _ready.TrySetResult(); }
            catch (Exception error) { _ready.TrySetException(error); }
        }
        public async Task<WindowState> ExecuteAsync(WindowCommand command, CancellationToken cancellationToken) =>
            await (await _initialized.Task.WaitAsync(cancellationToken)).ExecuteAsync(command, cancellationToken);
        public async Task<WindowState> ApplyAppearanceAsync(WindowAppearanceOptions appearance, CancellationToken cancellationToken) =>
            await (await _initialized.Task.WaitAsync(cancellationToken)).ApplyAppearanceAsync(appearance, cancellationToken);
        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            await (await _initialized.Task.WaitAsync(cancellationToken)).CloseAsync(cancellationToken);
            await _loop.Task;
        }
        public async ValueTask DisposeAsync()
        {
            if (!_started) { application.Dispose(); _loop.TrySetResult(); return; }
            if (_initialized.Task.IsCompletedSuccessfully) await _initialized.Task.Result.DisposeAsync();
            await _loop.Task;
        }
    }
}
