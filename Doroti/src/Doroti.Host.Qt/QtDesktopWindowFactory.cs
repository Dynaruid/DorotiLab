using System.Runtime.InteropServices;
using System.Text;
using Doroti.Desktop;
using Doroti.Hosting;

namespace Doroti.Host.Qt;

/// <summary>One Qt GUI loop, isolated managed view contexts and native owners.</summary>
internal sealed class QtDesktopWindowFactory(DorotiApplicationDescriptor descriptor,
    QtDesktopWindowHost main, DorotiApplicationBoundary application) : IWindowHostFactory, IDisposable
{
    private bool _mainAllocated;
    private readonly List<WindowResources> _resources = [];
    public WindowManagerCapabilities Capabilities { get; } = new(true, null);
    public WindowEvaluation Evaluate(WindowCreateOptions options) => main.Evaluate(options);
    public ValueTask<IWindowHost> CreateAsync(WindowId id, WindowCreateOptions options, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (!_mainAllocated) { _mainAllocated = true; return ValueTask.FromResult<IWindowHost>(main); }
        return ValueTask.FromResult<IWindowHost>(new QtDesktopWindowHost(Start));
    }

    private Task Start(QtDesktopWindowHost host, WindowOptions options, CancellationToken token) =>
        QtApplicationDispatcher.Post(() =>
        {
            // Reap only after native teardown has completed all callbacks.
            foreach (var old in _resources.Where(item => item.Host.State.Closed).ToArray())
            { old.Dispose(); _resources.Remove(old); }
            var views = new QtPlatformViewHost();
            var boundary = application.CreateWindowBoundary(views.Factories);
            var configuration = DesktopApplication.ToViewConfiguration(options, WindowLifetimePolicy.OnLastWindowClosed);
            if (descriptor.ViewConfiguration.Navigation is { } navigation)
                configuration = configuration with { Navigation = navigation with { ProtocolScheme = null, RestorationId = null } };
            var session = new DorotiHostSession(host.Content);
            var state = new DorotiQtRunner.QtManagedState(session, boundary, configuration, views, null) { Desktop = host };
            host.Fatal = state.CaptureFatal;
            var resources = new WindowResources(host, views, boundary, session, state);
            _resources.Add(resources);
            try
            {
                session.Start(deferFrameworkBootstrap: true);
                Create(configuration, resources.Context);
                state.ThrowIfFatal();
            }
            catch (Exception error) { host.Fail(error); throw; }
        }, token);

    private static unsafe void Create(Doroti.Ui.DorotiViewConfiguration configuration, nint context)
    {
        var bytes = Encoding.UTF8.GetBytes(configuration.title);
        fixed (byte* title = bytes)
        {
            var native = new QtNativeV2.Configuration(new(title, (ulong)bytes.Length),
                (int)configuration.logicalSize.width, (int)configuration.logicalSize.height, 1, 0, 0);
            var callbacks = new QtNativeV2.Callbacks(context);
            var status = CreateNative(in native, in callbacks);
            if (status != 0) throw new InvalidOperationException($"Qt additional window creation failed ({status}).");
        }
    }
    [DllImport("doroti_qt_host", EntryPoint = "doroti_qt_create_window_v2", CallingConvention = CallingConvention.Cdecl)]
    private static extern int CreateNative(in QtNativeV2.Configuration configuration, in QtNativeV2.Callbacks callbacks);

    public void Dispose()
    {
        // Run has returned; native windows and callbacks have already drained.
        List<Exception> failures = [];
        foreach (var resources in _resources)
            try { resources.Dispose(); } catch (Exception error) { failures.Add(error); }
        _resources.Clear();
        if (failures.Count != 0) throw new AggregateException(failures);
    }
    private sealed class WindowResources(QtDesktopWindowHost host, QtPlatformViewHost views,
        DorotiApplicationBoundary application, DorotiHostSession session, DorotiQtRunner.QtManagedState state) : IDisposable
    {
        private GCHandle _handle = GCHandle.Alloc(state);
        internal QtDesktopWindowHost Host => host;
        internal nint Context => GCHandle.ToIntPtr(_handle);
        public void Dispose()
        {
            try { state.Dispose(); session.Dispose(); views.Dispose(); application.Dispose(); }
            finally { if (_handle.IsAllocated) _handle.Free(); }
            state.ThrowIfFatal();
            state.ValidateTerminalCoverage();
            state.WriteDiagnostics();
        }
    }
}
