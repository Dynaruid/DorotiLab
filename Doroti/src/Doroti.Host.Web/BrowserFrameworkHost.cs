using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public sealed class BrowserFrameworkHost : IDisposable
{
    private readonly string _targetIdentity;
    private readonly Dictionary<ulong, (DorotiView View, BrowserHostAdapter Host, BrowserSkiaCapabilities Graphics)> _views = [];
    private readonly Dictionary<ulong, DorotiHostSession> _sessions = [];
    private bool _disposed;

    public BrowserFrameworkHost(string targetIdentity = "browser-wasm/document-canvas-webgl2") =>
        _targetIdentity = targetIdentity;

    public DorotiView CreateView(
        DorotiHostSession session,
        ulong viewId,
        string canvasId,
        DorotiViewConfiguration configuration,
        DorotiApplicationBoundary? application = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(configuration);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (session.state != DorotiHostSessionState.running)
            throw new InvalidOperationException("The Doroti host session must be running before a browser view is created.");

        var host = new BrowserHostAdapter(viewId, canvasId, configuration.logicalSize);
        var graphics = new BrowserSkiaCapabilities(viewId, host);
        var messages = new BrowserPlatformMessageCapability(_targetIdentity);
        var capabilities = new DorotiViewCapabilities(_targetIdentity)
            .Register<IViewHostCapability>(DorotiCapabilityIds.WindowLifecycle, host)
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, graphics)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, graphics)
            .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, graphics)
            .Register<ISemanticsHostCapability>(DorotiCapabilityIds.AccessibilitySemantics, graphics);
        if (application is null)
            capabilities.Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, messages);
        else
            application.Configure(capabilities, messages);

        DorotiView? view = null;
        try
        {
            view = session.dispatcher.RegisterView(viewId, capabilities);
            session.AttachView(view);
            _views.Add(viewId, (view, host, graphics));
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
            : throw new KeyNotFoundException($"Browser Doroti view {viewId} is not registered.");

    public BrowserFrameDiagnostics CaptureFrameDiagnostics(ulong viewId) =>
        _views.TryGetValue(viewId, out var value)
            ? value.Graphics.Diagnostics
            : throw new KeyNotFoundException($"Browser Doroti view {viewId} is not registered.");

    public void AttachSkiaSurface(ulong viewId, Action invalidate)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"Browser Doroti view {viewId} is not registered.");
        value.Graphics.AttachSurface(invalidate);
    }

    public void PaintSkiaSurface(ulong viewId, SkiaSharp.SKSurface surface, int pixelWidth, int pixelHeight)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"Browser Doroti view {viewId} is not registered.");
        value.Graphics.Paint(surface, pixelWidth, pixelHeight);
    }

    public string ResolveResourceUrl(ulong viewId, string relativeUrl) =>
        _views.TryGetValue(viewId, out var value)
            ? value.Host.ResolveResourceUrl(relativeUrl)
            : throw new KeyNotFoundException($"Browser Doroti view {viewId} is not registered.");

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
            ValueTask.FromException<ReadOnlyMemory<byte>?>(new DorotiCapabilityException(
                DorotiCapabilityIds.PlatformPlugins, null,
                DartUiInvocation.Managed($"platform-channel:{channel}"),
                "no target-manifest JavaScript plugin implements this channel", targetIdentity));

        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) =>
            throw new NotSupportedException("Browser framework channel handlers are owned by the G7-4 live adapter.");
    }
}
