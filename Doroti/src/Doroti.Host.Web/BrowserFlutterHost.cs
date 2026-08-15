using Doroti.Flutter.Hosting;
using Doroti.Flutter.Ui;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public sealed class BrowserFlutterHost : IDisposable
{
    private readonly string _targetIdentity;
    private readonly Dictionary<ulong, (FlutterView View, BrowserHostAdapter Host)> _views = [];
    private readonly Dictionary<ulong, FlutterHostSession> _sessions = [];
    private bool _disposed;

    public BrowserFlutterHost(string targetIdentity = "browser-wasm/document-canvas-webgl2") =>
        _targetIdentity = targetIdentity;

    public FlutterView CreateView(
        FlutterHostSession session,
        ulong viewId,
        string canvasId,
        FlutterViewConfiguration configuration,
        FlutterApplicationBoundary? application = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(configuration);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (session.state != FlutterHostSessionState.running)
            throw new InvalidOperationException("The Flutter host session must be running before a browser view is created.");

        var host = new BrowserHostAdapter(canvasId, configuration.logicalSize);
        var messages = new BrowserPlatformMessageCapability(_targetIdentity);
        var capabilities = new FlutterViewCapabilities(_targetIdentity)
            .Register<IViewHostCapability>(FlutterCapabilityIds.WindowLifecycle, host)
            .Register<IViewHostCapability>(FlutterCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(FlutterCapabilityIds.ViewFrameDispatch, host)
            .Register<IPlatformEnvironmentHostCapability>(FlutterCapabilityIds.PlatformEnvironment, host);
        if (application is null)
            capabilities.Register<IPlatformMessageHostCapability>(FlutterCapabilityIds.PlatformMessaging, messages);
        else
            application.Configure(capabilities, messages);

        FlutterView? view = null;
        try
        {
            view = session.dispatcher.RegisterView(viewId, capabilities);
            session.AttachView(view);
            _views.Add(viewId, (view, host));
            _sessions.Add(viewId, session);
            return view;
        }
        catch
        {
            if (view is null) capabilities.Dispose();
            else view.Dispose();
            throw;
        }
    }

    public BrowserHostSnapshot CaptureSnapshot(ulong viewId) =>
        _views.TryGetValue(viewId, out var value)
            ? value.Host.Snapshot
            : throw new KeyNotFoundException($"Browser Flutter view {viewId} is not registered.");

    public string ResolveResourceUrl(ulong viewId, string relativeUrl) =>
        _views.TryGetValue(viewId, out var value)
            ? value.Host.ResolveResourceUrl(relativeUrl)
            : throw new KeyNotFoundException($"Browser Flutter view {viewId} is not registered.");

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (var (viewId, value) in _views.Reverse().ToArray())
        {
            if (_sessions.Remove(viewId, out var session)) session.DetachView(value.View);
            value.View.Dispose();
        }
        _views.Clear();
    }

    private sealed class BrowserPlatformMessageCapability(string targetIdentity) : IPlatformMessageHostCapability
    {
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
            string channel,
            ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default) =>
            ValueTask.FromException<ReadOnlyMemory<byte>?>(new FlutterCapabilityException(
                FlutterCapabilityIds.PlatformPlugins, null,
                DartUiInvocation.Managed($"platform-channel:{channel}"),
                "no target-manifest JavaScript plugin implements this channel", targetIdentity));

        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
            throw new NotSupportedException("Browser framework channel handlers are owned by the G7-4 live adapter.");
    }
}
