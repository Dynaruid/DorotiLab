#if IOS && !MACCATALYST
using SKGLView = Doroti.Host.Maui.DorotiSkiaView;
#endif
using System.Runtime.InteropServices;
using System.Reflection;
using System.Runtime.Versioning;
using Doroti.Hosting;
using Doroti.Ui;
using Microsoft.Maui.Controls;
#if !MACOS
using SkiaSharp.Views.Maui.Controls;
#endif

namespace Doroti.Host.Maui;

public sealed class MauiFrameworkHost : IDisposable
{
    // Inspect only known assembly metadata; do not report stale hardcoded SDK/package versions.
    private static readonly string BuildFrameworkIdentity =
        (typeof(MauiFrameworkHost).Assembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? "unknown") + "/" +
        (typeof(MauiFrameworkHost).Assembly.GetCustomAttribute<TargetPlatformAttribute>()?.PlatformName ?? "unknown");
    private static readonly string MauiPackageIdentity =
        typeof(Microsoft.Maui.Controls.Application).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "unknown";
    private static readonly string SkiaPackageIdentity =
        typeof(SkiaSharp.SKCanvas).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.Split('+')[0] ?? "unknown";

    private readonly string _targetIdentity;
    private readonly Dictionary<ulong, (DorotiView View, MauiHostAdapter Host, MauiSkiaCapabilities Graphics)> _views = [];
    private readonly Dictionary<ulong, DorotiHostSession> _sessions = [];
    private bool _disposed;

    public MauiFrameworkHost(string? targetIdentity = null) => _targetIdentity = targetIdentity ??
#if WINDOWS
        (WindowsCompositionSurfaceFeature.GraphiteEnabled ? "win-x64/WinUI/CompositionDrawingSurface/Graphite-Vulkan" : "win-x64/win32-child-hwnd/offscreen-copy/Doroti-owned-D3D12-Skia");
#elif MACCATALYST
        (DorotiGraphiteView.Enabled ? "maccatalyst-arm64/UIKit/MTKView/Graphite-Metal" : "maccatalyst-arm64/UIKit-MacCatalyst/SKMetalView/Metal-Skia");
#elif IOS
        (DorotiGraphiteView.Enabled ? "ios/UIKit/MTKView/Graphite-Metal" : "ios/UIKit-iOS/SKMetalView/Metal-Skia");
#elif ANDROID
        (DorotiGraphiteView.Enabled ? $"{AndroidRuntimeIdentifier}/Android/SurfaceView/Graphite-Vulkan" : $"{AndroidRuntimeIdentifier}/Android/MauiSKGLTextureView/OpenGL-ES-Skia");
#elif MACOS
        $"osx-arm64/{DorotiMacOSMetalView.GraphicsBackendId}";
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
        if (surface is IMauiGraphiteSurface graphiteSurface) graphics.AttachGraphiteLifecycle(graphiteSurface);
#if MACOS
        if (surface is DorotiMacOSMetalSurface metalSurface) graphics.AttachNativeLifecycle(metalSurface);
#endif
        var messages = new MauiPlatformMessageCapability();
        var capabilities = new DorotiViewCapabilities(_targetIdentity)
            .Register<IViewHostCapability>(DorotiCapabilityIds.WindowLifecycle, host)
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
            .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
            .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, host)
            .Register<IUrlLauncherHostCapability>(DorotiCapabilityIds.UrlLauncher, host)
            .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host)
            .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, graphics)
            .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, graphics)
                .Register<IFontHostCapability>(DorotiCapabilityIds.GraphicsFont, graphics)
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
        int pixelHeight,
        out bool shouldPresent)
    {
        if (!_views.TryGetValue(viewId, out var value))
            throw new KeyNotFoundException($"MAUI Doroti view {viewId} is not registered.");
        return value.Graphics.Paint(surface, pixelWidth, pixelHeight, out shouldPresent);
    }

    internal void CompletePaint(ulong viewId, MauiPaintCompletion completion)
    {
        if (_views.TryGetValue(viewId, out var value)) value.Graphics.CompletePaint(completion);
    }

    internal void FailPaint(ulong viewId, MauiPaintCompletion completion, string reason)
    {
        if (_views.TryGetValue(viewId, out var value)) value.Graphics.FailPaint(completion, reason);
    }

    internal void SupersedePaint(ulong viewId, MauiPaintCompletion completion, string reason)
    {
        if (_views.TryGetValue(viewId, out var value)) value.Graphics.SupersedePaint(completion, reason);
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
            BuildFrameworkIdentity,
#if WINDOWS
            "win-x64",
#elif MACCATALYST
            "maccatalyst-arm64",
#elif IOS
            RuntimeInformation.RuntimeIdentifier,
#elif ANDROID
            AndroidRuntimeIdentifier,
#elif MACOS
            "osx-arm64",
#else
#error Doroti.Host.Maui requires an explicit runtime identifier.
#endif
            MauiPackageIdentity, SkiaPackageIdentity, value.Host.Snapshot, value.Graphics.Diagnostics,
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
