using System.Runtime.InteropServices;
using Doroti.Hosting;
using Doroti.Ui;
using Microsoft.Maui.Controls;
#if !MACOS
using SkiaSharp.Views.Maui.Controls;
#endif

namespace Doroti.Host.Maui;

public sealed class MauiFrameworkHost : IDisposable
{
    private readonly string _targetIdentity;
    private readonly Dictionary<ulong, (DorotiView View, MauiHostAdapter Host, MauiSkiaCapabilities Graphics)> _views = [];
    private readonly Dictionary<ulong, DorotiHostSession> _sessions = [];
    private bool _disposed;

    public MauiFrameworkHost(string? targetIdentity = null) => _targetIdentity = targetIdentity ??
#if WINDOWS
        "win-x64/winui3/SwapChainPanel/Doroti-owned-D3D12-Skia";
#elif MACCATALYST
        "maccatalyst-arm64/UIKit-MacCatalyst/SKMetalView/Metal-Skia";
#elif IOS
        "ios/UIKit-iOS/SKMetalView/Metal-Skia";
#elif ANDROID
        $"{AndroidRuntimeIdentifier}/Android/MauiSKGLTextureView/OpenGL-ES-Skia";
#elif MACOS
        "osx-arm64/AppKit/MTKView/Metal-Skia";
#else
#error Doroti.Host.Maui requires an explicit platform identity.
#endif

#if !MACOS && !WINDOWS
    public DorotiView CreateView(
        DorotiHostSession session,
        ulong viewId,
        SKGLView nativeView,
        DorotiViewConfiguration configuration,
        IMauiSemanticsBridge? semantics = null,
        DorotiApplicationBoundary? application = null,
        MauiTextInputBridge? textInput = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(nativeView);
        textInput ??= new(new Entry(), new Editor());
        return CreateView(session, viewId, new MauiSkglSurface(textInput, viewId),
            configuration, semantics, application, textInput);
    }
#endif

    internal DorotiView CreateView(
        DorotiHostSession session,
        ulong viewId,
        IMauiSkiaSurface surface,
        DorotiViewConfiguration configuration,
        IMauiSemanticsBridge? semantics = null,
        DorotiApplicationBoundary? application = null,
        MauiTextInputBridge? textInput = null)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(configuration);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (session.state != DorotiHostSessionState.running)
            throw new InvalidOperationException("The Doroti host session must be running before a MAUI view is created.");

        textInput ??= new(new Entry(), new Editor());
        var host = new MauiHostAdapter(viewId, surface, textInput, configuration.logicalSize, semantics);
        var graphics = new MauiSkiaCapabilities(
            viewId, host, configuration.backgroundColor, configuration.darkBackgroundColor);
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
            .Register<ISemanticsHostCapability>(DorotiCapabilityIds.AccessibilitySemantics, graphics);
        if (application is null)
            capabilities.Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, messages);
        else
            application.Configure(capabilities, messages);
        DorotiView? view = null;
        try
        {
            using var dispatcherScope = session.dispatcher.EnterScope();
            view = session.dispatcher.RegisterView(viewId, capabilities);
            graphics.AttachFrameworkTrace(view.FrameTrace);
            host.AttachFrameworkTrace(view.FrameTrace);
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

    internal void BeginPaint(ulong viewId, MauiSkiaPaintContext paint)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");
        value.Host.BeginPaint(paint);
    }

    internal void EndPaint(ulong viewId)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");
        value.Host.EndPaint();
    }

    internal MauiPaintCompletion? PaintSkiaSurface(
        ulong viewId,
        SkiaSharp.SKSurface surface,
        int pixelWidth,
        int pixelHeight)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");
        return value.Graphics.Paint(surface, pixelWidth, pixelHeight);
    }

    internal void CompletePaint(ulong viewId, MauiPaintCompletion completion)
    {
        if (_views.TryGetValue(viewId, out var value)) value.Graphics.CompletePaint(completion);
    }

    internal void FailPaint(ulong viewId, MauiPaintCompletion completion, string reason)
    {
        if (_views.TryGetValue(viewId, out var value)) value.Graphics.FailPaint(completion, reason);
    }

    internal void NotifyLifecycle(ulong viewId, AppLifecycleState state)
    {
        if (_views.TryGetValue(viewId, out var value)) value.Host.NotifyLifecycle(state);
    }

    internal void NotifyCloseRequested(ulong viewId)
    {
        if (_views.TryGetValue(viewId, out var value)) value.Host.NotifyCloseRequested();
    }

    public MauiFrameDiagnostics CaptureFrameDiagnostics(ulong viewId) =>
        _views.TryGetValue(viewId, out var value)
            ? value.Graphics.Diagnostics
            : throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");

    public MauiHostDiagnostics CaptureDiagnostics(ulong viewId, string applicationSource, string bootstrapSource)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");
        return new(applicationSource, bootstrapSource,
#if WINDOWS
            ".NETCoreApp,Version=v10.0/Windows,Version=10.0.19041.0",
#elif MACCATALYST
            ".NETCoreApp,Version=v10.0/MacCatalyst,Version=15.0",
#elif IOS
            ".NETCoreApp,Version=v10.0/iOS,Version=15.0",
#elif ANDROID
            ".NETCoreApp,Version=v10.0/Android,Version=21.0",
#elif MACOS
            ".NETCoreApp,Version=v10.0/macOS,Version=14.0",
#else
#error Doroti.Host.Maui requires an explicit target framework identity.
#endif
#if WINDOWS
            "win-x64",
#elif MACCATALYST
            "maccatalyst-arm64",
#elif IOS
            RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? "ios-arm64" : "iossimulator-x64",
#elif ANDROID
            AndroidRuntimeIdentifier,
#elif MACOS
            "osx-arm64",
#else
#error Doroti.Host.Maui requires an explicit runtime identifier.
#endif
            "10.0.90", "4.151.1", value.Host.Snapshot, value.Graphics.Diagnostics,
            value.Host.InvalidationsRequested, value.Host.InvalidationsCoalesced,
            value.Host.NativePointerEvents, value.Host.FrameRequestsCoalesced,
            value.Host.SemanticsDiagnostics, 0);
    }

#if ANDROID
    private static string AndroidRuntimeIdentifier => RuntimeInformation.ProcessArchitecture switch
    {
        Architecture.Arm64 => "android-arm64",
        Architecture.X64 => "android-x64",
        var architecture => throw new PlatformNotSupportedException(
            $"Doroti MAUI does not support Android process architecture '{architecture}'."),
    };
#endif

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
