using Doroti.Hosting;
using Doroti.Ui;
using SkiaSharp.Views.Maui.Controls;

namespace Doroti.Host.Maui;

public sealed class MauiFrameworkHost : IDisposable
{
    private readonly string _targetIdentity;
    private readonly Dictionary<ulong, (DorotiView View, MauiHostAdapter Host, MauiSkiaCapabilities Graphics)> _views = [];
    private readonly Dictionary<ulong, DorotiHostSession> _sessions = [];
    private bool _disposed;

    public MauiFrameworkHost(string? targetIdentity = null) => _targetIdentity = targetIdentity ??
#if WINDOWS
        "win-x64/winui3/SKSwapChainPanel/ANGLE-DirectX-Skia";
#else
        "maccatalyst-arm64/UIKit-MacCatalyst/SKMetalView/Metal-Skia";
#endif

    public DorotiView CreateView(
        DorotiHostSession session,
        ulong viewId,
        SKGLView nativeView,
        DorotiViewConfiguration configuration,
        IMauiSemanticsBridge? semantics = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(nativeView);
        ArgumentNullException.ThrowIfNull(configuration);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (session.state != DorotiHostSessionState.running)
            throw new InvalidOperationException("The Doroti host session must be running before a MAUI view is created.");

        var host = new MauiHostAdapter(viewId, nativeView, configuration.logicalSize, semantics);
        var graphics = new MauiSkiaCapabilities(viewId, host);
        var messages = new MauiPlatformMessageCapability();
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
            .Register<ISemanticsHostCapability>(DorotiCapabilityIds.AccessibilitySemantics, graphics)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, messages);
        DorotiView? view = null;
        try
        {
            using var dispatcherScope = session.dispatcher.EnterScope();
            view = session.dispatcher.RegisterView(viewId, capabilities);
            session.AttachView(view);
            _views.Add(viewId, (view, host, graphics));
            _sessions.Add(viewId, session);
            graphics.AttachSurface(host.RequestInvalidate);
            host.Show();
            return view;
        }
        catch
        {
            if (view is null) capabilities.Dispose();
            else view.Dispose();
            throw;
        }
    }

    internal void BeginPaint(ulong viewId, SkiaSharp.Views.Maui.SKPaintGLSurfaceEventArgs args,
        object? context, string nativeViewType)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");
        value.Host.BeginPaint(args, context, nativeViewType, _targetIdentity);
    }

    internal void PaintSkiaSurface(ulong viewId, SkiaSharp.SKSurface surface, int pixelWidth, int pixelHeight)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");
        value.Graphics.Paint(surface, pixelWidth, pixelHeight);
    }

    public MauiFrameDiagnostics CaptureFrameDiagnostics(ulong viewId) =>
        _views.TryGetValue(viewId, out var value)
            ? value.Graphics.Diagnostics
            : throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");

    public MauiHostDiagnostics CaptureDiagnostics(ulong viewId, string applicationSource, string bootstrapSource)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");
        return new(applicationSource, bootstrapSource, AppContext.TargetFrameworkName ?? "unknown",
#if WINDOWS
            "win-x64",
#else
            "maccatalyst-arm64",
#endif
            "10.0.90", "3.119.4", value.Host.Snapshot, value.Graphics.Diagnostics,
            value.Host.InvalidationsRequested, value.Host.InvalidationsCoalesced, 0);
    }

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

    private sealed class MauiPlatformMessageCapability : IPlatformMessageHostCapability
    {
        private readonly Dictionary<string, PlatformMessageHandler> _handlers = new(StringComparer.Ordinal);

        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _handlers.TryGetValue(channel, out var handler)
                ? handler(data, cancellationToken)
                : ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
        }

        public void SetMessageHandler(string channel, PlatformMessageHandler? handler)
        {
            if (handler is null) _handlers.Remove(channel);
            else _handlers[channel] = handler;
        }
    }
}
