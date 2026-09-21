using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;
using Color = Doroti.Ui.Color;
using UiImage = Doroti.Ui.Image;

namespace Doroti.Host.Maui;

internal sealed class MauiSkiaCapabilities
    : ISceneHostCapability,
        IParagraphHostCapability,
        IFontHostCapability,
        IImageHostCapability,
        ITextureHostCapability,
        ISemanticsHostCapability,
        IDisposable
{
    private readonly MauiHostAdapter _host;
    private readonly SkiaSceneRenderer _renderer;
    public TextureRegistry Textures => _renderer.Textures;
    private IMauiGraphiteSurface? _graphiteSurface;

    internal void AttachGraphiteLifecycle(IMauiGraphiteSurface surface)
    {
        _graphiteSurface = surface;
        surface.GpuResourcesReleasing += _renderer.InvalidateGpuContextResources;
    }

#if ANDROID
    internal void AttachSurfaceTextures(DorotiGraphiteView view)
    {
        _renderer.SetSurfaceTextureFactory(
            (width, height, cancellationToken) =>
            {
                if (!OperatingSystem.IsAndroidVersionAtLeast(33))
                    throw new PlatformNotSupportedException(
                        "Native surface textures currently require Android 13 / API 33 for explicit acquire fences."
                    );
                var surface =
                    (view.Handler?.PlatformView as DorotiAndroidViewContainer)?.Surface
                    ?? throw new InvalidOperationException(
                        "The Android texture host is not attached."
                    );
                return AndroidSurfaceTextureEntry.CreateAsync(
                    _renderer,
                    surface,
                    width,
                    height,
                    cancellationToken
                );
            }
        );
    }

    private IDisposable? _platformViewChannel;
    private AndroidPlatformViewHost? _platformViews;

    internal void AttachPlatformViews(AndroidPlatformViewHost platformViews, IDisposable channel)
    {
        _platformViews = platformViews;
        _platformViewChannel = channel;
        _renderer.PlatformScenePainter = (canvas, commands, descriptor, width, height) =>
            platformViews.Draw(_renderer, canvas, commands, descriptor, width, height);
    }
#endif
#if IOS && !MACCATALYST
    private IDisposable? _platformViewChannel;
    private UIKitPlatformViewHost? _platformViews;

    internal void AttachPlatformViews(UIKitPlatformViewHost platformViews, IDisposable channel)
    {
        _platformViews = platformViews;
        _platformViewChannel = channel;
        _renderer.PlatformScenePainter = (canvas, commands, descriptor, width, height) =>
            platformViews.Draw(
                _renderer,
                canvas,
                commands,
                descriptor,
                width,
                height,
                _host.Configuration.platformBrightness
            );
    }
#endif
#if MACOS
    private DorotiMacOSMetalSurface? _metalSurface;
    private IDisposable? _platformViewChannel;
    private AppKitPlatformViewHost? _platformViews;

    internal void AttachPlatformViews(AppKitPlatformViewHost platformViews, IDisposable channel)
    {
        _platformViews = platformViews;
        _platformViewChannel = channel;
        _renderer.PlatformScenePainter = (canvas, commands, descriptor, width, height) =>
            platformViews.Draw(
                _renderer,
                canvas,
                commands,
                descriptor,
                width,
                height,
                _host.Configuration.platformBrightness
            );
    }

    internal void AttachNativeLifecycle(DorotiMacOSMetalSurface surface)
    {
        _metalSurface = surface;
        surface.GpuResourcesReleasing += _renderer.InvalidateGpuContextResources;
    }
#endif

    internal MauiSkiaCapabilities(
        ulong viewId,
        MauiHostAdapter host,
        Color? backgroundColor,
        Color? darkBackgroundColor
    )
    {
        _host = host;
        _renderer = new(
            viewId,
            new HostBridge(host),
            backgroundColor,
            darkBackgroundColor,
#if MACOS
            "macos/mtkview",
            DorotiMacOSMetalView.UseGraphite
                ? DorotiSkiaRuntimeEffects.NativeGraphiteMetalBackend
                : DorotiSkiaRuntimeEffects.AppKitMetalBackend,
            DorotiMacOSMetalView.GraphicsBackendId,
            // AppKit Ganesh offscreen snapshots can expose a transparent
            // picture before its raster work is visible to the main surface.
            enablePictureRasterCache: false
        );
#elif MACCATALYST
            "maui/skglview",
            DorotiGraphiteView.Enabled
                ? DorotiSkiaRuntimeEffects.NativeGraphiteMetalBackend
                : DorotiSkiaRuntimeEffects.MauiGpuBackend,
            DorotiGraphiteView.Enabled
                ? "UIKit/MTKView/Graphite-Metal"
                : "skiasharp-maui-skglview-gpu",
            // Catalyst uses the same Ganesh/Metal offscreen path. During a
            // live resize, cached component pictures can become visible before
            // their replacement raster belongs to the new drawable epoch.
            enablePictureRasterCache: false
        );
#elif IOS
            "maui/skglview",
            DorotiGraphiteView.Enabled
                ? DorotiSkiaRuntimeEffects.NativeGraphiteMetalBackend
                : DorotiSkiaRuntimeEffects.MauiGpuBackend,
            DorotiGraphiteView.Enabled
                ? "UIKit/MTKView/Graphite-Metal"
                : "skiasharp-maui-skglview-gpu",
            // iOS uses the same Ganesh/Metal offscreen path as Catalyst.
            // Cached component pictures can otherwise expose a transparent
            // snapshot before their raster work is visible to the drawable.
            enablePictureRasterCache: false
        );
#elif ANDROID
            "maui/surfaceview",
            DorotiGraphiteView.Enabled
                ? DorotiSkiaRuntimeEffects.NativeGraphiteVulkanBackend
                : DorotiSkiaRuntimeEffects.MauiGpuBackend,
            DorotiGraphiteView.Enabled
                ? "Android/SurfaceView/Graphite-Vulkan"
                : "skiasharp-maui-skglview-gpu",
            enablePictureRasterCache: false
        );
#elif WINDOWS
            "maui/composition",
            WindowsCompositionSurfaceFeature.GraphiteEnabled
                ? DorotiSkiaRuntimeEffects.NativeGraphiteVulkanBackend
                : DorotiSkiaRuntimeEffects.MauiGpuBackend,
            WindowsCompositionSurfaceFeature.GraphiteEnabled
                ? "WinUI/CompositionDrawingSurface/Graphite-Vulkan"
                : "skiasharp-maui-skglview-gpu",
            enablePictureRasterCache: !WindowsCompositionSurfaceFeature.GraphiteEnabled
        );
#else
            "maui/skglview",
            DorotiSkiaRuntimeEffects.MauiGpuBackend,
            "skiasharp-maui-skglview-gpu"
        );
#endif
#if IOS || MACCATALYST
        if (DorotiGraphiteView.Enabled)
            _renderer.EnableNativeTextures(NativeTexturePlatform.Apple);
#elif MACOS
        if (DorotiMacOSMetalView.UseGraphite)
            _renderer.EnableNativeTextures(NativeTexturePlatform.Apple);
#endif
    }

    public event Action<SemanticsActionEvent>? Action
    {
        add => _renderer.Action += value;
        remove => _renderer.Action -= value;
    }

    internal MauiFrameDiagnostics Diagnostics
    {
        get
        {
            var value = _renderer.Diagnostics;
            return new(
                value.Submitted,
                value.Presented,
                value.Replayed,
                value.Failed,
                value.ContextGeneration,
                value.SurfaceGeneration,
                value.PendingScene,
                value.ShaderImageFiltersRendered,
                value.Backend,
                value.Superseded,
                value.Dropped,
                value.LastInputSequence,
                value.LastSubmittedInputSequence,
                value.LastPresentedInputSequence,
                value.ImageFilterSurfacesCreated,
                value.ImageFilterSurfaceReuses,
                value.ActiveImageFilterSurfaces,
                value.ShaderImageFilterCacheHits,
                value.ShaderImageFilterCacheMisses,
                value.PictureRasterCacheHits,
                value.PictureRasterCacheMisses,
                value.PictureRasterCacheEntries,
                value.Trace
            );
        }
    }

    internal void AttachFrameworkTrace(DorotiFrameTrace frameTrace)
    {
        // Diagnostic wall clock is separate from the causally clamped frame
        // timestamp. Both baseline and official candidates use the same switch.
        frameTrace.MeasureRecordingTime =
            Environment.GetEnvironmentVariable("DOROTI_MAUI_WALL_TRACE") == "1";
        _renderer.AttachFrameworkTrace(frameTrace);
    }

    internal void AttachSurface(Action invalidate) => _renderer.AttachSurface(invalidate);

    public void Submit(
        ulong viewId,
        DorotiSceneSubmission submission,
        DorotiUiInvocation invocation
    ) => _renderer.Submit(viewId, submission, invocation);

    internal MauiPaintCompletion? Paint(
        SKSurface surface,
        int pixelWidth,
        int pixelHeight,
        out bool shouldPresent
    )
    {
        var result = _renderer.Paint(surface, pixelWidth, pixelHeight, _host.ResizeTarget);
        shouldPresent = result.Disposition != SkiaPaintDisposition.superseded;
        return result.Completion is { } completion
            ? new(
                completion.InputSequence,
                completion.SceneSequence,
                completion.SurfaceGeneration,
                completion.IsNewFrame,
                completion.Descriptor
            )
            : null;
    }

    internal void CompletePaint(MauiPaintCompletion completion) =>
        _renderer.CompletePaint(
            new(
                completion.InputSequence,
                completion.SceneSequence,
                completion.SurfaceGeneration,
                completion.IsNewFrame,
                completion.Descriptor
            )
        );

    internal void FailPaint(MauiPaintCompletion completion, string reason) =>
        _renderer.FailPaint(
            new(
                completion.InputSequence,
                completion.SceneSequence,
                completion.SurfaceGeneration,
                completion.IsNewFrame,
                completion.Descriptor
            ),
            reason
        );

    internal void SupersedePaint(MauiPaintCompletion completion, string reason) =>
        _renderer.SupersedePaint(
            new(
                completion.InputSequence,
                completion.SceneSequence,
                completion.SurfaceGeneration,
                completion.IsNewFrame,
                completion.Descriptor
            ),
            reason
        );

    public Paragraph Layout(ParagraphRequest request, DorotiUiInvocation invocation) =>
        _renderer.Layout(request, invocation);

    public ValueTask<UiImage> DecodeSizedAsync(
        ReadOnlyMemory<byte> bytes,
        Func<long, long, TargetImageSize?> targetSize,
        bool allowUpscaling,
        DorotiUiInvocation invocation,
        CancellationToken cancellationToken = default
    ) =>
        _renderer.DecodeSizedAsync(
            bytes,
            targetSize,
            allowUpscaling,
            invocation,
            cancellationToken
        );

    public ValueTask<UiImage> RasterizeAsync(
        Picture picture,
        int width,
        int height,
        DorotiUiInvocation invocation,
        CancellationToken cancellationToken = default
    ) => _renderer.RasterizeAsync(picture, width, height, invocation, cancellationToken);

    public ValueTask<UiImage> DecodeAsync(
        ReadOnlyMemory<byte> bytes,
        DorotiUiInvocation invocation,
        CancellationToken cancellationToken = default
    ) => _renderer.DecodeAsync(bytes, invocation, cancellationToken);

    public void SetEnabled(bool enabled, DorotiUiInvocation invocation) =>
        _renderer.SetEnabled(enabled, invocation);

    public void Update(SemanticsUpdate update, DorotiUiInvocation invocation) =>
        _renderer.Update(update, invocation);

    public ValueTask RegisterFontAsync(
        ReadOnlyMemory<byte> bytes,
        string? family,
        CancellationToken cancellationToken = default
    ) => _renderer.RegisterFontAsync(bytes, family, cancellationToken);

    public void Dispose()
    {
        if (_graphiteSurface is { } graphiteSurface)
        {
            graphiteSurface.GpuResourcesReleasing -= _renderer.InvalidateGpuContextResources;
        }

        _graphiteSurface = null;
#if ANDROID || (IOS && !MACCATALYST)
        _renderer.PlatformScenePainter = null;
        _platformViewChannel?.Dispose();
        _platformViewChannel = null;
        _platformViews?.Dispose();
        _platformViews = null;
#endif
#if MACOS
        _renderer.PlatformScenePainter = null;
        _platformViewChannel?.Dispose();
        _platformViewChannel = null;
        _platformViews?.Dispose();
        _platformViews = null;
        if (_metalSurface is { } surface)
        {
            surface.GpuResourcesReleasing -= _renderer.InvalidateGpuContextResources;
        }

        _metalSurface = null;
#endif
        _renderer.Dispose();
    }

    private sealed class HostBridge(MauiHostAdapter host) : ISkiaSceneRendererHost
    {
        private readonly MauiHostAdapter _host = host;

        public long InputSequence => _host.InputSequence;
        public long SurfaceGeneration => _host.Snapshot.SurfaceGeneration;
        public DorotiViewEpoch ViewEpoch => _host.ViewEpoch;
        public DorotiResizeEpoch ResizeTarget => _host.ResizeTarget;
        public PlatformConfiguration Configuration => _host.Configuration;

        public event Action<int, SemanticsAction, object?>? SemanticsAction
        {
            add => _host.SemanticsAction += value;
            remove => _host.SemanticsAction -= value;
        }

        public event Action<long, TimeSpan>? InputReceived
        {
            add => _host.InputReceived += value;
            remove => _host.InputReceived -= value;
        }

        public event Action<PlatformConfiguration>? ConfigurationChanged
        {
            add => _host.ConfigurationChanged += value;
            remove => _host.ConfigurationChanged -= value;
        }

        public void UpdateSemantics(SemanticsUpdate update) => _host.UpdateSemantics(update);

        public void ClearSemantics() => _host.ClearSemantics();

        public void RequestInvalidate() => _host.RequestInvalidate();
    }
}
