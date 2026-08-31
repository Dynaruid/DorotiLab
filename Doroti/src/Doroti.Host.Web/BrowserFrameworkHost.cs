using Doroti.Hosting;
using Doroti.Ui;
using System.Text.Json;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
public sealed class BrowserFrameworkHost : IDisposable
{
    private readonly string _targetIdentity;
    private readonly Doroti.Skia.Rendering.SkiaFallbackFontCollection _fallbackFonts = new();
    private CanvasKitResourceRegistry? _canvasKitResources;
    private readonly Dictionary<ulong, (DorotiView View, BrowserHostAdapter Host, IBrowserGraphicsCapabilities Graphics)> _views = [];
    private readonly Dictionary<ulong, DorotiHostSession> _sessions = [];
    private bool _disposed;

    public BrowserFrameworkHost(string targetIdentity = "browser-wasm/document-canvas-webgl2") =>
        _targetIdentity = targetIdentity;

    public string RegisterFont(ReadOnlyMemory<byte> bytes)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (UsesCanvasKitRenderer())
            return EnsureCanvasKitResources().RegisterFont(bytes);
        return _fallbackFonts.Register(bytes);
    }

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
        var backendIdentity = _targetIdentity == "browser-wasm/auto"
            ? $"browser-wasm/{BrowserHostRuntime.RendererIdentity}"
            : _targetIdentity;
        IBrowserGraphicsCapabilities graphics = UsesCanvasKitRenderer()
            ? new BrowserCanvasKitCapabilities(
                viewId, host,
                configuration.backgroundColor, configuration.darkBackgroundColor,
                backendIdentity, EnsureCanvasKitResources())
            : new BrowserSkiaCapabilities(
                viewId, host,
                configuration.backgroundColor, configuration.darkBackgroundColor,
                backendIdentity, _fallbackFonts);
        var messages = new BrowserPlatformMessageCapability(host);
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
            graphics.AttachFrameworkTrace(session.dispatcher.frameTrace);
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

    public IReadOnlyList<DorotiResizeTraceEntry> CaptureResizeTrace(ulong viewId) =>
        _views.TryGetValue(viewId, out var value)
            ? value.Host.CaptureResizeTrace()
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

    public string PaintSkiaSurface(
        ulong viewId,
        SkiaSharp.SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        DorotiResizeEpoch target,
        long requestId)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"Browser Doroti view {viewId} is not registered.");
        return value.Graphics.Paint(surface, pixelWidth, pixelHeight, target, requestId);
    }

    public void CompleteSkiaSurfacePaint(
        ulong viewId, long requestId, long generation, string terminal, string reason)
    {
        if (!_views.TryGetValue(viewId, out var value)) return;
        value.Graphics.CompletePaint(requestId, terminal, reason);
    }

    public void InvalidateSkiaGpuContext(ulong viewId, long requestId, string reason)
    {
        if (_views.TryGetValue(viewId, out var value))
            value.Graphics.InvalidateGpuContext(requestId, reason);
    }

    public void InvalidateSkiaWindowSurface(ulong viewId)
    {
        if (_views.TryGetValue(viewId, out var value))
            value.Graphics.InvalidateWindowSurfaceResources();
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
        _sessions.Clear();
        _canvasKitResources?.Dispose();
        _canvasKitResources = null;
        _fallbackFonts.Dispose();
    }

    private bool UsesCanvasKitRenderer() =>
        string.Equals(BrowserHostRuntime.RendererIdentity, "worker-canvaskit-webgl", StringComparison.Ordinal) ||
        _targetIdentity.EndsWith("/worker-canvaskit-webgl", StringComparison.Ordinal);

    private CanvasKitResourceRegistry EnsureCanvasKitResources() =>
        _canvasKitResources ??= new();

    private sealed class BrowserPlatformMessageCapability : IPlatformMessageHostCapability
    {
        private const string ContextMenuChannel = "flutter/contextmenu";
        private static readonly ReadOnlyMemory<byte> SuccessEnvelope =
            JsonSerializer.SerializeToUtf8Bytes(new object?[] { true });

        private readonly object _gate = new();
        private readonly BrowserHostAdapter _host;
        private readonly Dictionary<string, PlatformMessageHandler> _handlers = new(StringComparer.Ordinal);

        public BrowserPlatformMessageCapability(BrowserHostAdapter host) => _host = host;

        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(
            string channel,
            ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(channel);
            cancellationToken.ThrowIfCancellationRequested();
            if (string.Equals(channel, ContextMenuChannel, StringComparison.Ordinal))
                return HandleContextMenuMessage(data);

            PlatformMessageHandler? handler;
            lock (_gate)
            {
                _handlers.TryGetValue(channel, out handler);
            }
            return handler is null
                ? ValueTask.FromResult<ReadOnlyMemory<byte>?>(null)
                : handler(data, cancellationToken);
        }

        private ValueTask<ReadOnlyMemory<byte>?> HandleContextMenuMessage(ReadOnlyMemory<byte>? data)
        {
            if (data is null) return ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);

            using var document = JsonDocument.Parse(data.Value);
            if (!document.RootElement.TryGetProperty("method", out var methodElement))
                return ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);

            var enabled = methodElement.GetString() switch
            {
                "enableContextMenu" => true,
                "disableContextMenu" => false,
                _ => (bool?)null,
            };
            if (enabled is null) return ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);

            _host.SetBrowserContextMenuEnabled(enabled.Value);
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(SuccessEnvelope);
        }

        public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(channel);
            lock (_gate)
            {
                if (handler is null)
                {
                    _handlers.Remove(channel);
                }
                else
                {
                    _handlers[channel] = handler;
                }
            }
        }
    }
}
